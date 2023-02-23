﻿using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Usuarios
    {
        bool allowRowSelectOnRowClick = false;
        IEnumerable<PersonaDto> LsUsuarios;
        IList<PersonaDto> selectedUsuarios;
        RadzenDataGrid<PersonaDto> grid;
        PersonaDto PersonaForm = new PersonaDto();
        public bool isInRole;
        public bool isAdmin;
        public bool isPiscologo;
        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                if(user.IsInRole("Psicologo"))
                    isPiscologo = true;
                if (user.IsInRole("Administrador"))
                    isAdmin = true;
                isInRole = true;
                LsUsuarios = await UsuarioServices.getPersonas();
            }
            



        }
        private void GuardarPersona()
        {
            //if (trastornoForm.Id == Guid.Empty)
            //    TrastornosServices.AñadirTrastorno(trastornoForm);
            //else
            //    TrastornosServices.EditarTrastornos(trastornoForm);
            //DialogService.Close();
            //grid.Reload();
        }
        private void BorrarPersona()
        {
            //TrastornosServices.BorrarTrastornos(selectedTrastornos);
            //DialogService.Close();
            //grid.Reload();
        }
    }
}
