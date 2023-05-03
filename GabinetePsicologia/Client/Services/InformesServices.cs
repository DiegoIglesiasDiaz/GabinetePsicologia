﻿using GabinetePsicologia.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GabinetePsicologia.Client.Services
{
    public class InformesServices
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _ClientFactory;
        private readonly NavigationManager _navigationManager;
        public InformesServices(HttpClient httpClient, NavigationManager navigationManager, IHttpClientFactory clientFactory)
        {
            _ClientFactory = clientFactory;
            _httpClient = _ClientFactory.CreateClient("private");
            _navigationManager = navigationManager;

        }
        public async Task<List<InformeDto>> GetInformes()
        {
            var inf = await _httpClient.GetFromJsonAsync<List<InformeDto>>("/Informe");
            return inf;

        }
        public async void CrearOActalizarInforme(InformeDto Informe, bool isNew)
        {
            Informe inf = new Informe()
            {
                AntecendentesPersonales = Informe.AntecendentesPersonales,
                EvaluacionPsicologica = Informe.EvaluacionPsicologica,
                Id = Informe.Id,
                PacienteId = Informe.PacienteId,
                PlandDeTratamiento = Informe.PlandDeTratamiento,
                PsicologoId = Informe.PsicologoId,
                Resultados = Informe.Resultados,
                Severidad = Informe.Severidad,
                TrastornoId = Informe.TrastornoId,
                UltimaFecha = Informe.UltimaFecha
                
                
            };
            if (isNew)
            {
                await _httpClient.PostAsJsonAsync("/Informe", inf);
            }
            else
            {
                await _httpClient.PostAsJsonAsync("/Informe/Actualizar", inf);
            }


        }
    }
}
