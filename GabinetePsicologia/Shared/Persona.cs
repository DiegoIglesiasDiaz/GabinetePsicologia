
namespace GabinetePsicologia.Shared;

public class Persona 
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido1 { get; set; }
    public string Apellido2 { get; set; }
    public string Email { get; set; }
    public string FullName
    {
        get
        {
            return Apellido1 + ", " + Apellido2;
        }
    }
  
    public string NIF { get; set; }
    public string ApplicationUserId { get;set; }

}