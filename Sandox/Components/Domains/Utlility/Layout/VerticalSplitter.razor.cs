using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FE.Components.Domains.Utlility.Layout;

public partial class VerticalSplitter : ComponentBase, IAsyncDisposable
{
    [Parameter] public RenderFragment? Top { get; set; }
    [Parameter] public RenderFragment? Bottom { get; set; }

    // Initial split position (top pane %); ignored on subsequent renders — it's a starting value, not a bound value
    [Parameter] public double InitialPct { get; set; } = 50;

    // Drag clamping — top pane can never be resized past these bounds
    [Parameter] public double MinPct { get; set; } = 10;
    [Parameter] public double MaxPct { get; set; } = 90;

    [Parameter] public int SplitterHeightPx { get; set; } = 8;
    [Parameter] public string SplitterClass { get; set; } = "bg-gray-200 hover:bg-blue-600 transition-colors";

    // Fires once, when the user releases the handle — not on every drag frame. JS owns the live visual
    // during the drag itself (direct style writes), so this only needs to sync final state back to .NET.
    [Parameter] public EventCallback<double> CurrentPctChanged { get; set; }

    protected double CurrentPct { get; set; }

    private ElementReference _containerRef;
    private ElementReference _handleRef;
    private DotNetObjectReference<VerticalSplitter>? _dotNetRef;
    private bool _jsInitialised;

    protected override void OnInitialized()
    {
        CurrentPct = InitialPct;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("verticalSplitter.init", _containerRef, _handleRef, _dotNetRef, MinPct, MaxPct, SplitterHeightPx);
            _jsInitialised = true;
        }
    }

    // Called once from JS when the user releases the handle (drag "commit") — not per frame
    [JSInvokable]
    public async Task OnSplitCommit(double newPct)
    {
        CurrentPct = newPct;
        StateHasChanged();

        if (CurrentPctChanged.HasDelegate)
        {
            await CurrentPctChanged.InvokeAsync(CurrentPct);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsInitialised)
        {
            await JS.InvokeVoidAsync("verticalSplitter.dispose", _handleRef);
        }

        _dotNetRef?.Dispose();
    }
}