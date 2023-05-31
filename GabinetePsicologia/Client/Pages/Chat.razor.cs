using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Pages
{
	public partial class Chat
	{
		[Inject] private NotificationService NotificationService { get; set; }
		[Inject] private DialogService DialogService { get; set; }
		[Inject] NavigationManager NavigationManager { get; set; }
		[Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] UsuarioServices UsuarioServices { get; set; }
		[Inject] TwoFactorServices TwoFactorServices { get; set; }
		public bool isInRole = false;
		public string correo = "";
		private ClaimsPrincipal? user;

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
			correo = user.Identity.Name;
			if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
			{
				isInRole = true;
			}
		}
		

	}
}
