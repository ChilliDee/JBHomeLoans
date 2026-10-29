using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;

namespace FE.Components.Domains.Pages.Home.Sections.WhyChooseUs.A;

public partial class Home_Sections_WhyChooseUs_C : ComponentBase, IDisposable
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
    // private Home_Sections_WhyChooseUs_S _s_internal { get; set; }

    private Data _i;
    private Navigation _nav;
    private StateSub _stateSub;
    private Switch _switch;
    private Inputs _inputs;
    private Request _req;
    private Dependencies _d;
    private Action _act;
    private State _state;
    private Home_Sections_WhyChooseUs_RC _rc_internal;

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

        public class InputsClass : ValidatableInputs
        {
            // Validator list, IsValid and FailedLabels come from ValidatableInputs.

            // Pre-populated from mock data
            public List<Home_Sections_WhyChooseUs_RC.MockBenefit> Items { get; set; } = new();
        }
    }

    public class Dependencies
    {
        public Data I;
        public Home_Sections_WhyChooseUs_RC RC_Internal;
        public Func<Task> StateUpdate;
        public NavigationManager NavManager;
        public List<Task> PendingOperations = new();
        public Home_Sections_WhyChooseUs_S S_Internal;

        public Dependencies(Data data,
                            Home_Sections_WhyChooseUs_RC internal_RC,
                            Func<Task> stateUpdate,
                            NavigationManager navManager,
                            IServiceProvider serviceProvider)
        {
            RC_Internal = internal_RC;
            StateUpdate = stateUpdate;
            NavManager = navManager;
            I = data;
            S_Internal = serviceProvider.GetRequiredService<Home_Sections_WhyChooseUs_S>();
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
            _d.I.Inputs.Items = _d.RC_Internal.Initialise_HD_.ResponseData.Items;
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

public class Home_Sections_WhyChooseUs_RC
{
    public Initialise_HD Initialise_HD_ { get; set; }

    public Home_Sections_WhyChooseUs_RC(IServiceProvider serviceProvider)
    {
        Initialise_HD_ = new Initialise_HD(serviceProvider);
    }

    public class MockBenefit
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
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
                //------------------------------
                await GetMockBenefitsAsync();
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

        private async Task GetMockBenefitsAsync()
        {
            await Task.Delay(150); // simulate a request so the loading state is visible

            // Placeholder copy only
            _i.Items = new()
            {
                new() { Id = 1, Title = "Quotes without the runaround", Description = "Placeholder copy: tell us about the vehicle once and get a clear answer on where you stand." },
                new() { Id = 2, Title = "Terms that fit your budget",   Description = "Placeholder copy: choose a term and repayment rhythm that works with how you get paid." },
                new() { Id = 3, Title = "Real people on the phone",     Description = "Placeholder copy: talk to someone who knows vehicle finance, not a script." },
                new() { Id = 4, Title = "Paperwork in plain English",   Description = "Placeholder copy: every cost set out up front, with nothing tucked away in the fine print." }
            };
        }

        protected override void SetRequestHasResults()
        {
            bool requestHasResults = _i.Items.Any();
            ResponseData.RequestStatus.RequestHasResults = requestHasResults;
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
            public List<MockBenefit> Items { get; set; } = new();
        }

        public class DTOResponse : ST_DTOResponse
        {
            public List<MockBenefit> Items { get; set; } = new();
        }

        public class DTORequest
        {
            public int Id { get; set; }
            public Func<Task> StateUpdate { get; set; }
            public int? DebounceTime { get; set; }
        }
    }
}

public class Home_Sections_WhyChooseUs_S
{
    public RootClass Root { get; private set; }
    public RefreshClass Refresh { get; private set; }

    public Home_Sections_WhyChooseUs_S()
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
