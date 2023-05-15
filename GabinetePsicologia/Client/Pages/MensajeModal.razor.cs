using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class MensajeModal
    {
        [Parameter]
        public Mensaje Mensaje { get; set; }
		[Inject] MensajesServices MensajesServices { get; set; }
		public string mailto = "mailto:";
		protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
          
        }

        protected async override void OnParametersSet()
        {
			if (Mensaje != null)
            {
                mailto +=  Mensaje.Correo;
			}	  
		}
        public async void Borrar()
        {
			bool? result = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar este Correo?");
            if(result == true)
            {
				var mensajes = new List<Mensaje>() { Mensaje };
				MensajesServices.Eliminar(mensajes);
				DialogService.Close(Mensaje);
			}
			
        }
    }

}
