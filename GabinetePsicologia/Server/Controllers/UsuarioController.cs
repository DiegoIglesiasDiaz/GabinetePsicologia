using GabinetePsicologia.Client.Pages.Psicologo;
using GabinetePsicologia.Server.Areas.Identity.Pages.Admin;
using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using MessagePack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Dynamic.Core;
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
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> register(PersonaDto data)
        {
            var user = Activator.CreateInstance<ApplicationUser>();
            var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);
            await _userStore.SetUserNameAsync(user, data.Email, CancellationToken.None);
            await _userManager.SetPhoneNumberAsync(user, data.Telefono);
            await _emailStore.SetEmailAsync(user, data.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user, data.Contraseña);


            if (result.Succeeded)
            {

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
            return BadRequest("Correo Ya existe");
        }

        [HttpPost("Borrar")]
        public async Task<IActionResult> Borrar([FromBody] IList<PersonaDto> data)
        {
            foreach (var user in data)
            {
                if (_context.Users.Where(x => x.Id == user.ApplicationUserId).Any())
                {
                    var remove = _context.Users.FirstOrDefault(x => x.Id == user.ApplicationUserId);
                    _context.Users.Remove(remove);
                }
                else if (user.ApplicationUserId == "" && _context.Users.Where(x => x.UserName == user.Email).Any())
                {
                    var remove = _context.Users.FirstOrDefault(x => x.UserName == user.Email);
                    _context.Users.Remove(remove);
                }

            }
            _context.SaveChanges();
            return Ok("Usuarios Eliminados Correctamente");
        }

        [HttpPost("Editar")]
        public async Task<IActionResult> Editar([FromBody] PersonaDto data)
        {
            var User = _context.Users.FirstOrDefault(x => x.UserName == data.Email);
            if (User == null) return BadRequest();
            //var token = await _userManager.GenerateChangePhoneNumberTokenAsync(User,data.Telefono);
            //await _userManager.ChangePhoneNumberAsync(User, data.Telefono, token);
            await _userManager.SetPhoneNumberAsync(User, data.Telefono);
            if (data.isPaciente)
            {
                if (_context.Pacientes.Where(x => x.ApplicationUserId == User.Id).Any())
                {
                    var paciente = _context.Pacientes.FirstOrDefault(x => x.ApplicationUserId == User.Id);
                    paciente.Nombre = data.Nombre;
                    paciente.Apellido1 = data.Apellido1;
                    paciente.Apellido2 = data.Apellido2;
                    paciente.NIF = data.NIF;
                    await _userManager.AddToRoleAsync(User, "Paciente");
                }
                else { await _pacienteController.RegisterPaciente(new Paciente { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = User.Id }); }


            }
            else if (_context.Pacientes.Where(x => x.ApplicationUserId == User.Id).Any())
            {
                var remove = _context.Pacientes.FirstOrDefault(x => x.ApplicationUserId == User.Id);
                _context.Pacientes.Remove(remove);
                await _userManager.RemoveFromRoleAsync(User, "Paciente");
            }
            if (data.isPsicologo)
            {
                if (_context.Psicologos.Where(x => x.ApplicationUserId == User.Id).Any())
                {
                    var pscilogo = _context.Psicologos.FirstOrDefault(x => x.ApplicationUserId == User.Id);
                    pscilogo.Nombre = data.Nombre;
                    pscilogo.Apellido1 = data.Apellido1;
                    pscilogo.Apellido2 = data.Apellido2;
                    pscilogo.NIF = data.NIF;
                    await _userManager.AddToRoleAsync(User, "Psicologo");
                }
                else { await _psicologoController.RegisterPsicologo(new Psicologo { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = User.Id }); }


            }
            else if (_context.Psicologos.Where(x => x.ApplicationUserId == User.Id).Any())
            {
                var remove = _context.Psicologos.FirstOrDefault(x => x.ApplicationUserId == User.Id);
                _context.Psicologos.Remove(remove);
                await _userManager.RemoveFromRoleAsync(User, "Psicologo");
            }
            if (data.isAdmin)
            {
                if (_context.Administradores.Where(x => x.ApplicationUserId == User.Id).Any())
                {
                    var Admin = _context.Administradores.FirstOrDefault(x => x.ApplicationUserId == User.Id);
                    Admin.Nombre = data.Nombre;
                    Admin.Apellido1 = data.Apellido1;
                    Admin.Apellido2 = data.Apellido2;
                    Admin.NIF = data.NIF;
                    await _userManager.AddToRoleAsync(User, "Administrador");

                }
                else
                {
                    await _administradorController.RegisterAdministrador(new Administrador { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = User.Id });

                }
               ;

            }
            else if (_context.Administradores.Where(x => x.ApplicationUserId == User.Id).Any())
            {
                var remove = _context.Administradores.FirstOrDefault(x => x.ApplicationUserId == User.Id);
                _context.Administradores.Remove(remove);
                await _userManager.RemoveFromRoleAsync(User, "Administrador");
            }

            _context.SaveChanges();
            return Ok("Usuarios Eliminados Correctamente");
        }
        [HttpPost("CambiarCorreo")]
        public async Task<IActionResult> CambiarCorreo([FromBody] string[] correos)
        {
            if (_context.Users.Where(x => x.UserName == correos[1]).Any()) return BadRequest("Este Email ya existe");
            var user = _context.Users.FirstOrDefault(x => x.UserName == correos[0]);
            await _userManager.SetUserNameAsync(user, correos[1]);
            await _userManager.SetEmailAsync(user, correos[1]);
            return Ok("Correo Actualizado");
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> OnPostAsync([FromBody] LoginDto usuario)
        {
            IList<AuthenticationScheme> ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Password, usuario.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok("Usuario Logueado");
                }
                //if (result.RequiresTwoFactor)
                //{
                //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                //}
                //if (result.IsLockedOut)
                //{
                //    _logger.LogWarning("Cuenta de usuario bloqueada.");
                //    return RedirectToPage("./Lockout");
                //}
                else
                {
                    ModelState.AddModelError(string.Empty, "Datos incorrectos.");
                    return BadRequest("Datos incorrectos.");
                }
            }

            // If we got this far, something failed, redisplay form
            return BadRequest("Datos incorrectos.");
        }
        [HttpPost("CambiarContraseña")]
        public async Task<IActionResult> CambiarContraseña([FromBody] string[] data)
        {
            string passwd = data[0];
            string correo = data[1];
            var user = _context.Users.FirstOrDefault(x => x.UserName == correo);
            if (user == null) return BadRequest("Usuario no encontrado");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, passwd);
            return Ok("Contraseña Cambiada");
        }
    }
}
