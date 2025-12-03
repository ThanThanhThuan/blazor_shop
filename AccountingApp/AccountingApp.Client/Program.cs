using AccountingApp.Client.Services;
using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

// --- PASTE THE LINE HERE ---
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7249/") });
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CartState>();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
