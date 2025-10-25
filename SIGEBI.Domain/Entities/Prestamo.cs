using System;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Entities
{
    public sealed class Prestamo : AggregateRoot
    {
        private const int MaxObservacionesLength = 500;

        private Prestamo() { }

        public Guid LibroId { get; private set; }
        public Guid UsuarioId { get; private set; }
        public PeriodoPrestamo Periodo { get; private set; } = null!;
        public EstadoPrestamo Estado { get; private set; } = EstadoPrestamo.Pendiente;
        public DateTime? FechaEntregaRealUtc { get; private set; }
        public string? Observaciones { get; private set; }

        public static Prestamo Solicitar(Guid libroId, Guid usuarioId, PeriodoPrestamo periodo)
        {
            if (libroId == Guid.Empty)
                throw new DomainException("Debe indicar el libro a prestar.", nameof(libroId));
            if (usuarioId == Guid.Empty)
                throw new DomainException("Debe indicar el usuario solicitante.", nameof(usuarioId));

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
                throw new DomainException("Solo un préstamo pendiente puede activarse.", nameof(Estado));
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
                throw new DomainException("Solo préstamos activos o vencidos pueden devolverse.", nameof(Estado));
            Estado = EstadoPrestamo.Devuelto;
            FechaEntregaRealUtc = fechaEntregaUtc;

            if (string.IsNullOrWhiteSpace(observaciones))
            {
                Observaciones = null;
            }
            else
            {
                var observacionesLimpias = observaciones.Trim();
                if (observacionesLimpias.Length > MaxObservacionesLength)
                    throw new DomainException($"Las observaciones no pueden exceder {MaxObservacionesLength} caracteres.", nameof(observaciones));

                Observaciones = observacionesLimpias;
            }
            Touch();
        }

        public void Cancelar(string motivo)
        {
            if (Estado is not (EstadoPrestamo.Pendiente or EstadoPrestamo.Activo))
                throw new DomainException("Solo préstamos pendientes o activos pueden cancelarse.", nameof(Estado));
            if (string.IsNullOrWhiteSpace(motivo))
                throw new DomainException("El motivo de cancelación es obligatorio.", nameof(motivo));

            var motivoLimpio = motivo.Trim();
            if (motivoLimpio.Length > MaxObservacionesLength)
                throw new DomainException($"El motivo de cancelación no puede exceder {MaxObservacionesLength} caracteres.", nameof(motivo));

            Estado = EstadoPrestamo.Cancelado;
            Observaciones = motivoLimpio;
            Touch();
        }

        public void Extender(int dias)
        {
            if (Estado != EstadoPrestamo.Activo)
                throw new DomainException("Solo préstamos activos pueden extenderse.", nameof(Estado));
            Periodo = Periodo.ExtenderDias(dias);
            Touch();
        }
    }
}
