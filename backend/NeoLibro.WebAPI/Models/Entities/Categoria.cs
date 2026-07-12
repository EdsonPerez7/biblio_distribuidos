namespace NeoLibroAPI.Models.Entities
{
    public class Categoria
    {
        public int CategoriaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
        // Propiedades de navegación
        public List<Libro>? Libros { get; set; }
    }
}
