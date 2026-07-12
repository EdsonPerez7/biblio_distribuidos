using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NeoLibroAPI.Interfaces;
using NeoLibroAPI.Models.Entities;

namespace NeoLibroAPI.Data
{
    public class HttpEjemplarRepository : IEjemplarRepository
    {
        private readonly HttpClient _httpClient;

        public HttpEjemplarRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Ejemplar? ObtenerPorId(int id)
        {
            try
            {
                var response = _httpClient.GetAsync($"api/Ejemplares/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadFromJsonAsync<Ejemplar>().Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HttpEjemplarRepository] Error al obtener ejemplar {id}: {ex.Message}");
            }
            return null;
        }

        public List<Ejemplar> Listar() => throw new NotImplementedException();
        public List<Ejemplar> ListarPorLibro(int libroId) => throw new NotImplementedException();
        public Ejemplar? ObtenerPorCodigoBarras(string codigoBarras) => throw new NotImplementedException();
        public bool Crear(Ejemplar ejemplar) => throw new NotImplementedException();
        public bool Modificar(Ejemplar ejemplar) => throw new NotImplementedException();
        public bool Eliminar(int id) => throw new NotImplementedException();
        public int ObtenerSiguienteNumeroEjemplar(int libroId) => throw new NotImplementedException();
        public bool ExisteCodigoBarras(string codigoBarras) => throw new NotImplementedException();
    }
}
