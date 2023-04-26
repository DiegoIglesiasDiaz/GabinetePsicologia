using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Radzen;
using GabinetePsicologia.Server.Controllers;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options => {
        options.IdentityResources["openid"].UserClaims.Add("role");
        options.ApiResources.Single().UserClaims.Add("role");
    }).AddDeveloperSigningCredential();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = "488885295151-2uif611ukrii2nlsd8spd5vconu09gl4.apps.googleusercontent.com";
        googleOptions.ClientSecret = "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";
    }).AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = "57bf0797-76f8-432f-9a2e-9280c05a17e7";
        microsoftOptions.ClientSecret = "TQ08Q~Kfp_LfXbq1EFt62AWR-YZotDX3XMjvoc9s";
    }

    );

    //.AddFacebook(facebook =>
    //{
    //    facebook.AppId = "966826294752064";
    //    facebook.AppSecret = "e3cacde7d0293d3a5d926968ea15f347";
    //});

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
builder.Services.AddScoped<CitaController>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy => { policy.AllowAnyOrigin(); });
});

builder.Services.AddLocalization();
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-ES");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthentication();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    //.SetIsOriginAllowed(origin => true)
    //.AllowCredentials()
    );
app.UseAuthorization();



app.MapRazorPages();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
