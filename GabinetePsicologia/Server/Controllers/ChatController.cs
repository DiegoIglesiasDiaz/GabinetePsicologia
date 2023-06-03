using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Cita = GabinetePsicologia.Shared.Cita;

namespace GabinetePsicologia.Server.Controllers
{
    [Authorize]
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
			return _context.Chat.Where(x=>(x.IdFrom == UserId && x.IdTo == ChatId) || ( x.IdTo == UserId && x.IdFrom == ChatId)).OrderBy(x => x.Date).ToList() ;
		}
		[HttpGet("AllMessages/{id}")]
		public List<ChatDto> GetAllMessages(string id)
		{
			return _context.Chat.Where(x => x.IdFrom == id || x.IdTo == id ).ToList();
		}
		[HttpPost]
		public void Send(ChatDto chat)
		{
			try
			{
				_context.Chat.Add(chat);
				var a = _context.SaveChanges();
			}
			catch(Exception ex)
			{
				
			}
			
			
		}
		[HttpGet("ChatedPeople/{id}")]
		public List<KeyValue> GetChatedPeople(string id)
		{
			var People = new List<KeyValue>();
			var LsTo= _context.Chat.Where(x => x.IdFrom == id && x.IdTo != id).OrderByDescending(x => x.Date).ToList();
			var Lsfrom = _context.Chat.Where(x => x.IdFrom != id && x.IdTo == id).OrderByDescending(x => x.Date).ToList();
			if(Lsfrom.First().Date>= LsTo.First().Date)
			{
				foreach (var from in Lsfrom)
				{
					if (!People.Where(x => x.Value == from.IdFrom).Any())
						People.Add(new KeyValue { Key = from.FromName, Value = from.IdFrom });
				}
				foreach (var to in LsTo)
				{
					if (!People.Where(x => x.Value == to.IdTo).Any())
						People.Add(new KeyValue { Key = to.ToName, Value = to.IdTo });
				}
			}
			else
			{
				foreach (var to in LsTo)
				{
					if (!People.Where(x => x.Value == to.IdTo).Any())
						People.Add(new KeyValue { Key = to.ToName, Value = to.IdTo });
				}
				foreach (var from in Lsfrom)
				{
					if (!People.Where(x => x.Value == from.IdFrom).Any())
						People.Add(new KeyValue { Key = from.FromName, Value = from.IdFrom });
				}
			}
			
			
			return People;
		}
		[HttpGet("AllPeople/{id}")]
		public async Task<List<KeyValue>> GetAllPeople(string id)
		{
			var People = new List<KeyValue>();
			var user = _context.Users.FirstOrDefault(x => x.Id == id);
			if (user != null)
			{
				if(await _userManager.IsInRoleAsync(user, "Paciente"))
				{
					var LsPsicologos = _context.Psicologos.Where(x => x.ApplicationUserId != id).ToList();
					var LsAdmin = _context.Administradores.Where(x => x.ApplicationUserId != id).ToList();
					foreach(var ps in LsPsicologos)
					{
						if(!People.Where(x=>x.Value == ps.ApplicationUserId).Any())
							People.Add(new KeyValue {Key= ps.FullName, Value = ps.ApplicationUserId });
					}
					foreach (var ad in LsAdmin)
					{
						if (!People.Where(x => x.Value == ad.ApplicationUserId).Any())
							People.Add(new KeyValue { Key = ad.FullName, Value = ad.ApplicationUserId });
					}
				}
				else
				{
					var LsPsicologos = _context.Psicologos.Where(x => x.ApplicationUserId != id).ToList();
					var LsAdmin = _context.Administradores.Where(x => x.ApplicationUserId != id).ToList();
					var LsPaciente = _context.Pacientes.Where(x => x.ApplicationUserId != id).ToList();
					foreach (var ps in LsPsicologos)
					{
						if (!People.Where(x => x.Value == ps.ApplicationUserId).Any())
							People.Add(new KeyValue { Key = ps.FullName, Value = ps.ApplicationUserId });
					}
					foreach (var ad in LsAdmin)
					{
						if (!People.Where(x => x.Value == ad.ApplicationUserId).Any())
							People.Add(new KeyValue { Key = ad.FullName, Value = ad.ApplicationUserId });
					}
					foreach (var pa in LsPaciente)
					{
						if (!People.Where(x => x.Value == pa.ApplicationUserId).Any())
							People.Add(new KeyValue { Key = pa.FullName, Value = pa.ApplicationUserId });
					}
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
			foreach(var rm in lsremove)
			{
				_context.Chat.Remove(rm);	
			}
			_context.SaveChanges();
		}
	}
}


