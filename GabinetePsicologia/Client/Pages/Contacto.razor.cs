using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Pages
{
	public partial class Contacto
	{
		[Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		ClaimsPrincipal? user;
		Mensaje mensaje = new Mensaje();
		public bool isInRole = false;
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
			if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
			{
				isInRole = true;
				mensaje.Correo = user.Identity.Name;
			}

		}
		public void Enviado()
		{
			mensaje.Visto = false;
			MensajesServices.Enviar(mensaje);
			NotificationService.Notify(NotificationSeverity.Success, "Ok", "Mensaje enviado correctamente");
			mensaje = new Mensaje();
			if (isInRole)
				mensaje.Correo = user.Identity.Name;
		}
	}
}
