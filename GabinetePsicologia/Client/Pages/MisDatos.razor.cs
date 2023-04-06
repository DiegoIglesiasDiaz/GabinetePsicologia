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
        public bool isInRole = false;
        bool isEdit = false;
        //bool isPsicologo = false;
        //bool isAdmin = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Psicologo") || user.IsInRole("Administrador") || user.IsInRole("Paciente"))
            {
                isInRole = true;
                userDto = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
            }
            
           

        }
        public void Edit()
        {
            isEdit=true;
        }
    }
}
