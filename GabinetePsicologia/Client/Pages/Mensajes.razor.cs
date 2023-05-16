using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Net.Http;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Mensajes
    {
		[Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] DialogService DialogService { get; set; }
		[Inject] NotificationService NotificationService { get; set; }
		[Inject] MensajesServices MensajesServices { get; set; }
		IList<Mensaje> selectedMensajes;
		IList<Mensaje> lsMensajes ;
		RadzenDataGrid<Mensaje> grid;
		public bool isAdmin = false;
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
			if (user.IsInRole("Administrador"))
			{
				isAdmin = true;
				lsMensajes = await MensajesServices.Get();

			}

		}
		public async void abrirModal(Mensaje mensaje)
		{
			mensaje.Visto = true;
			var result = await DialogService.OpenAsync<MensajeModal>("Mensaje", new Dictionary<string, object> { { "Mensaje", mensaje } });
			if(result != null && result is Mensaje)
			{
				lsMensajes.Remove(result);
				NotificationService.Notify(NotificationSeverity.Success, "Ok", "Borrado Correctamente");
			}
			else
			{
				MensajesServices.Actualizar(mensaje);
			}
			
			await grid.Reload();

		}
		public async void Borrar()
		{
			bool? result = false;
			if (selectedMensajes.Count > 0)
			{
				if(selectedMensajes.Count == 1)
					result = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar el Correo?");
				else
					result = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar los Correo?");
				if(result == true)
				{
					MensajesServices.Eliminar((List<Mensaje>)selectedMensajes);
					NotificationService.Notify(NotificationSeverity.Success, "Ok", "Eliminado Correctamente");
				}
				else
				{
					return;
				}
				
			}
			else{
				NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de seleccionar algún Mensaje");
			}
			foreach(var m in selectedMensajes)
			{
				lsMensajes.Remove(m);
			}
			await grid.Reload();
			selectedMensajes.Clear();
			
			
			
		}
	}
}
