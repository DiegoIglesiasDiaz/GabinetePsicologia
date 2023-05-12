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
        public string EnlaceTextBox = "";
        public string EnlacePrivateTextBox = "";
        public bool isEdit = false;
        public bool isPsicologo = false;
        public int NewSeveridad = 1;
        public Trastorno selectedTrastorno;
        public bool isEditSeveridad = false;
        public Guid IdTrastornoEdit = Guid.Empty;
        RadzenDropDown<Trastorno> DropDownTrastorno;
        List<Trastorno> lsTrastornos = new List<Trastorno>();
        List<Paciente> lsPacientes = new List<Paciente>();
        RadzenUpload upload = new RadzenUpload();
        List<string[]> lsFiles = new List<string[]>();
        List<string[]> lsEnlaces = new List<string[]>();
        string cssButtonArchivo = "";
        string cssButtonEnlace = "BotonPrincipal";
        bool verArchivos = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (Informe.Id != Guid.Empty)
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
                    }
                    catch (Exception ex)
                    {

                    }

                }
                if (!String.IsNullOrEmpty(Informe.Enlaces))
                {

                    var splitEnlaces = Informe.Enlaces.Split(";");
                    foreach (var enlace in splitEnlaces)
                    {
                        if (String.IsNullOrWhiteSpace(enlace)) continue;

                        lsEnlaces.Add(new string[]
                        {
                        enlace,
                        "Si"
                        });

                    }
                }
                if (!String.IsNullOrEmpty(Informe.EnlacesPrivate) && user.IsInRole("Psicologo"))
                {
                    var splitEnlacesPrivado = Informe.EnlacesPrivate.Split(";");
                    foreach (var enlace in splitEnlacesPrivado)
                    {
                        if (String.IsNullOrWhiteSpace(enlace)) continue;

                        lsEnlaces.Add(new string[]
                        {
                        enlace,
                        "No"
                        });

                    }

                }
            }
            if (user.IsInRole("Paciente"))
            {
                var pac = await PacientesServices.GetPacienteByUsername(user.Identity.Name);
                if (isNew)
                {
                    Informe.PacienteId = pac.Id;
                    Informe.PsicologoFullName = pac.FullName;
                }
                //if (!isNew)
                // lsFiles = await InformesServices.ListFilesPaciente(Informe.Id.ToString());
            }
            if (user.IsInRole("Psicologo"))
            {

                lsTrastornos = await TrastornosServices.getTrastornos();
                var ps = await PsicologoServices.GetPsicologoByUsername(user.Identity.Name);
                if (isNew)
                {
                    Informe.PsicologoId = ps.Id;
                    Informe.PacienteId = Guid.Empty;
                    lsPacientes = await PacientesServices.getPacientes();
                }

                isPsicologo = true;
                //  if (!isNew)
                // lsFiles = await InformesServices.ListFiles(Informe.Id.ToString());
            }


        }

        protected async override void OnParametersSet()
        {

        }
        public void Guardar()
        {
            if (isNew)
            {
                if (Informe.PacienteId == Guid.Empty || Informe.lsInformeTrastornos.Count < 1)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", "Debes de seleccionar al menos un Paciente y un Trastorno");
                    return;
                }
                Informe.PacienteFullName = lsPacientes.FirstOrDefault(x => x.Id == Informe.PacienteId).FullName;
            }


            Informe.UltimaFecha = DateTime.Now;
            if (!isNew)
                InformesServices.CrearOActalizarInforme(Informe, isNew, false);
            DialogService.Close(Informe);
        }
        public void changePaciente(object args)
        {
            Guid Id = Guid.Empty;
            if (args != null && Guid.TryParse(args.ToString(),out Id))
            {
                Informe.PacienteId = Id;
            }
        }
        public void changeTrastorno(object args)
        {
            if (args == null)
            {
                selectedTrastorno = null;
                return;
            }
            else
            {
                var trst = lsTrastornos.FirstOrDefault(x => x.Id == Guid.Parse(args.ToString()));
                selectedTrastorno = trst;

            }

        }
        public void GuardarTrastorno()
        {
            if (selectedTrastorno == null)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "No has seleccionado nigún Trastorno");
                return;
            }
            if (Informe.lsInformeTrastornos.Where(x => x.TrastornoId == selectedTrastorno.Id).Any())
            {
                NotificationService.Notify(NotificationSeverity.Warning, "", "Ya has seleccionado este Trastorno");
                return;
            }
            Informe.lsInformeTrastornos.Add(new InformeTrastorno()
            {
                Id = Guid.NewGuid(),
                TrastornoId = selectedTrastorno.Id,
                TrastornoName = selectedTrastorno.Nombre,
                TrastornoTipo = selectedTrastorno.Tipo,
                InformeId = Informe.Id,
                Severidad = NewSeveridad

            });
            DropDownTrastorno.SelectedItem = null;
            selectedTrastorno = null;
            NewSeveridad = 1;

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
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Archivo Subido correctamente");
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
            NotificationService.Notify(NotificationSeverity.Error, "Error", "Archivo muy pesado (Max 30mb)");
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
            if (a != null && a == true)
            {
                lsFiles.Remove(file);
                InformesServices.BorrarArchivo(folder, fileName);
                NotificationService.Notify(NotificationSeverity.Success, "Ok", "Archivo Borrado Correctamente");

            }

        }
        public async void BorrarEnlace(string[] enlace)
        {

            bool? a = await DialogService.OpenAsync<ConfirmModal>($"¿Desea Borrar el Enlace?");
            if (a == null || a == false)
                return;

            var enlacesBBDD = "";
            string[] splitEnlace;
            if (enlace[1] == "Si")
            {
                splitEnlace = Informe.Enlaces.Split(";");
            }
            else
            {
                splitEnlace = Informe.EnlacesPrivate.Split(";");

            }
            foreach (var enl in splitEnlace)
            {
                if (enl.Equals(enlace[0]))
                    continue;

                enlacesBBDD += enl + ";";
            }

            lsEnlaces.Remove(enlace);
            if (enlace[1] == "Si")
            {
                InformesServices.setEnlaces(Informe.Id.ToString(), enlacesBBDD);
            }
            else
            {
                InformesServices.setEnlacesPrivado(Informe.Id.ToString(), enlacesBBDD);
            }

            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Enlace Borrado Correctamente");
        }
        public void subirEnlace()
        {
            if (String.IsNullOrWhiteSpace(EnlaceTextBox) || String.IsNullOrWhiteSpace(EnlaceTextBox))
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "No has introducido ningún Enlace");
                return;
            }
            Informe.Enlaces += EnlaceTextBox + ";";
            lsEnlaces.Add(new string[]
                        {
                        EnlaceTextBox,
                        "Si"
                        });
            InformesServices.setEnlaces(Informe.Id.ToString(), Informe.Enlaces);
            EnlaceTextBox = "";

        }
        public void subirEnlacePrivate()
        {
            if (String.IsNullOrWhiteSpace(EnlacePrivateTextBox) || String.IsNullOrWhiteSpace(EnlacePrivateTextBox))
            {
                NotificationService.Notify(NotificationSeverity.Error, "Error", "No has introducido ningún Enlace");
                return;
            }
            Informe.EnlacesPrivate += EnlacePrivateTextBox + ";";
            lsEnlaces.Add(new string[]
                        {
                        EnlacePrivateTextBox,
                        "No"
                        });
            InformesServices.setEnlacesPrivado(Informe.Id.ToString(), Informe.EnlacesPrivate);
            EnlacePrivateTextBox = "";

        }
        public void BorrarTrastorno(InformeTrastorno inf)
        {
            Informe.lsInformeTrastornos.Remove(inf);
        }
        public void CloseModal()
        {
            DialogService.Close();
        }
        public void EditSeveridad(Guid id)
        {
            IdTrastornoEdit = id;
            isEditSeveridad = !isEditSeveridad;
        }
        public void GuardarSeveridad(Guid id)
        {
            if (!isNew)
            {
                var a = Informe.lsInformeTrastornos.FirstOrDefault(x => x.Id == id);
                InformesServices.setSeveridad(id, a.Severidad);
            }
            NotificationService.Notify(NotificationSeverity.Success, "Ok", "Guardado Correctamente");
            isEditSeveridad = false;
            IdTrastornoEdit = Guid.Empty;

        }

    }
}
