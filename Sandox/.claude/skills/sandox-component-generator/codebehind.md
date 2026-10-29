# Code-Behind Reference (Sandox)

## Standard Using Statements
Always include these at the top of every code-behind file:
```csharp
using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;
```

`System.*`, `Microsoft.Extensions.DependencyInjection` and `Microsoft.Extensions.Logging` come from the project's implicit usings. Add `using FE.Components.Domains.Utlility.Inputs;` (and `...Inputs.Table;`) **only** when the code-behind references an input option type such as `SingleSelectDropDown.SingleSelectDropDownOption`.

There are no `BusinessLogic.*` usings in Sandox. Never add them.

---

## Full Code-Behind Boilerplate Template

Replace `{FileName}` with the component name (without suffix). The namespace is `FE.` + the folder path under `C:\Projects\Sandox`, e.g. `FE.Components.Domains.Pages.Demo.A`.

```csharp
using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;

namespace FE.Components.Domains;

public partial class {FileName}_C : ComponentBase, IDisposable
{
    /*
    <div id="@_d.I.ClassName"></div>
    */
    // PARAMS ==============================================
    /*[Parameter]
    public int Id { get; set; }*/
    // ======================================================

    [Inject]
    private IServiceProvider _serviceProvider { get; set; }
    [Inject]
    private NavigationManager _navManager { get; set; }
    [Inject]
    private InterceptorErrorHandlingFactory _interceptorErrorHandlingFactory { get; set; }
    // [Inject]
    // private {FileName}_S _s_internal { get; set; }

    private Data _i;
    private Navigation _nav;
    private StateSub _stateSub;
    private Switch _switch;
    private Inputs _inputs;
    private Request _req;
    private Dependencies _d;
    private Action _act;
    private State _state;
    private {FileName}_RC _rc_internal;

    protected override async Task OnInitializedAsync()
    {
        _i = new();
        _rc_internal = new(_serviceProvider);
        _d = new(_i, _rc_internal, async () => await InvokeAsync(StateHasChanged), _navManager, _serviceProvider);

        _req = _interceptorErrorHandlingFactory.Create<Request, Dependencies>(typeof(Request), _d, new(_d), _d.I.ClassName);
        _stateSub = _interceptorErrorHandlingFactory.Create<StateSub, Dependencies>(typeof(StateSub), _d, new(_d), _d.I.ClassName);
        _nav = _interceptorErrorHandlingFactory.Create<Navigation, Dependencies>(typeof(Navigation), _d, new(_d), _d.I.ClassName);
        _inputs = _interceptorErrorHandlingFactory.Create<Inputs, Dependencies>(typeof(Inputs), _d, new(_d), _d.I.ClassName);
        _act = _interceptorErrorHandlingFactory.Create<Action, Dependencies>(typeof(Action), _d, new(_d), _d.I.ClassName);
        _state = _interceptorErrorHandlingFactory.Create<State, Dependencies>(typeof(State), _d, new(_d), _d.I.ClassName);
        _switch = _interceptorErrorHandlingFactory.Create<Switch, Dependencies>(typeof(Switch), _d, new(_d), _d.I.ClassName);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !_d.I.DidFirstRender)
        {
            _d.I.DidFirstRender = true;
            await SetupAsync();
            _d.I.InitialisationCompleted = true;
            await _d.StateUpdate();
        }
    }

    private async Task SetupAsync()
    {
        _stateSub.Subscribe();
        // Static component (no API calls): do NOT call _req.InitialRequestAsync() here.
        // The request and Initialise_HD stay in the file for later. When the component starts loading data, add:
        //            await _req.InitialRequestAsync();   (before SetInitialInputs)
        _inputs.SetInitialInputs();
        await _d.StateUpdate();
    }

    public async void Dispose()
    {
        _stateSub.UnSubscribe();
        await _act.CompletePendingOperationsAsync();
        // _d.S_Internal.Reset(); // uncomment if component owns an _S
    }

    // SET UP =====================================================================================================================
    public class Data
    {
        public string ClassName => this.GetType().DeclaringType?.Name;
        public InputsClass Inputs { get; set; } = new();
        public bool DidFirstRender { get; internal set; }
        public bool InitialisationCompleted { get; internal set; } = false;

        public class InputsClass : ValidatableInputs
        {
            // Validator list, IsValid and FailedLabels come from ValidatableInputs.
        }
    }

    public class Dependencies
    {
        public Data I;
        public {FileName}_RC RC_Internal;
        public Func<Task> StateUpdate;
        public NavigationManager NavManager;
        public List<Task> PendingOperations = new();
        public {FileName}_S S_Internal;

        public Dependencies(Data data,
                            {FileName}_RC internal_RC,
                            Func<Task> stateUpdate,
                            NavigationManager navManager,
                            IServiceProvider serviceProvider)
        {
            RC_Internal = internal_RC;
            StateUpdate = stateUpdate;
            NavManager = navManager;
            I = data;
            S_Internal = serviceProvider.GetRequiredService<{FileName}_S>();
            //OtherState_S = serviceProvider.GetRequiredService<OtherState_S>();
        }
    }
    // =============================================================================================================================
    // CLASSES =====================================================================================================================
    public class Request
    {
        private Dependencies _d;

        public Request(Dependencies d)
        {
            _d = d;
        }

        /*   public async Task InitialRequestAsync()
           {
               await _d.RC_Internal.Initialise_HD_.ExecuteAsync(new()
               {
               });
           } */
    }

    public class Inputs
    {
        private readonly Dependencies _d;

        public Inputs(Dependencies d)
        {
            _d = d;
        }

        public void SetInitialInputs()
        {
        }

        public void AddValidator(Func<(bool IsValid, string Label)> validator)
            => _d.I.Inputs.AddValidator(validator);

        public void RemoveValidator(Func<(bool IsValid, string Label)> validator)
            => _d.I.Inputs.RemoveValidator(validator);

        public void ValidateAll()
            => _d.I.Inputs.ValidateAll();
    }

    public class StateSub
    {
        private readonly Dependencies _d;

        public StateSub(Dependencies dependecies)
        {
            _d = dependecies;
        }

        public void Subscribe()
        {
            //_d.S_Internal.Refresh.Refresh.NoDataSubscribeAsync(Refresh, _d.I.ClassName);
        }

        public void UnSubscribe()
        {
            //_d.S_Internal.Refresh.Refresh.NoDataUnSubscribeAsync(Refresh);
        }

        //private async Task Refresh(StateChangeContext context)
        //{
        //    await _d.StateUpdate();
        //}
    }

    public class State
    {
        private readonly Dependencies _d;

        public State(Dependencies dependecies)
        {
            _d = dependecies;
        }
    }

    public class Navigation
    {
        private readonly Dependencies _d;

        public Navigation(Dependencies dependecies)
        {
            _d = dependecies;
        }
    }

    public class Action
    {
        private Dependencies _d;

        public Action(Dependencies d)
        {
            _d = d;
        }

        public async Task CompletePendingOperationsAsync()
        {
            if (_d.PendingOperations.Any())
            {
                try
                {
                    await Task.WhenAll(_d.PendingOperations);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }

    public class Switch
    {
        private readonly Dependencies _d;

        public Switch(Dependencies d)
        {
            _d = d;
        }
    }
    // ==============================================================================================================================
    // ==============================================================================================================================
}

public class {FileName}_RC
{
    public Initialise_HD Initialise_HD_ { get; set; }

    public {FileName}_RC(IServiceProvider serviceProvider)
    {
        Initialise_HD_ = new Initialise_HD(serviceProvider);
    }

    public class Initialise_HD : ST_Subscriptions<Initialise_HD.DTOResponse>
    {
        private CancellationTokenSource? _debounceCancellationTokenSource;

        public Initialise_HD(IServiceProvider serviceProvider) : base(new DTOResponse(), serviceProvider)
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
                // add your mock data steps here (see rc-patterns.md)
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
            bool requestHasResults = _i.
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
            // ResponseData.
        }

        public class Data
        {
        }

        public class DTOResponse : ST_DTOResponse
        {
        }

        public class DTORequest
        {
            public int Id { get; set; }
            public Func<Task> StateUpdate { get; set; }
            public int? DebounceTime { get; set; }
        }
    }
}

public class {FileName}_S
{
    public RootClass Root { get; private set; }
    public RefreshClass Refresh { get; private set; }

    public {FileName}_S()
    {
        Root = new RootClass();
        Refresh = new RefreshClass();
    }

    public class RootClass
    {
        public RootClass()
        {
            //   Placeholder = new(nameof(Placeholder), T, GetType().Name);
        }

        // public ObservableProperty<T> Placeholder { get; set; }
    }

    public class RefreshClass
    {
        public RefreshClass()
        {
            Refresh = new(nameof(Refresh), default, GetType().Name);
        }

        public ObservableProperty<int> Refresh { get; set; }
    }

    public void Reset()
    {
        Root = new();
        Refresh = new();
    }
}
```

---

## Adding _S Services to Dependencies

`S_Internal` is always resolved via `IServiceProvider` in the `Dependencies` constructor — never passed as a direct inject parameter. Additional `_S` services follow the same pattern:

```csharp
// In Dependencies:
public Dependencies(Data data, {FileName}_RC internal_RC, Func<Task> stateUpdate,
                    NavigationManager navManager, IServiceProvider serviceProvider)
{
    RC_Internal = internal_RC;
    StateUpdate = stateUpdate;
    NavManager = navManager;
    I = data;
    S_Internal = serviceProvider.GetRequiredService<{FileName}_S>();
    // SomeOther_S = serviceProvider.GetRequiredService<SomeOther_S>();
}

public {FileName}_S S_Internal;
// public SomeOther_S SomeOther_S;
```

No `[Inject]` is needed for `_S` services — they are resolved through the `IServiceProvider` that is already injected into the component. `Program.cs` registers every `*_S` class automatically, so there is nothing to register.

---

## Adding RC Sub-Classes

Add siblings to `Initialise_HD` inside `_RC`. Each follows identical structure — see `rc-patterns.md`.

```csharp
public class {FileName}_RC
{
    public Initialise_HD Initialise_HD_ { get; set; }
    public SaveRow SaveRow_ { get; set; }

    public {FileName}_RC(IServiceProvider serviceProvider)
    {
        Initialise_HD_ = new Initialise_HD(serviceProvider);
        SaveRow_ = new SaveRow(serviceProvider);
    }

    public class Initialise_HD : ST_Subscriptions<Initialise_HD.DTOResponse> { ... }
    public class SaveRow : ST_Subscriptions<SaveRow.DTOResponse> { ... }
}
```

---

## _S ObservableProperty Initialisation

```csharp
public class RootClass
{
    public RootClass()
    {
        SomeFlag = new(nameof(SomeFlag), false, GetType().Name);
        SomeId = new(nameof(SomeId), default, GetType().Name);
        SomeList = new(nameof(SomeList), new(), GetType().Name);
        SomeText = new(nameof(SomeText), string.Empty, GetType().Name);
    }

    public ObservableProperty<bool> SomeFlag { get; set; }
    public ObservableProperty<int?> SomeId { get; set; }
    public ObservableProperty<List<SomeType>> SomeList { get; set; }
    public ObservableProperty<string> SomeText { get; set; }
}
```

## _S Reset() — Individual Value Reset Pattern

When a component owns its `_S`, `Reset()` resets values individually to preserve subscribers:

```csharp
public void Reset()
{
    Root.SomeFlag.Value = false;
    Root.SomeId.Value = null;
    Root.SomeList.Value = new();
    Root.SomeText.Value = string.Empty;
    Refresh = new();
}
```

---

## Validation Pattern

`Data.InputsClass` inherits `ValidatableInputs` (global namespace, `Components/Domains/Utlility/Inputs/Validation/ValidatableInputs.cs`). It owns the validator list and exposes `AddValidator`, `RemoveValidator`, `ValidateAll()`, `IsValid` and `FailedLabels`. It accepts two validator shapes:

- `Func<(bool IsValid, string Label)>` — the tuple the input components pass to `ValidationRef`. Use this shape for hand-written validators too.
- `Func<ValidationResult>` — also accepted (adapted to the tuple internally), for code that already returns the `ValidationResult` record.

Validators get into the list in two ways:

1. **Self-registering inputs (the default for required fields).** Most standard inputs register their own validator when given `Required="true"` and `ValidationRef="_inputs.AddValidator"`, and remove it on dispose when given `ValidationUnref="_inputs.RemoveValidator"`. See `inputs.md` for which inputs support Ref/Unref.
2. **Manual validators in `SetInitialInputs()`** — only for inputs that cannot self-validate (all `Table.*` inputs, MobileInput, MinMaxCurrencyInput_C) and for cross-field rules. Register them on `_d.I.Inputs` directly (not through the inner class's own `AddValidator`, which would be a method calling a method). Never add a manual validator for a field whose input already has `ValidationRef`.

Before any `_req` method that submits data, the razor calls `_inputs.ValidateAll()` first. That runs every validator (each input shows its own inline error) and sets `_d.I.Inputs.IsValid` and `_d.I.Inputs.FailedLabels`. The `_req` method then checks `_d.I.Inputs.IsValid` before executing. Any failure makes the form invalid, including one with an empty label; only labelled failures appear in `FailedLabels`.

### Self-registering input in razor:
```razor
<FE.Components.Domains.Utlility.Inputs.CurrencyInput
    @bind-Value="_d.I.Inputs.RepaymentAmount"
    Label="Repayment" Id="repayment" Name="repayment"
    Required="true"
    ValidationRef="_inputs.AddValidator"
    ValidationUnref="_inputs.RemoveValidator" />
```

### Manual validators in `SetInitialInputs` (non-self-validating inputs / cross-field rules only):
```csharp
public void SetInitialInputs()
{
    // Table input (no self-validation)
    _d.I.Inputs.AddValidator(() => (
        _d.I.Inputs.RowAmount > 0,
        nameof(_d.I.Inputs.RowAmount)
    ));
    // Cross-field rule
    _d.I.Inputs.AddValidator(() => (
        _d.I.Inputs.EndDate is null || _d.I.Inputs.EndDate >= _d.I.Inputs.StartDate,
        "End date"
    ));
}
```

### Showing which fields failed (optional):
```razor
@if (!_d.I.Inputs.IsValid && _d.I.Inputs.FailedLabels.Any())
{
    <div class="text-sm text-red-700">Please check: @string.Join(", ", _d.I.Inputs.FailedLabels)</div>
}
```

### `_req` method with validation gate:
```csharp
public async Task CreateAsync()
{
    if (_d.I.Inputs.IsValid)
    {
        await _d.RC_Internal.Create_HD_.ExecuteAsync(new()
        {
            StartDate = _d.I.Inputs.StartDate,
            // ... other inputs
        });
    }
}
```

### Razor button — always call `_inputs.ValidateAll()` before `_req`:
```razor
<button type="button"
        class="inline-flex items-center gap-1.5 bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-medium px-3 py-1.5 rounded-lg transition-colors cursor-pointer"
        @onclick="async () => {
            _inputs.ValidateAll();
            await _req.CreateAsync();
            await _d.StateUpdate();
        }">
    Create
</button>
```

**Rules:**
- `_inputs.ValidateAll()` is always called in the razor `@onclick`, never inside the `_req` method itself
- The `_req` method owns the `if (_d.I.Inputs.IsValid)` gate — it never executes without checking
- `_state` and `_d.StateUpdate()` are always called after `_req`, regardless of whether the request ran
