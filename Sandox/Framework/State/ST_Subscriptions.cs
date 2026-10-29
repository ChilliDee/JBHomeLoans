using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Mortgage.State
{
    public abstract class ST_Subscriptions<T> where T : ST_DTOResponse, new()
    {
        //private List<Func<T, Task>> _dataCallbacks;
        //private Func<Task> _stateChangeCallBack;
        protected IServiceScope _serviceScope;
        protected IServiceProvider _serviceProvider;
        public T ResponseData { get; set; }


        public ST_Subscriptions(T responseData, IServiceProvider serviceProvider)
        {
            ResponseData = responseData;
            _serviceProvider = serviceProvider;
        }

        protected void SetRequestSucceeded()
        {
            ResponseData.RequestStatus.RequestIsLoading = false;
            ResponseData.RequestStatus.RequestHasCompleted = true;
            ResponseData.HasBeenDebounced = false;
            ResponseData.RequestStatus.InitialLoadCompletedSuccessfully = true;
        }

        protected void SetRequestFailed(string errorMessage = null)
        {
            ResponseData.RequestStatus.RequestIsLoading = false;
            ResponseData.RequestStatus.RequestHasFailed = true;
            ResponseData.ErrorMessage = errorMessage;   
        }

        protected void SetRequestLoading()
        {            
            ResponseData.RequestStatus.RequestIsLoading = true;
            ResponseData.RequestStatus.RequestHasFailed = false;
            ResponseData.RequestStatus.RequestHasResults = false;
            ResponseData.RequestStatus.RequestHasCompleted = false;
            ResponseData.ErrorMessage = string.Empty;       
        }

        protected abstract void SetRequestHasResults();

        protected abstract void CreateScope();

        protected void DisposeScope()
        {
            if (_serviceScope == null) return;
            _serviceScope.Dispose();
        }

        public async Task InvokeCallBackIfNoDebounceAsync(Func<Task> callback)
        {
            if (!ResponseData.HasBeenDebounced)
            {
                await callback();
            }
        }

        public void InvokeCallBackIfNoDebounce(Action callback)
        {
            if (!ResponseData.HasBeenDebounced)
            {
                callback();
            }
        }

        public void InvokeCallBackIfNoDebounce<T>(Action<T> callback, T param)
        {
            if (!ResponseData.HasBeenDebounced)
            {
                callback(param);
            }
        }

        public void InvokeCallBackIfCompleted(Action callback)
        {
            if (ResponseData.RequestStatus.RequestHasCompleted)
            {
                callback();
            }
        }

        public async Task InvokeCallBackIfCompletedAsync(Func<Task> callback)
        {
            if (ResponseData.RequestStatus.RequestHasCompleted)
            {
                await callback();
            }
        }

        public async Task InvokeSubscriptionCallbacksAsync()
        {  
        }
    }    
}