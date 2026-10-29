using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;

namespace FE.Components.Domains.Pages.Home.Footer.A;

public partial class Home_Footer_C : ComponentBase, IDisposable
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
    // private Home_Footer_S _s_internal { get; set; }

    private Data _i;
    private Navigation _nav;
    private StateSub _stateSub;
    private Switch _switch;
    private Inputs _inputs;
    private Request _req;
    private Dependencies _d;
    private Action _act;
    private State _state;
    private Home_Footer_RC _rc_internal;

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
            public string BrandName { get; set; } = string.Empty;

            // Pre-populated from mock data
            public string SmallPrint { get; set; } = string.Empty;

            // Pre-populated from mock data
            public List<Home_Footer_RC.MockFooterColumn> Items { get; set; } = new();
        }
    }

    public class Dependencies
    {
        public Data I;
        public Home_Footer_RC RC_Internal;
        public Func<Task> StateUpdate;
        public NavigationManager NavManager;
        public List<Task> PendingOperations = new();
        public Home_Footer_S S_Internal;

        public Dependencies(Data data,
                            Home_Footer_RC internal_RC,
                            Func<Task> stateUpdate,
                            NavigationManager navManager,
                            IServiceProvider serviceProvider)
        {
            RC_Internal = internal_RC;
            StateUpdate = stateUpdate;
            NavManager = navManager;
            I = data;
            S_Internal = serviceProvider.GetRequiredService<Home_Footer_S>();
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
            _d.I.Inputs.BrandName = _d.RC_Internal.Initialise_HD_.ResponseData.BrandName;
            _d.I.Inputs.SmallPrint = _d.RC_Internal.Initialise_HD_.ResponseData.SmallPrint;
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

public class Home_Footer_RC
{
    public Initialise_HD Initialise_HD_ { get; set; }

    public Home_Footer_RC(IServiceProvider serviceProvider)
    {
        Initialise_HD_ = new Initialise_HD(serviceProvider);
    }

    public class MockFooterColumn
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<MockFooterLink> Links { get; set; } = new();
    }

    public class MockFooterLink
    {
        public string Label { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
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
                await GetMockFooterColumnsAsync();
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

        private async Task GetMockFooterColumnsAsync()
        {
            await Task.Delay(100); // simulate a request so the loading state is visible

            // Placeholder copy only. Links point back at this page until the real pages exist.
            _i.BrandName = "JJ's Vehicle Finance";
            _i.SmallPrint = "Placeholder small print. This is a sandbox page: the rates, repayments and reviews shown are examples only and are not an offer of finance. Real terms, fees, lending criteria and licensing details go here.";
            _i.Items = new()
            {
                new()
                {
                    Id = 1,
                    Title = "Vehicle finance",
                    Links = new()
                    {
                        new() { Label = "Car finance",       Href = "/home#vehicle-types" },
                        new() { Label = "Ute finance",       Href = "/home#vehicle-types" },
                        new() { Label = "Motorbike finance", Href = "/home#vehicle-types" },
                        new() { Label = "Caravan finance",   Href = "/home#vehicle-types" }
                    }
                },
                new()
                {
                    Id = 2,
                    Title = "Tools",
                    Links = new()
                    {
                        new() { Label = "Repayment calculator", Href = "/home#calculator" },
                        new() { Label = "How it works",         Href = "/home#how-it-works" }
                    }
                },
                new()
                {
                    Id = 3,
                    Title = "Company",
                    Links = new()
                    {
                        new() { Label = "Why choose us",   Href = "/home#why-choose-us" },
                        new() { Label = "Customer stories", Href = "/home#reviews" },
                        new() { Label = "Contact us",      Href = "/home#get-started" }
                    }
                },
                new()
                {
                    Id = 4,
                    Title = "Legal",
                    Links = new()
                    {
                        new() { Label = "Privacy policy (placeholder)", Href = "/home" },
                        new() { Label = "Terms of use (placeholder)",   Href = "/home" },
                        new() { Label = "Complaints (placeholder)",     Href = "/home" }
                    }
                }
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
            ResponseData.BrandName = _i.BrandName;
            ResponseData.SmallPrint = _i.SmallPrint;
            ResponseData.Items = _i.Items;
        }

        public class Data
        {
            public string BrandName { get; set; } = string.Empty;
            public string SmallPrint { get; set; } = string.Empty;
            public List<MockFooterColumn> Items { get; set; } = new();
        }

        public class DTOResponse : ST_DTOResponse
        {
            public string BrandName { get; set; } = string.Empty;
            public string SmallPrint { get; set; } = string.Empty;
            public List<MockFooterColumn> Items { get; set; } = new();
        }

        public class DTORequest
        {
            public int Id { get; set; }
            public Func<Task> StateUpdate { get; set; }
            public int? DebounceTime { get; set; }
        }
    }
}

public class Home_Footer_S
{
    public RootClass Root { get; private set; }
    public RefreshClass Refresh { get; private set; }

    public Home_Footer_S()
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
