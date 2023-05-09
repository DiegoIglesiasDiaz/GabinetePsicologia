using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace GabinetePsicologia.Client.Pages
{
    public partial class ConfirmModal
    {
       
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
          
        }

        protected async override void OnParametersSet()
        {

        }
        public void si()
        {
            DialogService.Close(true);

        }
        public void no()
        {
            DialogService.Close(false);

        }

    }

}
