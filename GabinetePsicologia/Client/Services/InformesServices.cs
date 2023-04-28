using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GabinetePsicologia.Client.Services
{
    public class InformesServices
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;
        public InformesServices(HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;

        }
        public async Task<List<Informe>> GetInformes()
        {
            var inf = await _httpClient.GetFromJsonAsync<List<Informe>>("/Informe");
            return inf;

        }

    }
}
