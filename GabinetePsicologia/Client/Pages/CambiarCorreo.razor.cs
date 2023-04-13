using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class CambiarCorreo
    {
        [Parameter]
        public string CorreoAntiguo { get; set; }

        public string NewCorreo;
      
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private UsuarioServices UsuarioServices { get; set; }

       
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
          
        }

        protected async override void OnParametersSet()
        {
            
        }
        public async void CambiarMail()
        {

            if (CorreoAntiguo == NewCorreo)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "Has introducido el mismo correo que ya tienes.");

                return;
            }
            string substring = NewCorreo.Substring(NewCorreo.IndexOf("@"));
            if (!substring.Contains("."))
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Este Correo no es válido");
                return;
            }
            if (await UsuarioServices.CambiarCorreo(CorreoAntiguo, NewCorreo))
            {
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Correo cambiado correctamente.");
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "Este Correo ya existe.");
                return;
            }
            DialogService.Close(NewCorreo);
        }
        
    }

}
