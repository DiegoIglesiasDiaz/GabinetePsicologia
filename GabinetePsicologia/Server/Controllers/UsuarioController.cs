using GabinetePsicologia.Server.Areas.Identity.Pages.Admin;
using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Text;
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
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly PsicologoController _psicologoController;
        private readonly PacienteController _pacienteController;
        private readonly AdministradorController _administradorController;

        public UsuarioController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IUserStore<ApplicationUser> UserStore)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = UserStore;
            _emailStore = (IUserEmailStore<ApplicationUser>)_userStore; ;
            _administradorController = new AdministradorController(_context, userManager);
            _pacienteController = new PacienteController(_context, userManager);
            _psicologoController = new PsicologoController(_context, userManager);
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
            List<PersonaDto> LsPersonas = new List<PersonaDto>();
            List<Paciente> Pacientes = _context.Pacientes.ToList();
            List<Psicologo> Psicologos = _context.Psicologos.ToList();
            List<Administrador> Administradores = _context.Administradores.ToList();
            List<ApplicationUser> allUser = _context.Users.ToList();

            foreach (var user in allUser)
            {
                PersonaDto pers = new PersonaDto();
                if (Pacientes.Where(x => x.ApplicationUserId == user.Id).Any())
                {
                    Paciente? p = Pacientes.FirstOrDefault(x => x.ApplicationUserId == user.Id);
                    if (p != null)
                    {
                        pers.Rol = "Paciente";
                        pers.Apellido1 = p.Apellido1;
                        pers.Apellido2 = p.Apellido2;
                        pers.Nombre = p.Nombre;
                        pers.Email = user.Email;
                        pers.Telefono = user.PhoneNumber;
                        pers.ApplicationUserId = p.ApplicationUserId;
                        pers.Id = p.Id;
                        pers.NIF = p.NIF;
                    }
                }
                if (Psicologos.Where(x => x.ApplicationUserId == user.Id).Any())
                {
                    if (pers != null && pers.Id != Guid.Empty)
                        pers.Rol += " ,Psicologo";
                    if (pers.Id == Guid.Empty)
                    {
                        Psicologo? p = Psicologos.FirstOrDefault(x => x.ApplicationUserId == user.Id);
                        if (p != null)
                        {
                            pers.Rol = "Psicologo";
                            pers.Apellido1 = p.Apellido1;
                            pers.Apellido2 = p.Apellido2;
                            pers.Nombre = p.Nombre;
                            pers.Email = user.Email;
                            pers.Telefono = user.PhoneNumber;
                            pers.ApplicationUserId = p.ApplicationUserId;
                            pers.Id = p.Id;
                            pers.NIF = p.NIF;
                        }
                    }

                }
                if (Administradores.Where(x => x.ApplicationUserId == user.Id).Any())
                {
                    if (pers != null && pers.Id != Guid.Empty)
                        pers.Rol += " ,Administrador";
                    if (pers.Id == Guid.Empty)
                    {
                        Administrador? a = Administradores.FirstOrDefault(x => x.ApplicationUserId == user.Id);
                        if (a != null)
                        {
                            pers.Rol = "Administrador";
                            pers.Apellido1 = a.Apellido1;
                            pers.Apellido2 = a.Apellido2;
                            pers.Nombre = a.Nombre;
                            pers.Email = user.Email;
                            pers.Telefono = user.PhoneNumber;
                            pers.ApplicationUserId = a.ApplicationUserId;
                            pers.Id = a.Id;
                            pers.NIF = a.NIF;
                        }
                    }

                }
                if (pers != null && pers.Id != Guid.Empty)
                    LsPersonas.Add(pers);
            }
            return Ok(LsPersonas);
        }
        [HttpPost]
        public async Task<IActionResult> register(PersonaDto data)
        {
            var user = Activator.CreateInstance<ApplicationUser>();
            var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);
            await _userStore.SetUserNameAsync(user, data.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, data.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, data.Contraseña);


            if (result.Succeeded)
            {
                if (data.isPsicologo)
                    _userManager.AddToRoleAsync(user, "Psicologo").Wait();
                if (data.isAdmin)
                    _userManager.AddToRoleAsync(user, "Administrador").Wait();
                if (data.isPaciente)
                    _userManager.AddToRoleAsync(user, "Paciente").Wait();
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                if (data.isPsicologo)
                    await _psicologoController.RegisterPsicologo(new Psicologo { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = user.Id });
                if (data.isAdmin)
                    await _administradorController.RegisterAdministrador(new Administrador { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = user.Id });
                if (data.isPaciente)
                    await _pacienteController.RegisterPaciente(new Paciente { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = user.Id });

                return Ok("Usuario Creado");
            }
            return BadRequest();
        }
    }
}
