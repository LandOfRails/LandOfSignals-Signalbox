using Blazored.LocalStorage;
using LandOfSignals_Signalbox;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Signalbox.Engine.Storage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Game>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

// Dodgy!!
if (DI.ServiceLocator.GetService<ISignalboxStorage>() is BlazorSignalboxStorage storage)
{
    storage.AspNetCoreServices = host.Services;
}

await host.RunAsync();
