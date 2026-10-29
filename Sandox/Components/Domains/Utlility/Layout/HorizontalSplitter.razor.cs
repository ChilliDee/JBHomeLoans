using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FE.Components.Domains.Utlility.Layout;

public partial class HorizontalSplitter : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Parameter] public RenderFragment? Left { get; set; }
    [Parameter] public RenderFragment? Right { get; set; }
    [Parameter] public double DefaultLeftPct { get; set; } = 50;
    [Parameter] public double MinPct { get; set; } = 20;
    [Parameter] public double MaxPct { get; set; } = 80;
    [Parameter] public int SplitterWidthPx { get; set; } = 10;
    [Parameter] public string SplitterClass { get; set; } = "bg-gray-200 hover:bg-indigo-400 transition-colors";

    // Controlled mode (if you're using it)
    [Parameter] public double? LeftPct { get; set; }
    [Parameter] public EventCallback<double> LeftPctChanged { get; set; }

    private ElementReference _containerRef;
    private ElementReference _handleRef;
    private DotNetObjectReference<HorizontalSplitter>? _selfRef;

    private double _internalPct;
    private double CurrentPct => LeftPct ?? _internalPct;   // controlled wins

    protected override void OnInitialized()
        => _internalPct = LeftPct ?? DefaultLeftPct;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _selfRef = DotNetObjectReference.Create(this);
            await JS.InvokeVoidAsync("horizontalSplitter.init",
                _containerRef, _handleRef, _selfRef, MinPct, MaxPct, SplitterWidthPx);
        }
    }

    [JSInvokable]
    public async Task OnSplitCommit(double pct)
    {
        _internalPct = pct;                       // uncontrolled state
        if (LeftPctChanged.HasDelegate)
            await LeftPctChanged.InvokeAsync(pct); // controlled mode notify
        StateHasChanged();                         // re-render now matches JS-mutated CSS
    }

    public async ValueTask DisposeAsync()
    {
        try { await JS.InvokeVoidAsync("horizontalSplitter.dispose", _handleRef); }
        catch (JSDisconnectedException) { /* circuit already gone, nothing to clean up */ }
        _selfRef?.Dispose();
    }
}
