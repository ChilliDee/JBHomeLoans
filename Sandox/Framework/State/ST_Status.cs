namespace Mortgage.State
{
    public class ST_Status
    {
        public bool RequestPrerequisetsMet { get;  set; } = false;
        public bool RequestAttempted { get; set; } = false;
        public bool RequestHasFailed { get; set; } = false;
        public bool RequestHasResults { get;  set; } = false;
        public bool RequestHasCompleted { get; set; } = false;        
        public bool RequestIsLoading { get; set; } = false;
        public int RequestStage { get; set; } = 0;
        public bool InitialLoadCompletedSuccessfully { get; set; }

        public bool RequestHasCompletedWithResults
        {
            get
            {
                return RequestHasCompleted && RequestHasResults;
            }
        }      
        
        public bool RequestHasCompletedWithoutResults
        {
            get
            {
                return RequestHasCompleted && !RequestHasResults;
            }
        }          
    }
}