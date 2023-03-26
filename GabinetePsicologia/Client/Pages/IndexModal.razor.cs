using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor.Rendering;
using System.Collections.Generic;

namespace GabinetePsicologia.Client.Pages
{
    public partial class IndexModal
    {
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private PacientesServices PacientesServices { get; set; }
        [Inject] private PsicologoServices PsicologoServices { get; set; }
        //[Inject] private AdministradorServices AdministradorServices { get; set; }
        [Parameter]
        public PersonaDto Persona { get; set; }
        protected string RepiteContraseña;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }

        protected override void OnParametersSet()
        {


        }
        public async void GuardarPersona(PersonaDto persona)
        {
            if (persona.FecNacim < DateTime.Now.AddYears(-100) || persona.FecNacim > DateTime.Now)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de seleccionar una Fecha de Naciemiento válido");
                return;
            }
            await UsuarioServices.CambiarContraseña(persona.Contraseña, persona.Email);
            await UsuarioServices.CambiarTelefono(persona.Telefono, persona.Email);
          
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user != null && (user.IsInRole("Paciente")))
            {
                Paciente paciente = new Paciente()
                {
                    Nombre = persona.Nombre,
                    Apellido1 = persona.Apellido1,
                    Apellido2 = persona.Apellido2,
                    Direccion = persona.Direccion,
                    FecNacim = persona.FecNacim,
                    NIF = persona.NIF,
                    ApplicationUserId = persona.ApplicationUserId
                };
                PacientesServices.UpdatePaciente(paciente);
                
            }
            if (user != null && (user.IsInRole("Psicologo")))
            {
                Psicologo psicologo = new Psicologo()
                {
                    Nombre = persona.Nombre,
                    Apellido1 = persona.Apellido1,
                    Apellido2 = persona.Apellido2,
                    Direccion = persona.Direccion,
                    FecNacim = persona.FecNacim,
                    NIF = persona.NIF,
                    ApplicationUserId = persona.ApplicationUserId
                };
                PsicologoServices.UpdatePaciente(psicologo);
            }
            if (user != null && (user.IsInRole("Administrador")))
            {
                Administrador Administrador = new Administrador()
                {
                    Nombre = persona.Nombre,
                    Apellido1 = persona.Apellido1,
                    Apellido2 = persona.Apellido2,
                    Direccion = persona.Direccion,
                    FecNacim = persona.FecNacim,
                    NIF = persona.NIF,
                    ApplicationUserId = persona.ApplicationUserId
                };

            }
            DialogService.Close(true);
        }
    }
}
