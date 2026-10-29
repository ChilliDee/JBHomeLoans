using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;

namespace FE.Components.Domains.Utlility.Logo.MM
{
    public partial class MMLogo : ComponentBase, IDisposable
    {
        // PARAMS ==============================================
        // ======================================================

        [Inject]
        private IServiceProvider _serviceProvider { get; set; }
        [Inject]
        private NavigationManager _navManager { get; set; }
        [Inject]
        private InterceptorErrorHandlingFactory _interceptorErrorHandlingFactory { get; set; }
        [Inject]
        private IJSRuntime _js { get; set; }

        private Data _i;
        private Navigation _nav;
        private StateSub _stateSub;
        private Switch _switch;
        private Inputs _inputs;
        private Request _req;
        private Dependencies _d;
        private Action _act;
        private State _state;

        protected override async Task OnInitializedAsync()
        {
            _i = new();
            _d = new(_i, async () => await InvokeAsync(StateHasChanged), _navManager, _serviceProvider, _js);

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
            await _d.StateUpdate();
        }

        public async void Dispose()
        {
            _stateSub.UnSubscribe();
            await _act.CompletePendingOperationsAsync();
        }

        // SET UP =====================================================================================================================
        public class Data
        {
            public string ClassName => GetType().DeclaringType?.Name;
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
            public Func<Task> StateUpdate;
            public NavigationManager NavManager;
            public List<Task> PendingOperations = new();
            public IJSRuntime JS;

            public Dependencies(Data data,
                                Func<Task> stateUpdate,
                                NavigationManager navManager,
                                IServiceProvider serviceProvider,
                                IJSRuntime js)
            {
                StateUpdate = stateUpdate;
                NavManager = navManager;
                I = data;
                JS = js;
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
            }

            public void UnSubscribe()
            {
            }
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

            public async Task ReplayAsync()
            {
                await _d.JS.InvokeVoidAsync("mmLogoReplay");
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
    }
}