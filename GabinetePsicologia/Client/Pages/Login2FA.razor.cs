using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using QRCoder;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Login2FA
	{

		[Parameter]
		public bool remember { get; set; }
		[Inject] private TwoFactorServices TwoFactorServices { get; set; }
		[Inject] private NotificationService NotificationService { get; set; }
		public string VerificarPin = "";
        public string SharedKey;
        public string model = "";
        public bool RememberMachine = false;


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
           
		
          
		}
        public async void Verificar()
        {
			var verificationCode = VerificarPin.Replace(" ", string.Empty).Replace("-", string.Empty);
			if (verificationCode.Length != 6) { 
				NotificationService.Notify(NotificationSeverity.Error, "Error", "No es un código válido.");
				return;
			}
			var result = await TwoFactorServices.Verify2FA(VerificarPin, remember, RememberMachine);

			if (result.Contains("Ok"))
			{
				DialogService.Close(true);
			}
			else
			{
				NotificationService.Notify(NotificationSeverity.Error, "Error", "Código Incorrecto");
			}
		}
               

    }

}
