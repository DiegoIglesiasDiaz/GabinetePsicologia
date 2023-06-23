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
  
    public class AdministradorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdministradorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Paciente>>> GetAdministradores()
        {
            //    var user = await _context.Users.Include(
            //        u=> u.LsPaciente).FirstOrDefaultAsync(
            //        u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            //await _userManager.AddToRoleAsync(user, "Paciente");
            if (user == null) return NotFound();
            return Ok(user.LsAdmin);
        }
        [HttpPost]
        public async Task<ActionResult> RegisterAdministrador(Administrador admin)
        {

            if(admin == null) return BadRequest();
            _context.Administradores.Add(admin);
            var user = _context.Users.FirstOrDefault(x => x.Id == admin.ApplicationUserId);
            await _userManager.AddToRoleAsync(user, "Administrador");
            _context.SaveChanges();
            return Ok();
        }
    
    }
}
