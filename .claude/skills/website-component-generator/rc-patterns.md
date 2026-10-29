# RC Sub-Class Patterns (Sandox)

In the DMP platform an RC sub-class reads from the database through `IUnitOfWork`. **In Sandox there is no database**: an RC sub-class builds its data in memory (mock data) and everything else about it is identical, so the component code on the other side of it never changes. When a component moves to the real CRM, only the data steps inside the RC get swapped for repository calls.

## ExecuteAsync Rules

1. `ExecuteAsync` is the **only orchestrator** — calls step methods in vertical sequence
2. Each step method is **flat and self-contained** — never calls another method
3. Step methods **never return values** — they store results into `_i` (the `Data` instance)
4. Subsequent steps read from `_i` what they need
5. Allowed methods in an RC sub-class: the fixed boilerplate (`DebouncePreviousOperationAsync`, `StateUpdateAsync`, `SetRequestHasResults`, `PrepareResponseData`) plus flat **step methods** called from `ExecuteAsync` (e.g. `GetMockRowsAsync`, `FilterRows`, `SaveMockRowAsync`). `PrepareResponseData()` may only do simple passthrough assignments (`ResponseData.X = _i.X`). No LINQ, no transformation, no logic in it.
6. **Never reference** `IUnitOfWork`, repositories, `IFrontEndErrorLog_B`, `CoreException` or `FrontEndException`. They do not exist in Sandox.

## Standard ExecuteAsync Sequence

```
_req = requestData;
_i = new();
await DebouncePreviousOperationAsync();   // always first
CreateScope();                             // always second
SetRequestLoading();                       // always third
await StateUpdateAsync();                  // always fourth — triggers spinner
// --- your mock data steps here ---
// e.g. await GetMockRowsAsync();
// e.g. FilterRows();
// e.g. await SaveMockRowAsync();
PrepareResponseData();                     // always second to last
SetRequestSucceeded();                     // always last
SetRequestHasResults();                    // always last
```

## Full RC Sub-Class Template

```csharp
public class {SubClassName} : ST_Subscriptions<{SubClassName}.DTOResponse>
{
    private CancellationTokenSource? _debounceCancellationTokenSource;

    public {SubClassName}(IServiceProvider serviceProvider) : base(new DTOResponse(), serviceProvider)
    {
    }

    protected override void CreateScope()
    {
        _serviceScope = _serviceProvider.CreateScope();
    }

    private Data _i;
    private DTORequest _req;

    public async Task ExecuteAsync(DTORequest requestData)
    {
        try
        {
            _req = requestData;
            _i = new();
            await DebouncePreviousOperationAsync();
            CreateScope();
            SetRequestLoading();
            await StateUpdateAsync();
            // add your mock data steps here
            //------------------------------
            PrepareResponseData();
            SetRequestSucceeded();
            SetRequestHasResults();
        }
        catch (TaskCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            SetRequestFailed(ex.Message);
        }
        finally
        {
            DisposeScope();
        }
    }

    private async Task DebouncePreviousOperationAsync()
    {
        if (_req.DebounceTime != null)
        {
            ResponseData.HasBeenDebounced = true;
            _debounceCancellationTokenSource?.Cancel();
            _debounceCancellationTokenSource = new CancellationTokenSource();
            await Task.Delay((int)_req.DebounceTime, _debounceCancellationTokenSource.Token);
        }
    }

    protected override void SetRequestHasResults()
    {
        /*
        bool requestHasResults = _i.SomeList.Any();
        ResponseData.RequestStatus.RequestHasResults = requestHasResults;
        */
    }

    private async Task StateUpdateAsync()
    {
        if (_req.StateUpdate != null)
        {
            await _req.StateUpdate.Invoke();
        }
    }

    private void PrepareResponseData()
    {
        // ResponseData.SomeProperty = _i.SomeProperty;
    }

    public class Data
    {
        // intermediate storage between methods
        // e.g. public List<SomeType> Items { get; set; } = new();
    }

    public class DTOResponse : ST_DTOResponse
    {
        // output data mapped in PrepareResponseData()
        // e.g. public List<SomeType> Items { get; set; } = new();
    }

    public class DTORequest
    {
        public int Id { get; set; }
        public Func<Task> StateUpdate { get; set; }
        public int? DebounceTime { get; set; }
        // add request-specific properties here
    }
}
```

## Mock Data Rules

### Mock a data step
Every step that would have called the database becomes a flat step method that builds the data and waits briefly, so the loading gate is exercised the way it would be against a real request:

```csharp
private async Task GetMockRowsAsync()
{
    await Task.Delay(400); // simulate a request so the loading state is visible

    _i.Rows = new()
    {
        new() { Id = 1, Name = "Mock item one",   Amount = 120.00m },
        new() { Id = 2, Name = "Mock item two",   Amount = 85.50m  },
        new() { Id = 3, Name = "Mock item three", Amount = 240.25m }
    };
}
```

- Put the row/DTO classes the mock needs in the `_RC` class (e.g. `public class MockRow { ... }` next to the sub-classes) unless the same shape is shared by several components, then use a small file under `Components/Domains/Mock/`.
- Use obviously fake values. Never real client names, phone numbers, account numbers or other personal data.
- Keep it to 3–8 rows and vary the values enough to exercise the UI (an empty string, a long name, a zero, a negative if relevant).
- To exercise the empty and failure states, you may add a step that reads a request flag (e.g. `if (_req.SimulateEmpty) _i.Rows = new();`). Only do this when the user asks for it.

### A save / update / delete
A "save" step delays briefly and reports success. It does not need to persist:

```csharp
private async Task SaveMockRowAsync()
{
    await Task.Delay(300); // simulate a request
    _i.Saved = true;
}
```

### Making a save visible on the next load
If the user wants a change to still be there after "Reload", the mock store has to outlive the RC sub-class. Two rules:

1. Keep the store in the component's `_S` (an `ObservableProperty<List<...>>`), and have the RC read and write it.
2. **Resolve that `_S` from `_serviceProvider`, never from `_serviceScope.ServiceProvider`.** `CreateScope()` makes a *new child scope*, and scoped services resolved from it are different instances from the ones the component uses. `_serviceProvider.GetRequiredService<{FileName}_S>()` gives you the component's instance.

```csharp
private async Task SaveMockRowAsync()
{
    var s = _serviceProvider.GetRequiredService<{FileName}_S>();   // NOT _serviceScope.ServiceProvider
    s.Root.Rows.Value = s.Root.Rows.Value.Append(_req.NewRow).ToList();
    await Task.Delay(300);
}
```

The plan summary must say when a component uses a persistent mock store.

## Data Flow Example

```csharp
// _i is the scratchpad — steps write to it, subsequent steps read from it
private async Task GetMockCreditorsAsync()
{
    await Task.Delay(400);
    _i.AllCreditors = new()
    {
        new() { Id = 1, Name = "Mock creditor A", IsActive = true },
        new() { Id = 2, Name = "Mock creditor B", IsActive = false }
    };  // store into _i
}

private void FilterActiveCreditors()
{
    _i.ActiveCreditors = _i.AllCreditors.Where(c => c.IsActive).ToList();  // read from _i, write to _i
}

private void PrepareResponseData()
{
    ResponseData.Creditors = _i.ActiveCreditors;  // map from _i to ResponseData
}
```

## InitialiseHD — Dropdown Options Pattern

When a component has dropdown inputs, `Initialise_HD` is responsible for making the options and default selected values available. In Sandox they are built in the mock step:

- **RC `Data`** — holds the already-typed option lists and default selected values
- **RC `PrepareResponseData()`** — simple passthrough only: `ResponseData.X = _i.X`
- **RC `DTOResponse`** — mirrors the same typed properties as `Data`
- **`Inputs.SetInitialInputs()`** — reads from `ResponseData` and assigns into `_d.I.Inputs`

### Option Type → Use Case Mapping

Each option type is a **nested class** of its input component, so refer to it as `Component.OptionType` (e.g. `SingleSelectDropDown.SingleSelectDropDownOption`), and add `using FE.Components.Domains.Utlility.Inputs;` to the code-behind.

| Option Type | Dropdown Component |
|---|---|
| `SingleSelectDropDown.SingleSelectDropDownOption` | `SingleSelectDropDown` (outside table) |
| `SingleSelectDropDownWithSearch.SingleSelectDropDownWithSearchOption` | `SingleSelectDropDownWithSearch` (outside table) |
| `SingleSelectDropDownTwoColumnDisplayWithSearch.SingleSelectDropDownTwoColumnDisplayWithSearchOption` | `SingleSelectDropDownTwoColumnDisplayWithSearch` (outside table) |
| `MultiSelectDropdown.MultiSelectDropDownOption` | `MultiSelectDropdown` |
| `Table.TableSingleSelectDropDown.TableSingleSelectDropDownOption` | `Table.TableSingleSelectDropDown` (inside table) |
| `Table.TableSingleDropDownWithSearch.TableSingleDropDownWithSearchOption` | `Table.TableSingleDropDownWithSearch` (inside table) |

### RC Sub-Class — `Initialise_HD` with Options

```csharp
public class Initialise_HD : ST_Subscriptions<Initialise_HD.DTOResponse>
{
    private CancellationTokenSource? _debounceCancellationTokenSource;

    public Initialise_HD(IServiceProvider serviceProvider) : base(new DTOResponse(), serviceProvider) { }

    protected override void CreateScope()
    {
        _serviceScope = _serviceProvider.CreateScope();
    }

    private Data _i;
    private DTORequest _req;

    public async Task ExecuteAsync(DTORequest requestData)
    {
        try
        {
            _req = requestData;
            _i = new();
            await DebouncePreviousOperationAsync();
            CreateScope();
            SetRequestLoading();
            await StateUpdateAsync();
            //------------------------------
            await GetMockStatusOptionsAsync();
            //------------------------------
            PrepareResponseData();
            SetRequestSucceeded();
            SetRequestHasResults();
        }
        catch (TaskCanceledException) { return; }
        catch (Exception ex) { SetRequestFailed(ex.Message); }
        finally { DisposeScope(); }
    }

    private async Task GetMockStatusOptionsAsync()
    {
        await Task.Delay(300);
        _i.StatusOptions = new()
        {
            new("Open", 1),
            new("In progress", 2),
            new("Closed", 3)
        };
        _i.SelectedStatus = _i.StatusOptions[0];   // pre-populated; leave null for a blank dropdown
    }

    private async Task DebouncePreviousOperationAsync() { /* boilerplate */ }
    private async Task StateUpdateAsync() { /* boilerplate */ }

    protected override void SetRequestHasResults()
    {
        // ResponseData.RequestStatus.RequestHasResults = ...;
    }

    private void PrepareResponseData()
    {
        // Simple passthrough only — no mapping, no LINQ
        ResponseData.StatusOptions = _i.StatusOptions;
        ResponseData.SelectedStatus = _i.SelectedStatus;
    }

    public class Data
    {
        // Already-typed option lists and default selected values
        public List<SingleSelectDropDown.SingleSelectDropDownOption> StatusOptions { get; set; } = new();
        public SingleSelectDropDown.SingleSelectDropDownOption? SelectedStatus { get; set; }
    }

    public class DTOResponse : ST_DTOResponse
    {
        // Mirrors Data exactly
        public List<SingleSelectDropDown.SingleSelectDropDownOption> StatusOptions { get; set; } = new();
        public SingleSelectDropDown.SingleSelectDropDownOption? SelectedStatus { get; set; }
    }

    public class DTORequest
    {
        public int Id { get; set; }
        public Func<Task> StateUpdate { get; set; }
        public int? DebounceTime { get; set; }
    }
}
```

### Wiring into the Component

**`Data.InputsClass`** — declares option lists and selected value placeholders:

```csharp
public class InputsClass : ValidatableInputs
{
    public List<SingleSelectDropDown.SingleSelectDropDownOption> StatusOptions { get; set; } = new();
    public SingleSelectDropDown.SingleSelectDropDownOption? SelectedStatus { get; set; }
}
```

**`Inputs.SetInitialInputs()`** — called in `SetupAsync()` after `_req.InitialRequestAsync()`, reads from `ResponseData` and assigns into `_d.I.Inputs`:

```csharp
public void SetInitialInputs()
{
    _d.I.Inputs.StatusOptions = _d.RC_Internal.Initialise_HD_.ResponseData.StatusOptions;
    _d.I.Inputs.SelectedStatus = _d.RC_Internal.Initialise_HD_.ResponseData.SelectedStatus;
}
```

**Razor** binds directly to `_d.I.Inputs`:

```razor
<FE.Components.Domains.Utlility.Inputs.SingleSelectDropDown
    @bind-SelectedOption="_d.I.Inputs.SelectedStatus"
    Options="_d.I.Inputs.StatusOptions"
    Label="Status"
    Id="status-dd"
    Name="status-dd"
    Placeholder="Select a status..." />
```

### Key Rules

- **No mapping in the RC** — `Data`, `DTOResponse`, and `PrepareResponseData()` all use already-typed option types. No LINQ or transformation in `PrepareResponseData()`.
- **`PrepareResponseData()` is passthrough only** — `ResponseData.X = _i.X`. Nothing else.
- **`Inputs.SetInitialInputs()` owns the wiring** — it reads from `ResponseData` and assigns into `_d.I.Inputs`. Not `Request`.
- **`SetupAsync()` order matters** — always call `await _req.InitialRequestAsync()` before `_inputs.SetInitialInputs()`.
- **Razor uses `_d.I.Inputs.XxxOptions` directly** — never access `RC_Internal` from razor.
- **Match option type to dropdown component exactly** — mismatches cause binding errors (see table above).

---

## SetRequestFailed Variants

```csharp
SetRequestFailed();                    // no message
SetRequestFailed(ex.Message);          // with the exception message (the standard catch block)
SetRequestFailed("Mock failure");      // to exercise the failed state deliberately
```

## StateSub Exception — RC Called Directly

The one case where RC is called outside of `_req` is in `StateSub` subscription callbacks:

```csharp
public void Subscribe()
{
    _d.S_Internal.Root.SomeProperty.SubscribeAsync(Refresh, _d.I.ClassName);
}

public void UnSubscribe()
{
    _d.S_Internal.Root.SomeProperty.AsyncUnsubscribe(Refresh);
}

private async Task Refresh(SomeType arg1, StateChangeContext context)
{
    await _d.RC_Internal.Initialise_HD_.ExecuteAsync(new()
    {
        Id = _d.SomeOther_S.Root.SomeId.Value,
        StateUpdate = _d.StateUpdate  // pass StateUpdate so RC can trigger re-render
    });
    await _d.StateUpdate();
}
```

Note: When calling RC from `StateSub`, always pass `StateUpdate` in the DTORequest so the RC can trigger re-renders during loading.
