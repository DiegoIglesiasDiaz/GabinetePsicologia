using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using QRCoder;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Disable2Fa
	{

		[Parameter]
		public string Correo { get; set; }
		[Inject] private TwoFactorServices TwoFactorServices { get; set; }
		[Inject] private NavigationManager NavigationManager { get; set; }
		[Inject] private NotificationService NotificationService { get; set; }



        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if(Correo == null)
            {
				DialogService.Close();
				
			}
		
          
		}
        public async void Deshabilitar()
        {
			var chckPasswd = await DialogService.OpenAsync<ContraseñaParaContinuar>("Introduce la Contraseña Para Continuar", new Dictionary<string, object> { { "email", Correo } });
			if(chckPasswd != null && chckPasswd)
			{
				var result = await DialogService.OpenAsync<ConfirmModal>("¿Desea desactivar la doble autenticación? ");
				if (result == true)
				{
					if (await TwoFactorServices.Disable2FA(Correo))
						DialogService.Close(true);
					else
						NotificationService.Notify(NotificationSeverity.Error, "Error", "No se ha podido desactivar la doble autenticación");
				}
			}
			
		}
               

    }

}
