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
            var paciente = await _httpClient.GetFromJsonAsync<List<Psicologo>>("/Psicologo");
            return paciente;

        }

    }
}
