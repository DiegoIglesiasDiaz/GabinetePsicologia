using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GabinetePsicologia.Client.Services
{
    public class UsuarioServices
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;


        public UsuarioServices(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;

        }
        public async Task<bool> RegisterPaciente(PersonaDto data)
        {
           var result =  await _httpClient.PostAsJsonAsync("/Usuario", data);
            return result.IsSuccessStatusCode;
           
        }

        //public async Task<List<Usuarios>> getUsuarios()
        //{
        //    List<Usuarios> a = await _httpClient.GetFromJsonAsync<List<Usuarios>>("/Usuario");
        //    return a;
        //}
        public async Task<List<PersonaDto>> getPersonas()
        {
            List<PersonaDto> a = await _httpClient.GetFromJsonAsync<List<PersonaDto>>("/Usuario/Persona");
            return a;

        }
        public async Task Logout()
        {
            var a = await _httpClient.GetStringAsync("/Usuario/Logout");
            _navigationManager.NavigateTo("/", true);


        }
        public async void BorrarUsuarios(IList<PersonaDto> persona)
        {
            await _httpClient.PostAsJsonAsync("/Usuario/Borrar", persona);

        }
    }
}
