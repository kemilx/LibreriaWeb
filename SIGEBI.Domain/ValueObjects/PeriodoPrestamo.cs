using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.ValueObjects
{
    public sealed class PeriodoPrestamo
    {
        public DateTime FechaInicioUtc { get; private set; }
        public DateTime FechaFinCompromisoUtc { get; private set; }

        private PeriodoPrestamo()
        {
        }

        private PeriodoPrestamo(DateTime fechaInicioUtc, DateTime fechaFinCompromisoUtc)
        {
            FechaInicioUtc = fechaInicioUtc;
            FechaFinCompromisoUtc = fechaFinCompromisoUtc;
        }

        public static PeriodoPrestamo Create(DateTime fechaInicioUtc, DateTime fechaFinCompromisoUtc)
        {
            DomainValidation.EnsureDateOrder(fechaInicioUtc, fechaFinCompromisoUtc, nameof(fechaInicioUtc), nameof(fechaFinCompromisoUtc));
            return new PeriodoPrestamo(fechaInicioUtc, fechaFinCompromisoUtc);
        }

        public bool EstaVencido(DateTime referenciaUtc) => referenciaUtc > FechaFinCompromisoUtc;

        public PeriodoPrestamo ExtenderDias(int dias)
        {
            DomainValidation.Positive(dias, nameof(dias));
            return new PeriodoPrestamo(FechaInicioUtc, FechaFinCompromisoUtc.AddDays(dias));
        }

        public override bool Equals(object? obj)
            => obj is PeriodoPrestamo other &&
               FechaInicioUtc == other.FechaInicioUtc &&
               FechaFinCompromisoUtc == other.FechaFinCompromisoUtc;

        public override int GetHashCode() => HashCode.Combine(FechaInicioUtc, FechaFinCompromisoUtc);
    }
}
