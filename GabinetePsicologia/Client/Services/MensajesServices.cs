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
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
    

        public MensajesServices(HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
			_httpClientAnonymous = _ClientFactory.CreateClient("public");
			_httpClient = _ClientFactory.CreateClient("private");


		}

        public async void Enviar(Mensaje mensaje)
        {
            await _httpClientAnonymous.PostAsJsonAsync<Mensaje>("/Mensaje",mensaje);
        }
		public async void Actualizar(Mensaje mensaje)
		{
			await _httpClient.PostAsJsonAsync<Mensaje>("/Mensaje/Actualizar", mensaje);
		}
		public async void Eliminar(List<Mensaje> mensajes)
		{
			await _httpClient.PostAsJsonAsync<List<Mensaje>>("/Mensaje/Delete", mensajes);
		}
		public async Task<List<Mensaje>> Get()
		{
			var a = await _httpClient.GetFromJsonAsync<List<Mensaje>>("/Mensaje");
			return a;
		}
	}
}
