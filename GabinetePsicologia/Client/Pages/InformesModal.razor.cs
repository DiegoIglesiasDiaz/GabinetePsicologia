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
                Informe.PacienteId = pac.Id;
            }
            if (user.IsInRole("Psicologo"))
            {
                var ps = await PsicologoServices.GetPsicologoByUsername(user.Identity.Name);
                Informe.PsicologoId = ps.Id;
                Informe.PacienteId = Guid.Empty;
            }

            lsTrastornos = await TrastornosServices.getTrastornos();
            lsPacientes = await PacientesServices.getPacientes();
            lsFiles = await InformesServices.ListFiles(Informe.Id.ToString());
        }

        protected async override void OnParametersSet()
        {
          
        }
        public void Guardar()
        {
            Informe.UltimaFecha = DateTime.Now;
            Informe.Id = Guid.NewGuid();
            InformesServices.CrearOActalizarInforme(Informe,isNew);
           
            DialogService.Close(Informe);
        }
        public void changePaciente(object args)
        {
            Informe.PacienteId = Guid.Parse(args.ToString());
        }
        public void changeTrastorno(object args)
        {
            Informe.TrastornoId = Guid.Parse(args.ToString());
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
        public void Descargar()
        {
             InformesServices.Descargar(Informe.Id.ToString());
        }

    }

}
