using Blazored.LocalStorage;
using Ed.BudgetVisualizer.Logic;
using Ed.BudgetVisualizer;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<ParserLogic, ParserLogic>();
builder.Services.AddBlazoredLocalStorage();

// TODO: Support other cultures?
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("lv-LV");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("lv-LV");

await builder.Build().RunAsync();
