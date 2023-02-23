using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace GabinetePsicologia.Client.Pages.Psicologo
{
    public partial class Trastornos
    {
        bool allowRowSelectOnRowClick = false;
        IEnumerable<Trastorno> LsTrastornos;
        IList<Trastorno> selectedTrastornos;
        RadzenDataGrid<Trastorno> grid;
        Trastorno trastornoForm = new Trastorno();

        [Inject] private TrastornosServices TrastornosServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            LsTrastornos = await TrastornosServices.getTrastornos();

        }
        private void GuardarTrastorno()
        {
            if(trastornoForm.Id == Guid.Empty)
                TrastornosServices.AñadirTrastorno(trastornoForm);
            else
                TrastornosServices.EditarTrastornos(trastornoForm);
            DialogService.Close();
            grid.Reload();
        }
        private void BorrarTrastorno()
        {
            TrastornosServices.BorrarTrastornos(selectedTrastornos);
            DialogService.Close();
            grid.Reload();
        }

    }
}
