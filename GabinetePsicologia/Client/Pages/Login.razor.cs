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
        [Inject] private NotificationService _notificationService { get; set; }

        async Task OnLoginAsync(LoginArgs args, string name)
        {
            LoginDto user = new LoginDto();
            user.Email = args.Username;
            user.Password = args.Password;
            user.RememberMe = args.RememberMe;
            if (await UsuarioServices.Login(user))
                _navigationManager.NavigateTo("/", true);
            else
                _notificationService.Notify(NotificationSeverity.Error, "Incorrecto", "Usuario o contraseña incorrectos.");
        }

        void OnRegister(string name)
        {
        }

        void OnResetPassword(string value, string name)
        {
        }

        
    }
}
