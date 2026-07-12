using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeoLibroAPI.Interfaces;
using NeoLibroAPI.Models.Entities;
using NeoLibroAPI.Models.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;


namespace NeoLibroAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ILibroBusiness _libroBusiness;
        private readonly IDistributedCache _cache;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(ILibroBusiness libroBusiness, IDistributedCache cache, ILogger<LibrosController> logger)
        {
            _libroBusiness = libroBusiness;
            _cache = cache;
            _logger = logger;
        }

        // GET: api/Libros
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            string cacheKey = "Catalog_Libros_List";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("==================================================");
                _logger.LogInformation("🚀 [REDIS] Cache Hit - Libros obtenidos de memoria");
                _logger.LogInformation("==================================================");
                var listaCached = JsonSerializer.Deserialize<IEnumerable<LibroDTO>>(cachedData);
                return Ok(listaCached);
            }

            _logger.LogInformation("==================================================");
            _logger.LogInformation("🔍 [SQL] Cache Miss - Consultando base de datos");
            _logger.LogInformation("==================================================");

            var lista = _libroBusiness.Listar();

            var serializedData = JsonSerializer.Serialize(lista);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

            return Ok(lista);
        }

        // GET: api/Libros/buscar?autor=nombreAutor&titulo=nombreTitulo
        [HttpGet("buscar")]
        public IActionResult Buscar([FromQuery] string? autor, [FromQuery] string? titulo)
        {
            var resultado = _libroBusiness.Buscar(autor, titulo);
            return Ok(resultado);
        }

        // GET: api/Libros/{id}
        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var libro = _libroBusiness.ObtenerPorId(id);
            if (libro != null)
                return Ok(libro);
            else
                return NotFound(new { mensaje = "Libro no encontrado" });
        }

        // POST: api/Libros
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult Crear([FromBody] Libro libro)
        {
            var resultado = _libroBusiness.Crear(libro);
            if (resultado)
                return Ok(new { mensaje = "Libro creado correctamente" });
            else
                return BadRequest(new { mensaje = "No se pudo crear el libro" });
        }

        // PUT: api/Libros/{id}
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public IActionResult Modificar(int id, [FromBody] Libro libro)
        {
            if (id != libro.LibroID)
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el del cuerpo." });

            var resultado = _libroBusiness.Modificar(libro);
            if (resultado)
                return Ok(new { mensaje = "Libro modificado correctamente" });
            else
                return BadRequest(new { mensaje = "No se pudo modificar el libro" });
        }

        // DELETE: api/Libros/{id}
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var resultado = _libroBusiness.Eliminar(id);
            if (resultado)
                return Ok(new { mensaje = "Libro eliminado correctamente" });
            else
                return BadRequest(new { mensaje = "No se pudo eliminar el libro" });
        }
    }
}