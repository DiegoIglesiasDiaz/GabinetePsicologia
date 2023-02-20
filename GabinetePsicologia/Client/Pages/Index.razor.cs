using GabinetePsicologia.Shared;
using System.Net.Http.Json;

namespace GabinetePsicologia.Client.Pages
{
    public partial class Index
    {
        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user != null && user.IsInRole("Administrador"))
            {
                
            }

        }
    }
}
