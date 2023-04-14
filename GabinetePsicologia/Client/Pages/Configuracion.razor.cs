using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Configuracion
    {
        [Inject] private NotificationService NotificationService { get; set; } 
        [Inject] private DialogService DialogService { get; set; } 
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] UsuarioServices UsuarioServices { get; set; }
        public bool isInRole = false;
        public string correo = "";
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            correo = user.Identity.Name;
            if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                isInRole = true;

            } 
        }
        public void Logout()
        {
            NavigationManager.NavigateTo("/Logout", true);
        }
        public async void CambiarContrasenia()
        {
            var chckPasswd = await DialogService.OpenAsync<ContraseñaParaContinuar>("Introduce la Contraseña Para Continuar", new Dictionary<string, object> { { "email", correo } });
            if(chckPasswd != null && chckPasswd)
            {
                var result = await DialogService.OpenAsync<CambiarContraseña>("Cambiar Contraseña", new Dictionary<string, object> { { "email", correo } });
            }
           
        }
        public async void CambiarCorreo()
        {
            var chckPasswd = await DialogService.OpenAsync<ContraseñaParaContinuar>("Introduce la Contraseña Para Continuar", new Dictionary<string, object> { { "email", correo } });
            if (chckPasswd != null && chckPasswd)
            {
                var result =  await DialogService.OpenAsync<CambiarCorreo>("Cambiar Correo", new Dictionary<string, object> { { "CorreoAntiguo", correo } });
                if (result != null)
                    NavigationManager.NavigateTo("/Configuracion", true);
            }
 
        }
        public async void BorrarCuenta()
        {
            var chckPasswd = await DialogService.OpenAsync<ContraseñaParaContinuar>("Introduce la Contraseña Para Continuar", new Dictionary<string, object> { { "email", correo } });
            if (chckPasswd != null && chckPasswd)
            {
                var result = await DialogService.Confirm("¿Quieres Borrar la Cuenta?", "Borrar Cuenta");
                if(result != null && result==true)
                {
                    var claims = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
                    if (claims != null)
                    {
                        var user = await  UsuarioServices.getPersonaByUsername(claims.Identity.Name);
                        await UsuarioServices.BorrarCuenta(user.Id);
                        NavigationManager.NavigateTo("/Logout", true);
                    }
                  

                }
            }

        }

    }
}
