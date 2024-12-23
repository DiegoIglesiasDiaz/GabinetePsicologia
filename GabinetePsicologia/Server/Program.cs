using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Radzen;
using GabinetePsicologia.Server.Controllers;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GabinetePsicologia.Shared;
using static System.Net.WebRequestMethods;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.AccessTokenValidation;
using GabinetePsicologia.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<TenantController>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<HostResolutionStrategy>();
var serviceProvider = builder.Services.BuildServiceProvider();
var tenantController = serviceProvider.GetRequiredService<HostResolutionStrategy>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{

    options.UseSqlServer(tenantController.GetConnectionString());
}
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
    {
        options.IdentityResources["openid"].UserClaims.Add("role");
        options.ApiResources.Single().UserClaims.Add("role");
    });

//sigue dando 401 al llamar la api intentar poner ValidateIssuer false;
builder.Services.AddAuthentication().AddIdentityServerJwt()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["GoogleClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["GoogleSecretId"]!;
    }).AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["MicrosoftClientId"]!;
        microsoftOptions.ClientSecret = builder.Configuration["MicrosoftSecretId"]!;
    }

    );


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();
builder.Services.AddScoped<UsuarioController>();
builder.Services.AddScoped<TrastornoController>();
builder.Services.AddScoped<PsicologoController>();
builder.Services.AddScoped<AdministradorController>();
builder.Services.AddScoped<PacienteController>();
builder.Services.AddScoped<InformeController>();
builder.Services.AddScoped<CitaController>();
builder.Services.AddScoped<MensajeController>();
builder.Services.AddScoped<TwoFactorController>();
builder.Services.AddScoped<ChatController>();

builder.Services.AddSignalR();
builder.Services.AddMultiTenancy()
    .WithResolutionStrategy<HostResolutionStrategy>()
    .WithStore<InMemoryTenantStore>();
//SignalR no obligatorio
builder.Services.AddResponseCompression(options =>
                       options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                       {
                           "application/octet-stream"
                       }));

builder.Services.AddCors();

builder.Services.AddLocalization();
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-ES");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    //app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("AllowSpecificOrigins");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapHub<ChatHub>("/chathub");
//SignalR no obligatorio
app.UseResponseCompression();
app.UseIdentityServer();
app.UseAuthentication();


app.UseAuthorization();



app.MapRazorPages();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
