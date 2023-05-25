using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Cita = GabinetePsicologia.Shared.Cita;

namespace GabinetePsicologia.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
  
    public class TwoFactorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UrlEncoder _urlEncoder;
		private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
		public TwoFactorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder)
        {
            _context = context;
            _userManager = userManager;
			_urlEncoder = urlEncoder;

		}

		public string SharedKey { get; set; }


		public string AuthenticatorUri { get; set; }



		[HttpGet("{correo}")]
		public async Task<string[]> OnGetAsync(string correo)
		{
			
			var user =  _context.Users.FirstOrDefault(x=> x.UserName.ToLower() == correo.ToLower());
			if (user == null)
			{
				return new string[0];
			}

			

			return await LoadSharedKeyAndQrCodeUriAsync(user);
		}
		[HttpGet("code/{codeEmail}")]
		public async Task<bool> OnPostAsync(string codeEmail)
		{
			var split = codeEmail.Split(";");
			var code = split[0];
			var correo = split[1];
			
			var user = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == correo.ToLower());
			if (user == null)
			{
				return false;
			}


			// Strip spaces and hyphens
			var verificationCode = code.Replace(" ", string.Empty).Replace("-", string.Empty);

			var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
				user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

			if (!is2faTokenValid)
			{
				ModelState.AddModelError("Input.Code", "Verification code is invalid.");
				await LoadSharedKeyAndQrCodeUriAsync(user);
				return false;
			}

			await _userManager.SetTwoFactorEnabledAsync(user, true);
			var userId = await _userManager.GetUserIdAsync(user);
			
		
			if (await _userManager.CountRecoveryCodesAsync(user) == 0)
			{
				var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
				var RecoveryCodes = recoveryCodes.ToArray();
				return true;
				//return RedirectToPage("./ShowRecoveryCodes");
			}
			else
			{
				return true;
				//return RedirectToPage("./TwoFactorAuthentication");
			}
		}

		private async Task<string[]> LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
		{
			// Load the authenticator key & QR code URI to display on the form
			var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
			if (string.IsNullOrEmpty(unformattedKey))
			{
				await _userManager.ResetAuthenticatorKeyAsync(user);
				unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
			}

			SharedKey = FormatKey(unformattedKey);

			var email = await _userManager.GetEmailAsync(user);
			AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
			return new string[] {SharedKey,AuthenticatorUri};
		}

		private string FormatKey(string unformattedKey)
		{
			var result = new StringBuilder();
			int currentPosition = 0;
			while (currentPosition + 4 < unformattedKey.Length)
			{
				result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
				currentPosition += 4;
			}
			if (currentPosition < unformattedKey.Length)
			{
				result.Append(unformattedKey.AsSpan(currentPosition));
			}

			return result.ToString().ToLowerInvariant();
		}

		private string GenerateQrCodeUri(string email, string unformattedKey)
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				AuthenticatorUriFormat,
				_urlEncoder.Encode("GabinetePsicologia"),
				_urlEncoder.Encode(email),
				unformattedKey);
		}

	}
}


