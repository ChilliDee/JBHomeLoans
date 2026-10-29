using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;

namespace FE.Components.Domains.Pages.Demo.A;

public partial class Demo_P_C : ComponentBase, IDisposable
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

    private Data _i;
    private Navigation _nav;
    private StateSub _stateSub;
    private Switch _switch;
    private Inputs _inputs;
    private Request _req;
    private Dependencies _d;
    private Action _act;
    private State _state;
    private Demo_P_RC _rc_internal;

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
            // Pre-populated from mock data
            public List<Demo_P_RC.MockNature> Natures { get; set; } = new();
            // Pre-populated from mock data
            public List<Demo_P_RC.MockTicket> Tickets { get; set; } = new();
            // Left blank on load (optional, no validation)
            public string? Search { get; set; }
        }
    }

    public class Dependencies
    {
        public Data I;
        public Demo_P_RC RC_Internal;
        public Func<Task> StateUpdate;
        public NavigationManager NavManager;
        public List<Task> PendingOperations = new();
        public Demo_P_S S_Internal;

        public Dependencies(Data data,
                            Demo_P_RC internal_RC,
                            Func<Task> stateUpdate,
                            NavigationManager navManager,
                            IServiceProvider serviceProvider)
        {
            RC_Internal = internal_RC;
            StateUpdate = stateUpdate;
            NavManager = navManager;
            I = data;
            S_Internal = serviceProvider.GetRequiredService<Demo_P_S>();
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

        public async Task LogAttemptAsync(int ticketId, int? currentAttempt)
        {
            await _d.RC_Internal.Update_HD_.ExecuteAsync(new()
            {
                TicketId = ticketId,
                Kind = "attempt",
                CurrentAttempt = currentAttempt,
                StateUpdate = _d.StateUpdate
            });
        }

        public async Task CloseTicketAsync(int ticketId, int? currentAttempt)
        {
            await _d.RC_Internal.Update_HD_.ExecuteAsync(new()
            {
                TicketId = ticketId,
                Kind = "close",
                CurrentAttempt = currentAttempt,
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
            _d.I.Inputs.Natures = _d.RC_Internal.Initialise_HD_.ResponseData.Natures;
            _d.I.Inputs.Tickets = _d.RC_Internal.Initialise_HD_.ResponseData.Tickets;
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
            _d.S_Internal.Root.SelectedTicketId.NoDataSubscribeAsync(Refresh, _d.I.ClassName);
            _d.S_Internal.Root.NatureFilter.NoDataSubscribeAsync(Refresh, _d.I.ClassName);
        }

        public void UnSubscribe()
        {
            _d.S_Internal.Root.SelectedTicketId.NoDataUnSubscribeAsync(Refresh);
            _d.S_Internal.Root.NatureFilter.NoDataUnSubscribeAsync(Refresh);
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

        public void SelectTicket(int id)
        {
            _d.S_Internal.Root.SelectedTicketId.Value = id;
        }

        public void SetNatureFilter(string key)
        {
            _d.S_Internal.Root.NatureFilter.Value = key;
        }

        public void ShowAllNatures()
        {
            _d.S_Internal.Root.NatureFilter.Value = "all";
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

        public List<Demo_P_RC.MockTicket> VisibleTickets()
        {
            var nature = _d.S_Internal.Root.NatureFilter.Value;
            var search = _d.I.Inputs.Search?.Trim();

            return _d.I.Inputs.Tickets
                .Where(t => nature == "all" || t.NatureKey == nature)
                .Where(t => string.IsNullOrEmpty(search)
                         || t.Objective.Contains(search, StringComparison.OrdinalIgnoreCase)
                         || t.Client.Contains(search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.DueAt)
                .ToList();
        }

        public Demo_P_RC.MockTicket? SelectedTicket()
        {
            var id = _d.S_Internal.Root.SelectedTicketId.Value;
            return _d.I.Inputs.Tickets.FirstOrDefault(t => t.Id == id);
        }

        public string NatureName(string key)
            => _d.I.Inputs.Natures.FirstOrDefault(n => n.Key == key)?.Name ?? key;

        public string NatureBlurb(string key)
            => _d.I.Inputs.Natures.FirstOrDefault(n => n.Key == key)?.Blurb ?? string.Empty;

        public int CountNature(string key)
            => key == "all"
                ? _d.I.Inputs.Tickets.Count
                : _d.I.Inputs.Tickets.Count(t => t.NatureKey == key);

        public int CountOverdue()
            => _d.I.Inputs.Tickets.Count(t => t.DueAt < DateTime.Now);

        public int CountDueToday()
            => _d.I.Inputs.Tickets.Count(t => t.DueAt.Date == DateTime.Today && t.DueAt >= DateTime.Now);

        public string DueLabel(DateTime due)
        {
            var days = (due.Date - DateTime.Today).Days;

            if (days < 0) return $"Overdue {Math.Abs(days)}d";
            if (days == 0 && due < DateTime.Now) return "Overdue today";
            if (days == 0) return $"Today {due:h:mm tt}";
            if (days == 1) return "Tomorrow";
            return $"In {days} days";
        }

        public bool IsUpdating()
            => _d.RC_Internal.Update_HD_.ResponseData.RequestStatus.RequestIsLoading;

        public bool HasUpdateFailed()
            => _d.RC_Internal.Update_HD_.ResponseData.RequestStatus.RequestHasFailed;

        public void ApplyUpdateResponse()
        {
            var response = _d.RC_Internal.Update_HD_.ResponseData;
            if (!response.RequestStatus.RequestHasCompleted) return;

            if (response.Closed)
            {
                _d.I.Inputs.Tickets.RemoveAll(t => t.Id == response.TicketId);
                return;
            }

            var ticket = _d.I.Inputs.Tickets.FirstOrDefault(t => t.Id == response.TicketId);
            if (ticket == null) return;

            ticket.Attempt = response.NewAttempt;
            ticket.DueAt = response.NewDueAt ?? ticket.DueAt;
            ticket.Activity.Insert(0, new("Just now", $"Attempt {response.NewAttempt} logged, next check pushed out"));
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

        public string NatureChip(string key)
        {
            var baseClass = "inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-inset";

            return key switch
            {
                "us"       => $"{baseClass} bg-rose-50 text-rose-700 ring-rose-200",
                "client"   => $"{baseClass} bg-amber-50 text-amber-800 ring-amber-200",
                "third"    => $"{baseClass} bg-violet-50 text-violet-700 ring-violet-200",
                "internal" => $"{baseClass} bg-indigo-50 text-indigo-700 ring-indigo-200",
                _          => $"{baseClass} bg-gray-50 text-gray-700 ring-gray-200"
            };
        }

        public string NatureDot(string key)
        {
            var baseClass = "h-2 w-2 shrink-0 rounded-full";

            return key switch
            {
                "us"       => $"{baseClass} bg-rose-500",
                "client"   => $"{baseClass} bg-amber-500",
                "third"    => $"{baseClass} bg-violet-500",
                "internal" => $"{baseClass} bg-indigo-500",
                _          => $"{baseClass} bg-gray-400"
            };
        }

        public string FilterChip(string key)
        {
            var baseClass = "inline-flex items-center gap-2 rounded-lg px-3 py-1.5 text-xs font-medium transition-colors cursor-pointer";

            return _d.S_Internal.Root.NatureFilter.Value == key
                ? $"{baseClass} bg-indigo-600 text-white hover:bg-indigo-500"
                : $"{baseClass} bg-white text-gray-700 ring-1 ring-inset ring-gray-200 hover:bg-gray-50";
        }

        public string FilterCount(string key)
        {
            var baseClass = "rounded-full px-1.5 py-0.5 text-[10px] font-semibold";

            return _d.S_Internal.Root.NatureFilter.Value == key
                ? $"{baseClass} bg-indigo-500 text-white"
                : $"{baseClass} bg-gray-100 text-gray-600";
        }

        public string RowClass(int id)
        {
            var baseClass = "flex w-full items-center gap-4 border-b border-gray-100 px-4 py-3 text-left transition-colors cursor-pointer";

            return _d.S_Internal.Root.SelectedTicketId.Value == id
                ? $"{baseClass} bg-indigo-50"
                : $"{baseClass} hover:bg-gray-50";
        }

        public string DueBadge(DateTime due)
        {
            var baseClass = "inline-flex items-center rounded-md px-2 py-0.5 text-xs font-medium ring-1 ring-inset";

            if (due < DateTime.Now) return $"{baseClass} bg-red-50 text-red-700 ring-red-200";
            if (due.Date == DateTime.Today) return $"{baseClass} bg-amber-50 text-amber-800 ring-amber-200";
            return $"{baseClass} bg-gray-50 text-gray-700 ring-gray-200";
        }
    }
    // ==============================================================================================================================
    // ==============================================================================================================================
}

public class Demo_P_RC
{
    public Initialise_HD Initialise_HD_ { get; set; }
    public Update_HD Update_HD_ { get; set; }

    public Demo_P_RC(IServiceProvider serviceProvider)
    {
        Initialise_HD_ = new Initialise_HD(serviceProvider);
        Update_HD_ = new Update_HD(serviceProvider);
    }

    // Mock shapes. Every value is placeholder data: no real clients, staff or amounts.
    public class MockNature
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Blurb { get; set; } = string.Empty;
    }

    public class MockTicket
    {
        public int Id { get; set; }
        public string NatureKey { get; set; } = string.Empty;
        public string Objective { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string Assignee { get; set; } = string.Empty;
        public DateTime DueAt { get; set; }
        public int? Attempt { get; set; }
        public int? AttemptMax { get; set; }
        public string? ReplacesStatus { get; set; }
        public string Note { get; set; } = string.Empty;
        public List<MockActivity> Activity { get; set; } = new();
    }

    public record MockActivity(string When, string Text);

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
                GetMockNatures();
                await GetMockTicketsAsync();
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

        private void GetMockNatures()
        {
            _i.Natures = new()
            {
                new() { Key = "us",       Name = "Client waiting on us",   Blurb = "The client expects to hear from us. Overdue means they have waited too long." },
                new() { Key = "client",   Name = "Waiting on client",      Blurb = "We are chasing the client. Each attempt pushes the next check out." },
                new() { Key = "third",    Name = "Waiting on third party", Blurb = "A creditor or lender owes us a reply. We chase them on a schedule." },
                new() { Key = "internal", Name = "Internal work",          Blurb = "Our team's move. Nobody outside is waiting yet." }
            };
        }

        private async Task GetMockTicketsAsync()
        {
            await Task.Delay(600); // simulate a request so the loading state is visible

            _i.Tickets = new()
            {
                new()
                {
                    Id = 1, NatureKey = "us", Objective = "Send account statement", Stage = "Client request",
                    Client = "Mock client A", Assignee = "Mock user 1", DueAt = DateTime.Today.AddDays(1).AddMinutes(-30),
                    Note = "Client asked for a statement by email. Nothing has been sent yet.",
                    Activity = new() { new("2 hours ago", "Client requested a statement"), new("2 hours ago", "Ticket created") }
                },
                new()
                {
                    Id = 2, NatureKey = "us", Objective = "Call client for assessment", Stage = "Assessment",
                    Client = "Mock client B", Assignee = "Mock user 2", DueAt = DateTime.Today.AddDays(-1).AddHours(15),
                    ReplacesStatus = "New Application",
                    Note = "New application received. The assessment call has not happened yet.",
                    Activity = new() { new("Yesterday", "Application received"), new("Yesterday", "Ticket created") }
                },
                new()
                {
                    Id = 3, NatureKey = "client", Objective = "Collect documents", Stage = "Checklist",
                    Client = "Mock client C", Assignee = "Mock user 1", DueAt = DateTime.Today.AddDays(1).AddHours(10),
                    Attempt = 3, AttemptMax = 7,
                    ReplacesStatus = "Checklist Follow Up 1 to 6, Follow Up Final",
                    Note = "Three of five documents still outstanding.",
                    Activity = new() { new("2 days ago", "Reminder 2 sent"), new("4 days ago", "Reminder 1 sent"), new("6 days ago", "Checklist sent") }
                },
                new()
                {
                    Id = 4, NatureKey = "client", Objective = "Get application pack back", Stage = "Application",
                    Client = "Mock client D", Assignee = "Mock user 2", DueAt = DateTime.Today.AddDays(-2).AddHours(9),
                    Attempt = 1, AttemptMax = 3,
                    ReplacesStatus = "Application Pack Sent",
                    Note = "Pack was sent. No reply yet.",
                    Activity = new() { new("3 days ago", "Application pack sent") }
                },
                new()
                {
                    Id = 5, NatureKey = "client", Objective = "Re-engage client", Stage = "Retention",
                    Client = "Mock client E", Assignee = "Mock user 3", DueAt = DateTime.Today.AddDays(5).AddHours(11),
                    Attempt = 1, AttemptMax = 3,
                    ReplacesStatus = "Sales To Re-engage 1, 2, 3",
                    Note = "Client went quiet after the first call.",
                    Activity = new() { new("Today", "First re-engage attempt logged") }
                },
                new()
                {
                    Id = 6, NatureKey = "third", Objective = "Send SOFP to creditors", Stage = "Statement of financial position",
                    Client = "Mock client F", Assignee = "Mock user 2", DueAt = DateTime.Today.AddDays(3).AddHours(14),
                    ReplacesStatus = "Statement of Financial Position Sent to Creditors",
                    Note = "Waiting for two creditors to acknowledge receipt.",
                    Activity = new() { new("Yesterday", "SOFP sent to 2 creditors") }
                },
                new()
                {
                    Id = 7, NatureKey = "internal", Objective = "Prepare SOFP", Stage = "Statement of financial position",
                    Client = "Mock client G", Assignee = "Mock user 1", DueAt = DateTime.Today.AddDays(1).AddMinutes(-90),
                    ReplacesStatus = "Preparing Statement Of Financial Position",
                    Note = "Income and expense details are complete. Ready to draft.",
                    Activity = new() { new("This morning", "Assessment completed"), new("This morning", "Ticket created") }
                },
                new()
                {
                    Id = 8, NatureKey = "internal", Objective = "Assess income and bank statements", Stage = "Assessment",
                    Client = "Mock client H", Assignee = "Mock user 3", DueAt = DateTime.Today.AddDays(2).AddHours(16),
                    ReplacesStatus = "Assesing Income & Bank Statements",
                    Note = "Statements arrived. Needs checking before the next step.",
                    Activity = new() { new("Today", "Statements received"), new("Today", "Ticket created") }
                }
            };
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
            bool requestHasResults = _i.Tickets.Any();
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
            ResponseData.Natures = _i.Natures;
            ResponseData.Tickets = _i.Tickets;
        }

        public class Data
        {
            public List<MockNature> Natures { get; set; } = new();
            public List<MockTicket> Tickets { get; set; } = new();
        }

        public class DTOResponse : ST_DTOResponse
        {
            public List<MockNature> Natures { get; set; } = new();
            public List<MockTicket> Tickets { get; set; } = new();
        }

        public class DTORequest
        {
            public int Id { get; set; }
            public Func<Task> StateUpdate { get; set; }
            public int? DebounceTime { get; set; }
        }
    }

    public class Update_HD : ST_Subscriptions<Update_HD.DTOResponse>
    {
        private CancellationTokenSource? _debounceCancellationTokenSource;

        public Update_HD(IServiceProvider serviceProvider) : base(new DTOResponse(), serviceProvider)
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
                await SimulateUpdateAsync();
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

        private async Task SimulateUpdateAsync()
        {
            await Task.Delay(400); // simulate a request

            _i.TicketId = _req.TicketId;
            _i.Closed = _req.Kind == "close";
            _i.NewAttempt = (_req.CurrentAttempt ?? 0) + 1;
            _i.NewDueAt = DateTime.Today.AddDays(2).AddHours(9);
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
            ResponseData.RequestStatus.RequestHasResults = _i.TicketId != 0;
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
            ResponseData.TicketId = _i.TicketId;
            ResponseData.Closed = _i.Closed;
            ResponseData.NewAttempt = _i.NewAttempt;
            ResponseData.NewDueAt = _i.NewDueAt;
        }

        public class Data
        {
            public int TicketId { get; set; }
            public bool Closed { get; set; }
            public int? NewAttempt { get; set; }
            public DateTime? NewDueAt { get; set; }
        }

        public class DTOResponse : ST_DTOResponse
        {
            public int TicketId { get; set; }
            public bool Closed { get; set; }
            public int? NewAttempt { get; set; }
            public DateTime? NewDueAt { get; set; }
        }

        public class DTORequest
        {
            public int Id { get; set; }
            public int TicketId { get; set; }
            public string Kind { get; set; } = string.Empty;
            public int? CurrentAttempt { get; set; }
            public Func<Task> StateUpdate { get; set; }
            public int? DebounceTime { get; set; }
        }
    }
}

public class Demo_P_S
{
    public RootClass Root { get; private set; }
    public RefreshClass Refresh { get; private set; }

    public Demo_P_S()
    {
        Root = new RootClass();
        Refresh = new RefreshClass();
    }

    public class RootClass
    {
        public RootClass()
        {
            SelectedTicketId = new(nameof(SelectedTicketId), 0, GetType().Name);
            NatureFilter = new(nameof(NatureFilter), "all", GetType().Name);
        }

        public ObservableProperty<int> SelectedTicketId { get; set; }
        public ObservableProperty<string> NatureFilter { get; set; }
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
        Root.SelectedTicketId.Value = 0;
        Root.NatureFilter.Value = "all";
        Refresh = new();
    }
}
