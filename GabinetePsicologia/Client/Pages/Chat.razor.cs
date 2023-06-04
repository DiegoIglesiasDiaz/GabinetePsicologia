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
		public bool isFirstSend = false;
		public bool isRemove = false;
		public bool isUserModal = false;
		public string correo = "";
		public string NombreChat = "";
		public string IdChat;
		public ChatDto NewChat = new ChatDto();

		private ClaimsPrincipal? userClaim;
		private PersonaDto user;
		public List<ChatDto> LsChats;
		public List<ChatDto> LsAllChats;
		public List<KeyValue> LsPeople = new List<KeyValue>();
		public List<KeyValue> LsAllPeople = new List<KeyValue>();
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
						await Connect();
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
			await hubConnection.StartAsync();
		}
		private async Task SendSignalR()
		{
			if (hubConnection != null)
			{
				var FromTo = user.Id.ToString() + ";" + IdChat + ";"+NombreChat;
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
					Message = message,
					Date = DateTime.Now,
					FromName = NombreChat,
					IdFrom = FromUser

				};
				if (FromUser == IdChat)
				{
					LsChats.Add(msg);
					jSRuntime.InvokeVoidAsync("BajarScroll");
				}
				LsAllChats.Add(msg);
				if(!LsPeople.Where(x=> x.Key == FromUser).Any())
				{
					LsPeople.Add(new KeyValue { Key = FromName, Value = FromUser });
					OrdenarPersonas();
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
				if (!LsPeople.Where(x => x.Value == IdChat).Any())
				{
					isFirstSend = true;
					LsPeople.Add(new KeyValue { Key = NombreChat, Value = IdChat });

				}
				if (isFirstSend)
					OrdenarPersonas();
				ChatServices.Send(NewChat);
				NewChat = new ChatDto();
				//jSRuntime.InvokeVoidAsync("active", IdChat);
			
			}

		}
		public void selectChat(string id, string Nombre)
		{
			jSRuntime.InvokeVoidAsync("RemoveNewChat");

			IdChat = id;
			NombreChat = Nombre;
			//jSRuntime.InvokeVoidAsync("active", id);
			LsChats = LsAllChats.Where(x => x.IdFrom == id || x.IdTo == id).OrderBy(x => x.Date).ToList();
			jSRuntime.InvokeVoidAsync("BajarScroll");
			jSRuntime.InvokeVoidAsync("FillPage");
			isFirstSend = true;
			if (isUserModal)
				DialogService.Close(true);
		}
		public void CreateChat(object args)
		{
			if (args != null)
			{
				var id = args.ToString();
				var Nombre = LsAllPeople.FirstOrDefault(x => x.Value == id).Key;
				LsChats = LsAllChats.Where(x => x.IdFrom == id || x.IdTo == id).OrderBy(x => x.Date).ToList() ?? new List<ChatDto>();
				IdChat = id;
				NombreChat = Nombre;
				if (!LsPeople.Where(x => x.Value == id).Any())
				{
					jSRuntime.InvokeVoidAsync("AddPerson", Nombre);
					
				}
				jSRuntime.InvokeVoidAsync("BajarScroll");
				jSRuntime.InvokeVoidAsync("FillPage");
				if (isUserModal)
					DialogService.Close(true);
					
			}
		}
		private void OrdenarPersonas()
		{
			LsAllChats = LsAllChats.OrderByDescending(x => x.Date).ToList();
			LsPeople.Clear();
			foreach (var chat in LsAllChats)
			{
				if (chat.IdFrom != user.Id.ToString())
				{
					if (!LsPeople.Where(x => x.Value == chat.IdFrom).Any())
						LsPeople.Add(new KeyValue { Key = chat.FromName, Value = chat.IdFrom });
				}
				else
				{
					if (!LsPeople.Where(x => x.Value == chat.IdTo).Any())
						LsPeople.Add(new KeyValue { Key = chat.ToName, Value = chat.IdTo });
				}

			}

		}
		public void BorrarChat()
		{

			LsPeople.Remove(LsPeople.FirstOrDefault(x => x.Value == IdChat));
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
			LsPeople.Remove(LsPeople.FirstOrDefault(x => x.Value == IdChat));
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
	}
}
