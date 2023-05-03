using GabinetePsicologia.Server.Data;
using GabinetePsicologia.Server.Data.Migrations;
using GabinetePsicologia.Server.Models;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

using Paciente = GabinetePsicologia.Shared.Paciente;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace GabinetePsicologia.Server.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]

    public class InformeController : Controller
    {
        private readonly ApplicationDbContext _context;


        public InformeController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<IActionResult> getInformes()
        {
            var lsInforme = _context.Informes.ToList();
            var lsInformeDto  =  new List<InformeDto>();
            foreach (var inf in lsInforme)
            {
                InformeDto infDto = new InformeDto();
                infDto.PacienteId= inf.PacienteId;
                infDto.PsicologoId = inf.PsicologoId;
                infDto.TrastornoId = inf.TrastornoId;
                infDto.Id = inf.Id;
                infDto.Resultados = inf.Resultados;
                infDto.PlandDeTratamiento = inf.PlandDeTratamiento;
                infDto.AntecendentesPersonales = inf.AntecendentesPersonales;
                infDto.EvaluacionPsicologica = inf.EvaluacionPsicologica;
                infDto.UltimaFecha = inf.UltimaFecha;
                infDto.Severidad = inf.Severidad;
                Paciente paciente = _context.Pacientes.FirstOrDefault(x=>x.Id == inf.PacienteId);
                if(paciente != null)
                    infDto.PacienteFullName = paciente.FullName;
                Trastorno trastorno = _context.Trastornos.FirstOrDefault(x => x.Id == inf.TrastornoId);
                if (trastorno != null)
                {
                    infDto.TrastornoName = trastorno.Nombre;
                    infDto.TrastornoTipo = trastorno.Tipo;
                }
                   
                Psicologo psicologo = _context.Psicologos.FirstOrDefault(x => x.Id == inf.PsicologoId);
                if (psicologo != null)
                    infDto.PsicologoFullName = psicologo.FullName;
                lsInformeDto.Add(infDto);
            }

            return Ok(lsInformeDto);
        }

        [HttpPost]
        public IActionResult Register([FromBody] Informe trastorno)
        {
            _context.Informes.Add(trastorno);
            _context.SaveChanges();
            return Ok("Informe Añadido");
        }
        [HttpPost("Actualizar")]
        public IActionResult Actualizar([FromBody] Informe informe)
        {
            var inf = _context.Informes.FirstOrDefault(x=> x.Id == informe.Id);
            _context.Informes.Remove(inf);
            _context.Add(informe);
            _context.SaveChanges();
            return Ok("Informe Actualizado");
        }

        //[HttpPost("Borrar")]

        //public IActionResult Borrar([FromBody] IList<Trastorno> trastornos)
        //{
        //    foreach (var a in trastornos)
        //    {
        //        if (_context.Trastornos.Where(x => x.Id != Guid.Empty && x.Id == a.Id).Any())
        //            _context.Trastornos.Remove(a);
        //    }
        //    _context.SaveChanges();
        //    return Ok("Trastornos Eliminados Correctamente");
        //}

        //[HttpPost("Editar")]

        //public IActionResult Editar([FromBody] Trastorno trastorno)
        //{
        //    _context.Trastornos.Update(trastorno);
        //    _context.SaveChanges();
        //    return Ok("Trastorno editado Correctamente");
        //}

    }
}