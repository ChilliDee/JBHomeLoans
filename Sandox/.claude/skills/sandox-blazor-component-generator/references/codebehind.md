# Code-Behind Reference (Sandox — no DB, no logging)

## Standard Using Statements

Always include these at the top of every code-behind file:

```csharp
using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;
```

These come from the `BusinessLogic.Core` package and are all Sandox needs. **Do not add** `BusinessLogic.Core.UnitOfWork`, `BusinessLogic.Core.Logging.*`, or any repository/logging namespace — Sandox has no database and no logging.

Implicit usings cover `System`, `System.Linq`, `System.Threading.Tasks` and `Microsoft.Extensions.DependencyInjection` (needed for `CreateScope()` and `GetRequiredService`).

If the component uses input option types (dropdown options etc.) add a `using static` for each input component whose nested types it uses — see "Nested option types" in `inputs.md`. Example:

```csharp
using static FE.Components.Domains.Utlility.Inputs.SingleSelectDropDown;
```

---

## Namespace

File-scoped, derived from the folder path (`FE` + folder path). See `SKILL.md` → "Namespace". Example for `Components/Domains/Pages/Demo/A/`:

```csharp
namespace FE.Components.Domains.Pages.Demo.A;
```

---

## Full Code-Behind Boilerplate Template

Replace `{FileName}` with the component name (without the `_C` suffix) and `{Namespace}` with the folder-derived namespace.

```csharp
using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;

namespace {Namespace};

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
        //            await _req.InitialRequestAsync();
        //            _inputs.SetInitialInputs();
        //            await _d.StateUpdate();
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

        public class InputsClass
        {
            public readonly List<Func<(bool IsValid, string Label)>> Validators = new();
            public bool IsValid;
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
                   StateUpdate = _d.StateUpdate
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
            => _d.I.Inputs.Validators.Add(validator);

        public void ValidateAll()
        {
            var results = _d.I.Inputs.Validators.Select(v => v()).ToList();
            var failed = results.Where(r => !r.IsValid).Select(r => r.Label).ToList();
            _d.I.Inputs.IsValid = !failed.Any();
        }
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

    // No database: data comes from memory instead of IUnitOfWork
    public class Initialise_HD : ST_Subscriptions<Initialise_HD.DTOResponse>
    {
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
                CreateScope();
                SetRequestLoading();
                await StateUpdateAsync();
                // add your code here (e.g. await GetMockItemsAsync();)
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
            public Func<Task> StateUpdate { get; set; }
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

## Differences from the DB-backed (CRM) version

The RC sub-class has been stripped down to match `Demo_P_List_RC`:

- No `IFrontEndErrorLog_B`, `IUnitOfWork`, or other scoped services resolved in `CreateScope()` — it only creates the scope.
- No `CancellationTokenSource`, `DebouncePreviousOperationAsync()`, or `DTORequest.DebounceTime`.
- One `catch (Exception ex)` that calls `SetRequestFailed(ex.Message)`. No `CoreException` / `TaskCanceledException` / `FrontEndException` branches, and no error-log call. The message flows to `ResponseData.ErrorMessage`, which the razor failure state renders.
- `DTORequest` has no `Id` (it was only used by the logging catch blocks). Add request-specific properties as needed.
- Namespace is derived from the folder, not hard-coded.

Everything in `_C` (all inner classes, lifecycle, validation) and `_S` is unchanged from the original pattern.

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

No `[Inject]` is needed for `_S` services — `Program.cs` registers every `*_S` class under `FE.Components` as scoped automatically.

---

## Adding RC Sub-Classes

Add siblings to `Initialise_HD` inside `_RC`. Each follows the same structure — see `rc-patterns.md`.

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

Validators are registered in `SetInitialInputs()` via `_inputs.AddValidator(...)`. Each validator is a `Func<(bool IsValid, string Label)>` — a lambda that returns whether the input is valid and a label identifying it.

Before any `_req` method that submits data, call `_inputs.ValidateAll()` first. The `_req` method then checks `_d.I.Inputs.IsValid` before executing.

### Registering validators in `SetInitialInputs`:
```csharp
public void SetInitialInputs()
{
    _inputs.AddValidator(() => (
        _d.I.Inputs.StartDate != default,
        nameof(_d.I.Inputs.StartDate)
    ));
    _inputs.AddValidator(() => (
        _d.I.Inputs.RepaymentAmount > 0,
        nameof(_d.I.Inputs.RepaymentAmount)
    ));
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
            StateUpdate = _d.StateUpdate
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
            _state.RefreshRelevantComponents();
            await _d.StateUpdate();
        }">
    Create
</button>
```

**Rules:**
- `_inputs.ValidateAll()` is always called in the razor `@onclick`, never inside the `_req` method itself
- The `_req` method owns the `if (_d.I.Inputs.IsValid)` gate — it never executes without checking
- `_state` and `_d.StateUpdate()` are always called after `_req`, regardless of whether the request ran
