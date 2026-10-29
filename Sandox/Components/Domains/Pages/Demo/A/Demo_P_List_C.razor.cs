using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;

namespace FE.Components.Domains.Pages.Demo.A;

public partial class Demo_P_List_C : ComponentBase, IDisposable
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
    // private Demo_P_List_S _s_internal { get; set; }

    private Data _i;
    private Navigation _nav;
    private StateSub _stateSub;
    private Switch _switch;
    private Inputs _inputs;
    private Request _req;
    private Dependencies _d;
    private Action _act;
    private State _state;
    private Demo_P_List_RC _rc_internal;

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
        await _req.InitialRequestAsync();
        _inputs.SetInitialInputs();
        await _d.StateUpdate();
    }

    public async void Dispose()
    {
        _stateSub.UnSubscribe();
        await _act.CompletePendingOperationsAsync();
        _d.S_Internal.Reset();
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

            // Pre-populated from mock/initial data
            public List<Demo_P_List_RC.MockItem> Items { get; set; } = new();
        }
    }

    public class Dependencies
    {
        public Data I;
        public Demo_P_List_RC RC_Internal;
        public Func<Task> StateUpdate;
        public NavigationManager NavManager;
        public List<Task> PendingOperations = new();
        public Demo_P_List_S S_Internal;

        public Dependencies(Data data,
                            Demo_P_List_RC internal_RC,
                            Func<Task> stateUpdate,
                            NavigationManager navManager,
                            IServiceProvider serviceProvider)
        {
            RC_Internal = internal_RC;
            StateUpdate = stateUpdate;
            NavManager = navManager;
            I = data;
            S_Internal = serviceProvider.GetRequiredService<Demo_P_List_S>();
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

        public async Task InitialRequestAsync()
        {
            await _d.RC_Internal.Initialise_HD_.ExecuteAsync(new()
            {
                StateUpdate = _d.StateUpdate
            });
        }
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
            // Items: pre-populated from mock/initial data. No form inputs, so no validators are registered.
            _d.I.Inputs.Items = _d.RC_Internal.Initialise_HD_.ResponseData.Items;
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
            _d.S_Internal.Root.SelectedItemId.NoDataSubscribeAsync(Refresh, _d.I.ClassName);
        }

        public void UnSubscribe()
        {
            _d.S_Internal.Root.SelectedItemId.NoDataUnSubscribeAsync(Refresh);
        }

        private async Task Refresh(StateChangeContext context)
        {
            await _d.StateUpdate();
        }
    }

    public class State
    {
        private readonly Dependencies _d;

        public State(Dependencies dependecies)
        {
            _d = dependecies;
        }

        public void SelectItem(int id)
        {
            _d.S_Internal.Root.SelectedItemId.Value = id;
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

        public string RowClass(int id)
        {
            var baseClass = "flex items-center justify-between px-4 py-3 border-b border-gray-100 text-sm cursor-pointer transition-colors";

            return _d.S_Internal.Root.SelectedItemId.Value == id
                ? $"{baseClass} bg-indigo-50 text-indigo-700"
                : $"{baseClass} hover:bg-gray-50 text-gray-700";
        }
    }
    // ==============================================================================================================================
    // ==============================================================================================================================
}

public class Demo_P_List_RC
{
    public Initialise_HD Initialise_HD_ { get; set; }

    public Demo_P_List_RC(IServiceProvider serviceProvider)
    {
        Initialise_HD_ = new Initialise_HD(serviceProvider);
    }

    public class MockItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
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
                //------------------------------
                await GetMockItemsAsync();
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

        private async Task GetMockItemsAsync()
        {
            await Task.Delay(600); // simulate a request

            _i.Items = new()
            {
                new() { Id = 1, Name = "Mock item one", Amount = 120.00m },
                new() { Id = 2, Name = "Mock item two", Amount = 85.50m },
                new() { Id = 3, Name = "Mock item three", Amount = 240.25m }
            };
        }

        protected override void SetRequestHasResults()
        {
            ResponseData.RequestStatus.RequestHasResults = _i.Items.Any();
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
            ResponseData.Items = _i.Items;
        }

        public class Data
        {
            public List<MockItem> Items { get; set; } = new();
        }

        public class DTOResponse : ST_DTOResponse
        {
            public List<MockItem> Items { get; set; } = new();
        }

        public class DTORequest
        {
            public Func<Task> StateUpdate { get; set; }
        }
    }
}

public class Demo_P_List_S
{
    public RootClass Root { get; private set; }
    public RefreshClass Refresh { get; private set; }

    public Demo_P_List_S()
    {
        Root = new RootClass();
        Refresh = new RefreshClass();
    }

    public class RootClass
    {
        public RootClass()
        {
            SelectedItemId = new(nameof(SelectedItemId), 0, GetType().Name);
        }

        public ObservableProperty<int> SelectedItemId { get; set; }
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
        Root.SelectedItemId.Value = 0;
        Refresh = new();
    }
}
