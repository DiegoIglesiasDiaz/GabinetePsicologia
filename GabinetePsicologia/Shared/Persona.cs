
using System.Diagnostics.CodeAnalysis;

namespace GabinetePsicologia.Shared;

public abstract class Persona 
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido1 { get; set; }
    [AllowNull]
    public string? Apellido2 { get; set; }
    [AllowNull]
    public DateTime? FecNacim { get; set; }
    [AllowNull]
    public string? Direccion { get; set; }
    public string FullName
    {
        get
        {
            return Nombre +" " + Apellido1 + " " + Apellido2;
        }

    }
    [AllowNull]
    public string? NIF { get; set; }
    public string ApplicationUserId { get;set; }

}