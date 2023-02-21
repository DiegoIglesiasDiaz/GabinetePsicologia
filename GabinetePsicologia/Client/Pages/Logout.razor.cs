using GabinetePsicologia.Client.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Logout 
    {
        [Inject] private UsuarioServices UsuarioServices { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await UsuarioServices.Logout();
          


        }
    }
}
