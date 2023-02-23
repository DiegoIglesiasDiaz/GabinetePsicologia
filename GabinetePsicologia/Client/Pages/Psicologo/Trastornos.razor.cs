using GabinetePsicologia.Client.Services;
using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Radzen.Blazor;
using System;

namespace GabinetePsicologia.Client.Pages.Psicologo
{
    public partial class Trastornos
    {
        bool allowRowSelectOnRowClick = true;
        IEnumerable<Trastorno> LsTrastornos;
        IList<Trastorno> selectedTrastornos;
        RadzenDataGrid<Trastorno> grid;
        Trastorno trastornoForm = new Trastorno();
        public bool isInRole = false;
        [Inject] private TrastornosServices TrastornosServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider{ get; set; }
    protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var User = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (User.IsInRole("Psicologo") || User.IsInRole("Administrador"))isInRole = true;
            LsTrastornos = await TrastornosServices.getTrastornos();

        }
        private void GuardarTrastorno()
        {
            TrastornosServices.AñadirTrastorno(trastornoForm);
            DialogService.Close();
            NavigationManager.NavigateTo("/Psicologo/Trastornos",true);


        }
        
    }
}
