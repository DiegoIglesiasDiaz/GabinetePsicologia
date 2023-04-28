using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Agenda
    {
       
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private CitasServices CitasServices { get; set; }
        [Inject] private PacientesServices PacientesServices { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private PsicologoServices PsicologoServices { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public bool isInRole = false;
        RadzenScheduler<Cita> scheduler;
        GabinetePsicologia.Shared.Psicologo SelectedPsciologo;
        Paciente SelectedPaciente;
        List<GabinetePsicologia.Shared.Psicologo> lsPsicologos = new List<GabinetePsicologia.Shared.Psicologo>();
        List<Paciente> lsPacientes = new List<Paciente>();
        bool isPaciente = false;
        bool isPsicologo = false;
        bool isAdmin = false;

        List<Cita> allList = new List<Cita>();
        List<Cita> data = new List<Cita>();
        List<Cita> ProximasCitas = new List<Cita>();
        public TimeSpan StartDate = new TimeSpan(7, 0, 0);
        public TimeSpan EndDate = new TimeSpan(22, 0, 0);
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Psicologo") || user.IsInRole("Administrador") || user.IsInRole("Paciente"))
            {
                isInRole = true;
            }
            else return;
            lsPsicologos = await PsicologoServices.getPsicologos();
            lsPacientes = await PacientesServices.getPacientes();
            data = await CitasServices.GetCitas();
            allList = await CitasServices.GetCitas();
            ProximasCitas = allList.Where(x => x.FecInicio > DateTime.Now).ToList();
           
            if (user.IsInRole("Psicologo"))
            {
                SelectedPsciologo = await PsicologoServices.GetPsicologoByUsername(user.Identity.Name);
                allList = allList.Where(x => x.PsicologoId == SelectedPsciologo.Id).ToList();
                data = allList;
                isPsicologo = true;
                ProximasCitas = ProximasCitas.Where(x => x.PsicologoId == SelectedPsciologo.Id).ToList();
            }
            else if (user.IsInRole("Paciente"))
            {
                SelectedPaciente = await PacientesServices.GetPacienteByUsername(user.Identity.Name);
                allList = allList.Where(x => x.PacienteId == SelectedPaciente.Id).ToList();
                data = allList;
                isPaciente = true;
                if (user.IsInRole("Psicologo"))
                {
                    ProximasCitas.AddRange(allList.Where(x => x.PacienteId == SelectedPsciologo.Id).ToList());
                }
                else
                {
                    ProximasCitas = ProximasCitas.Where(x => x.PacienteId == SelectedPaciente.Id).ToList();
                }
               
            }
            if (user.IsInRole("Administrador"))
            {
                ProximasCitas = allList.Where(x => x.FecInicio > DateTime.Now).ToList();
                isAdmin = true;
            }
            ProximasCitas = ProximasCitas.OrderBy(x=> x.FecInicio).ToList();
            if (ProximasCitas.Count > 5)
            {
                ProximasCitas = ProximasCitas.Take(5).ToList();
            }
        }
        void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            //// Highlight today in month view
            //if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
            //{
            //    args.Attributes["style"] = "background: rgba(255,220,40,.2);";
            //}

            //// Highlight working hours (9-18)
            //if ((args.View.Text == "Week" || args.View.Text == "Day") && args.Start.Hour > 8 && args.Start.Hour < 19)
            //{
            //    args.Attributes["style"] = "background: rgba(255,220,40,.2);";
            //}
        }
        async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {
            if (isPaciente && !isAdmin && !isPsicologo) return;
            //if (SelectedPsciologo == null)
            //{
            //    NotificationService.Notify(NotificationSeverity.Warning, "Psicologo", "Debes de seleccioanr un Psicologo");
            //    return;
            //}

            var citaArgs = new Cita() { FecInicio = args.Start, FecFin = args.Start.AddHours(1) };
            var Cita = await DialogService.OpenAsync<CalendarModal>("Añadir Cita", new Dictionary<string, object> { { "Appointment", citaArgs }, { "Psicologo", SelectedPsciologo ?? new Psicologo()}, { "Paciente", SelectedPaciente ?? new Paciente() },{ "isPaciente", false }, { "isEdit", false } });

            if (Cita != null && Cita is Cita)
            {

                CitasServices.InsertCita(Cita);
                data.Add(Cita);
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Cita Añadida Correctamente");
                await scheduler.Reload();
            }

        }
        void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Cita> args)
        {
            // Never call StateHasChanged in AppointmentRender - would lead to infinite loop

            //if (args.Data.Nombre == "Birthday")
            //{
            //    args.Attributes["style"] = "background: red";
            //}
        }
        async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Cita> args)
        {
            Cita Cita = args.Data;
            Guid id = args.Data.Id;
            Psicologo psicologo = lsPsicologos.FirstOrDefault(x => x.Id == Cita.PsicologoId);
            Paciente paciente = lsPacientes.FirstOrDefault(x => x.Id == Cita.PacienteId);
            bool isPacienteForModal = false;
            if (isPaciente && !isAdmin && !isPsicologo)
            {
                isPacienteForModal = true;
            }
            var result = await DialogService.OpenAsync<CalendarModal>("Editar Cita", new Dictionary<string, object> { { "Appointment", args.Data }, { "Psicologo", psicologo ?? new Psicologo() }, { "Paciente", paciente ?? new Paciente() }, { "isPaciente", isPacienteForModal }, { "isEdit", true } });
            if (result == null || !(result is Cita)) return;
            Cita = result;
            if ( Cita is Cita && Cita.Id != Guid.Empty )
            {
                Cita.Id = id;
                CitasServices.ActualizarCita(Cita);
                data.Remove(args.Data);
                data.Add(Cita);
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Cita Editada correctamente");

            }
            else
            {
                CitasServices.EliminarCita(args.Data);
                data.Remove(args.Data);
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Cita Borrada correctamente");


            }


            await scheduler.Reload();
        }
        private void change(object args)
        {
            SelectedPsciologo = null;
            data = allList.ToList();
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

                scheduler.Reload();

            }
            else if (SelectedPaciente != null)
            {
                data = data.Where(x => x.PacienteId == SelectedPaciente.Id).ToList();
                scheduler.Reload();
            }
        }
        private void changePaciente(object args)
        {
            SelectedPaciente = null;
            data = allList.ToList();

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

                scheduler.Reload();
            }
            else if (SelectedPsciologo != null)
            {
                data = data.Where(x => x.PsicologoId == SelectedPsciologo.Id).ToList();
                scheduler.Reload();
            }
        }
        public string PsicologoName(Guid id)
        {
            var a = lsPsicologos.FirstOrDefault(x => x.Id == id).FullName;
            if(a == null)
                return "";
            else
                return a;
        }
    }
}
