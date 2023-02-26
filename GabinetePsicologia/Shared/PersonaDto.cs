
using System.ComponentModel.DataAnnotations;

namespace GabinetePsicologia.Shared;

public class PersonaDto : Persona
{
    [EmailAddress]
    public string Email { get;set; }
    public string? Telefono { get;set; }
    public string? Rol { get;set; }
    public bool isAdmin { get;set; }
    public bool isPsicologo { get; set; }
    public bool isPaciente { get; set; }
    public string? Contraseña { get; set; }
}