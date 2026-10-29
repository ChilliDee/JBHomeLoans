using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;
using Microsoft.JSInterop;
namespace FE.Components.Domains.Pages.Jbhl.A;

public partial class Jbhl_P_C : ComponentBase, IDisposable
{
    /*
    <div id="@_d.I.ClassName"></div>
    */
    // PARAMS ==============================================
    [Parameter]
    public string? Section { get; set; }
    // ======================================================

    [Inject]
    private IServiceProvider _serviceProvider { get; set; }
    [Inject]
    private NavigationManager _navManager { get; set; }
    [Inject]
    private InterceptorErrorHandlingFactory _interceptorErrorHandlingFactory { get; set; }
    // [Inject]
    // private Jbhl_P_S _s_internal { get; set; }

    private Data _i;
    private Navigation _nav;
    private StateSub _stateSub;
    private Switch _switch;
    private Inputs _inputs;
    private Request _req;
    private Dependencies _d;
    private Action _act;
    private State _state;
    private Jbhl_P_RC _rc_internal;

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

    protected override void OnParametersSet()
    {
        // the route changed (for example /jbhl/refinance to /jbhl/about): tell the state which section is showing
        _state?.SetCurrentSection(_nav.CurrentSection());
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

        if (_d.I.InitialisationCompleted)
        {
            // a new section starts at the top, like a fresh page load
            await _act.ScrollToTopIfSectionChangedAsync(_nav.CurrentSection());
        }
    }

    private async Task SetupAsync()
    {
        _stateSub.Subscribe();
        // Static component (no API calls): the request is not called on startup.
        // When this component starts loading data, add here (before SetInitialInputs):
        //            await _req.InitialRequestAsync();
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
        public string? LastSection { get; set; }

        public class InputsClass : ValidatableInputs
        {
        }
    }

    public class Dependencies
    {
        public Data I;
        public Jbhl_P_RC RC_Internal;
        public Func<Task> StateUpdate;
        public NavigationManager NavManager;
        public List<Task> PendingOperations = new();
        public Jbhl_P_S S_Internal;
        public IJSRuntime JS;

        public Dependencies(Data data,
                            Jbhl_P_RC internal_RC,
                            Func<Task> stateUpdate,
                            NavigationManager navManager,
                            IServiceProvider serviceProvider)
        {
            RC_Internal = internal_RC;
            StateUpdate = stateUpdate;
            NavManager = navManager;
            I = data;
            S_Internal = serviceProvider.GetRequiredService<Jbhl_P_S>();
            JS = serviceProvider.GetRequiredService<IJSRuntime>();
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
        }

        public void UnSubscribe()
        {
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

        public void SetCurrentSection(string section)
        {
            _d.S_Internal.Root.CurrentSection.Value = section;
        }

        public void NotifyPageClick()
        {
            _d.S_Internal.Root.PageClicks.Value = _d.S_Internal.Root.PageClicks.Value + 1;
        }

    }

    public class Navigation
    {
        private readonly Dependencies _d;

        public Navigation(Dependencies dependecies)
        {
            _d = dependecies;
        }

        public string CurrentSection()
        {
            var path = _d.NavManager.ToBaseRelativePath(_d.NavManager.Uri);
            var end = path.IndexOfAny(new[] { '?', '#' });
            if (end >= 0) path = path.Substring(0, end);

            var parts = path.Trim('/').Split('/');
            return parts.Length > 1 ? parts[1].ToLowerInvariant() : string.Empty;
        }
    }

    public class Action
    {
        private Dependencies _d;

        public Action(Dependencies d)
        {
            _d = d;
        }

        public bool IsKnownSection(string section)
            => section == string.Empty || new[] { "refinance", "buy-a-home", "investment-property", "second-mortgages", "property-debt-consolidation", "about", "contact" }.Contains(section);

        public async Task ScrollToTopIfSectionChangedAsync(string section)
        {
            if (_d.I.LastSection == null)
            {
                _d.I.LastSection = section;
                return;
            }

            if (_d.I.LastSection == section) return;

            _d.I.LastSection = section;
            var module = await _d.JS.InvokeAsync<IJSObjectReference>("import", "/jbhl/js/jbhl.js");
            await module.InvokeVoidAsync("scrollToTop", _d.I.ClassName);
        }

        public string PageTitle(string section)
            => section switch
            {
                "" => "Home loan and refinancing support | Just Budget Home Loans",
                "refinance" => "Refinance your home loan | Just Budget Home Loans",
                "buy-a-home" => "Buy a home | Just Budget Home Loans",
                "investment-property" => "Investment property loans | Just Budget Home Loans",
                "second-mortgages" => "Second mortgages | Just Budget Home Loans",
                "property-debt-consolidation" => "Property backed debt consolidation | Just Budget Home Loans",
                "about" => "About us | Just Budget Home Loans",
                "contact" => "Contact us | Just Budget Home Loans",
                _ => "Page not found | Just Budget Home Loans"
            };

        public string PageDescription(string section)
            => section switch
            {
                "" => "Explore refinancing, home purchase, investment property and other property lending options with Just Budget.",
                "refinance" => "Explore a different rate, repayment structure or loan features with support from a familiar team.",
                "buy-a-home" => "Understand your borrowing options and what happens from your first conversation through to settlement.",
                "investment-property" => "Explore lending for an investment property with a focus on structure, repayments and your wider financial position.",
                "second-mortgages" => "A second mortgage may provide a way to access funds while keeping an existing home loan in place. We will help you weigh the costs and risks.",
                "property-debt-consolidation" => "If you own property, consolidating debts into a home loan may change your repayments. Understand the total cost and security risk first.",
                "about" => "Learn about Just Budget Home Loans and our approach to supporting mortgage and property lending decisions.",
                "contact" => "Contact Just Budget Home Loans to discuss refinancing, a home purchase or other property lending options.",
                _ => "This page could not be found."
            };

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

public class Jbhl_P_RC
{
    public Initialise_HD Initialise_HD_ { get; set; }

    public Jbhl_P_RC(IServiceProvider serviceProvider)
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
                //------------------------------
                // Nothing to load
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
            ResponseData.RequestStatus.RequestHasResults = true;
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

public class Jbhl_P_S
{
    public RootClass Root { get; private set; }
    public RefreshClass Refresh { get; private set; }

    public Jbhl_P_S()
    {
        Root = new RootClass();
        Refresh = new RefreshClass();
    }

    public class RootClass
    {
        public RootClass()
        {
            CurrentSection = new(nameof(CurrentSection), string.Empty, GetType().Name);
            PageClicks = new(nameof(PageClicks), 0, GetType().Name);
        }

        public ObservableProperty<string> CurrentSection { get; set; }
        public ObservableProperty<int> PageClicks { get; set; }
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
        Root.CurrentSection.Value = string.Empty;
        Root.PageClicks.Value = 0;
        Refresh = new();
    }
}
