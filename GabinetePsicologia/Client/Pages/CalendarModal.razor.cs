using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Pages
{
    public partial class CalendarModal
    {
        [Parameter]
        public Cita Appointment { get; set; }
        [Parameter]
        public GabinetePsicologia.Shared.Psicologo Psicologo { get; set; }
        [Parameter]
        public Paciente Paciente { get; set; }
        [Parameter]
        public bool isPaciente { get; set; }
        [Parameter]
        public bool isEdit { get; set; }
        [Parameter]
        public ClaimsPrincipal User { get; set; }


        Cita cita = new Cita();
        [Inject] private PacientesServices PacientesServices { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private PsicologoServices PsicologoServices { get; set; }
        [Inject] private CitasServices CitasServices { get; set; }
        Paciente selectedPaciente = new Paciente();
        Psicologo selectedPsicologo =  new Psicologo();
        List<Paciente> lsPacientes;
        List<Psicologo> lsPsicologo;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!User.IsInRole("Paciente") && !isEdit)
            {
                lsPacientes = await PacientesServices.getPacientes();
               
            }
            if (User.IsInRole("Administrador"))
            {
                lsPsicologo = await PsicologoServices.getPsicologos();
            }
           
        }

        protected async override void OnParametersSet()
        {

            cita.Nombre = Appointment.Nombre;
            cita.PacienteId = Appointment.PacienteId;
            cita.PsicologoId = Appointment.PsicologoId;
            cita.Id = Appointment.Id;
            cita.FecInicio = Appointment.FecInicio;
            cita.FecFin = Appointment.FecFin;

            if (Psicologo.Id != Guid.Empty)
                selectedPsicologo = Psicologo;
            if (Paciente.Id != Guid.Empty)
                selectedPaciente = Paciente;


        }

        async void Guardar()
        {
            if (selectedPaciente == null)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Paciente Vacío", "debe de seleccionar un paciente");
                return;
            }
            if (selectedPsicologo == null)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Psicólogo Vacío", "debe de seleccionar un psicológo");
                return;
            }
            if (cita.FecInicio > cita.FecFin)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Fecha Inicio no puede ser después de la Fecha Fin");
                return;

            }
            if (! await comprobarDisponibilidad())
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Un Psicólogo o un Paciente ya tienen una Cita sobre esa Hora");
                return;
            }
            else
            {
                cita.Nombre = selectedPaciente.FullName;
                cita.PacienteId = selectedPaciente.Id;
                cita.PsicologoId = selectedPsicologo.Id;
                if(!isEdit)
                    cita.Id = Guid.NewGuid();
                DialogService.Close(cita);

            }


        }
        void Change(object args)
        {
            selectedPaciente = null;
            if (args != null)
            {
                Guid Id = Guid.Parse(args.ToString());
                selectedPaciente = lsPacientes.FirstOrDefault(x => x.Id == Id);
            }
        }
        void ChangePsicolgo(object args)
        {
            selectedPsicologo = null;
            if (args != null)
            {
                Guid Id = Guid.Parse(args.ToString());
                selectedPsicologo = lsPsicologo.FirstOrDefault(x => x.Id == Id);
            }
        }
        async void Borrar()
        {
			bool? result = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar esta Cita?");
            if(result != null && result == true)
            {
				DialogService.Close(new Cita());
			}
			
        }
        private async Task<bool> comprobarDisponibilidad()
        {
            if (User.IsInRole("Paciente")) return false;
            var lsCitas = await CitasServices.GetCitas();
            var citaExiste = lsCitas.FirstOrDefault(x => x.Id == cita.Id);
            if (citaExiste != null)
                lsCitas.Remove(citaExiste);
            if (lsCitas == null || lsCitas.Count == 0) return true;
            if (lsCitas.Where(x => x.PacienteId == cita.PacienteId || x.PsicologoId == cita.PsicologoId).Any())
            {
                var citasfilter = lsCitas.Where(x => x.PacienteId == cita.PacienteId || x.PsicologoId == cita.PsicologoId).ToList();
                if (citasfilter.Where(x => cita.FecInicio < x.FecInicio && cita.FecFin <= x.FecInicio).Any() || lsCitas.Where(x => cita.FecInicio >= x.FecFin && cita.FecFin > x.FecFin).Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            else
            {
                return true;
            }

        }
    }

}
