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
        bool isEdit = false;
        List<Paciente> lsPacientes;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            lsPacientes = await PacientesServices.getPacientes();
        }
       
        protected async override void OnParametersSet()
        {
          
            cita = Appointment;
            if(lsPacientes == null)
            
                selectedPaciente = new Paciente();
            else
                selectedPaciente = lsPacientes.FirstOrDefault(x=> x.Id == cita.PacienteId);
            if (selectedPaciente != null) isEdit = true;

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
