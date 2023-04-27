using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Informe = GabinetePsicologia.Shared.Informe;

namespace GabinetePsicologia.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
  
    public class InformeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public InformeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Informe>>> GetInforme()
        {
            return Ok(_context.Informes.ToList());
        }
        [HttpPost]
        public async Task<ActionResult> InsertInforme(Informe Informe)
        {

            if(Informe == null) return BadRequest();
            _context.Informes.Add(Informe);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost("Actualizar")]
        public async Task<ActionResult> ActualizarInforme([FromBody] Informe Informe)
        {

            if (Informe == null) return BadRequest();
            _context.Informes.Update(Informe);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost("Eliminar")]
        public async Task<ActionResult<string>> EliminarInforme([FromBody] Informe Informe)
        {
            if (Informe == null) return BadRequest();
            _context.Informes.Remove(Informe);
            _context.SaveChanges();
            return Ok();
        }
    }
}


