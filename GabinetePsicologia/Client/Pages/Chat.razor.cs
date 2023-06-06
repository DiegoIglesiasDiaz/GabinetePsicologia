using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using Radzen;
using Microsoft.AspNetCore.Components.Authorization;
using GabinetePsicologia.Client.Services;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;
using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.AspNet.SignalR;

using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks.Dataflow;

namespace GabinetePsicologia.Client.Pages
{
	public partial class Chat
	{
		[Inject] private NotificationService NotificationService { get; set; }
		[Inject] DialogService DialogService { get; set; }
		[Inject] private IJSRuntime jSRuntime { get; set; }
		[Inject] NavigationManager NavigationManager { get; set; }
		[Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }
		[Inject] UsuarioServices UsuarioServices { get; set; }
		[Inject] ChatServices ChatServices { get; set; }
		[Inject] TwoFactorServices TwoFactorServices { get; set; }

		private HubConnection? hubConnection;
		private string messages = String.Empty;
		public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

		public bool isInRole = false;
		
		public bool isRemove = false;
		public string correo = "";
		public string NombreChat = "";
		public string IdChat;
		public ChatDto NewChat = new ChatDto();

		private ClaimsPrincipal? userClaim;
		private PersonaDto user;
		public List<ChatDto> LsChats;
		public List<ChatDto> LsAllChats;
		public List<ChatPerson> LsPeople = new List<ChatPerson>();
		public List<ChatPerson> LsAllPeople = new List<ChatPerson>();
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			userClaim = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
			correo = userClaim.Identity.Name;

			if (userClaim.IsInRole("Paciente") || userClaim.IsInRole("Psicologo") || userClaim.IsInRole("Administrador"))
			{
				isInRole = true;
				if (correo != null)
				{
					user = await UsuarioServices.getPersonaByUsername(correo);
					if (user != null)
					{
						LsPeople = await ChatServices.GetChatedPeople(user.Id.ToString());
						LsAllPeople = await ChatServices.GetAllPeople(user.Id.ToString());
						LsAllChats = await ChatServices.GetAllMessages(user.Id.ToString());
						if (LsPeople != null && LsAllPeople != null && LsChats != null)
							OrdenarPersonas();
						try
						{
							await Connect();
						}
						catch (Exception ex)
						{
							NavigationManager.NavigateTo("/Chat", true);
						}

					}
				}
			}

		}
		private async Task Connect()
		{

			hubConnection = new HubConnectionBuilder()
							.WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
							.Build();

			hubConnection.On<string, string>("ReceiveMessage", HandleReceivedMessage);
			hubConnection.On<List<string>>("ConnectedUser", HandleConnectedUsers);
			await hubConnection.StartAsync();
		}
		private async Task SendSignalR()
		{
			if (hubConnection != null)
			{
				var FromTo = user.Id.ToString() + ";" + IdChat + ";" + user.FullName;
				await hubConnection.SendAsync("SendMessage", FromTo, NewChat.Message);

			}
		}
		public async ValueTask DisposeAsync()
		{
			if (hubConnection != null)
			{
				await hubConnection.DisposeAsync();
			}
		}
		private void HandleConnectedUsers(List<string> users)
		{
		
			foreach(var user in users)
			{
				if(LsAllPeople.Where(x=> x.UserName == user).Any())
				{
					LsAllPeople.FirstOrDefault(x => x.UserName == user).isOnline = true;
				}
				if (LsPeople.Where(x => x.UserName.ToLower() == user.ToLower()).Any())
				{
					LsPeople.FirstOrDefault(x => x.UserName == user).isOnline = true;
				}
			}
			StateHasChanged();
		}

		private void HandleReceivedMessage(string usr, string message)
		{

			var split = usr.Split(";");
			var FromUser = split[0];
			var ToUser = split[1];
			var FromName = split[2];
			if (ToUser == user.Id.ToString())
			{

				var msg = new ChatDto
				{
					Id = Guid.NewGuid(),
					Message = message,
					Date = DateTime.Now,
					FromName = FromName,
					IdFrom = FromUser,
					IdTo = ToUser,
					View = false

				};
				if (FromUser == IdChat)
				{
					msg.View = true;
					LsChats.Add(msg);
					jSRuntime.InvokeVoidAsync("BajarScroll");

				}

				LsAllChats.Add(msg);
				if (!LsPeople.Where(x => x.Id == FromUser).Any())
				{
					LsPeople.Add(new ChatPerson { Name = FromName, Id = FromUser, hasNotViewMessage = true, lastMessage = DateTime.Now , isOnline = true});

				}
				else
				{
					LsPeople.FirstOrDefault(x => x.Id == FromUser).hasNotViewMessage = true;
					LsPeople.FirstOrDefault(x => x.Id == FromUser).lastMessage = DateTime.Now ;
				}
				OrdenarPersonas();
				if (FromUser == IdChat)
				{
					Thread.Sleep(100);
					ChatServices.ViewMessage(msg.IdFrom, msg.IdTo);
					jSRuntime.InvokeVoidAsync("BajarScroll");
				}
				
			}

			StateHasChanged();
		}
		public void Send()
		{

			jSRuntime.InvokeVoidAsync("RemoveNewChat");
			if (!String.IsNullOrWhiteSpace(NewChat.Message))
			{
				NewChat.Id = Guid.NewGuid();
				NewChat.IdFrom = user.Id.ToString();
				NewChat.FromName = user.FullName;
				NewChat.IdTo = IdChat;
				NewChat.ToName = NombreChat;
				NewChat.Date = DateTime.Now;
				NewChat.View = false;
				SendSignalR();
				LsChats.Add(NewChat);
				LsAllChats.Add(NewChat);
				jSRuntime.InvokeVoidAsync("BajarScroll");
				if (!LsPeople.Where(x => x.Id == IdChat).Any())
				{
					
					LsPeople.Add(new ChatPerson { Name = NombreChat, Id = IdChat, lastMessage = DateTime.Now });

				}
				else
				{
					LsPeople.FirstOrDefault(x => x.Id == IdChat).lastMessage = DateTime.Now;
				}
				
				OrdenarPersonas();
				ChatServices.Send(NewChat);
				NewChat = new ChatDto();
				jSRuntime.InvokeVoidAsync("active", IdChat);

			}

		}
		public void selectChat(string id, string Nombre)
		{
			jSRuntime.InvokeVoidAsync("RemoveNewChat");
			IdChat = id;
			NombreChat = Nombre;
		
			LsChats = LsAllChats.Where(x => x.IdFrom == id || x.IdTo == id).OrderBy(x => x.Date).ToList();
			jSRuntime.InvokeVoidAsync("FillPage");
			//jSRuntime.InvokeVoidAsync("active", id);
			
			LsPeople.FirstOrDefault(x => x.Id == id).hasNotViewMessage = false;
			LsAllPeople.FirstOrDefault(x => x.Id == id).hasNotViewMessage = false;
			var chats = LsAllChats.Where(x => x.IdFrom == id).ToList();
			foreach (var ch in chats)
			{
				ch.View = true;
			}
			ChatServices.ViewMessage(id, user.Id.ToString());
			jSRuntime.InvokeVoidAsync("BajarScrollTime");

			if(LsAllChats.Where(x=>  x.IdTo == user.Id.ToString() && x.IdFrom == id).Any())
				jSRuntime.InvokeVoidAsync("MessageOnHide");
			
		}
		public void CreateChat(object args)
		{
			if (args != null)
			{
				var id = args.ToString();
				var Nombre = LsAllPeople.FirstOrDefault(x => x.Id == id).Name;
				LsChats = LsAllChats.Where(x => x.IdFrom == id || x.IdTo == id).OrderBy(x => x.Date).ToList() ?? new List<ChatDto>();
				IdChat = id;
				NombreChat = Nombre;
				if (!LsPeople.Where(x => x.Id == id).Any())
				{
					jSRuntime.InvokeVoidAsync("AddPerson", Nombre);

				}
				else
				{
					LsPeople.FirstOrDefault(x => x.Id == id).hasNotViewMessage = false;
					ChatServices.ViewMessage(id, user.Id.ToString());
					if (LsAllChats.Where(x => x.View == false && x.IdTo == user.Id.ToString()).Any())
						jSRuntime.InvokeVoidAsync("MessageOnHide");
					
				}
				jSRuntime.InvokeVoidAsync("FillPage");
				jSRuntime.InvokeVoidAsync("BajarScroll");
			}
		}
		private void OrdenarPersonas()
		{
			LsPeople = LsPeople.OrderByDescending(x => x.lastMessage).ToList();

		}
		public void BorrarChat()
		{

			LsPeople.Remove(LsPeople.FirstOrDefault(x => x.Id == IdChat));
			LsChats = null;
			var remove = LsAllChats.Where(x => x.IdFrom == IdChat);
			foreach (var rm in remove)
			{
				LsAllChats.Remove(rm);
			}
			IdChat = null;
			NombreChat = "";
		}
		public void Si()
		{
			LsPeople.Remove(LsPeople.FirstOrDefault(x => x.Id == IdChat));
			LsChats = null;
			var remove = LsAllChats.Where(x => x.IdFrom == IdChat || (x.IdFrom == user.Id.ToString() && x.IdTo == IdChat)).ToList();
			foreach (var rm in remove)
			{
				LsAllChats.Remove(rm);
			}

			ChatServices.Remove(user.Id.ToString(), IdChat);
			IdChat = null;
			NombreChat = "";
			NotificationService.Notify(NotificationSeverity.Success, "Ok", "Borrado Correctamente");
			DialogService.Close(true);

		}
		public void Volver()
		{
			LsChats = null;
			jSRuntime.InvokeVoidAsync("SubirScroll");
		}
	}
}
