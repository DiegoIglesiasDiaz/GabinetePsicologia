using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Services
{
    public class MensajesServices
	{
        private readonly HttpClient _httpClientAnonymous;
        private readonly IHttpClientFactory _ClientFactory;
    

        public MensajesServices(HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
			_httpClientAnonymous = _ClientFactory.CreateClient("public");


		}

        public async void Enviar(Mensaje mensaje)
        {
            await _httpClientAnonymous.PostAsJsonAsync<Mensaje>("/Mensaje",mensaje);
        }
    }
}
