using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class InformesModal
    {
        [Parameter]
        public InformeDto Informe { get; set; }
        [Parameter]
        public bool isNew { get; set; }
        public string cssClass = "textArea calendar";
        public bool isEdit = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
          
        }

        protected async override void OnParametersSet()
        {

        }
        public void CambiarEdit()
        {
            isEdit=!isEdit;
        }
        
    }

}
