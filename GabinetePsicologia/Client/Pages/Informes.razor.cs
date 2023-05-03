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
        [Inject] private NotificationService NotificationService { get; set; } 
        [Inject] private DialogService DialogService { get; set; } 
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] PsicologoServices PsicologoServices { get; set; }
        [Inject] PacientesServices PacientesServices { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] UsuarioServices UsuarioServices { get; set; }
        [Inject] InformesServices InformesServices { get; set; }
        public bool isInRole = false;
        public bool isPsicologo = false;
        RadzenDataGrid<InformeDto> grid;
        List<Psicologo> lsPsicologos = new List<Psicologo>();
        List<Paciente> lsPacientes = new List<Paciente>();
        Psicologo SelectedPsciologo;
        Paciente SelectedPaciente;
        ClaimsPrincipal? user;
        List<InformeDto> data = new List<InformeDto>();
        List<InformeDto> allListInforme = new List<InformeDto>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                if (user.IsInRole("Psicologo"))
                {
                    isPsicologo = true;
                }
                isInRole = true;

                lsPsicologos = await PsicologoServices.getPsicologos();
                lsPacientes = await PacientesServices.getPacientes();
                LsInformes = await InformesServices.GetInformes();
            } 
        }
      
        public async void AbrirModal(InformeDto informe, bool isNew)
        {
            InformeDto? a = await DialogService.OpenAsync<InformesModal>("Informe", new Dictionary<string, object> { { "Informe", informe }, { "isNew", isNew },{ "user", user } }, new DialogOptions() { Height="90%",Width="90%"} );
            if(a != null )
            {               
                if (a.Id != Guid.Empty)
                {
                    await grid.UpdateRow(a);
                    NotificationService.Notify(NotificationSeverity.Success, "Ok", "Informe Actualizado Correctamente");
                }
                   
                else
                {
                    LsInformes.Add(informe);
                    NotificationService.Notify(NotificationSeverity.Success, "Ok", "Informe Creado Correctamente");
                }
                await grid.Reload();


            }
        }
        public void BorrarInforme()
        {

        }

    }
}
