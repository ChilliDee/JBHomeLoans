using Microsoft.AspNetCore.Components;
using Mortgage.Interceptors.ErrorHandling.Factory;

namespace FE.Components.Domains.Utlility.Modal;

public partial class GenericModal_C : ComponentBase
{
    [Inject] private InterceptorErrorHandlingFactory _interceptorErrorHandlingFactory { get; set; }

    [Parameter] public string Title { get; set; }
    [Parameter] public string Width { get; set; }
    [Parameter] public string Height { get; set; }
    [Parameter] public bool ShowCloseButton { get; set; } = true;
    [Parameter] public bool FullScreen { get; set; } = false;
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    private Switch _switch;
    private Action _act;
    private Dependencies _d;

    protected override void OnInitialized()
    {
        var data = new Data
        {
            Title = Title,
            Width = Width,
            Height = Height,
            ShowCloseButton = ShowCloseButton,
            FullScreen = FullScreen,
            ChildContent = ChildContent
        };

        _d = new(data, async () => await InvokeAsync(StateHasChanged), OnClose);

        _act = _interceptorErrorHandlingFactory.Create<Action, Dependencies>(typeof(Action), _d, new(_d), data.ClassName);
        _switch = _interceptorErrorHandlingFactory.Create<Switch, Dependencies>(typeof(Switch), _d, new(_d), data.ClassName);
    }

    protected override void OnParametersSet()
    {
        _d.I.Title = Title;
        _d.I.Width = Width;
        _d.I.Height = Height;
        _d.I.ShowCloseButton = ShowCloseButton;
        _d.I.FullScreen = FullScreen;
        _d.I.ChildContent = ChildContent;
        _d.OnClose = OnClose;
    }

    // SET UP =========================================================================================================================
    public class Data
    {
        public string ClassName => GetType().DeclaringType?.Name;
        public string Title { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public bool ShowCloseButton { get; set; }
        public bool FullScreen { get; set; }
        public RenderFragment ChildContent { get; set; }
    }

    public class Dependencies
    {
        public Data I;
        public Func<Task> StateUpdate;
        public EventCallback OnClose;

        public Dependencies(Data data,
                            Func<Task> stateUpdate,
                            EventCallback onClose)
        {
            I = data;
            StateUpdate = stateUpdate;
            OnClose = onClose;
        }
    }
    // ================================================================================================================================
    // CLASSES ========================================================================================================================
    public class Action
    {
        private readonly Dependencies _d;
        public Action(Dependencies d) => _d = d;

        public async Task Close()
        {
            await _d.OnClose.InvokeAsync();
            await _d.StateUpdate();
        }
    }

    public class Switch
    {
        private readonly Dependencies _d;
        public Switch(Dependencies d) => _d = d;

        public string GetModalSizeCss()
        {
            if (_d.I.FullScreen)
                return "width: 100vw; height: 100vh;";

            var width = _d.I.Width != null ? $"width: {_d.I.Width}px;" : "";
            var height = _d.I.Height != null ? $"height: calc(100vh - {_d.I.Height}px);" : "";
            return $"{width} {height}".Trim();
        }
    }
    // ================================================================================================================================
}
