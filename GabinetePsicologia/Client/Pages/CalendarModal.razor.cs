using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;

namespace GabinetePsicologia.Client.Pages
{
    public partial class CalendarModal
    {
        [Parameter]
        public Cita Appointment { get; set; }
        [Parameter]
        public GabinetePsicologia.Shared.Psicologo psicologo { get; set; }

        Cita cita = new Cita();
        [Inject] private PacientesServices PacientesServices { get; set; }
        [Inject] private PsicologoServices PsicologoServices { get; set; }
        Paciente selecetedPaciente;
        List<Paciente> lsPacientes;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            lsPacientes = await PacientesServices.getPacientes();
        }
       
        protected override void OnParametersSet()
        {
            cita = Appointment;
        }

        void Guardar()
        {
            cita.Nombre = selecetedPaciente.Nombre;
            DialogService.Close(new Cita());
        }

        void OnSubmit(Cita model)
        {
            if (Appointment.Id == Guid.Empty)
                model.Id = Guid.NewGuid();
            DialogService.Close(model);
        }
    }

}
