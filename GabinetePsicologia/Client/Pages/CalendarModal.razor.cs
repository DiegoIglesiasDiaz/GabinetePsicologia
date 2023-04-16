using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

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
        public bool isPaciente { get; set; }
        Cita cita = new Cita();
        [Inject] private PacientesServices PacientesServices { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private PsicologoServices PsicologoServices { get; set; }
        [Inject] private CitasServices CitasServices { get; set; }
        Paciente selectedPaciente;
        bool isEdit = false;
        List<Paciente> lsPacientes;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            lsPacientes = await PacientesServices.getPacientes();
        }

        protected async override void OnParametersSet()
        {

            cita.Nombre = Appointment.Nombre;
            cita.PacienteId = Appointment.PacienteId;
            cita.PsicologoId = Appointment.PsicologoId;
            cita.Id = Appointment.Id;
            cita.FecInicio = Appointment.FecInicio;
            cita.FecFin = Appointment.FecFin;
            if (lsPacientes == null)

                selectedPaciente = new Paciente();
            else
                selectedPaciente = lsPacientes.FirstOrDefault(x => x.Id == cita.PacienteId);
            if (selectedPaciente != null) isEdit = true;

        }

        async void Guardar()
        {
            if (selectedPaciente == null)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Paciente Vacio", "debe de seleccionar un paciente");
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
                cita.PsicologoId = Psicologo.Id;
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
        void Borrar()
        {
            DialogService.Close(new Cita());
        }
        private async Task<bool> comprobarDisponibilidad()
        {
            var lsCitas = await CitasServices.GetCitas();

            if (lsCitas.Where(x => x.PacienteId == cita.PacienteId || x.PsicologoId == cita.PsicologoId).Any())
            {

                if (lsCitas.Where(x => cita.FecInicio < x.FecInicio && cita.FecFin <= x.FecInicio && cita.Id != x.Id).Any() || lsCitas.Where(x => cita.FecInicio >= x.FecFin && cita.FecFin > x.FecFin && cita.Id != x.Id).Any())
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
