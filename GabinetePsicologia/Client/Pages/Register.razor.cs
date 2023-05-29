using GabinetePsicologia.Shared;
using System.Collections.Generic;
using GabinetePsicologia.Client.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen.Blazor;
using Microsoft.AspNetCore.Mvc;

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
        public bool ConfirmCondicionesYTerminos = false;
        public bool verPasswd = false;
        public bool verRePasswd = false;
        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }
        private async void GuardarPersona(PersonaDto data)
        {
            if(!ConfirmCondicionesYTerminos)
            {
				NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de Aceptar las Condiciones de Servicio para Continuar");
				return;
			}
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

                var result = await UsuarioServices.Login(user);
				if(result.Contains("Ok"))
					NavigationManager.NavigateTo("/", true);
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Error al Iniciar Sesión");
				}
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
        public void CondicionesYTerminosChange()
        {
            ConfirmCondicionesYTerminos = !ConfirmCondicionesYTerminos ;
			
		}
		public async void CondicionesYTerminos()
		{
            await DialogService.Alert("Al registrarte, aceptas que tus datos personales sean almacenados y procesados de acuerdo con nuestra política de privacidad.\r\nSolo se permite un registro por persona. Si se descubre que tienes múltiples cuentas, podemos suspender o cancelar todas ellas sin previo aviso.\r\nEs responsabilidad del usuario mantener su información de cuenta actualizada y segura. Si se sospecha de cualquier actividad sospechosa, debes notificarlo inmediatamente a nuestro equipo de soporte.\r\nNos reservamos el derecho de suspender o cancelar cualquier cuenta que infrinja nuestras políticas o términos de servicio en cualquier momento, sin previo aviso.\r\nAl registrarte, aceptas recibir comunicaciones de nuestra parte relacionadas con el servicio que ofrecemos.\r\nCualquier contenido que publiques o compartas a través de nuestra plataforma es tu responsabilidad y debes asegurarte de que cumpla con todas las leyes y regulaciones aplicables. Nos reservamos el derecho de eliminar cualquier contenido que infrinja nuestras políticas o términos de servicio en cualquier momento.\r\nAl registrarte, aceptas cumplir con todas nuestras políticas y términos de servicio, y cualquier cambio que hagamos en ellos en el futuro.", "Condiciones de Servicio", new AlertOptions() { Width="300",OkButtonText="Cerrar" });

		}
	}
}
