
using System.ComponentModel.DataAnnotations;

namespace GabinetePsicologia.Shared;

public class LoginDto 
{
    [EmailAddress]
    public string Email { get;set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}