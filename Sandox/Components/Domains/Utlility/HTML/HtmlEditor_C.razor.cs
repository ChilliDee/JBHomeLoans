//using BusinessLogic.Core.Logging.CoreErrorLogger.Models.Exceptions;
//using BusinessLogic.Core.Logging.FrontEndLogger.Builder;
//using BusinessLogic.Core.Logging.FrontEndLogger.Models.Exceptions;
//using Microsoft.AspNetCore.Components;
//using Mortgage.Interceptors.ErrorHandling.Factory;
//using Mortgage.State;
//using BusinessLogic.Core.UnitOfWork;
//using Blazored.TextEditor;

//namespace FE.Components.Domains.Utlility.HTML
//{



//	public partial class HtmlEditor_C : ComponentBase, IDisposable
//	{

//		/*

//		<div id="@_d.I.ClassName"></div>

//		*/
//		// PARAMS ==============================================
//		/*[Parameter]
//        public int Id { get; set; }*/
//		// ======================================================

//        [Parameter]
//        public string Html { get; set; }
//		[Inject]
//		private IServiceProvider _serviceProvider { get; set; }
//		[Inject]
//		private NavigationManager _navManager { get; set; }
//		[Inject]
//		private InterceptorErrorHandlingFactory _interceptorErrorHandlingFactory { get; set; }

//        BlazoredTextEditor QuillHtml;
//        // [Inject]
//        // private HtmlEditor_S _s_internal { get; set; }

//        private Data _i;
//		private Navigation _nav;
//		private StateSub _stateSub;
//		private Switch _switch;
//		private Inputs _inputs;
//		private Request _req;
//		private Dependencies _d;
//		private Action _act;
//		private State _state;
//		private HtmlEditor_RC _rc_internal;

//		protected override async Task OnInitializedAsync()
//		{
//			_i = new();
//			_rc_internal = new(_serviceProvider);
//			_d = new(_i, _rc_internal, async () => await InvokeAsync(StateHasChanged), _navManager, QuillHtml, Html);

//			_req = _interceptorErrorHandlingFactory.Create<Request, Dependencies>(typeof(Request), _d, new(_d), _d.I.ClassName);
//			_stateSub = _interceptorErrorHandlingFactory.Create<StateSub, Dependencies>(typeof(StateSub), _d, new(_d), _d.I.ClassName);
//			_nav = _interceptorErrorHandlingFactory.Create<Navigation, Dependencies>(typeof(Navigation), _d, new(_d), _d.I.ClassName);
//			_inputs = _interceptorErrorHandlingFactory.Create<Inputs, Dependencies>(typeof(Inputs), _d, new(_d), _d.I.ClassName);
//			_act = _interceptorErrorHandlingFactory.Create<Action, Dependencies>(typeof(Action), _d, new(_d), _d.I.ClassName);
//			_state = _interceptorErrorHandlingFactory.Create<State, Dependencies>(typeof(State), _d, new(_d), _d.I.ClassName);
//			_switch = _interceptorErrorHandlingFactory.Create<Switch, Dependencies>(typeof(Switch), _d, new(_d), _d.I.ClassName);
//		}



//		protected override async Task OnAfterRenderAsync(bool firstRender)
//		{
//			if (firstRender && !_d.I.DidFirstRender)
//			{
//				_d.I.DidFirstRender = true;
//				await SetupAsync();
//				_d.I.InitialisationCompleted = true;
//				await _d.StateUpdate();
//			}
//		}

//		private async Task SetupAsync()
//		{
//			_stateSub.Subscribe();
//			await _act.InjectHtmlAsync();
//			//            await _req.InitialRequestAsync();
//			//            _inputs.SetInitialInputs();
//			await _d.StateUpdate();

//		}

//		public async void Dispose()
//		{
//			_stateSub.UnSubscribe();
//			await _act.CompletePendingOperationsAsync();


//		}

//		// SET UP =====================================================================================================================
//		public class Data
//		{
//			public string ClassName => this.GetType().DeclaringType?.Name;
//			public InputsClass Inputs { get; set; } = new();
//			public bool DidFirstRender { get; internal set; }
//			public bool InitialisationCompleted { get; internal set; } = false;


//			public class InputsClass
//			{
//			}
//		}

//		public class Dependencies
//		{
//			public Data I;
//			public HtmlEditor_RC RC_Internal;
//			public Func<Task> StateUpdate;
//			public NavigationManager NavManager;
//			public List<Task> PendingOperations = new();
//			public HtmlEditor_S S_Internal;

//			public Dependencies(Data data,
//								HtmlEditor_RC internal_RC,
//								Func<Task> stateUpdate,
//								NavigationManager navManager,
//                                BlazoredTextEditor quillHtml,
//                                string html)
//			{
//				RC_Internal = internal_RC;
//				StateUpdate = stateUpdate;
//				NavManager = navManager;
//                QuillHtml = quillHtml;
//                Html = html;
//                I = data;
//			}

//            public BlazoredTextEditor QuillHtml { get; set; }
//            public string Html { get; set; }
//        }
//		// =============================================================================================================================
//		// CLASSES =====================================================================================================================
//		public class Request
//		{
//			private Dependencies _d;

//			public Request(Dependencies d)
//			{
//				_d = d;
//			}

//			/*   public async Task InitialRequestAsync()
//			   {
//			   } */
//		}
//		public class Inputs
//		{
//			private readonly Dependencies _d;

//			public Inputs(Dependencies d)
//			{
//				_d = d;
//			}

//			public void SetInitialInputs()
//			{
//			}

//		}

//		public class StateSub
//		{
//			private readonly Dependencies _d;

//			public StateSub(Dependencies dependecies)
//			{
//				_d = dependecies;
//			}

//			public void Subscribe()
//			{

//			}

//			public void UnSubscribe()
//			{

//			}
//		}

//		public class State
//		{
//			private readonly Dependencies _d;

//			public State(Dependencies dependecies)
//			{
//				_d = dependecies;
//			}
//		}

//		public class Navigation
//		{
//			private readonly Dependencies _d;

//			public Navigation(Dependencies dependecies)
//			{
//				_d = dependecies;
//			}
//		}


//		public class Action
//		{
//			private Dependencies _d;
//			public Action(Dependencies d)
//			{
//				_d = d;
//			}

//			public async Task InjectHtmlAsync()
//			{
//				await _d.QuillHtml.LoadHTMLContent(_d.Html);
//			}
//            public async Task CompletePendingOperationsAsync()
//			{

//				if (_d.PendingOperations.Any())
//				{
//					try
//					{
//						await Task.WhenAll(_d.PendingOperations);
//					}
//					catch (Exception ex)
//					{
//					}
//				}
//			}
//		}

//		public class Switch
//		{
//			private readonly Dependencies _d;

//			public Switch(Dependencies d)
//			{
//				_d = d;
//			}
//		}
//		// ==============================================================================================================================
//		// ==============================================================================================================================
//	}

//	public class HtmlEditor_RC
//	{
//		public Initialise_HD Initialise_HD_ { get; set; }
//		public HtmlEditor_RC(IServiceProvider serviceProvider)
//		{
//			Initialise_HD_ = new Initialise_HD(serviceProvider);
//		}

//		public class Initialise_HD : ST_Subscriptions<Initialise_HD.DTOResponse>
//		{
//			private CancellationTokenSource? _debounceCancellationTokenSource;
//			private IFrontEndErrorLog_B _frontEndErrorLog_B;
//			private IUnitOfWork _unitOfWork;

//			public Initialise_HD(IServiceProvider serviceProvider) : base(new DTOResponse(), serviceProvider)
//			{
//			}

//			protected override void CreateScope()
//			{
//				_serviceScope = _serviceProvider.CreateScope();
//				_frontEndErrorLog_B = _serviceScope.ServiceProvider.GetRequiredService<IFrontEndErrorLog_B>();
//				_unitOfWork = _serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
//			}


//			private Data _i;

//			private DTORequest _req;

//			public async Task ExecuteAsync(DTORequest requestData)
//			{
//				try
//				{
//					_req = requestData;
//					_i = new();
//					await DebouncePreviousOperationAsync();
//					CreateScope();
//					SetRequestLoading();
//					await StateUpdateAsync();
//					// add your code here
//					//------------------------------
//					PrepareResponseData();
//					SetRequestSucceeded();
//					SetRequestHasResults();

//				}
//				catch (CoreException ex)
//				{
//					ex.AddToClassStack($"{GetType().DeclaringType.Name}.{GetType().Name}", nameof(_req.Id), _req.Id.ToString());
//					var frontEndErrorLog = new FrontEndException(ex);
//					await _frontEndErrorLog_B.ExecuteAsync(new()
//					{
//						Exception = frontEndErrorLog
//					});
//					SetRequestFailed();
//				}
//				catch (TaskCanceledException ex)
//				{
//					return;
//				}
//				catch (Exception ex)
//				{
//					var frontEndErrorLog = new FrontEndException(ex.Message, ex.StackTrace, $"{GetType().DeclaringType.Name}.{GetType().Name}", nameof(_req.Id), _req.Id.ToString());

//					await _frontEndErrorLog_B.ExecuteAsync(new()
//					{
//						Exception = frontEndErrorLog
//					});
//					SetRequestFailed();
//				}
//				finally
//				{
//					DisposeScope();
//				}

//			}



//			private async Task DebouncePreviousOperationAsync()
//			{
//				if (_req.DebounceTime != null)
//				{
//					// Cancel any existing debounce operation
//					ResponseData.HasBeenDebounced = true;
//					_debounceCancellationTokenSource?.Cancel();
//					_debounceCancellationTokenSource = new CancellationTokenSource();

//					await Task.Delay((int)_req.DebounceTime, _debounceCancellationTokenSource.Token);
//				}
//			}

//			protected override void SetRequestHasResults()
//			{
//				/*
//				bool requestHasResults = _i.
//				ResponseData.RequestStatus.RequestHasResults = requestHasResults;
//				*/
//			}

//			private async Task StateUpdateAsync()
//			{
//				if (_req.StateUpdate != null)
//				{
//					await _req.StateUpdate.Invoke();
//				}
//			}

//			private void PrepareResponseData()
//			{
//				// ResponseData.
//			}



//			public class Data
//			{

//			}

//			public class DTOResponse : ST_DTOResponse
//			{

//			}

//			public class DTORequest
//			{
//				public int Id { get; set; }
//				public Func<Task> StateUpdate { get; set; }
//				public int DebounceTime { get; internal set; }
//			}
//		}
//	}

//	public class HtmlEditor_S
//	{

//		public RootClass Root { get; private set; }
//		public RefreshClass Refresh { get; private set; }

//		public HtmlEditor_S()
//		{
//			Root = new RootClass();
//			Refresh = new RefreshClass();
//		}

//		public class RootClass
//		{
//			public RootClass()
//			{
//				//   Placeholder = new(nameof(Placeholder), T ,  GetType().Name);
//			}

//			// public ObservableProperty<T> Placeholder { get; set; }
//		}

//		public class RefreshClass
//		{
//			public RefreshClass()
//			{
//				//   Placeholder = new(nameof(Placeholder), T ,  GetType().Name);
//			}

//			// public ObservableProperty<T> Placeholder { get; set; }
//		}

//		public void Reset()
//		{
//			Root = new();
//			Refresh = new();
//		}
//	}




//}
