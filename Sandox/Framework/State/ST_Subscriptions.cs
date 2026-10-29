namespace Mortgage.State;

// Local Sandox version of the CRM framework class of the same name (no BusinessLogic.Core package).
// Base class for every _RC sub-class: owns ResponseData and the request status flags.
public abstract class ST_Subscriptions<T> where T : ST_DTOResponse
{
    protected IServiceScope? _serviceScope;
    protected IServiceProvider _serviceProvider;

    public T ResponseData { get; set; }

    protected ST_Subscriptions(T responseData, IServiceProvider serviceProvider)
    {
        ResponseData = responseData;
        _serviceProvider = serviceProvider;
    }

    protected void SetRequestLoading()
    {
        ResponseData.ErrorMessage = null;
        ResponseData.RequestStatus.RequestAttempted = true;
        ResponseData.RequestStatus.RequestIsLoading = true;
        ResponseData.RequestStatus.RequestHasFailed = false;
        ResponseData.RequestStatus.RequestHasCompleted = false;
    }

    protected void SetRequestSucceeded()
    {
        ResponseData.RequestStatus.RequestIsLoading = false;
        ResponseData.RequestStatus.RequestHasFailed = false;
        ResponseData.RequestStatus.RequestHasCompleted = true;
        ResponseData.RequestStatus.InitialLoadCompletedSuccessfully = true;
    }

    protected void SetRequestFailed(string? errorMessage = null)
    {
        ResponseData.ErrorMessage = errorMessage;
        ResponseData.RequestStatus.RequestIsLoading = false;
        ResponseData.RequestStatus.RequestHasFailed = true;
        ResponseData.RequestStatus.RequestHasCompleted = false;
        ResponseData.RequestStatus.RequestHasResults = false;
    }

    protected abstract void SetRequestHasResults();

    protected abstract void CreateScope();

    protected void DisposeScope()
    {
        _serviceScope?.Dispose();
        _serviceScope = null;
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

    public void InvokeCallBackIfNoDebounce<TParam>(Action<TParam> callback, TParam param)
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

    // Kept so components copied from the CRM compile. Nothing subscribes to an RC in Sandox.
    public Task InvokeSubscriptionCallbacksAsync() => Task.CompletedTask;
}
