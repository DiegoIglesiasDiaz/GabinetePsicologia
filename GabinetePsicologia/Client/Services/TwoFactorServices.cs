using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Xml.Linq;

namespace GabinetePsicologia.Client.Services
{
    public class TwoFactorServices
	{
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;


        public TwoFactorServices( HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;
            
        }
        public async Task<bool> Enable2FA(string code, string correo)
        {
            string query = code + ";" + correo;

			var result = await _httpClient.GetFromJsonAsync<bool>($"/TwoFactor/code/{query}");
            return result;

        }
		public async Task<bool> Reset2FA(string correo)
		{
			var result = await _httpClient.GetFromJsonAsync<bool>($"/TwoFactor/ResetCode/{correo}");
            return result;
		}
		public async Task<string[]> GetSharedAndQr(string correo)
		{
			var result = await _httpClient.GetFromJsonAsync<string[]>($"/TwoFactor/{correo}");
            return result;
		}
	}
}
