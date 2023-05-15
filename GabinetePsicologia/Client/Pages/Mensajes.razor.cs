using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen.Blazor;
using System.Net.Http;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Mensajes
    {
		[Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] MensajesServices MensajesServices { get; set; }
		IList<Mensaje> selectedMensajes;
		IList<Mensaje> lsMensajes;
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
	}
}
