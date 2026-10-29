namespace Mortgage.State
{
    public class ST_DTOResponse
    {
        public ST_Status RequestStatus { get; set; } = new();
        public bool HasBeenDebounced { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}