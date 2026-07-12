using System;

namespace Shared.Contracts
{
    public record PrestamoCreadoEvent
    {
        public int PrestamoId { get; init; }
        public int UsuarioId { get; init; }
        public string EmailUsuario { get; init; } = string.Empty;
        public string TituloLibro { get; init; } = string.Empty;
        public DateTime FechaDevolucionPrevista { get; init; }
    }
}
