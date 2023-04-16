
using GabinetePsicologia.Client.Pages;
using GabinetePsicologia.Server.Areas.Identity.Pages.Admin;
using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using MessagePack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using Paciente = GabinetePsicologia.Shared.Paciente;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace GabinetePsicologia.Server.Controllers
{


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
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> BorrarCuenta(Guid id)
        {
            ApplicationUser? user =  _context.Users.FirstOrDefault(x => x.Id == id.ToString());
            if (user == null) return BadRequest();
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
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
                        pers.Direccion = p.Direccion;
                        pers.FecNacim = p.FecNacim;
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
                            pers.Direccion = p.Direccion;
                            pers.FecNacim = p.FecNacim;
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
                            pers.Direccion = a.Direccion;
                            pers.FecNacim = a.FecNacim;
                        }
                    }

                }
                if (pers != null && pers.Id != Guid.Empty)
                    LsPersonas.Add(pers);
            }
            return Ok(LsPersonas);
        }
        [HttpGet("Persona/{Username}")]
        public async Task<IActionResult> getApplicationUserByUsername(string UserName)
        {
            ApplicationUser? user = _context.Users.FirstOrDefault(x => x.UserName.ToLower().Equals(UserName.ToLower()));
            if (user == null) return BadRequest(null);
            PersonaDto UserValue = new PersonaDto()
            {
                Contraseña = user.PasswordHash,
                Email = UserName,
                Id = Guid.Parse(user.Id)
            };
            if(await _userManager.IsInRoleAsync(user, "Administrador"))
            {
                Administrador? a = _context.Administradores.First(x=>x.ApplicationUserId == user.Id);
                  
                if (a != null)
                {
                    UserValue.Rol = "Administrador";
                    UserValue.Apellido1 = a.Apellido1;
                    UserValue.Apellido2 = a.Apellido2;
                    UserValue.Nombre = a.Nombre;
                    UserValue.Email = user.Email;
                    UserValue.Telefono = user.PhoneNumber;
                    UserValue.ApplicationUserId = a.ApplicationUserId;
                    UserValue.NIF = a.NIF;
                    UserValue.Direccion = a.Direccion;
                    UserValue.FecNacim = a.FecNacim;
                }
            }
            if (await _userManager.IsInRoleAsync(user, "Psicologo"))
            {
                Psicologo? a = _context.Psicologos.First(x => x.ApplicationUserId == user.Id);

                if (a != null)
                {
                    UserValue.Rol = "Psicologo";
                    UserValue.Apellido1 = a.Apellido1;
                    UserValue.Apellido2 = a.Apellido2;
                    UserValue.Nombre = a.Nombre;
                    UserValue.Email = user.Email;
                    UserValue.Telefono = user.PhoneNumber;
                    UserValue.ApplicationUserId = a.ApplicationUserId;
                    UserValue.NIF = a.NIF;
                    UserValue.Direccion = a.Direccion;
                    UserValue.FecNacim = a.FecNacim;
                }
            }
            if (await _userManager.IsInRoleAsync(user, "Paciente"))
            {
                Paciente? a = _context.Pacientes.First(x => x.ApplicationUserId == user.Id);

                if (a != null)
                {
                    UserValue.Rol = "Paciente";
                    UserValue.Apellido1 = a.Apellido1;
                    UserValue.Apellido2 = a.Apellido2;
                    UserValue.Nombre = a.Nombre;
                    UserValue.Email = user.Email;
                    UserValue.Telefono = user.PhoneNumber;
                    UserValue.ApplicationUserId = a.ApplicationUserId;
                    UserValue.NIF = a.NIF;
                    UserValue.Direccion = a.Direccion;
                    UserValue.FecNacim = a.FecNacim;
                }
            }
            return Ok(UserValue);
           
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> register(PersonaDto data)
        {
            var user = Activator.CreateInstance<ApplicationUser>();
            var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);
            await _userStore.SetUserNameAsync(user, data.Email, CancellationToken.None);
            if(data.Telefono != null)
                await _userManager.SetPhoneNumberAsync(user, data.Telefono);
            await _emailStore.SetEmailAsync(user, data.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(user,data.Contraseña);


            if (result.Succeeded)
            {

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                if (data.isPsicologo)
                    await _psicologoController.RegisterPsicologo(new Psicologo { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = user.Id, FecNacim = data.FecNacim, Direccion = data.Direccion });
                if (data.isAdmin)
                    await _administradorController.RegisterAdministrador(new Administrador { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = user.Id, FecNacim = data.FecNacim, Direccion = data.Direccion });
                if (data.isPaciente)
                    await _pacienteController.RegisterPaciente(new Paciente { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = user.Id, FecNacim = data.FecNacim, Direccion = data.Direccion });

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
                    if(!String.IsNullOrEmpty(data.Direccion))
                        paciente.Direccion = data.Direccion;
                    paciente.FecNacim = data.FecNacim;
                    await _userManager.AddToRoleAsync(User, "Paciente");
                }
                else { await _pacienteController.RegisterPaciente(new Paciente { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = User.Id, FecNacim = data.FecNacim, Direccion = data.Direccion }); }


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
                    if (!String.IsNullOrEmpty(data.Direccion))
                        pscilogo.Direccion = data.Direccion;
                    pscilogo.FecNacim = data.FecNacim;
                    await _userManager.AddToRoleAsync(User, "Psicologo");
                }
                else { await _psicologoController.RegisterPsicologo(new Psicologo { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = User.Id, FecNacim = data.FecNacim, Direccion = data.Direccion }); }


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
                    if (!String.IsNullOrEmpty(data.Direccion))
                        Admin.Direccion = data.Direccion;
                    Admin.FecNacim = data.FecNacim;
                    await _userManager.AddToRoleAsync(User, "Administrador");

                }
                else
                {
                    await _administradorController.RegisterAdministrador(new Administrador { Apellido1 = data.Apellido1, NIF = data.NIF, Apellido2 = data.Apellido2, Nombre = data.Nombre, ApplicationUserId = User.Id, FecNacim = data.FecNacim, Direccion = data.Direccion });

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
            return Ok("Usuarios Editados Correctamente");
        }
        [HttpPost("CambiarCorreo")]
        public async Task<IActionResult> CambiarCorreo([FromBody] string[] correos)
        {
            if (_context.Users.Where(x => x.UserName == correos[1]).Any()) return BadRequest("Este Email ya existe");
            var user = _context.Users.FirstOrDefault(x => x.UserName == correos[0]);
            await _userManager.SetUserNameAsync(user, correos[1]);
            await _userManager.SetEmailAsync(user, correos[1]);
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);
            return Ok("Correo Actualizado");
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> OnPostAsync([FromBody] LoginDto usuario)
        {
 
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(usuario.Email, usuario.Password, usuario.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                  //  _userManager.AddClaimAsync(User)
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
        [HttpPost("CambiarTelefono")]
        public async Task<IActionResult> CambiarTelefono([FromBody] string[] data)
        {
            string telefono = data[0];
            string correo = data[1];
            var user = _context.Users.FirstOrDefault(x => x.UserName == correo);
            if (user == null) return BadRequest("Usuario no encontrado");
            await _userManager.SetPhoneNumberAsync(user, telefono);
            return Ok("Contraseña Cambiada");
        }
        [AllowAnonymous]
        [HttpPost("ExternalLogin/{provider}")]
        public IActionResult ExternalLogin(string provider)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, "/Usuario/ExternalLogin/Success");
            return new ChallengeResult(provider, properties);

        }
        [AllowAnonymous]
        [HttpGet("ExternalLogin/Success")]
        public async Task<IActionResult> ExternalSuccessLogin()
        {
            string Email = "";
            string Name = "";
            string Apellido1 = "";
            string Apellido2 = "";
            string SurNames = ""; 
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                Email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                Name = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                SurNames = info.Principal.FindFirstValue(ClaimTypes.Surname);

            var SurnamesSplit = splitSurames(SurNames);
            Apellido1 = SurnamesSplit[0];
            Apellido2 = SurnamesSplit[1];

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (!result.Succeeded)
            {

                var user = Activator.CreateInstance<ApplicationUser>();

                await _userStore.SetUserNameAsync(user, Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Email, CancellationToken.None);
                var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);

                var _result = await _userManager.CreateAsync(user);
                if (_result.Succeeded)
                {
                    await _pacienteController.RegisterPaciente(new Paciente { Apellido1 = Apellido1, NIF = "", Apellido2 = Apellido2, Nombre = Name, ApplicationUserId = user.Id, FecNacim = new DateTime(), Direccion = "dsad" });
                    await _userManager.AddToRoleAsync(user, "Paciente");
                    _result = await _userManager.AddLoginAsync(user, info);
                    var resultLogin = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

                }
            }
            return LocalRedirect("/");
        }
        private string[] splitSurames(string Surnames)
        {
            string[] result = { "", "" };
            var split = Surnames.Split(" ");
            if (split.Length == 2)
            {
                result[0] = split[0];
                result[1] = split[1];
            }
            else
            if (split.Length == 1) result[0] = split[0];
            else
            {
                bool isAp1 = true;
                foreach (var s in split)
                {
                    if (s == "") continue;
                    if (isAp1)
                    {
                        if (s.Length <= 2)
                        {
                            result[0] += s;
                        }
                        else
                        {
                            result[0] += s;
                            isAp1 = false;
                        }
                    }
                    else
                    {
                        result[1] = s;
                    }


                }
            }
            return result;
        }
        [HttpPost("CheckPasswd")]
        public async Task<IActionResult> CheckPasswd([FromBody] string[] data)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == data[0]);
            if (user == null) return BadRequest("Contraseña Incorrecta");
            bool result =  await _userManager.CheckPasswordAsync(user, data[1]);
            if (result)
            {
                return Ok("Contraseña Correcta");
            }
            else
            {
                return BadRequest("Contraseña Incorrecta");
            }
        }
    }
}
