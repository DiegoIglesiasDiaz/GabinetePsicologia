using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using QRCoder;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Enable2Fa
	{

		[Parameter]
		public string Correo { get; set; }
		[Inject] private TwoFactorServices TwoFactorServices { get; set; }
		[Inject] private NotificationService NotificationService { get; set; }
		public string VerificarPin = "";
        public string SharedKey;
        public string model = "";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if(Correo != null)
            {
				var UriQr = await TwoFactorServices.GetSharedAndQr(Correo);
				if (UriQr != null && UriQr.Length>1)
				{
					SharedKey = UriQr[0];
					var AuthenticatorUri = UriQr[1];
					var qrGenerator = new QRCodeGenerator();
					var qrCodeData = qrGenerator.CreateQrCode(AuthenticatorUri, QRCodeGenerator.ECCLevel.Q);
					PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
					byte[] qrCodeImage = qrCode.GetGraphic(20); // Tamaño del código QR en píxeles
					model = Convert.ToBase64String(qrCodeImage);
				}
			}
		
          
		}
        public async void Verificar()
        {
			var verificationCode = VerificarPin.Replace(" ", string.Empty).Replace("-", string.Empty);
			if (verificationCode.Length != 6) { 
				NotificationService.Notify(NotificationSeverity.Error, "Error", "No es un código válido.");
				return;
			}
            if(await TwoFactorServices.Enable2FA(VerificarPin, Correo))
				DialogService.Close(true);
			else
			{
				NotificationService.Notify(NotificationSeverity.Error, "Error", "Código Incorrecto");
			}
		}
               

    }

}
