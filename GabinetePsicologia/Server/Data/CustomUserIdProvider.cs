using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Server.Controllers;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabinetePsicologia.Server.Data
{
	public class CustomUserIdProvider: IUserIdProvider
	{
		[Inject] private UsuarioController UsuarioController { get; set; }


		public string? GetUserId(HubConnectionContext connection)
		{
			var userId = UsuarioController;
			return userId.ToString();
		}
	}
}
