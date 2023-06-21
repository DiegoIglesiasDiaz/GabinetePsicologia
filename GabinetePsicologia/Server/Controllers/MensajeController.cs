using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GabinetePsicologia.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
	[ApiController]
	
	public class MensajeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public MensajeController(ApplicationDbContext context)
		{
			_context = context;

		}
		[HttpPost]
		[AllowAnonymous]
		public IActionResult Insertar(Mensaje mensaje)
		{
			_context.Mensajes.Add(mensaje);
			_context.SaveChanges();
			return Ok("Insertado Correctamente");
		}
		[HttpPost("Actualizar")]
		public IActionResult Actualizar(Mensaje mensaje)
		{
			if(_context.Mensajes.Where(x=> x.Id == mensaje.Id).Any())
			{
				_context.Mensajes.Update(mensaje);
				_context.SaveChanges();
			}		
			return Ok("Actualizado Correctamente");
		}
		[HttpPost("Delete")]
		public IActionResult Delete(List<Mensaje> mensajes)
		{
			foreach(var m in mensajes)
			{
				_context.Mensajes.Remove(m);
			}
			_context.SaveChanges();
			return Ok("Eliminado Correctamente");
		}
		[HttpGet]
		public IActionResult Get()
		{			
			return Ok(_context.Mensajes.ToList());
		}
	}
}
