using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net.Http.Json;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Index
    {
        [Inject] private UsuarioServices UsuarioServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NotificationService NotificationService { get; set; }
       
        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user != null &&( user.IsInRole("Administrador") || user.IsInRole("Psicologo") || user.IsInRole("Paciente")))
            {
                PersonaDto userDto = await UsuarioServices.getPersonaByUsername(user.Identity.Name);
                if(userDto.Contraseña == null)
                {
                    var result = await DialogService.OpenAsync<IndexModal>("Información Adicional", new Dictionary<string, object> { { "Persona", userDto }});
                    if (result)
                        NotificationService.Notify(NotificationSeverity.Success, "Ok", "Datos Guardado Correctamente");
                }
            }
           
        }

        void OnParentClicked(MenuItemEventArgs args)
        {
        }

        void OnChildClicked(MenuItemEventArgs args)
        {
            
        }
    }
}
