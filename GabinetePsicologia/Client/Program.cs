using GabinetePsicologia.Client;
using GabinetePsicologia.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("private", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddHttpClient("public", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("GabinetePsicologia.ServerAPI"));
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<UsuarioServices>();
builder.Services.AddScoped<PsicologoServices>();
builder.Services.AddScoped<TrastornosServices>();
builder.Services.AddScoped<PacientesServices>();
builder.Services.AddScoped<CitasServices>();
builder.Services.AddScoped<InformesServices>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<MensajesServices>();
builder.Services.AddApiAuthorization();


await builder.Build().RunAsync();
