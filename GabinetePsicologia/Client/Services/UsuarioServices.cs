using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Services
{
    public class UsuarioServices
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
    

        public UsuarioServices( HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
         
        }


        public async Task Logout()
        {
            var a = await _httpClient.GetStringAsync("/Usuario/Logout");
            _navigationManager.NavigateTo("/", true);


        }
    }
}
