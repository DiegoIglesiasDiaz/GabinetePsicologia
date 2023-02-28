using GabinetePsicologia.Shared;
using System.Collections.Generic;
using GabinetePsicologia.Client.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen.Blazor;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Register
    {
        bool allowRowSelectOnRowClick = false;
        IList<PersonaDto> LsUsuarios;
        IList<PersonaDto> selectedUsuarios;
        RadzenDataGrid<PersonaDto> grid;
        PersonaDto PersonaForm = new PersonaDto();
        public bool isInRole;
        public bool isAdmin;
        public bool isPiscologo;
        public bool isEdit;
        IEnumerable<int> values = new int[] { };
        string LoginUser;
        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            //if (user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            //{
            //    if (user.IsInRole("Psicologo"))
            //        isPiscologo = true;
            //    if (user.IsInRole("Administrador"))
            //        isAdmin = true;
            //    isInRole = true;
            //    LsUsuarios = await UsuarioServices.getPersonas();
            //}
            LoginUser = user.Identity.Name;

        }
        private async void GuardarPersona(PersonaDto data)
        {
            data.Rol = "";
            data.isPaciente = true;
            data.isPsicologo = false;
            data.isAdmin = false;

            if (isEdit)
            {
                UsuarioServices.EditarPaciente(data);
                var remove = LsUsuarios.FirstOrDefault(x => x.Email == data.Email);
                LsUsuarios.Remove(remove);
                LsUsuarios.Add(data);
                grid.Reload();
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Usuario editado correctamente.");
            }
            else
            {
                data.Id = Guid.NewGuid();
                data.ApplicationUserId = "";

                if (await UsuarioServices.RegisterPersonaAnonymous(data))
                {
                    LsUsuarios.Add(data);
                    grid.Reload();
                    NotificationService.Notify(NotificationSeverity.Success, "Ok", "Usuario creado correctamente.");
                   

                }
                else
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Este Correo Ya existe.");

            }
            DialogService.Close();




        }
    }
}
