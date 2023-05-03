using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Claims;

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
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            lsTrastornos = await TrastornosServices.getTrastornos();
            if(Informe.Id !=Guid.Empty)
            {
                Paciente paciente = await PacientesServices.GetPacienteByid(Informe.PacienteId);
                int edadNum = DateTime.Now.Year - paciente.FecNacim.Value.Year;
                edad = edadNum.ToString() + " años";

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
    }

}
