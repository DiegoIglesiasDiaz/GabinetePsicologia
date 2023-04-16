using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Informes
    {
        [Inject] private NotificationService NotificationService { get; set; } 
        [Inject] private DialogService DialogService { get; set; } 
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] UsuarioServices UsuarioServices { get; set; }
        public bool isInRole = false;
      
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                isInRole = true;

            } 
        }

    }
}
