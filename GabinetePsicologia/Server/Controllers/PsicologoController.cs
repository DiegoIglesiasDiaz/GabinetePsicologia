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
  
    public class PsicologoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PsicologoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Psicologo>>> GetPsicologos()
        {
            //    var user = await _context.Users.Include(
            //        u=> u.LsPaciente).FirstOrDefaultAsync(
            //        u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _context.Psicologos.ToList();
        }
        [HttpPost]
        public async Task<ActionResult> RegisterPsicologo(Psicologo psicologo)
        {

            if(psicologo == null) return BadRequest();
            _context.Psicologos.Add(psicologo);
            var user = _context.Users.FirstOrDefault(x => x.Id == psicologo.ApplicationUserId);
            await _userManager.AddToRoleAsync(user, "Psicologo");
            _context.SaveChanges();
            return Ok();
        }
    }
}
