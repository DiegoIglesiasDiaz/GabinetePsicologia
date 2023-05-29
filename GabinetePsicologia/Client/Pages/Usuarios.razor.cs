using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using System;


namespace GabinetePsicologia.Client.Pages
{
    public partial class Usuarios
    {

        bool allowRowSelectOnRowClick = false;
        IList<PersonaDto> LsUsuarios;
        IList<PersonaDto> selectedUsuarios;
        string NewCorreo;
        public RadzenDataGrid<PersonaDto> grid;
        PersonaDto PersonaForm = new PersonaDto();
        public bool isInRole;
        public bool isAdmin;
        public bool isPiscologo;
        public bool isEdit;
        Random random = new Random();
        IEnumerable<int> values = new int[] { };
        string LoginUser;
        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private TwoFactorServices TwoFactorServices { get; set; }
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
                if (user.IsInRole("Psicologo"))
                    isPiscologo = true;
                if (user.IsInRole("Administrador"))
                    isAdmin = true;
                isInRole = true;
                LsUsuarios = await UsuarioServices.getPersonas();
              
            }
            LoginUser = user.Identity.Name;
        }
        
        private async void BorrarPersona()
        {
            if (selectedUsuarios != null)
            {

                var remove = selectedUsuarios.FirstOrDefault(x => x.Email == LoginUser);
                if (remove != null)
                    selectedUsuarios.Remove(remove);
            }
            else { selectedUsuarios = new List<PersonaDto>(); }

            if (selectedUsuarios == null || selectedUsuarios.Count == 0)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "No has seleccionado ningún Usuario.");
                return;
            }
			bool? result = false;
			if (selectedUsuarios.Count == 1)
				result = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar el Usuario seleccionado?");
			else
				result = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar los Usuarios Seleccionados?");

			if (result == false)
			{
				return;
			}


			UsuarioServices.BorrarUsuarios(selectedUsuarios);
            foreach (var user in selectedUsuarios)
            {
                LsUsuarios.Remove(user);

            }
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Borrado correctamente.");
            selectedUsuarios = new List<PersonaDto>();
            grid.Reload();
            DialogService.Close();

        }
		private async void Deshabilitar2fa()
		{
			if (selectedUsuarios != null)
			{

				var remove = selectedUsuarios.FirstOrDefault(x => x.Email == LoginUser);
				if (remove != null)
					selectedUsuarios.Remove(remove);
			}
			else { selectedUsuarios = new List<PersonaDto>(); }

			if (selectedUsuarios == null || selectedUsuarios.Count == 0)
			{
				NotificationService.Notify(NotificationSeverity.Warning, "", "No has seleccionado ningún Usuario.");
				return;
			}
            var lsCorreos = new List<string>();
            foreach(var user in selectedUsuarios)
            {
                if(user.TwoFA == "Si")
                    lsCorreos.Add(user.Email);
            }
			var result = await TwoFactorServices.DisableList2FA(lsCorreos);
		    if(result != null && result == true)
            {
				foreach (var user in selectedUsuarios)
				{
                    var tmp = LsUsuarios.FirstOrDefault(x=>user.Id == x.Id);
                    if(tmp != null)
                    {
						tmp.TwoFA = "No";
					}
				}
                await grid.Reload();
				NotificationService.Notify(NotificationSeverity.Success, "Ok", "Doble autenticación deshabilitado correctamente.");
			}
			    
            else
				NotificationService.Notify(NotificationSeverity.Error, "Error", "Error al deshabilitar la doble autenticación.");
            
			selectedUsuarios = new List<PersonaDto>();
			DialogService.Close();

		}
		//private async void CambiarCorreo(string correoAntiguo)
		//{
		//    selectedUsuarios = new List<PersonaDto>();
		//    if (correoAntiguo == NewCorreo)
		//    {
		//        NotificationService.Notify(NotificationSeverity.Warning, "", "Has introducido el mismo correo que ya tienes.");

		//        return;
		//    }
		//    if (!(NewCorreo.Contains('@') && NewCorreo.Contains('.')))
		//    {
		//        NotificationService.Notify(NotificationSeverity.Warning, "", "No has Introducido un correo válido.");

		//        return;
		//    }
		//    if (await UsuarioServices.CambiarCorreo(correoAntiguo, NewCorreo))
		//    {
		//        NotificationService.Notify(NotificationSeverity.Success, "Ok", "Correo cambiado correctamente.");
		//        var edit = LsUsuarios.FirstOrDefault(x => x.Email == correoAntiguo);
		//        if (edit != null)
		//        {
		//            LsUsuarios.Remove(edit);
		//            edit.Email = NewCorreo;
		//            LsUsuarios.Add(edit);
		//            await grid.Reload();
		//        }

		//    }
		//    else
		//    {
		//        NotificationService.Notify(NotificationSeverity.Warning, "", "Este Correo ya existe.");
		//        return;
		//    }

		//    await grid.Reload();
		//    DialogService.Close();
		//}
		//private async void CambiarContraseña(string passwd)
		//{
		//    string email = selectedUsuarios.First().Email;
		//    selectedUsuarios = new List<PersonaDto>();
		//    if (await UsuarioServices.CambiarContraseña(passwd, email))
		//   {
		//        NotificationService.Notify(NotificationSeverity.Success, "Ok", "Contraseña cambiado correctamente.");

		//   }
		//   else
		//   {
		//        NotificationService.Notify(NotificationSeverity.Warning, "Error", "No se ha podido cambiar la contraseña.");
		//   }
		//    DialogService.Close();
		//}
		private async void GuardarPersona(PersonaDto data)
        {
            if (data.FecNacim < DateTime.Now.AddYears(-100) || data.FecNacim > DateTime.Now)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de seleccionar una Fecha de Naciemiento válido");
                return;
            }
            data.Rol = "";
            data.isPaciente = false;
            data.isPsicologo = false;
            data.isAdmin = false;
            foreach (int value in values)
            {
                if (value == 1)
                {
                    data.isPaciente = true;
                    data.Rol = "Paciente";
                }
                if (value == 2)
                {
                    data.isPsicologo = true;
                    if (data.Rol == null || data.Rol == "") data.Rol = "Psicologo";
                    else data.Rol += ", Psicologo";
                }
                if (value == 3)
                {
                    data.isAdmin = true;
                    if (data.Rol == null || data.Rol == "") data.Rol = "Administrador";
                    else data.Rol += ", Administrador";
                }
            }

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

                if (await UsuarioServices.RegisterPersona(data))
                {
                    LsUsuarios.Add(data);
                    if (LsUsuarios.Count == 1) 
                        NavigationManager.NavigateTo("/Usuarios", true);
                    else
                        grid.Reload();
                    NotificationService.Notify(NotificationSeverity.Success, "Ok", "Usuario creado correctamente.");

                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Este Correo Ya existe.");
                    return;
                }

            }
            DialogService.Close();
        }
        public async Task CambiarContraseñaModal()
        {

            if (selectedUsuarios != null)
            {


                var remove = selectedUsuarios.FirstOrDefault(x => x.Email == LoginUser);
                if (remove != null)
                    selectedUsuarios.Remove(remove);
            }
            else { selectedUsuarios = new List<PersonaDto>(); }
            if (selectedUsuarios.Count == null || selectedUsuarios.Count == 0)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "Ningún Usuario seleccionado.");
                return;
            }
            if (selectedUsuarios.Count != 1)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "Debes de seleccionar solo un Usuario.");
                return;
            }
            string passwd = "";
            string email = selectedUsuarios.First().Email;
            selectedUsuarios = new List<PersonaDto>();
            var result = await DialogService.OpenAsync<CambiarContraseña>("Cambiar Contraseña", new Dictionary<string, object> { { "email", email } });
        }
        public async Task CambiarCorreoModal()
        {

            if (selectedUsuarios != null)
            {


                var remove = selectedUsuarios.FirstOrDefault(x => x.Email == LoginUser);
                if (remove != null)
                    selectedUsuarios.Remove(remove);
            }
            else { selectedUsuarios = new List<PersonaDto>(); }
            if (selectedUsuarios.Count == null || selectedUsuarios.Count == 0)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "Ningún Usuario seleccionado.");
                return;
            }
            if (selectedUsuarios.Count != 1)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "Debes de seleccionar solo un Usuario.");
                return;
            }
            string CorreoAntiguo = selectedUsuarios.FirstOrDefault().Email;
            selectedUsuarios = new List<PersonaDto>();
            var result = await DialogService.OpenAsync<CambiarCorreo>("Cambiar Correo", new Dictionary<string, object> { { "CorreoAntiguo", CorreoAntiguo } });
            if (result != null)
            {
                var edit = LsUsuarios.FirstOrDefault(x => x.Email == CorreoAntiguo);
                if (edit != null && edit.Id != Guid.Empty)
                {
                    LsUsuarios.Remove(edit);
                    edit.Email = result;
                    LsUsuarios.Add(edit);
                    await grid.Reload();
                }

            }
        }
        
    }
}
