﻿using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Pages
{
	public partial class Configuracion
	{
		[Inject] private NotificationService NotificationService { get; set; }
		[Inject] private DialogService DialogService { get; set; }
		[Inject] NavigationManager NavigationManager { get; set; }
		[Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] UsuarioServices UsuarioServices { get; set; }
		[Inject] TwoFactorServices TwoFactorServices { get; set; }
		public bool isInRole = false;
		public bool is2FA = false;
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
				is2FA = await TwoFactorServices.isEnable2FA(correo);
			}
		}
		public void Logout()
		{
			NavigationManager.NavigateTo("/Logout", true);
		}
		public async void CambiarContrasenia()
		{
			var chckPasswd = await DialogService.OpenAsync<ContraseñaParaContinuar>("Introduce la Contraseña Para Continuar", new Dictionary<string, object> { { "email", correo } });
			if (chckPasswd != null && chckPasswd)
			{
				var result = await DialogService.OpenAsync<CambiarContraseña>("Cambiar Contraseña", new Dictionary<string, object> { { "email", correo } });
			}

		}
		public async void CambiarCorreo()
		{
			var chckPasswd = await DialogService.OpenAsync<ContraseñaParaContinuar>("Introduce la Contraseña Para Continuar", new Dictionary<string, object> { { "email", correo } });
			if (chckPasswd != null && chckPasswd)
			{
				var result = await DialogService.OpenAsync<CambiarCorreo>("Cambiar Correo", new Dictionary<string, object> { { "CorreoAntiguo", correo } });
				if (result != null)
					NavigationManager.NavigateTo("/Configuracion", true);
			}

		}
		public async void BorrarCuenta()
		{
			var chckPasswd = await DialogService.OpenAsync<ContraseñaParaContinuar>("Introduce la Contraseña Para Continuar", new Dictionary<string, object> { { "email", correo } });
			if (chckPasswd != null && chckPasswd)
			{
				bool? result = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar tu Cuenta?");
				if (result != null && result == true)
				{
					var claims = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
					if (claims != null)
					{
						var user = await UsuarioServices.getPersonaByUsername(claims.Identity.Name);
						await UsuarioServices.BorrarCuenta(user.Id);
						NavigationManager.NavigateTo("/Logout", true);
					}


				}
			}

		}
		public async void Verficar2fa()
		{
			var result = await DialogService.OpenAsync<Enable2Fa>($"Activar 2FA", new Dictionary<string, object> { { "Correo", user.Identity.Name } });
			if (result != null && result == true)
			{
				
				NotificationService.Notify(NotificationSeverity.Success, "Ok", "Doble Autenticación Habilitada, Recargando..");
				await Task.Delay(1500);
				NavigationManager.NavigateTo("/Configuracion", true);
			}

		}
		public async void Resetear2fa()
		{
			var result = await DialogService.OpenAsync<Reset2Fa>($"Restablecer clave Doble Autenticación", new Dictionary<string, object> { { "Correo", user.Identity.Name } });
			if (result != null && result == true)
			{
				NotificationService.Notify(NotificationSeverity.Success, "Ok", "Clave de doble autenticación Reseteada, Recargando..");
				await Task.Delay(1500);
				NavigationManager.NavigateTo("/Configuracion", true);
			}

		}
		public async void Deshabilitar2fa()
		{

			var result = await DialogService.OpenAsync<Disable2Fa>("Deshabilitar doble Autenticación", new Dictionary<string, object> { { "Correo", user.Identity.Name } });
			if (result != null && result == true)
			{
				NotificationService.Notify(NotificationSeverity.Success, "Ok", "Doble Autenticación Deshabilitada, Recargando..");
				await Task.Delay(1500);
				NavigationManager.NavigateTo("/Configuracion", true);
			}

		}

	}
}
