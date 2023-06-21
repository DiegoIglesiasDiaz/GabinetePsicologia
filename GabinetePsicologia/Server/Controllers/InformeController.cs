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
using Radzen;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using InformeTrastorno = GabinetePsicologia.Shared.InformeTrastorno;
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
            var lsInformeDto = new List<InformeDto>();
            foreach (var inf in lsInforme)
            {
                InformeDto infDto = new InformeDto();
                infDto.PacienteId = inf.PacienteId;
                infDto.PsicologoId = inf.PsicologoId;
                infDto.Id = inf.Id;
                infDto.Resultados = inf.Resultados;
                infDto.PlandDeTratamiento = inf.PlandDeTratamiento;
                infDto.AntecendentesPersonales = inf.AntecendentesPersonales;
                infDto.EvaluacionPsicologica = inf.EvaluacionPsicologica;
                infDto.UltimaFecha = inf.UltimaFecha;
                infDto.Enlaces = inf.Enlaces;
                infDto.EnlacesPrivate = inf.EnlacesPrivate;
                Paciente paciente = _context.Pacientes.FirstOrDefault(x => x.Id == inf.PacienteId);
                if (paciente != null)
                    infDto.PacienteFullName = paciente.FullName;
                infDto.lsInformeTrastornos = _context.InformeTrastorno.Where(x => x.InformeId == infDto.Id).ToList();

                Psicologo psicologo = _context.Psicologos.FirstOrDefault(x => x.Id == inf.PsicologoId);
                if (psicologo != null)
                    infDto.PsicologoFullName = psicologo.FullName;
                lsInformeDto.Add(infDto);
            }

            return Ok(lsInformeDto);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> getInformesById(Guid id)
        {
            var lsInforme = _context.Informes.ToList();
            var lsInformeDto = new List<InformeDto>();

            Psicologo? psicologo = _context.Psicologos.FirstOrDefault(x => x.ApplicationUserId == id.ToString());
            Paciente? paciente = _context.Pacientes.FirstOrDefault(x => x.ApplicationUserId == id.ToString());

            if (psicologo == null && paciente == null) return Ok(lsInformeDto);

            foreach (var inf in lsInforme)
            {
                bool volver = true;
                if (psicologo != null && psicologo.Id == inf.PsicologoId)
                {
                    volver = false;
                }
                if (paciente != null && paciente.Id == inf.PacienteId)
                {
                    volver = false;
                }

                if (volver) continue;

                InformeDto infDto = new InformeDto();
                infDto.PacienteId = inf.PacienteId;
                infDto.Resultados = inf.Resultados;
                infDto.PlandDeTratamiento = inf.PlandDeTratamiento;
                infDto.Id = inf.Id;
                infDto.AntecendentesPersonales = inf.AntecendentesPersonales;
                infDto.EvaluacionPsicologica = inf.EvaluacionPsicologica;
                infDto.PsicologoId = inf.PsicologoId;
                infDto.EnlacesPrivate = inf.EnlacesPrivate;
                infDto.lsInformeTrastornos = _context.InformeTrastorno.Where(x => x.InformeId == infDto.Id).ToList();


                infDto.UltimaFecha = inf.UltimaFecha;
                infDto.Enlaces = inf.Enlaces;

                Paciente pacienteInforme = _context.Pacientes.FirstOrDefault(x => x.Id == inf.PacienteId);
                if (pacienteInforme != null)
                    infDto.PacienteFullName = pacienteInforme.FullName;

                Psicologo psicologoInforme = _context.Psicologos.FirstOrDefault(x => x.Id == inf.PsicologoId);
                if (psicologoInforme != null)
                    infDto.PsicologoFullName = psicologoInforme.FullName;
                lsInformeDto.Add(infDto);
            }

            return Ok(lsInformeDto);
        }

        [HttpGet("GetInformePaciente/{id:guid}")]
        public async Task<IActionResult> getInformesPacienteById(Guid id)
        {
            var lsInforme = _context.Informes.ToList();
            var lsInformeDto = new List<InformeDto>();

            Psicologo? psicologo = _context.Psicologos.FirstOrDefault(x => x.ApplicationUserId == id.ToString());
            Paciente? paciente = _context.Pacientes.FirstOrDefault(x => x.ApplicationUserId == id.ToString());

            if (psicologo == null && paciente == null) return Ok(lsInformeDto);

            foreach (var inf in lsInforme)
            {
                bool volver = true;
                if (psicologo != null && psicologo.Id == inf.PsicologoId)
                {
                    volver = false;
                }
                if (paciente != null && paciente.Id == inf.PacienteId)
                {
                    volver = false;
                }

                if (volver) continue;

                InformeDto infDto = new InformeDto();
                infDto.PacienteId = inf.PacienteId;
                infDto.Resultados = inf.Resultados;
                infDto.PlandDeTratamiento = inf.PlandDeTratamiento;
                infDto.UltimaFecha = inf.UltimaFecha;
                infDto.Enlaces = inf.Enlaces;

                Paciente pacienteInforme = _context.Pacientes.FirstOrDefault(x => x.Id == inf.PacienteId);
                if (pacienteInforme != null)
                    infDto.PacienteFullName = pacienteInforme.FullName;

                Psicologo psicologoInforme = _context.Psicologos.FirstOrDefault(x => x.Id == inf.PsicologoId);
                if (psicologoInforme != null)
                    infDto.PsicologoFullName = psicologoInforme.FullName;
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
        [HttpPost("Borrar")]
        public IActionResult Borrar([FromBody] IList<InformeDto> Informes)
        {
            foreach (var inf in Informes)
            {
                string folder = inf.Id.ToString();

                string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", folder);
                if (Directory.Exists(DirectoryPath))
                {
                    DirectoryInfo di = new DirectoryInfo(DirectoryPath);
                    di.Delete(true);

                }
                var i = _context.Informes.FirstOrDefault(x => x.Id == inf.Id);
                if (i != null)
                    _context.Informes.Remove(i);

                var LsinfTrst = _context.InformeTrastorno.Where(x => x.InformeId == inf.Id).ToList();
                foreach (var item in LsinfTrst)
                {
                    _context.InformeTrastorno.Remove(item);
                }
            }
            _context.SaveChanges();
            return Ok("Informe Eliminado");
        }
        [HttpPost("Actualizar")]
        public IActionResult Actualizar([FromBody] Informe informe)
        {
            var inf = _context.Informes.FirstOrDefault(x => x.Id == informe.Id);
            _context.Informes.Remove(inf);
            _context.Add(informe);
            _context.SaveChanges();
            return Ok("Informe Actualizado");
        }
        [AllowAnonymous]
        [HttpPost("UploadFile")]
        public async Task<IActionResult> Upload([FromHeader] string InformeId)
        {
            List<string> filesnames = new List<string>();
            var files = Request.Form.Files;
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "uploads"));
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "uploads", InformeId));
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", InformeId, fileName);
                    if (!System.IO.File.Exists(filePath))
                    {
                        filesnames.Add(fileName);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);

                    }
                }
            }
            return Ok(filesnames);
        }
        [AllowAnonymous]
        [HttpPost("UploadFilePrivate")]
        public async Task<IActionResult> UploadPrivate([FromHeader] string InformeId)
        {
            List<string> filesnames = new List<string>();
            var files = Request.Form.Files;
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "uploads"));
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "uploads", InformeId));
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "uploads", InformeId, "Private"));
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", InformeId, "Private", fileName);
                    if (!System.IO.File.Exists(filePath))
                    {
                        filesnames.Add(fileName);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);

                    }
                }
            }
            return Ok(filesnames);
        }
        [HttpGet("Files/{id}")]
        public IActionResult GetFiles(string id)
        {
            var lsFiles = new List<string[]>();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", id);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var f in di.GetFiles())
                {
                    var a = new String[]
                    {
                    f.Name,
                    "Si"
                    };
                    lsFiles.Add(a);
                }

            }
            path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", id, "Private");
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var f in di.GetFiles())
                {
                    var a = new String[]
                    {
                    f.Name,
                    "No"
                    };
                    lsFiles.Add(a);
                }

            }
            return Ok(lsFiles);
        }
        [HttpGet("FilesPaciente/{id}")]
        public IActionResult GetFilesPaciente(string id)
        {
            var lsFiles = new List<string[]>();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "uploads", id);
            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var f in di.GetFiles())
                {
                    var a = new String[]
                    {
                    f.Name,
                    "Si"
                    };
                    lsFiles.Add(a);
                }

            }

            return Ok(lsFiles);
        }

        [HttpPost("Files/Download")]
        public IActionResult Descargar([FromBody] string[] file)
        {
            string folder = file[0];
            string fileName = file[1];
            string localFilePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", folder, fileName);
            if (System.IO.File.Exists(localFilePath))
            {
                var bytes = System.IO.File.ReadAllBytes(localFilePath);
                //FileStream fs = new FileStream(localFilePath, FileMode.Open);
                return File(bytes, "application/octec-stream", fileName);
            }

            return Content("Archivo No Encotrado");


        }
        [HttpPost("Files/Borrar")]
        public IActionResult BorrarFiles([FromBody] string[] file)
        {
            string folder = file[0];
            string fileName = file[1];
            string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", folder);
            if (Directory.Exists(DirectoryPath))
            {
                DirectoryInfo di = new DirectoryInfo(DirectoryPath);
                foreach (var f in di.GetFiles())
                {
                    if (f.Name == fileName)
                    {
                        f.Delete();
                        return Ok();
                    }

                }

            }

            return Ok();

        }
        [HttpPost("Enlaces")]
        public IActionResult setEnlaces([FromBody] string[] file)
        {
            string InformeId = file[0];
            string enlaces = file[1];
            if (enlaces == "") enlaces = null;
            var Informe = _context.Informes.FirstOrDefault(x => x.Id == Guid.Parse(InformeId));
            if (Informe != null)
            {
                Informe.Enlaces = enlaces;
                _context.SaveChanges();
            }

            return Ok("Se ha actualizado Correctamente");

        }
        [HttpPost("EnlacesPrivate")]
        public IActionResult setEnlacesPrivate([FromBody] string[] file)
        {
            string InformeId = file[0];
            string enlaces = file[1];
            if (enlaces == "") enlaces = null;
            var Informe = _context.Informes.FirstOrDefault(x => x.Id == Guid.Parse(InformeId));
            if (Informe != null)
            {
                Informe.EnlacesPrivate = enlaces;
                _context.SaveChanges();
            }

            return Ok("Se ha actualizado Correctamente");

        }
        [HttpPost("InformeTrastorno/Insertar")]
        public IActionResult InformeTrastornoInsertar([FromBody] List<InformeTrastorno> infTrst)
        {
            foreach (var item in infTrst)
            {
                _context.InformeTrastorno.Add(item);
            }
            _context.SaveChanges();
            return Ok("Insertado Correctamenre");
        }
        [HttpPost("InformeTrastorno/Delete")]
        public IActionResult InformeTrastornoDelete([FromBody] Informe inf)
        {
            var lsInfTrst = _context.InformeTrastorno.Where(x => x.InformeId == inf.Id).ToList();
            if (lsInfTrst == null) return Ok("Eliminado Correctamente");
            foreach (var infTrst in lsInfTrst)
            {
                _context.InformeTrastorno.Remove(infTrst);
            }
            _context.SaveChanges();
            return Ok("Eliminado Correctamente");
        }
        [HttpPost("Severidad")]
        public IActionResult setSeveridad([FromBody] string[] data)
        {
            string Id = data[0];
            int Severidad = Int16.Parse(data[1]);
            var a = _context.InformeTrastorno.FirstOrDefault(x => x.Id == Guid.Parse(Id));
            if (a != null)
                a.Severidad = Severidad;
            _context.SaveChanges();
            return Ok("Actualizado Correctamente");
        }
    }
}