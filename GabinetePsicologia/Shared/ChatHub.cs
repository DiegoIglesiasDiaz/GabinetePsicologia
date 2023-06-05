using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Shared
{
	public class ChatHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			//await SendMessage("TestUser", "Se ha conectado.");
		}
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			//await SendMessage("TestUser", "Se ha desconectado.");
			await base.OnDisconnectedAsync(exception);
		}
		public async Task SendMessage(string fromTo, string message)
		{
			//intentar hacer con Clients.User
			await Clients.All.SendAsync("ReceiveMessage", fromTo, message);
			await Clients.All.SendAsync("NotificationMessage", fromTo, message);
		}
	}
}
