using System;
using System.Threading.Tasks;
using MassTransit;
using Shared.Contracts;
using Microsoft.Extensions.Logging;

namespace Notificaciones.Service.Consumers
{
    public class PrestamoCreadoConsumer : IConsumer<PrestamoCreadoEvent>
    {
        private readonly ILogger<PrestamoCreadoConsumer> _logger;

        public PrestamoCreadoConsumer(ILogger<PrestamoCreadoConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PrestamoCreadoEvent> context)
        {
            var evt = context.Message;
            _logger.LogInformation("==================================================");
            _logger.LogInformation("📬 NUEVO EVENTO RECIBIDO: PrestamoCreadoEvent");
            _logger.LogInformation("   ID Prestamo: {PrestamoId}", evt.PrestamoId);
            _logger.LogInformation("   ID Usuario: {UsuarioId}", evt.UsuarioId);
            _logger.LogInformation("   Email: {EmailUsuario}", evt.EmailUsuario);
            _logger.LogInformation("   Libro: {TituloLibro}", evt.TituloLibro);
            _logger.LogInformation("   Devolucion Prevista: {FechaDevolucionPrevista:dd/MM/yyyy}", evt.FechaDevolucionPrevista);
            _logger.LogInformation("   Simulacion: Enviando email a {EmailUsuario} para el libro '{TituloLibro}'...", evt.EmailUsuario, evt.TituloLibro);
            _logger.LogInformation("==================================================");

            return Task.CompletedTask;
        }
    }
}
