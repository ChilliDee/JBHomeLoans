# RC Sub-Class Patterns (Sandox — no DB, no logging)

Sandox has no database and no error logging. RC sub-classes produce their data **in memory** (mock data), exactly like `Demo_P_List_RC.Initialise_HD` in `Components/Domains/Pages/Demo/A/Demo_P_List_C.razor.cs`.

## ExecuteAsync Rules

1. `ExecuteAsync` is the **only orchestrator** — it calls methods in vertical sequence.
2. Each method it calls is **flat and self-contained** — never calls another method.
3. Methods **never return values** — they store results into `_i` (the `Data` instance).
4. Subsequent methods read from `_i` what they need.
5. Data-producing methods (e.g. `GetMockItemsAsync`) are allowed, but only `ExecuteAsync` may call them. Beyond those and the fixed boilerplate, add nothing else.
6. `PrepareResponseData()` may only do simple passthrough assignments (`ResponseData.X = _i.X`). No LINQ, no transformation, no logic.
7. No `IUnitOfWork`, repositories, or logging. Mock data is built inline in a data method, with `await Task.Delay(...)` to simulate a request where a loading state is useful.

## Standard ExecuteAsync Sequence

```
_req = requestData;
_i = new();
CreateScope();                             // always first after init
SetRequestLoading();                       // always second
await StateUpdateAsync();                  // always third — triggers spinner
// --- your methods here ---
// e.g. await GetMockItemsAsync();
// e.g. FilterItems();
// e.g. await SaveMockAsync();
PrepareResponseData();                     // always second to last
SetRequestSucceeded();                     // always last
SetRequestHasResults();                    // always last
```

## Full RC Sub-Class Template

```csharp
public class {SubClassName} : ST_Subscriptions<{SubClassName}.DTOResponse>
{
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
            CreateScope();
            SetRequestLoading();
            await StateUpdateAsync();
            // add your code here
            //------------------------------
            PrepareResponseData();
            SetRequestSucceeded();
            SetRequestHasResults();
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
        public Func<Task> StateUpdate { get; set; }
        // add request-specific properties here
    }
}
```

## Data Flow Example (mock data)

```csharp
// _i is the scratchpad — methods write to it, subsequent methods read from it
private async Task GetMockItemsAsync()
{
    await Task.Delay(600); // simulate a request

    _i.AllItems = new()
    {
        new() { Id = 1, Name = "Mock item one", Amount = 120.00m },
        new() { Id = 2, Name = "Mock item two", Amount = 85.50m },
        new() { Id = 3, Name = "Mock item three", Amount = 240.25m }
    };  // store into _i
}

private void FilterActiveItems()
{
    _i.ActiveItems = _i.AllItems.Where(x => x.Amount > 100).ToList();  // read from _i, write to _i
}

private void PrepareResponseData()
{
    ResponseData.Items = _i.ActiveItems;  // passthrough from _i to ResponseData
}
```

A save-style sub-class has nothing to persist. Simulate it (`await Task.Delay(...)`) and, if the change should be visible elsewhere, write to the component's `_S` via `_state` after the request — do not invent a persistence layer.

## Initialise_HD — Dropdown Options Pattern

When a component has dropdown inputs, `Initialise_HD` makes the options and default selected values available:

- **RC `Data`** — holds the already-typed option lists and default selected values
- **RC `PrepareResponseData()`** — simple passthrough only: `ResponseData.X = _i.X`
- **RC `DTOResponse`** — mirrors the same typed properties as `Data`
- **`Inputs.SetInitialInputs()`** — reads from `ResponseData` and assigns into `_d.I.Inputs`

The options are built already-typed inline in `ExecuteAsync` between the dashed lines (or in a flat data method called from it). No mapping or transformation happens in the RC sub-class.

### Option Type → Use Case Mapping

Option types are nested inside their input component — add the matching `using static` (see `inputs.md` → "Nested option types").

| Option Type | Dropdown Component |
|---|---|
| `SingleSelectDropDownOption` | `SingleSelectDropDown` (outside table) |
| `SingleSelectDropDownWithSearchOption` | `SingleSelectDropDownWithSearch` (outside table) |
| `SingleSelectDropDownTwoColumnDisplayWithSearchOption` | `SingleSelectDropDownTwoColumnDisplayWithSearch` (outside table) |
| `MultiSelectDropDownOption` | `MultiSelectDropdown` (outside table) |
| `SelectableListInputOption` | `SelectableListInput` (outside table) |
| `MultiSelectableListInputOption` | `MultiSelectableListInput` (outside table) |
| `TableSingleSelectDropDownOption` | `Table.TableSingleSelectDropDown` (inside table) |
| `TableSingleDropDownWithSearchOption` | `Table.TableSingleDropDownWithSearch` (inside table) |

### RC Sub-Class — `Initialise_HD` with Options

```csharp
public class Initialise_HD : ST_Subscriptions<Initialise_HD.DTOResponse>
{
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
            CreateScope();
            SetRequestLoading();
            await StateUpdateAsync();
            //------------------------------
            _i.StatusOptions = new()
            {
                new("Active", 1),
                new("On hold", 2),
                new("Closed", 3)
            };
            _i.SelectedStatus = _i.StatusOptions[0];   // omit if the selection is left blank on load
            //------------------------------
            PrepareResponseData();
            SetRequestSucceeded();
            SetRequestHasResults();
        }
        catch (Exception ex) { SetRequestFailed(ex.Message); }
        finally { DisposeScope(); }
    }

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
        public List<SingleSelectDropDownOption> StatusOptions { get; set; }
        public SingleSelectDropDownOption SelectedStatus { get; set; }
    }

    public class DTOResponse : ST_DTOResponse
    {
        // Mirrors Data exactly
        public List<SingleSelectDropDownOption> StatusOptions { get; set; }
        public SingleSelectDropDownOption SelectedStatus { get; set; }
    }

    public class DTORequest
    {
        public Func<Task> StateUpdate { get; set; }
    }
}
```

### Wiring into the Component

**`Data.InputsClass`** — declares option lists and selected value placeholders:

```csharp
public class InputsClass
{
    public List<SingleSelectDropDownOption> StatusOptions { get; set; }
    public SingleSelectDropDownOption SelectedStatus { get; set; }
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

- **No mapping in the RC** — `Data`, `DTOResponse`, and `PrepareResponseData()` all use already-typed option types. No LINQ, no transformation in `PrepareResponseData()`.
- **`PrepareResponseData()` is passthrough only** — `ResponseData.X = _i.X`.
- **`Inputs.SetInitialInputs()` owns the wiring** — it reads from `ResponseData` and assigns into `_d.I.Inputs`. Not `Request`.
- **`SetupAsync()` order matters** — always `await _req.InitialRequestAsync()` before `_inputs.SetInitialInputs()`.
- **Razor uses `_d.I.Inputs.XxxOptions` directly** — never access `RC_Internal` from razor for input options.
- **Match option type to dropdown component exactly** — mismatches cause binding errors (see table above).

---

## SetRequestFailed Variants

```csharp
SetRequestFailed();                    // no message
SetRequestFailed(ex.Message);          // with the exception message — the Sandox default
```

With no logging in Sandox, the message on `ResponseData.ErrorMessage` (rendered by the razor failure state) is the only trace of a failure, so pass it.

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
        StateUpdate = _d.StateUpdate  // pass StateUpdate so RC can trigger re-render
    });
    await _d.StateUpdate();
}
```

Note: When calling RC from `StateSub`, always pass `StateUpdate` in the DTORequest so the RC can trigger re-renders during loading.
