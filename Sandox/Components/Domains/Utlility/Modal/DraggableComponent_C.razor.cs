using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FE.Components.Domains.Utlility.Modal
{
    public partial class DraggableComponent_C : ComponentBase, IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; } = "Draggable";

        [Parameter]
        public int InitialX { get; set; } = 50;

        [Parameter]
        public int InitialY { get; set; } = 50;

        [Parameter]
        public int? MinWidth { get; set; } = 900;
        [Parameter]
        public int? MaxWidth { get; set; } = 1200;       
        [Parameter]
        public int Height { get; set; } = 300;

        [Parameter]
        public bool ShowCloseButton { get; set; } = true;

        [Parameter]
        public EventCallback OnClose { get; set; }

        private ElementReference draggableElement;
        private string componentId;
        private bool isVisible = true;

        protected override void OnInitialized()
        {
            componentId = Guid.NewGuid().ToString("N")[..8];
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeDraggable", componentId, InitialX, InitialY);
            }
        }

        private async Task Close()
        {
            isVisible = false;
            await OnClose.InvokeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("destroyDraggable", componentId);
            }
            catch
            {
                // Component may already be disposed
            }
        }
    }
}