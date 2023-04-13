using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class CambiarContraseña
    {
        [Parameter]
        public string email { get; set; }

        public string passwd;
        public bool verPasswd = false;
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private UsuarioServices UsuarioServices { get; set; }

       
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
          
        }

        protected async override void OnParametersSet()
        {

        }
        public void VerPasswd()
        {
            verPasswd = !verPasswd;
        }
        public async  void CambiarContrasenia(string passwd)
        {
            if (await UsuarioServices.CambiarContraseña(passwd, email))
            {
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Contraseña cambiado correctamente.");

            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Error", "No se ha podido cambiar la contraseña.");
            }
            DialogService.Close();
        }
        
    }

}
