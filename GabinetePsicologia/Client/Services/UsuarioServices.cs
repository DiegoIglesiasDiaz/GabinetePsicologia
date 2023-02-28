using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using System;
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
        private readonly IHttpClientFactory _ClientFactory;
        private readonly HttpClient _httpClientAnonymous;
        public UsuarioServices(HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory ClientFactory)
        {
            _ClientFactory = ClientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;
            _httpClientAnonymous = _ClientFactory.CreateClient("public");
        }
        public async Task<bool> RegisterPersona(PersonaDto data)
        {

            var result = await _httpClient.PostAsJsonAsync("/Usuario", data);
            return result.IsSuccessStatusCode;

        }
        public async Task<bool> RegisterPersonaAnonymous(PersonaDto data)
        {

            var result = await _httpClientAnonymous.PostAsJsonAsync("/Usuario", data);
            return result.IsSuccessStatusCode;

        }
        public async void EditarPaciente(PersonaDto data)
        {
            await _httpClient.PostAsJsonAsync("/Usuario/Editar", data);


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
        public async Task<bool> CambiarCorreo(string correoAntiguo, string newCorreo)
        {
            string[] correos = new string[] { correoAntiguo, newCorreo };
            var result = await _httpClient.PostAsJsonAsync("/Usuario/CambiarCorreo", correos);
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Login(LoginDto usuario)
        {

            var result = await _httpClientAnonymous.PostAsJsonAsync("/Usuario/Login", usuario);
            return result.IsSuccessStatusCode;
        }
        public async Task<bool> CambiarContraseña(string passwd,string correo)
        {
           string[] data = new string[] { passwd, correo };
           var result = await _httpClient.PostAsJsonAsync("/Usuario/CambiarContraseña", data);
           return result.IsSuccessStatusCode;
        }
    }
}