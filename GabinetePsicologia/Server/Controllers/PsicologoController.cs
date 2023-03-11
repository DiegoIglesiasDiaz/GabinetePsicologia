using GabinetePsicologia.Client.Pages;
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
  
    public class PsicologoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public PsicologoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<List<Psicologo>>> GetPsicologos()
        {
            //    var user = await _context.Users.Include(
            //        u=> u.LsPaciente).FirstOrDefaultAsync(
            //        u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _context.Psicologos.ToList();
        }
        [HttpPost]
        public async Task<ActionResult> RegisterPsicologo(Psicologo psicologo)
        {

            if(psicologo == null) return BadRequest();
            _context.Psicologos.Add(psicologo);
            var user = _context.Users.FirstOrDefault(x => x.Id == psicologo.ApplicationUserId);
            await _userManager.AddToRoleAsync(user, "Psicologo");
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("Username/{username}")]
        public Psicologo GetPsicologoByUsername( string username)
        {
            using (SqlConnection con = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetPsicologoByUserName", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    con.Open();
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        Psicologo psicologo = new Psicologo();
                        while (reader.Read())
                        {
                           

                            psicologo.Id = (Guid)reader["Id"];
                            psicologo.Nombre = (string)reader["Nombre"];
                            psicologo.Apellido1 = (string)reader["Apellido1"];
                            psicologo.Apellido2 = (string)reader["Apellido2"];
                            psicologo.NIF = (string)reader["NIF"];
                            psicologo.Direccion = (string)reader["Direccion"];
                            psicologo.FecNacim = (DateTime)reader["FecNacim"];
                            psicologo.ApplicationUserId = (string)reader["ApplicationUserId"]; 
                          
                        }
                        con.Close();
                        return psicologo;
                    }

                }


            }
        }
    }
}
