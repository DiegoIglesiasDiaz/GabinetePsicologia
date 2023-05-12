using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Services
{
    public class CitasServices
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;


        public CitasServices( HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;
            
        }


        public async Task<List<Cita>> GetCitas()
        {
            var paciente = await _httpClient.GetFromJsonAsync<List<Cita>>("/Cita");
            return paciente;

        }
        public async Task<List<Cita>> GetCitasByPsicologoId(Guid id)
        {
            var paciente = await _httpClient.GetFromJsonAsync<List<Cita>>($"/Cita/Psicologo/{id}");
            return paciente;

        }
        public async Task<List<Cita>> GetCitasByPacienteId(Guid id)
        {
            var paciente = await _httpClient.GetFromJsonAsync<List<Cita>>($"/Cita/Paciente/{id}");
            return paciente;

        }
        public async void InsertCita(Cita cita)
        {
            await _httpClient.PostAsJsonAsync("/Cita", cita);
        }
        public async void ActualizarCita(Cita cita)
        {
            await _httpClient.PostAsJsonAsync("/Cita/Actualizar", cita);
            //await _httpClient.PostAsJsonAsync("/Cita/Eliminar", cita);
            //await _httpClient.PostAsJsonAsync("/Cita", cita);

        }
        public async void EliminarCita(Cita cita)
        {
            await _httpClient.PostAsJsonAsync("/Cita/Eliminar", cita);
        }
    }
}
