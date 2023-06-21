using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

using Paciente = GabinetePsicologia.Shared.Paciente;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace GabinetePsicologia.Server.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class TrastornoController : Controller
    {
        private readonly ApplicationDbContext _context;


        public TrastornoController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<IActionResult> get()
        {
            return Ok(_context.Trastornos.ToList());
        }

        [HttpPost]
        public IActionResult Register([FromBody] Trastorno trastorno)
        {
            _context.Trastornos.Add(trastorno);
            _context.SaveChanges();
            return Ok("Trastorno Añadido");
        }

        [HttpPost("Borrar")]

        public IActionResult Borrar([FromBody] IList<Trastorno> trastornos)
        {
            foreach (var a in trastornos)
            {
                if (_context.Trastornos.Where(x => x.Id != Guid.Empty && x.Id == a.Id).Any())
                {
                    _context.Trastornos.Remove(a);
                    var LsinfTrst = _context.InformeTrastorno.Where(x => x.TrastornoId == a.Id).ToList();
                    foreach (var infTrst in LsinfTrst)
                    {
                        _context.Remove(infTrst);
                    }
                }


            }

            _context.SaveChanges();
            return Ok("Trastornos Eliminados Correctamente");
        }

        [HttpPost("Editar")]

        public IActionResult Editar([FromBody] Trastorno trastorno)
        {
            _context.Trastornos.Update(trastorno);
            _context.SaveChanges();
            return Ok("Trastorno editado Correctamente");
        }

    }
}
