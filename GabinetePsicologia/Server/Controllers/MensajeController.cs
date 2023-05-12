using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabinetePsicologia.Server.Controllers
{
	[Route("[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class MensajeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public MensajeController(ApplicationDbContext context)
		{
			_context = context;

		}
		[HttpPost]
		public IActionResult Insertar(Mensaje mensaje)
		{
			_context.Mensajes.Add(mensaje);
			_context.SaveChanges();
			return Ok("Insertado Correctamente");
		}
	}
}
