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

        Cita cita = new Cita();
        [Inject] private PacientesServices PacientesServices { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private PsicologoServices PsicologoServices { get; set; }
        Paciente selectedPaciente;
        string selectNamePersona;
        List<Paciente> lsPacientes;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            lsPacientes = await PacientesServices.getPacientes();
        }
       
        protected override void OnParametersSet()
        {
          
            cita = Appointment;
            selectedPaciente = lsPacientes.FirstOrDefault(x=> x.Id == cita.PacienteId);
            //if (selectedPaciente != null)
            //    selectNamePersona = "paciente";

        }

        void Guardar()
        {
            if(selectedPaciente == null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Paciente Vacio", "debe de seleccionar un paciente");
                return;
            }
            cita.Nombre = selectedPaciente.FullName;
            cita.PacienteId = selectedPaciente.Id;
            cita.PsicologoId = Psicologo.Id;
            cita.Id = Guid.NewGuid();
            DialogService.Close(cita);
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
    }

}
