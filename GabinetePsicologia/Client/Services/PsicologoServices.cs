using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Services
{
    public class PsicologoServices
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;


        public PsicologoServices( HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;
            _ClientFactory = clientFactory;
        }


        public async Task<List<Psicologo>> getPsicologos()
        {
            var psicologo = await _httpClient.GetFromJsonAsync<List<Psicologo>>("/Psicologo");
            return psicologo;

        }
        public async Task<Psicologo> GetPsicologoByUsername(string Username)
        {
            var psicologo = await _httpClient.GetFromJsonAsync<Psicologo>($"/Psicologo/Username/{Username}");
            return psicologo;

        }
        public async void UpdatePaciente(Psicologo psicologo)
        {
            await _httpClient.PostAsJsonAsync("/Psicologo/Update", psicologo);
        }

        public async Task<Psicologo> getPsicologoById(Guid id)
        {
            var psicologo = await _httpClient.GetFromJsonAsync<Psicologo>($"/Psicologo/{id}");
            return psicologo;
        }
    }
}
