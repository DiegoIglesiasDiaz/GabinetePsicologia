using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
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
        private readonly NavigationManager NavigationManager;
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
        public async Task<bool> CheckPasswd(string correo, string passwd)
        {
            string[] data = new string[] { correo, passwd };
            var result = await _httpClient.PostAsJsonAsync("/Usuario/CheckPasswd", data);
            return result.IsSuccessStatusCode;

        }
        public async Task<bool> BorrarCuenta(Guid id)
        {
            var result = await _httpClient.DeleteAsync($"/Usuario/{id}");
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
        public async Task<PersonaDto> getPersonaByUsername(string username)
        {
            PersonaDto a = await _httpClient.GetFromJsonAsync<PersonaDto>($"/Usuario/Persona/{username}");
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
        public async Task<bool> CambiarTelefono(string telefono,string correo)
        {
           string[] data = new string[] { telefono, correo };
           var result = await _httpClient.PostAsJsonAsync("/Usuario/CambiarTelefono", data);
           return result.IsSuccessStatusCode;
        }
        public void ExternalLogin(string Provider)
        {


            var result = _httpClientAnonymous.GetFromJsonAsync<ChallengeResult>($"/Usuario/ExternalLogin/{Provider}");
            //_navigationManager.NavigateTo("https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id=488885295151-2uif611ukrii2nlsd8spd5vconu09gl4.apps.googleusercontent.com&redirect_uri=https%3A%2F%2Flocalhost%3A7112%2Fsignin-google&scope=openid%20profile%20email&state=CfDJ8CV2pE7oYEtHr3ZItb2Jvpyt8k_0Wxy6qr4I1aRJuZePNV20-dW8a7Oj38pNsM7LUXf7Jk6dEEbQAfNJMUxPx6H7mfYAZL72tWErJ-pdlAnYiG8iHE53DKScFR92edoTHgkQ8U869mFDE7gaSDdqCzPjmfdbs5f_pARsP3bocgsXeiNeUZqjIIwb9m9NvSoR1PyEn3-Adbe_0Uv5AJ2GQ0c8H9U1R8jxtCJWw_Nluybbhcgl44U8UU-9M8UbsmhZiA", true);
        }
    }
}