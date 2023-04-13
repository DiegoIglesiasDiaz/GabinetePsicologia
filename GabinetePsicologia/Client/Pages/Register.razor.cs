using GabinetePsicologia.Shared;
using System.Collections.Generic;
using GabinetePsicologia.Client.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen.Blazor;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Register
    {

        protected string RepiteContraseña;
        PersonaDto PersonaForm = new PersonaDto() { FecNacim = DateTime.Today };
        public bool isInRole;
        public bool isAdmin;
        public bool isPiscologo;
        public bool isEdit;
        public bool verPasswd = false;
        public bool verRePasswd = false;
        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }
        private async void GuardarPersona(PersonaDto data)
        {
            if (data.FecNacim < DateTime.Now.AddYears(-100) || data.FecNacim > DateTime.Now)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de seleccionar una Fecha de Naciemiento válido");
                return;
            }
            string substring = data.Email.Substring(data.Email.IndexOf("@"));
            if (!substring.Contains("."))
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Este Correo no es válido");
                return;
            }
            data.Rol = "";
            data.isPaciente = true;
            data.isPsicologo = false;
            data.isAdmin = false;
            data.Id = Guid.NewGuid();
            data.ApplicationUserId = "";

            if (await UsuarioServices.RegisterPersonaAnonymous(data))
            {
                LoginDto user = new LoginDto();
                user.Email = data.Email;
                user.Password = data.Contraseña;
                user.RememberMe = false;
                if (await UsuarioServices.Login(user))
                    NavigationManager.NavigateTo("/", true);

            }
            else
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Este Correo Ya existe.");
        }
        public void VerPasswd()
        {
            verPasswd = !verPasswd;
        }
        public void VerRePasswd()
        {
            verRePasswd = !verRePasswd;
        }
    }
}
