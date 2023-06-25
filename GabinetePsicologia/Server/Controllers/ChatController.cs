using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Cita = GabinetePsicologia.Shared.Cita;

namespace GabinetePsicologia.Server.Controllers
{
	[Auth]
    [Route("[controller]")]
	[ApiController]
   

    public class ChatController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder, SignInManager<ApplicationUser> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;

		}


		[HttpGet("{query}")]
		public List<ChatDto> GetMessages(string query)
		{
			var split = query.Split(";");
			var UserId = split[0];
			var ChatId = split[1];
			return _context.Chat.Where(x => (x.IdFrom == UserId && x.IdTo == ChatId) || (x.IdTo == UserId && x.IdFrom == ChatId)).OrderBy(x => x.Date).ToList();
		}
		[HttpGet("AllMessages/{id}")]
		public List<ChatDto> GetAllMessages(string id)
		{
			return _context.Chat.Where(x => x.IdFrom == id || x.IdTo == id).ToList();
		}
		[HttpPost]
		public void Send(ChatDto chat)
		{
			try
			{
				_context.Chat.Add(chat);
				var a = _context.SaveChanges();

			}
			catch (Exception ex)
			{

			}


		}
		[HttpGet("ChatedPeople/{id}")]
		public List<ChatPerson> GetChatedPeople(string id)
		{
			var People = new List<ChatPerson>();
			var LsTo = _context.Chat.Where(x => x.IdFrom == id && x.IdTo != id).OrderByDescending(x => x.Date).ToList();
			var Lsfrom = _context.Chat.Where(x => x.IdFrom != id && x.IdTo == id).OrderByDescending(x => x.Date).ToList();

			if (Lsfrom == null || Lsfrom.Count == 0)
			{

				if (LsTo == null || LsTo.Count == 0)
				{
					return new List<ChatPerson>();
				}
				else
				{
					foreach (var to in LsTo)
					{
						if (!People.Where(x => x.Id == to.IdTo).Any())
						{
							var tmp = _context.Users.FirstOrDefault(x => x.Id == to.IdTo);
							People.Add(new ChatPerson { Name = to.ToName, Id = to.IdTo, lastMessage = to.Date, UserName = tmp.UserName });
						}

					}
					foreach (var p in People)
					{
						if (_context.Chat.Where(x => x.IdFrom == p.Id && x.IdTo == id && !x.View).Any())
						{
							p.hasNotViewMessage = true;
						}
					}
					return People;
				}
			}
			else if (LsTo == null || LsTo.Count == 0)
			{
				foreach (var from in Lsfrom)
				{

					if (!People.Where(x => x.Id == from.IdFrom).Any())
					{
						var tmp = _context.Users.FirstOrDefault(x => x.Id == from.IdFrom);
						People.Add(new ChatPerson { Name = from.FromName, Id = from.IdFrom, lastMessage = from.Date, UserName = tmp.UserName });

					}

				}
				foreach (var p in People)
				{
					if (_context.Chat.Where(x => x.IdFrom == p.Id && x.IdTo == id && !x.View).Any())
					{
						p.hasNotViewMessage = true;
					}
				}
				return People;
			}
			else
			if (Lsfrom.First().Date >= LsTo.First().Date)
			{
				foreach (var from in Lsfrom)
				{
					if (!People.Where(x => x.Id == from.IdFrom).Any())
					{
						var tmp = _context.Users.FirstOrDefault(x => x.Id == from.IdFrom);
						People.Add(new ChatPerson { Name = from.FromName, Id = from.IdFrom, lastMessage = from.Date, UserName = tmp.UserName });

					}
				}
				foreach (var to in LsTo)
				{
					if (!People.Where(x => x.Id == to.IdTo).Any())
					{
						var tmp = _context.Users.FirstOrDefault(x => x.Id == to.IdTo);
						People.Add(new ChatPerson { Name = to.ToName, Id = to.IdTo, lastMessage = to.Date, UserName = tmp.UserName });
					}
				}

			}
			else
			{
				foreach (var to in LsTo)
				{
					if (!People.Where(x => x.Id == to.IdTo).Any())
					{
						var tmp = _context.Users.FirstOrDefault(x => x.Id == to.IdTo);
						People.Add(new ChatPerson { Name = to.ToName, Id = to.IdTo, lastMessage = to.Date, UserName = tmp.UserName });
					}
				}
				foreach (var from in Lsfrom)
				{
					if (!People.Where(x => x.Id == from.IdFrom).Any())
					{
						var tmp = _context.Users.FirstOrDefault(x => x.Id == from.IdFrom);
						People.Add(new ChatPerson { Name = from.FromName, Id = from.IdFrom, lastMessage = from.Date, UserName = tmp.UserName });

					}
				}
			}
			foreach (var p in People)
			{
				if (_context.Chat.Where(x => x.IdFrom == p.Id && x.IdTo == id && !x.View).Any())
				{
					p.hasNotViewMessage = true;
				}
			}

			return People;
		}
		[HttpGet("AllPeople/{id}")]
		public async Task<List<ChatPerson>> GetAllPeople(string id)
		{
			var People = new List<ChatPerson>();
			var user = _context.Users.FirstOrDefault(x => x.Id == id);
			if (user != null)
			{
				if (await _userManager.IsInRoleAsync(user, "Paciente"))
				{
					var LsPsicologos = _context.Psicologos.Where(x => x.ApplicationUserId != id).ToList();
					var LsAdmin = _context.Administradores.Where(x => x.ApplicationUserId != id).ToList();
					foreach (var ps in LsPsicologos)
					{
						if (!People.Where(x => x.Id == ps.ApplicationUserId).Any())
						{
							var tmp = _context.Users.FirstOrDefault(x => x.Id == ps.ApplicationUserId);
							People.Add(new ChatPerson { Name = ps.FullName, Id = ps.ApplicationUserId , UserName = tmp.UserName});
						}
							
					}
					foreach (var ad in LsAdmin)
					{
						if (!People.Where(x => x.Id == ad.ApplicationUserId).Any())
						{
							var tmp = _context.Users.FirstOrDefault(x => x.Id == ad.ApplicationUserId);
							People.Add(new ChatPerson { Name = ad.FullName, Id = ad.ApplicationUserId , UserName = tmp.UserName});
						}
							
					}
				}
				else
				{
					var LsPsicologos = _context.Psicologos.Where(x => x.ApplicationUserId != id).ToList();
					var LsAdmin = _context.Administradores.Where(x => x.ApplicationUserId != id).ToList();
					var LsPaciente = _context.Pacientes.Where(x => x.ApplicationUserId != id).ToList();
					foreach (var ps in LsPsicologos)
					{
						if (!People.Where(x => x.Id == ps.ApplicationUserId).Any())
						{
							var tmp = _context.Users.FirstOrDefault(x => x.Id == ps.ApplicationUserId);
							People.Add(new ChatPerson { Name = ps.FullName, Id = ps.ApplicationUserId, UserName = tmp.UserName });
						}
					}
					foreach (var ad in LsAdmin)
					{
						if (!People.Where(x => x.Id == ad.ApplicationUserId).Any())
						{
							var tmp = _context.Users.FirstOrDefault(x => x.Id == ad.ApplicationUserId);
							People.Add(new ChatPerson { Name = ad.FullName, Id = ad.ApplicationUserId, UserName = tmp.UserName });
						}
					}
					foreach (var pa in LsPaciente)
					{
						if (!People.Where(x => x.Id == pa.ApplicationUserId).Any())
						{
							var tmp = _context.Users.FirstOrDefault(x => x.Id == pa.ApplicationUserId);
							People.Add(new ChatPerson { Name = pa.FullName, Id = pa.ApplicationUserId , UserName = tmp.UserName});
						}
							
					}
				}
			}
			foreach (var p in People)
			{
				if (_context.Chat.Where(x => x.IdFrom == p.Id && x.IdTo == id && !x.View).Any())
				{
					p.hasNotViewMessage = true;
				}
			}

			return People;
		}
		[HttpGet("remove/{query}")]
		public void remove(string query)
		{
			var split = query.Split(";");
			var id = split[0];
			var id2 = split[1];
			var lsremove = _context.Chat.Where(x => x.IdFrom == id && x.IdTo == id2).ToList();
			lsremove.AddRange(_context.Chat.Where(x => x.IdFrom == id2 && x.IdTo == id).ToList());
			foreach (var rm in lsremove)
			{
				_context.Chat.Remove(rm);
			}
			_context.SaveChanges();
		}
		[HttpGet("View/{query}")]
		public void ViewMessage(string query)
		{
			var split = query.Split(";");
			var IdFrom = split[0];
			var IdTo = split[1];
			var lsmssg = _context.Chat.Where(x => x.IdFrom == IdFrom && x.IdTo == IdTo && x.View == false).ToList();
			foreach (var ms in lsmssg)
			{
				ms.View = true;
				_context.Chat.Update(ms);
			}
			_context.SaveChanges();

		}
		
		[HttpGet("NonViewMessage/{id}")]
        
        public string NonViewMessage(string id)
		{
			if(_context.Chat.Where(x => x.IdTo == id && !x.View).Any())
			{
				return "True";
			}
			else
			{
				return "False";
			}
			

		}
	}
}


