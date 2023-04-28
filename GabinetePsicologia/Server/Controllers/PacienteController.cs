using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using Paciente = GabinetePsicologia.Shared.Paciente;

namespace GabinetePsicologia.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
  
    public class PacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PacienteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Paciente>>> GetPacientes()
        {
            return Ok(_context.Pacientes.ToList());
        }
    
        [HttpPost]
        public async Task<ActionResult> RegisterPaciente(Paciente paciente)
        {

            if(paciente == null) return BadRequest();
            _context.Pacientes.Add(paciente);
            var user = _context.Users.FirstOrDefault(x => x.Id == paciente.ApplicationUserId);
            await _userManager.AddToRoleAsync(user, "Paciente");
            _context.SaveChanges();
            return Ok();
        }
        [HttpPost("Update")]
        public async Task<ActionResult> UpdatePaciente(Paciente paciente)
        {
            var removePaciente = _context.Pacientes.First(x => x.ApplicationUserId == paciente.ApplicationUserId);
            _context.Pacientes.Remove(removePaciente);
            _context.SaveChanges();
            _context.Pacientes.Add(paciente);
            _context.SaveChanges();
            return Ok();
        }
        [HttpGet("Username/{username}")]
        public Paciente GetPsicologoByUsername( string username)
        {
            using (SqlConnection con = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetPacienteByUserName", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    con.Open();
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        Paciente paciente = new Paciente();
                        while (reader.Read())
                        {


                            paciente.Id = (Guid)reader["Id"];
                            paciente.Nombre = (string)reader["Nombre"];
                            paciente.Apellido1 = (string)reader["Apellido1"];
                            paciente.Apellido2 = (string)reader["Apellido2"];
                            paciente.NIF = (string)reader["NIF"];
                            paciente.Direccion = (string)reader["Direccion"];
                            paciente.FecNacim = (DateTime)reader["FecNacim"];
                            paciente.ApplicationUserId = (string)reader["ApplicationUserId"];

                        }
                        con.Close();
                        return paciente;
                    }

                }


            }
        }
    }
}
