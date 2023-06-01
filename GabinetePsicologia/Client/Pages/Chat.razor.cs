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
		[Inject] ChatServices ChatServices { get; set; }
		[Inject] TwoFactorServices TwoFactorServices { get; set; }
		public bool isInRole = false;
		public string correo = "";
		public ChatDto NewChat = new ChatDto();
		private ClaimsPrincipal? userClaim;
		private PersonaDto user;
		public List<ChatDto> LsChats = new List<ChatDto>();
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			userClaim = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
			correo = userClaim.Identity.Name;
			if (userClaim.IsInRole("Paciente") || userClaim.IsInRole("Psicologo") || userClaim.IsInRole("Administrador"))
			{
				isInRole = true;
				if(correo != null)
				{
					user = await UsuarioServices.getPersonaByUsername(correo);
					LsChats = await ChatServices.GetMessages(user.Id.ToString());
					await Task.Delay(500);
				await jSRuntime.InvokeVoidAsync("BajarScroll");
				}
			}
			
		}
		public void Send()
		{
			 jSRuntime.InvokeVoidAsync("BajarScroll");
			if (!String.IsNullOrWhiteSpace(NewChat.Message))
			{
				NewChat.Id = Guid.NewGuid();
				NewChat.IdFrom = user.Id.ToString();
				NewChat.FromName = user.FullName;
				NewChat.IdTo = Guid.Empty.ToString();
				NewChat.ToName = "Antnio Garcia Lopez";
				NewChat.Date = DateTime.Now;
				NewChat.View = false;
				
				LsChats.Add(NewChat);
				ChatServices.Send(NewChat);	
				NewChat = new ChatDto();
				
			}
		}
	}
}
