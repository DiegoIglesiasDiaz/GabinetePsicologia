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
using Cita = GabinetePsicologia.Shared.Cita;

namespace GabinetePsicologia.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
  
    public class CitaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CitaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Cita>>> GetCitas()
        {
            return Ok(_context.Citas.ToList());
        }
        [HttpGet("Psicologo/{id:guid}")]
        public async Task<ActionResult<List<Cita>>> GetCitasByPsicologoId(Guid id)
        {
            return Ok(_context.Citas.Where(x => x.PsicologoId == id).ToList());
        }
        [HttpGet("Paciente/{id:guid}")]
        public async Task<ActionResult<List<Cita>>> GetCitasByPacienteId(Guid id)
        {
           
            return Ok(_context.Citas.Where(x => x.PacienteId == id).ToList());
        }
        [HttpPost]
        public async Task<ActionResult> InsertCita(Cita cita)
        {

            if(cita == null) return BadRequest();

            var psicologo = _context.Psicologos.FirstOrDefault(x => x.Id == cita.PsicologoId);
            if(psicologo != null)
                cita.PsicologoFullName = psicologo.FullName;
            var paciente = _context.Pacientes.FirstOrDefault(x => x.Id == cita.PacienteId);
            if (paciente != null)
                cita.PacienteFullName = paciente.FullName;

            _context.Citas.Add(cita);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost("Actualizar")]
        public async Task<ActionResult> ActualizarCita([FromBody] Cita cita)
        {
            if (cita == null) return BadRequest();

            var psicologo = _context.Psicologos.FirstOrDefault(x => x.Id == cita.PsicologoId);
            if (psicologo != null)
                cita.PsicologoFullName = psicologo.FullName;
            var paciente = _context.Pacientes.FirstOrDefault(x => x.Id == cita.PacienteId);
            if (paciente != null)
                cita.PacienteFullName = paciente.FullName;

            _context.Citas.Update(cita);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost("Eliminar")]
        public async Task<ActionResult<string>> EliminarCita([FromBody] Cita cita)
        {
            if (cita == null) return BadRequest();
            _context.Citas.Remove(cita);
            _context.SaveChanges();
            return Ok();
        }
    }
}


