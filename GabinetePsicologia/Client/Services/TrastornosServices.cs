using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Services
{
    public class TrastornosServices
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
    

        public TrastornosServices( HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
         
        }


        public async Task<List<Trastorno>> getTrastornos()
        {
            var trastornos = await _httpClient.GetFromJsonAsync<List<Trastorno>>("/Trastorno");
            return trastornos;


        }
        public async void AñadirTrastorno(Trastorno Trastorno)
        {
            await _httpClient.PostAsJsonAsync<Trastorno>("/Trastorno",Trastorno);
        }
    }
}
