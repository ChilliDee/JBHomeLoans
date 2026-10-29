using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;
using Mortgage.State;


namespace FE.Components.Domains.Utlility.Modal
{

    public partial class CautionModal_C : ComponentBase, IDisposable
    {

        [Inject]
        private NavigationManager _navManager { get; set; }
        [Inject]
        private CautionModal_S _s_internal { get; set; }
        [Inject]
        private InterceptorErrorHandlingFactory _InterceptorErrorHandlingFactory { get; set; }

        private Data _i;
        private Navigation _nav;
        private StateSub _stateSub;
        private Switch _switch;
        private Condition _con;
        private Request _req;
        private Dependencies _d;
        private Action _act;
        private State _state;

        protected override async Task OnInitializedAsync()
        {
            _i = new();
            _d = new(_i, _s_internal, async () => await InvokeAsync(StateHasChanged), _navManager);

            _req = _InterceptorErrorHandlingFactory.Create<Request, Dependencies>(typeof(Request), _d, new(_d), _d.I.ClassName);
            _stateSub = _InterceptorErrorHandlingFactory.Create<StateSub, Dependencies>(typeof(StateSub), _d, new(_d), _d.I.ClassName);
            _nav = _InterceptorErrorHandlingFactory.Create<Navigation, Dependencies>(typeof(Navigation), _d, new(_d), _d.I.ClassName);
            _con = _InterceptorErrorHandlingFactory.Create<Condition, Dependencies>(typeof(Condition), _d, new(_d), _d.I.ClassName);
            _act = _InterceptorErrorHandlingFactory.Create<Action, Dependencies>(typeof(Action), _d, new(_d), _d.I.ClassName);
            _state = _InterceptorErrorHandlingFactory.Create<State, Dependencies>(typeof(State), _d, new(_d), _d.I.ClassName);
            _switch = new(_d);
            await SetupAsync();
        }

        private async Task SetupAsync()
        {
            _stateSub.Subscribe();
            // await _req.InitialRequestAsync();
        }

        public void Dispose()
        {
            _stateSub.UnSubscribe();
            _d.S_Internal.Reset();      
        }

        // SET UP =====================================================================================================================
        public class Data
        {
            public string ClassName => this.GetType().DeclaringType?.Name;
        }

        public class Dependencies
        {
            public Data I;
            public Func<Task> StateUpdate;
            public NavigationManager NavManager;
            public CautionModal_S S_Internal;

            public Dependencies(Data data,
                                CautionModal_S s_Internal,
                                Func<Task> stateUpdate,
                                NavigationManager navManager)
            {
                StateUpdate = stateUpdate;
                NavManager = navManager;
                I = data;
                S_Internal = s_Internal;
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

        public class StateSub
        {
            private readonly Dependencies _d;

            public StateSub(Dependencies dependecies)
            {
                _d = dependecies;
            }

            public void Subscribe()
            {
                _d.S_Internal.Root.ShowModal.NoDataSubscribeAsync(Refresh, _d.I.ClassName); 
            }


            public void UnSubscribe()
            {
                _d.S_Internal.Root.ShowModal.NoDataUnSubscribeAsync(Refresh);

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

            public void ToggleModalDisplay()
            {
                _d.S_Internal.Root.ShowModal.Value = !_d.S_Internal.Root.ShowModal.Value;                     
            }

            public void RemoveModalOptions()
            {
                _d.S_Internal.ModalOptions.Title.Value = "";
                _d.S_Internal.ModalOptions.Message.Value = "";
                _d.S_Internal.ModalOptions.AsyncCallback.Value = null;
                _d.S_Internal.ModalOptions.PostCallBack.Value = null;        
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



        public class Condition
        {
            private readonly Dependencies _d;

            public Condition(Dependencies dependecies)
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

            public async Task ExecuteAsyncCallBackAsync()
            {
                if(_d.S_Internal.ModalOptions.AsyncCallback.Value != null)
                {
                    await _d.S_Internal.ModalOptions.AsyncCallback.Value.Invoke();          
                }
            }

            public void ExecutePostCallBack()
            {
                if (_d.S_Internal.ModalOptions.PostCallBack.Value != null)
                {
                    _d.S_Internal.ModalOptions.PostCallBack.Value.Invoke();
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


            public string ShowModal()
            {
                if (_d.S_Internal.Root.ShowModal.Value)
                {
                    return "";
                }
                else
                {
                    return "hidden";    
                }
            }
        }
        // ==============================================================================================================================
        // ==============================================================================================================================
    }

    public class CautionModal_S
    {

        public RootClass Root { get; private set; }
        public ModalOptionsClass ModalOptions { get; private set; }

        public CautionModal_S()
        {
            Root = new RootClass();
            ModalOptions = new ModalOptionsClass();     
        }

        public class RootClass
        {
            public RootClass()
            {
                ShowModal = new(nameof(ShowModal), false, GetType().Name);
            }

            public ObservableProperty<bool> ShowModal { get; set; }
        }

        public class ModalOptionsClass
        {
            public ModalOptionsClass()
            {
                Title = new(nameof(Title), "Caution", GetType().Name);
                Message = new(nameof(Message), "Are you sure you want to proceed?", GetType().Name);
                AsyncCallback = new(nameof(AsyncCallback), default, GetType().Name);
                PostCallBack = new(nameof(PostCallBack), default, GetType().Name);
            }
            public ObservableProperty<string> Title { get; set; }
            public ObservableProperty<string> Message { get; set; }
            public ObservableProperty<Func<Task>> AsyncCallback { get; set; }
            public ObservableProperty<Action> PostCallBack { get; set; }

        }

        public void Reset()
        {
            Root = new();
            ModalOptions = new();
        }
    }
}
