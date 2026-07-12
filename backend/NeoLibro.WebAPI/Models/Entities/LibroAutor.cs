using NeoLibroAPI.Models.Entities;

namespace NeoLibroAPI.Models.Entities
{
    public class LibroAutor
    {
        public int LibroID { get; set; }
        public int AutorID { get; set; }
        public bool EsAutorPrincipal { get; set; } = false;
        public int OrdenAutor { get; set; } = 1;
        
        // Propiedades de navegación
        public Libro? Libro { get; set; }
        public Autor? Autor { get; set; }
    }
}
