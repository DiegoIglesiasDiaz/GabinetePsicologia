using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Services
{
    public class PacientesServices
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;


        public PacientesServices( HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;
            _ClientFactory = clientFactory;
        }


        public async Task<List<Paciente>> getPacientes()
        {
            var paciente = await _httpClient.GetFromJsonAsync<List<Paciente>>("/Paciente");
            return paciente;

        }
        public async void RegisterPaciente(Paciente paciente)
        {
            await _httpClient.PostAsJsonAsync<Paciente>("/Paciente", paciente);
        }

        internal async Task<Paciente> GetPacienteByUsername(string username)
        {
            var paciente = await _httpClient.GetFromJsonAsync<Paciente>($"/Paciente/Username/{username}");
            return paciente;
        }
    }
}
