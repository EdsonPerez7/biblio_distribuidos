using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NeoLibroAPI.Interfaces;
using NeoLibroAPI.Models.DTOs;
using NeoLibroAPI.Models.Entities;

namespace NeoLibroAPI.Data
{
    public class HttpLibroRepository : ILibroRepository
    {
        private readonly HttpClient _httpClient;

        public HttpLibroRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public LibroDTO? ObtenerPorId(int id)
        {
            try
            {
                var response = _httpClient.GetAsync($"api/Libros/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadFromJsonAsync<LibroDTO>().Result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HttpLibroRepository] Error al obtener libro {id}: {ex.Message}");
            }
            return null;
        }

        public List<LibroDTO> Listar() => throw new NotImplementedException();
        public bool Crear(Libro libro) => throw new NotImplementedException();
        public bool Modificar(Libro libro) => throw new NotImplementedException();
        public bool Eliminar(int id) => throw new NotImplementedException();
        public List<LibroDTO> Buscar(string? autor, string? titulo) => throw new NotImplementedException();
        public List<string> ObtenerAutoresPorLibro(int libroId) => throw new NotImplementedException();
        public List<string> ObtenerCategoriasPorLibro(int libroId) => throw new NotImplementedException();
    }
}
