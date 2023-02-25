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
        bool allowRowSelectOnRowClick = false;
        IList<Trastorno> LsTrastornos;
        IList<Trastorno> selectedTrastornos;
        RadzenDataGrid<Trastorno> grid;
        Trastorno formTrastorno = new Trastorno();
        public bool isInRole;
        [Inject] private TrastornosServices TrastornosServices { get; set; }
        [Inject] private DialogService DialogService { get; set; }
        [Inject] AuthenticationStateProvider AuthenticationStateProvider{ get; set; }
        [Inject] NotificationService NotificationService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;
            if (user.IsInRole("Psicologo") || user.IsInRole("Administrador"))
            {
                isInRole = true;
                LsTrastornos = await TrastornosServices.getTrastornos();
            }



        }
        private  void GuardarTrastorno()
        {
            if (formTrastorno.Id == Guid.Empty)
            {
                formTrastorno.Id = Guid.NewGuid();
                TrastornosServices.AñadirTrastorno(formTrastorno);
                LsTrastornos.Add(formTrastorno);
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Trastorno añadido correctamente.");
            }
            else{
                TrastornosServices.EditarTrastornos(formTrastorno);
                var remove = LsTrastornos.FirstOrDefault(x => x.Id == formTrastorno.Id);
                remove.Nombre = formTrastorno.Nombre;
                remove.Tipo = formTrastorno.Tipo;
                remove.Sintomas = formTrastorno.Sintomas;
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Trastorno editado correctamente.");
            }

  
            grid.Reload();
            DialogService.Close();
        }
        private  void  BorrarTrastorno()
        {
            if (selectedTrastornos  == null|| selectedTrastornos.Count == 0)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "No has seleccionado ningún trastorno.");
                return;
            }
            TrastornosServices.BorrarTrastornos(selectedTrastornos);
            foreach(var trast in selectedTrastornos)
            {
                LsTrastornos.Remove(trast);
                
            }
            selectedTrastornos.Clear();
            grid.Reload();
            DialogService.Close();
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Borrado correctamente.");
          
        }

    }
}
