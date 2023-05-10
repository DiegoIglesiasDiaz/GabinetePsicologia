using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using System;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace GabinetePsicologia.Client.Pages
{
    public partial class InformesModal
    {
        [Parameter]
        public InformeDto Informe { get; set; }
        [Parameter]
        public bool isNew { get; set; }
        [Parameter]
        public ClaimsPrincipal? user { get; set; }

        public string edad = "";
        [Inject] public PacientesServices PacientesServices { get; set; }
        [Inject] public PsicologoServices PsicologoServices { get; set; }
        [Inject] public NotificationService NotificationService { get; set; }
        [Inject] public TrastornosServices TrastornosServices { get; set; }
        [Inject] public InformesServices InformesServices { get; set; }
       
        public string cssClass = "textArea calendar";
        public bool isEdit = false;
        public bool isPsicologo = false;
      
        List<Trastorno> lsTrastornos = new List<Trastorno>();
        List<Paciente> lsPacientes = new List<Paciente>();
        RadzenUpload upload = new RadzenUpload();
        List<string[]> lsFiles = new List<string[]>();
        string cssButtonArchivo = "";
        string cssButtonEnlace = "BotonPrincipal";
        bool verArchivos = true;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            lsTrastornos = await TrastornosServices.getTrastornos();
            if(Informe.Id !=Guid.Empty)
            {
                Paciente paciente = await PacientesServices.GetPacienteByid(Informe.PacienteId);
                if (paciente != null)
                {
                    try
                    {
                        int edadNum = 0;
                        if (paciente != null)
                        {
                            edadNum = DateTime.Now.Year - paciente.FecNacim.Value.Year;
                            if (edadNum > 0)
                                edad = edadNum.ToString() + " años";
                        }
                    }catch(Exception ex)
                    {
                        
                    }
                    
                }
            }
            if (user.IsInRole("Paciente"))
            {
                var pac =  await PacientesServices.GetPacienteByUsername(user.Identity.Name);
                if (isNew)
                {
                    Informe.PacienteId = pac.Id;
                    Informe.PsicologoFullName = pac.FullName;
                }
                 if(!isNew)
                    lsFiles = await InformesServices.ListFilesPaciente(Informe.Id.ToString());
            }
            if (user.IsInRole("Psicologo"))
            {
                var ps = await PsicologoServices.GetPsicologoByUsername(user.Identity.Name);
                if (isNew)
                {
                    Informe.PsicologoId = ps.Id;
                    Informe.PacienteId = Guid.Empty;
                }
                    
                isPsicologo = true;
                if(!isNew)
                    lsFiles = await InformesServices.ListFiles(Informe.Id.ToString());
            }

            lsTrastornos = await TrastornosServices.getTrastornos();
            lsPacientes = await PacientesServices.getPacientes();
           
        }

        protected async override void OnParametersSet()
        {
          
        }
        public void Guardar()
        {
            if (isNew)
            {
                if (Informe.PacienteId == Guid.Empty || Informe.TrastornoId == Guid.Empty)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de seleccionar al menos un Paciente y un Trastorno");
                    return;
                }
                Informe.PacienteFullName = lsPacientes.FirstOrDefault(x => x.Id == Informe.PacienteId).FullName;
                Informe.TrastornoName = lsTrastornos.FirstOrDefault(x => x.Id == Informe.TrastornoId).Nombre;
                Informe.TrastornoTipo = lsTrastornos.FirstOrDefault(x => x.Id == Informe.TrastornoId).Tipo;
            }

           
            Informe.UltimaFecha = DateTime.Now;
            if(!isNew)
                InformesServices.CrearOActalizarInforme(Informe,isNew);
            DialogService.Close(Informe);
        }
        public void changePaciente(object args)
        {
            Informe.PacienteId = Guid.Parse(args.ToString());
        }
        public void changeTrastorno(object args)
        {
            if(args == null)
            {
                Informe.TrastornoId = Guid.Empty;
            }
            else
            {
                Informe.TrastornoId = Guid.Parse(args.ToString());
            }
          
        }
        public async void complete(UploadCompleteEventArgs args)
        {
            var data = args.RawResponse.Split('"');
            foreach (var item in data)
            {
                if (!Regex.IsMatch(item, @"[a-zA-Z]"))
                {
                    continue;
                }
                //string output = Regex.Replace(item, @"[^\w\s.\!@$%^&*()\-\/]+", "");
                lsFiles.Add(new string[]
                {
                    item,
                    "Si"
                });
            }
            NotificationService.Notify(NotificationSeverity.Success,"Ok", "Archivo Subido correctamente");
        }
        public async void completePrivate(UploadCompleteEventArgs args)
        {
            var data = args.RawResponse.Split('"');
            foreach (var item in data)
            {
                if (!Regex.IsMatch(item, @"[a-zA-Z]"))
                {
                    continue;
                }
                //string output = Regex.Replace(item, @"[^\w\s.\!@$%^&*()\-\/]+", "");
                lsFiles.Add(new string[]
                {
                    item,
                    "No"
                });
            }
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Archivo Subido correctamente");
        }
        public void error(UploadErrorEventArgs args)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Error","Archivo muy pesado (Max 30mb)");
        }
        public void clickArchivo()
        {
             cssButtonArchivo = "";
            cssButtonEnlace = "BotonPrincipal";
            verArchivos = true;


        } 
        public void clickEnlace()
        {
             cssButtonEnlace = "";
            cssButtonArchivo = "BotonPrincipal";
            verArchivos = false;
        }
        public void Descargar(string[] file)
        {
            string fileName = file[0];
            string folder = Informe.Id.ToString();
            if (file[1] == "No")
            {
                folder += "//Private";
            }

            InformesServices.Descargar(folder, fileName);
        }
        public async void Borrar(string[] file)
        {
            
            string fileName = file[0];
            string folder = Informe.Id.ToString();
            if (file[1] == "No")
            {
                folder += "//Private";
            }
            bool? a = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar {fileName}?");
            if (a!= null && a == true)
            {
                lsFiles.Remove(file);
                InformesServices.BorrarArchivo(folder, fileName);
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Archivo Borrado Correctamente");

            }

            

        }

    }

}
