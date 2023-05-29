using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;

namespace GabinetePsicologia.Client.Pages
{
	public partial class Login
	{
		[Inject] private UsuarioServices UsuarioServices { get; set; }
		[Inject] private NavigationManager _navigationManager { get; set; }
		[Inject] private DialogService DialogService { get; set; }
		[Inject] private NotificationService _notificationService { get; set; }

		async Task OnLoginAsync(LoginArgs args, string name)
		{
			LoginDto user = new LoginDto();
			user.Email = args.Username;
			user.Password = args.Password;
			user.RememberMe = args.RememberMe;
			string result = await UsuarioServices.Login(user);
			if (result.Contains("Ok"))
				_navigationManager.NavigateTo("/", true);
			else
			{
				if (result.Contains("2FA"))
				{
					var result2FA = await DialogService.OpenAsync<Login2FA>("Doble Autenticación", new Dictionary<string, object>() { { "remember", user.RememberMe } });
					if(result2FA!= null && result2FA == true)
					{
						_navigationManager.NavigateTo("/", true);
					}
				}
				else
				{
					_notificationService.Notify(NotificationSeverity.Error, "Incorrecto", "Usuario o contraseña incorrectos.");
				}
			}
				

			
		}

		void OnRegister()
		{
			_navigationManager.NavigateTo("/Register", true);
		}

		void OnResetPassword(string value, string name)
		{

		}

		void Google()
		{
			UsuarioServices.ExternalLogin("Google");

		}
		public async void RecuperarContraseña()
		{
			await DialogService.OpenAsync<RecuperarContraseña>("Recuperar Contraseña");
		}
	}
}
