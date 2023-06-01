using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;
using Microsoft.JSInterop;

namespace GabinetePsicologia.Client.Pages
{
	public partial class Chat
	{
		[Inject] private NotificationService NotificationService { get; set; }
		[Inject] private DialogService DialogService { get; set; }
		[Inject] private IJSRuntime jSRuntime { get; set; }
		[Inject] NavigationManager NavigationManager { get; set; }
		[Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] UsuarioServices UsuarioServices { get; set; }
		[Inject] TwoFactorServices TwoFactorServices { get; set; }
		public bool isInRole = false;
		public string correo = "";
		public ChatDto NewChat = new ChatDto();
		private ClaimsPrincipal? user;
		public List<ChatDto> LsChats = new List<ChatDto> {
				new ChatDto { Date = DateTime.Now,Message="Te Amo" },
				new ChatDto { Date = DateTime.Now,Message="Yo a ti más"},
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" },
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" },
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" },
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" },
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" },
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" },
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" },
				new ChatDto {Date = DateTime.Now,Message="GUAPAAA que te amo micho, hoy nos tatuamos, que guayy" }
			};
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
			correo = user.Identity.Name;
			if (user.IsInRole("Paciente") || user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
			{
				isInRole = true;
				await Task.Delay(500);
				await jSRuntime.InvokeVoidAsync("BajarScroll");
			}
			
		}
		public void Send()
		{
			 jSRuntime.InvokeVoidAsync("BajarScroll");
			if (!String.IsNullOrWhiteSpace(NewChat.Message))
			{
				NewChat.Id = Guid.NewGuid();
				NewChat.IdFrom = Guid.NewGuid().ToString();
				NewChat.IdTo = Guid.NewGuid().ToString();
				NewChat.Date = DateTime.Now;
				LsChats.Add(NewChat);
				NewChat = new ChatDto();
			}
		}
	}
}
