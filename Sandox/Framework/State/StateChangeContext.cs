namespace Mortgage.State;

// Local Sandox version of the CRM framework class of the same name (no BusinessLogic.Core package).
// Passed to every ObservableProperty subscriber: says which state change triggered the callback.
public class StateChangeContext
{
    private readonly string _className;

    public StateChangeContext(string className)
    {
        _className = className;
    }

    public List<string> CallHierarchy { get; private set; } = new();

    public string GetHierarchyString => string.Join(" -> ", CallHierarchy);

    public int Depth => CallHierarchy.Count;

    public void AddToChain(string propertyName)
    {
        CallHierarchy.Add($"{_className}.{propertyName}");
    }
}
