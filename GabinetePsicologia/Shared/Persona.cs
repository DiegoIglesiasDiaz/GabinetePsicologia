
namespace GabinetePsicologia.Shared;

public abstract class Persona 
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido1 { get; set; }
    public string Apellido2 { get; set; }
    public DateTime FecNacim { get; set; }
    public string Direccion { get; set; }
    public string FullName
    {
        get
        {
            return Apellido1 + " " + Apellido2;
        }

    }
  
    public string NIF { get; set; }
    public string ApplicationUserId { get;set; }

}