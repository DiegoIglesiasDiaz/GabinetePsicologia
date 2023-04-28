using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class ContraseñaParaContinuar
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
        public async  void Contrasenia(string passwd)
        {
            if(await UsuarioServices.CheckPasswd(email, passwd))
            {
                DialogService.Close(true);
                NotificationService.Notify(NotificationSeverity.Success, "Ok","Contraseña Correcta");

            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error,"Error","Contraseña Incorrecta");
                return;
            }
        }
        
    }

}
