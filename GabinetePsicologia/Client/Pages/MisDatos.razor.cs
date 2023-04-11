using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;

namespace GabinetePsicologia.Client.Pages
{
    public partial class MisDatos
    {
        [Inject] private NotificationService NotificationService { get; set; } 
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] UsuarioServices UsuarioServices { get; set; }
        PersonaDto userDto;
        public string cssClass = "textArea calendar" ;
        public bool isInRole = false;
        bool isEdit = false;
        //bool isPsicologo = false;
        //bool isAdmin = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Paciente"))
            {
                isInRole = true;
                if(userDto==null)
                    userDto = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
                userDto.isPaciente = true;
            }
            if (user.IsInRole("Psicologo"))
            {
                isInRole = true;
                if (userDto == null)
                    userDto = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
                userDto.isPsicologo = true;
            }
            if (user.IsInRole("Administrador"))
            {
                isInRole = true;
                if (userDto == null)
                    userDto = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
                userDto.isAdmin = true;
            }



        }
        public void Edit()
        {
            isEdit=true;
            cssClass = "EditTextArea calendar";
        }
        public void GuardarPersona(PersonaDto data)
        {
            if (data.FecNacim < DateTime.Now.AddYears(-100) || data.FecNacim > DateTime.Now)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de seleccionar una Fecha de Naciemiento válido");
                return;
            }
            isEdit = false;
            cssClass = "textArea calendar";
            

            UsuarioServices.EditarPaciente(data);
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Usuario editado correctamente.");
        }
        public void Cancel()
        {
            isEdit = false;
            cssClass = "textArea calendar";
        }
    }
}
