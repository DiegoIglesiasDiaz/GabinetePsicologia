﻿using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Calendar
    {
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] private PsicologoServices PsicologoServices { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        public bool isInRole = false;
        RadzenScheduler<Cita> scheduler;
        GabinetePsicologia.Shared.Psicologo SelectedPsciologo ;
        List<GabinetePsicologia.Shared.Psicologo> lsPsicologos = new List<GabinetePsicologia.Shared.Psicologo>();

        List<Cita> data = new List<Cita>()
    {
      new Cita
      {
        FecInicio = DateTime.Today,
        FecFin = DateTime.Today.AddDays(1),
        Nombre = "Birthday",
        Id=Guid.NewGuid()
      },
    };
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                isInRole = true;
            }
            lsPsicologos = await PsicologoServices.getPsicologos();
        }
        void OnSlotRender(SchedulerSlotRenderEventArgs args)
        {
            // Highlight today in month view
            if (args.View.Text == "Month" && args.Start.Date == DateTime.Today)
            {
                args.Attributes["style"] = "background: rgba(255,220,40,.2);";
            }

            // Highlight working hours (9-18)
            if ((args.View.Text == "Week" || args.View.Text == "Day") && args.Start.Hour > 8 && args.Start.Hour < 19)
            {
                args.Attributes["style"] = "background: rgba(255,220,40,.2);";
            }
        }
        async Task OnSlotSelect(SchedulerSlotSelectEventArgs args)
        {

            Cita citaArgs = new Cita() { FecInicio = args.Start, FecFin = args.End };
            Cita Cita = await DialogService.OpenAsync<CalendarModal>("Añadir Cita", new Dictionary<string, object> { { "Appointment", citaArgs }, { "psicologo", SelectedPsciologo } });

            if (Cita != null)
            {

                data.Add(Cita);
                await scheduler.Reload();
                NotificationService.Notify(NotificationSeverity.Success,"Ok","Cita Añadida Correctamente");

            }

        }
        void OnAppointmentRender(SchedulerAppointmentRenderEventArgs<Cita> args)
        {
            // Never call StateHasChanged in AppointmentRender - would lead to infinite loop

            if (args.Data.Nombre == "Birthday")
            {
                args.Attributes["style"] = "background: red";
            }
        }
        async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Cita> args)
        {

            Cita Cita = await DialogService.OpenAsync<CalendarModal>("Editar Cita", new Dictionary<string, object> { { "Appointment", args.Data } });
            if (Cita != null && Cita.Id != Guid.Empty)
            {
                data.Remove(Cita);
                DialogService.Alert("Cita Borrado Correctamente", "Borrar");
            }
            else if (Cita != null) DialogService.Alert("Cita Editada Correctamente", "Editar");

            await scheduler.Reload();
        }
    }
}
