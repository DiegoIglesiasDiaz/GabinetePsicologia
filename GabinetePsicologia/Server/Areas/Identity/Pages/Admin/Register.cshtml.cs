// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using GabinetePsicologia.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using GabinetePsicologia.Shared;
using GabinetePsicologia.Server.Controllers;
using GabinetePsicologia.Server.Data.Migrations;
using Paciente = GabinetePsicologia.Shared.Paciente;
using GabinetePsicologia.Server.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GabinetePsicologia.Server.Areas.Identity.Pages.Admin
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly PsicologoController _psicologoController;
        private readonly PacienteController _pacienteController;
        private readonly AdministradorController _administradorController;
        private readonly ApplicationDbContext _context;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context
            )
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _administradorController = new AdministradorController(_context , userManager);
            _pacienteController = new PacienteController(_context, userManager);
            _psicologoController = new PsicologoController(_context , userManager);
        }
        [Inject] public NavigationManager NavigationManager { get; set; }

      




        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

       
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "*Email Requerido")]
            [EmailAddress]
            [Display(Name = "Email")]

            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "*Contraseña Requerida")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            
            [DataType(DataType.Text)]
            [Required(ErrorMessage = "*Nombre Requerido")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [DataType(DataType.Text)]
            [Required(ErrorMessage = "*Primer Apellido Requerido")]
            [Display(Name = "Primer Apellido")]
            public string Apellido1 { get; set; }


            [DataType(DataType.Text)]
            [Display(Name = "Segundo Apellido")]
            public string Apellido2 { get; set; }

            [Required(ErrorMessage = "*NIF Requerido")]
            [DataType(DataType.Text)]
            [Display(Name = "NIF")]
            public string NIF { get; set; }

            [Required(ErrorMessage = "*Seleccione un Rol")]
            [DataType(DataType.Text)]
            [Display(Name = "Rol")]
            public string Rol { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                var emailConfirmationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, emailConfirmationCode);
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                
             
                if (result.Succeeded)
                {
                    if (Input.Rol == "Psicologo")
                        _userManager.AddToRoleAsync(user, "Psicologo").Wait();
                    if (Input.Rol == "Administrador")
                        _userManager.AddToRoleAsync(user, "Administrador").Wait();
                    else
                        _userManager.AddToRoleAsync(user, "Paciente").Wait();
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    if (Input.Rol == "Psicologo")
                        await _psicologoController.RegisterPsicologo(new Psicologo { Apellido1=Input.Apellido1,NIF=Input.NIF, Apellido2=Input.Apellido2, Nombre=Input.Nombre, ApplicationUserId=user.Id});
                    if (Input.Rol == "Administrador")
                        await _administradorController.RegisterAdministrador(new Administrador { Apellido1 = Input.Apellido1, NIF = Input.NIF, Apellido2 = Input.Apellido2, Nombre = Input.Nombre, ApplicationUserId = user.Id });
                    else
                        await _pacienteController.RegisterPaciente(new Paciente { Apellido1 = Input.Apellido1, NIF = Input.NIF, Apellido2 = Input.Apellido2, Nombre = Input.Nombre, ApplicationUserId = user.Id });

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                   
                    ViewData["Message"] = "Usuario Creado Correctamente";
                    
                        
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
