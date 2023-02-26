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
using Paciente = GabinetePsicologia.Shared.Paciente;

namespace GabinetePsicologia.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
  
    public class PacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PacienteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Paciente>>> GetPacientes()
        {
            return Ok(_context.Pacientes.ToList());
        }
        [HttpPost]
        public async Task<ActionResult> RegisterPaciente(Paciente paciente)
        {

            if(paciente == null) return BadRequest();
            _context.Pacientes.Add(paciente);
            var user = _context.Users.FirstOrDefault(x => x.Id == paciente.ApplicationUserId);
            await _userManager.AddToRoleAsync(user, "Paciente");
            _context.SaveChanges();
            return Ok();
        }
    }
}
