namespace Mortgage.State;

// Local Sandox version of the CRM framework class of the same name (no BusinessLogic.Core package).

public class ST_Status
{
    public bool RequestPrerequisetsMet { get; set; }
    public bool RequestAttempted { get; set; }
    public bool RequestHasFailed { get; set; }
    public bool RequestHasResults { get; set; }
    public bool RequestHasCompleted { get; set; }
    public bool RequestIsLoading { get; set; }
    public int RequestStage { get; set; }
    public bool InitialLoadCompletedSuccessfully { get; set; }

    public bool RequestHasCompletedWithResults => RequestHasCompleted && RequestHasResults;
    public bool RequestHasCompletedWithoutResults => RequestHasCompleted && !RequestHasResults;
}

public class ST_DTOResponse
{
    public ST_Status RequestStatus { get; set; } = new();
    public bool HasBeenDebounced { get; set; }
    public string? ErrorMessage { get; set; }
}
