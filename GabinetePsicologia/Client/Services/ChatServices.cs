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
using Radzen;

namespace GabinetePsicologia.Client.Services
{
    public class ChatServices
	{
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;


        public ChatServices( HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
			_navigationManager = navigationManager;
            
        }

		public async Task<List<ChatDto>> GetMessages(string id)
		{
			var result = await _httpClient.GetFromJsonAsync<List<ChatDto>>($"/Chat/{id}");
            return result;
		}
		public async void Send(ChatDto chat)
		{
			var result = await _httpClient.PostAsJsonAsync<ChatDto>($"/Chat",chat);
			
		}
	}
}
