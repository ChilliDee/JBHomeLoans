namespace Mortgage.State
{
    public class StateChangeContext
    {
        private string _className;
        public List<string> CallHierarchy { get; private set; } = new();


        public StateChangeContext(string className)
        {
            _className = className;
        }

        //public StateChangeContext(string rootComponent)
        //{
        //    RootTrigger = rootComponent;
        //    InitiatedAt = DateTime.UtcNow;
        //    CallHierarchy.Add(rootComponent);
        //}

        //private StateChangeContext(string rootTrigger, DateTime initiatedAt, Guid changeId)
        //{
        //    RootTrigger = rootTrigger;
        //    InitiatedAt = initiatedAt;
        //    ChangeId = changeId;
        //}

        public void AddToChain(string propertyName)
        {
            var componentNameWithProperty = $"{_className}.{propertyName}";  
            CallHierarchy.Add(componentNameWithProperty);
        }

        public string GetHierarchyString { get => string.Join(" → ", CallHierarchy); }
        public int Depth => CallHierarchy.Count;
    }
}