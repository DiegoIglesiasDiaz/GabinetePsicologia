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

		
		[HttpGet("{id}")]
		public List<ChatDto> GetMessages(string id)
		{
			return _context.Chat.Where(x=>x.IdFrom == id || x.IdTo == id).OrderBy(x => x.Date).ToList() ;
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

	}
}


