using Blazored.LocalStorage;
using DI;
using LandOfSignals_Signalbox.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Signalbox.Engine.Storage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

if (ServiceLocator.GetService<ISignalboxStorage>() is BlazorGameStorage storage)
{
    storage.AspNetCoreServices = host.Services;
}

await host.RunAsync();
