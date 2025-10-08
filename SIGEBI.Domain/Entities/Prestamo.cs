using System;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Entities
{
    public sealed class Prestamo : AggregateRoot
    {
        private Prestamo() { }

        public Guid LibroId { get; private set; }
        public Guid UsuarioId { get; private set; }
        public PeriodoPrestamo Periodo { get; private set; } = null!;
        public EstadoPrestamo Estado { get; private set; } = EstadoPrestamo.Pendiente;
        public DateTime? FechaEntregaRealUtc { get; private set; }
        public string? Observaciones { get; private set; }

        public static Prestamo Solicitar(Guid libroId, Guid usuarioId, PeriodoPrestamo periodo)
        {
            return new Prestamo
            {
                LibroId = libroId,
                UsuarioId = usuarioId,
                Periodo = periodo
            };
        }

        public void Activar()
        {
            if (Estado != EstadoPrestamo.Pendiente)
                throw new InvalidOperationException("Solo un préstamo pendiente puede activarse.");
            Estado = EstadoPrestamo.Activo;
            Touch();
        }

        public void MarcarVencido(DateTime ahoraUtc)
        {
            if (Estado != EstadoPrestamo.Activo) return;
            if (!Periodo.EstaVencido(ahoraUtc)) return;
            Estado = EstadoPrestamo.Vencido;
            Touch();
        }

        public void MarcarDevuelto(DateTime fechaEntregaUtc, string? observaciones = null)
        {
            if (Estado != EstadoPrestamo.Activo && Estado != EstadoPrestamo.Vencido)
                throw new InvalidOperationException("Solo préstamos activos o vencidos pueden devolverse.");
            Estado = EstadoPrestamo.Devuelto;
            FechaEntregaRealUtc = fechaEntregaUtc;
            Observaciones = observaciones;
            Touch();
        }

        public void Cancelar(string motivo)
        {
            if (Estado is not (EstadoPrestamo.Pendiente or EstadoPrestamo.Activo))
                throw new InvalidOperationException("Solo préstamos pendientes o activos pueden cancelarse.");
            Estado = EstadoPrestamo.Cancelado;
            Observaciones = motivo;
            Touch();
        }

        public void Extender(int dias)
        {
            if (Estado != EstadoPrestamo.Activo)
                throw new InvalidOperationException("Solo préstamos activos pueden extenderse.");
            Periodo = Periodo.ExtenderDias(dias);
            Touch();
        }
    }
}