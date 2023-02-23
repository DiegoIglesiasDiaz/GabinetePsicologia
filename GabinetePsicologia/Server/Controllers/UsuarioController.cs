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
  
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
       
        public UsuarioController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            
        }
        
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok("Logout Succesful");
        }
        [HttpGet("Persona")]
        public async Task<IActionResult> getPersonas()
        {
            List<Persona> ls = new List<Persona>();
            List<Paciente> a = _context.Pacientes.ToList();
            foreach (var p in a)
            {
                ls.Add(new Persona() { Apellido1=p.Apellido1,Apellido2=p.Apellido2,Nombre=p.Nombre,Email=p.Email,ApplicationUserId=p.ApplicationUserId,Id=p.Id,NIF=p.NIF});
            }
            List<Administrador> b = _context.Administradores.ToList();
            foreach (var p in a)
            {
                ls.Add(new Persona() { Apellido1 = p.Apellido1, Apellido2 = p.Apellido2, Nombre = p.Nombre, Email = p.Email, ApplicationUserId = p.ApplicationUserId, Id = p.Id, NIF = p.NIF });
            }
            List<Psicologo> c = _context.Psicologos.ToList();
            foreach (var p in a)
            {
                ls.Add(new Persona() { Apellido1 = p.Apellido1, Apellido2 = p.Apellido2, Nombre = p.Nombre, Email = p.Email, ApplicationUserId = p.ApplicationUserId, Id = p.Id, NIF = p.NIF });
            }
            return Ok(ls);
        }

    }
}
