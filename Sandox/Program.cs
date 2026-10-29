using Mortgage.Interceptors.ErrorHandling.Factory;
using FE.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Interceptor factory used by the _C pattern (_interceptorErrorHandlingFactory.Create<...>)
builder.Services.AddFEInterceptorErrorHandlingFactory();

// Register every *_S state class as scoped, same convention as FE's StateExtensions
var stateTypes = typeof(Program).Assembly
    .GetTypes()
    .Where(t => t.IsClass
             && !t.IsAbstract
             && t.Name.EndsWith("_S")
             && t.Namespace?.StartsWith("FE.Components") == true);

foreach (var type in stateTypes)
{
    builder.Services.AddScoped(type);
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
