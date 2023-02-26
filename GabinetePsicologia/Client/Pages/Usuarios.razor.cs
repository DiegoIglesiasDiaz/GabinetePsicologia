using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using GabinetePsicologia.Client.Pages.Psicologo;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Usuarios
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
        IEnumerable<int> values = new int[] {  };

        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
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
        private async void GuardarPersona(PersonaDto data)
        {
            data.Id = Guid.NewGuid();
            foreach (int value in values)
            {
                if (value == 1) { 
                    data.isPaciente = true;
                    data.Rol = "Paciente";
                }
                if (value == 2)
                {
                    data.isPsicologo = true;
                    if (data.Rol == null) data.Rol = "Psicologo";
                    else data.Rol += ", Psicologo";
                }
                if (value == 3)
                {
                    data.isAdmin = true;
                    if (data.Rol == null) data.Rol = "Administrador";
                    else data.Rol += ", Administrador";
                }
            }
            data.ApplicationUserId = "";
            if (await UsuarioServices.RegisterPaciente(data))
            {
                LsUsuarios.Add(data);
                grid.Reload();
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Usuario creado correctamente.");

            }
            else
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Este Correo Ya existe.");
            DialogService.Close();


        }
        private void BorrarPersona()
        {
            if (selectedUsuarios == null || selectedUsuarios.Count == 0)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "No has seleccionado ningún Usuario.");
                return;
            }
            UsuarioServices.BorrarUsuarios(selectedUsuarios);
            foreach (var user in selectedUsuarios)
            {
                LsUsuarios.Remove(user);

            }
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Borrado correctamente.");
            selectedUsuarios.Clear();
            grid.Reload();
            DialogService.Close();
           
        }
    }
}
