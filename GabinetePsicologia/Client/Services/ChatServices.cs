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

		public async Task<List<ChatDto>> GetMessages(string Userid,string ChatId)
		{
			var query = Userid + ";" + ChatId;
			var result = await _httpClient.GetFromJsonAsync<List<ChatDto>>($"/Chat/{query}");
            return result;
		}
		public async void Send(ChatDto chat)
		{
			var result = await _httpClient.PostAsJsonAsync<ChatDto>($"/Chat",chat);
			
		}
		public async Task<List<ChatPerson>> GetChatedPeople(string id)
		{			
			var result = await _httpClient.GetFromJsonAsync<List<ChatPerson>>($"/Chat/ChatedPeople/{id}");
			return result;
		}

		public async Task<List<ChatDto>> GetAllMessages(string id)
		{
			var result = await _httpClient.GetFromJsonAsync<List<ChatDto>>($"/Chat/AllMessages/{id}");
			return result;
		}
		public async Task<List<ChatPerson>> GetAllPeople(string id)
		{
			var result = await _httpClient.GetFromJsonAsync<List<ChatPerson>>($"/Chat/AllPeople/{id}");
			return result;
		}
		public  void Remove(string id, string id2)
		{
			var query = id+ ";" + id2;	
			_httpClient.GetAsync($"/Chat/remove/{query}");
			
		}
		public void ViewMessage(string idFrom, string IdTo)
		{
			string query = idFrom + ";" + IdTo;
			
			_httpClient.GetAsync($"/Chat/View/{query}");

		}
		public async Task<bool> hasNonViewMessage(string id)
		{

			string result = await _httpClient.GetStringAsync($"/Chat/NonViewMessage/{id}");
			if(result != null && result== "True") {
				return true;
			}
			else
			{
				return false;
			}
			
		}
	}
}
