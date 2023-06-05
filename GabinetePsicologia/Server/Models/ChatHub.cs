using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Server.Models
{
	public class ChatHub : Hub
	{
		private static List<string> connectedUsers = new List<string>();
		
	
		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			connectedUsers.Add(Context.User.Identity.Name);
			await Clients.All.SendAsync("ConnectedUser", connectedUsers);
			//await SendMessage("online", "");
		}
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			//await SendMessage("online", "");
			await base.OnDisconnectedAsync(exception);
			connectedUsers.Remove(Context.User.Identity.Name);
			await Clients.All.SendAsync("ConnectedUser", connectedUsers);
		}
		public async Task SendMessage(string fromTo, string message)
		{
			//intentar hacer con Clients.User
			await Clients.All.SendAsync("ReceiveMessage", fromTo, message);
			await Clients.All.SendAsync("NotificationMessage", fromTo, message);
		}
		public async Task GetConnectedUsers()
		{
			await Clients.All.SendAsync("ConnectedUser", connectedUsers);
		}



	}
}
