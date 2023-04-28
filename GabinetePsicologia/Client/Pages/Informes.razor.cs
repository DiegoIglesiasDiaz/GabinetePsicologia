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
        bool allowRowSelectOnRowClick = false;
        IList<Informe> LsInformes;
        IList<Informe> selectedInforme;
        [Inject] private NotificationService NotificationService { get; set; } 
        [Inject] private DialogService DialogService { get; set; } 
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] PsicologoServices PsicologoServices { get; set; }
        [Inject] PacientesServices PacientesServices { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject] UsuarioServices UsuarioServices { get; set; }
        [Inject] InformesServices InformesServices { get; set; }
        public bool isInRole = false;
        RadzenDataGrid<Informe> grid;
        List<Psicologo> lsPsicologos = new List<Psicologo>();
        List<Paciente> lsPacientes = new List<Paciente>();
        Psicologo SelectedPsciologo;
        Paciente SelectedPaciente;
        List<Informe> data = new List<Informe>();
        List<Informe> allListInforme = new List<Informe>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                isInRole = true;

                lsPsicologos = await PsicologoServices.getPsicologos();
                lsPacientes = await PacientesServices.getPacientes();
                LsInformes = await InformesServices.GetInformes();
            } 
        }
        private void change(object args)
        {
            SelectedPsciologo = null;
            data = allListInforme.ToList();
            if (args != null)
            {
                Guid Id = Guid.Parse(args.ToString());
                SelectedPsciologo = lsPsicologos.FirstOrDefault(x => x.Id == Id);
                if (SelectedPaciente != null)
                {
                    data = data.Where(x => x.PsicologoId == Id && x.PacienteId == SelectedPaciente.Id).ToList();
                }
                else
                {
                    data = data.Where(x => x.PsicologoId == Id).ToList();
                }

                grid.Reload();

            }
            else if (SelectedPaciente != null)
            {
                data = data.Where(x => x.PacienteId == SelectedPaciente.Id).ToList();
                grid.Reload();
            }
        }
        private void changePaciente(object args)
        {
            SelectedPaciente = null;
            data = allListInforme.ToList();

            if (args != null)
            {
                Guid Id = Guid.Parse(args.ToString());
                SelectedPaciente = lsPacientes.FirstOrDefault(x => x.Id == Id);
                if (SelectedPsciologo != null)
                {
                    data = data.Where(x => x.PacienteId == Id && x.PsicologoId == SelectedPsciologo.Id).ToList();
                }
                else
                {
                    data = data.Where(x => x.PacienteId == Id).ToList();
                }

                grid.Reload();
            }
            else if (SelectedPsciologo != null)
            {
                data = data.Where(x => x.PsicologoId == SelectedPsciologo.Id).ToList();
                grid.Reload();
            }
        }
        public void AbrirModal(Informe informe)
        {
            var a = DialogService.OpenAsync<InformesModal>("Informe", new Dictionary<string, object> { { "Informe", informe } });

        }

    }
}
