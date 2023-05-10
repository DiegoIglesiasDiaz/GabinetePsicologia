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
    public partial class Informes
    {
        bool allowRowSelectOnRowClick = false;
        IList<InformeDto> LsInformes;
        IList<InformeDto> selectedInforme;
        [Inject] private NavigationManager NavigationManager { get; set; } 
        [Inject] private NotificationService NotificationService { get; set; } 
        [Inject] private UsuarioServices UsuarioServices { get; set; } 
        [Inject] private DialogService DialogService { get; set; } 
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] InformesServices InformesServices { get; set; }
        public bool isInRole = false;
        public bool isPsicologo = false;
        public bool isPaciente = false;
        RadzenDataGrid<InformeDto> grid;

        ClaimsPrincipal? user;
     
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                PersonaDto Persona = await  UsuarioServices.getPersonaByUsername(user.Identity.Name);
                if (user.IsInRole("Psicologo"))
                {
                    isPsicologo = true;
                   
                }
                if (user.IsInRole("Paciente"))
                {
                    isPaciente = true;
                    
                }            
                isInRole = true;
                LsInformes = await InformesServices.GetInformesById(Persona.Id);
            } 
        }
      
        public async void AbrirModal(InformeDto informe, bool isNew)
        {

            InformeDto? a = await DialogService.OpenAsync<InformesModal>("Informe", new Dictionary<string, object> { { "Informe", informe }, { "isNew", isNew }, { "user", user } }, new DialogOptions() { Height = "90%", Width = "90%" });
            if (a != null)
            {
                if (a.Id != Guid.Empty)
                {
                   
                    await grid.UpdateRow(a);
                    NotificationService.Notify(NotificationSeverity.Success, "Ok", "Informe Actualizado Correctamente");
                    await grid.Reload();
                }
                else
                {
                    a.Id = Guid.NewGuid();
                    InformesServices.CrearOActalizarInforme(a, true);
                    LsInformes.Add(informe);
                    NotificationService.Notify(NotificationSeverity.Success, "Ok", "Informe Creado Correctamente");
                    if (LsInformes.Count == 1)
                        NavigationManager.NavigateTo("/Informes", true);
                    else
                        await grid.Reload();
                }    
                 
            }
        }
        public void BorrarInforme()
        {
             if (selectedInforme  == null|| selectedInforme.Count == 0)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "No has seleccionado ningún Informe.");
                return;
            }
            InformesServices.BorrarInforme(selectedInforme);
            foreach(var inf in selectedInforme)
            {
                LsInformes.Remove(inf);
            }
            selectedInforme.Clear();
            grid.Reload();
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Borrado correctamente.");
        }

    }
}
