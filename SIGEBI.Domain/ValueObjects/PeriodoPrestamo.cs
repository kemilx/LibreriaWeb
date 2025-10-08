using System;

namespace SIGEBI.Domain.ValueObjects
{
    public sealed class PeriodoPrestamo
    {
        public DateTime FechaInicioUtc { get; }
        public DateTime FechaFinCompromisoUtc { get; }

        private PeriodoPrestamo(DateTime inicioUtc, DateTime finUtc)
        {
            FechaInicioUtc = inicioUtc;
            FechaFinCompromisoUtc = finUtc;
        }

        public static PeriodoPrestamo Create(DateTime inicioUtc, DateTime finUtc)
        {
            if (finUtc <= inicioUtc)
                throw new ArgumentException("La fecha fin debe ser posterior a la fecha de inicio.");
            return new PeriodoPrestamo(inicioUtc, finUtc);
        }

        public bool EstaVencido(DateTime referenciaUtc) => referenciaUtc > FechaFinCompromisoUtc;

        public PeriodoPrestamo ExtenderDias(int dias)
        {
            if (dias <= 0) throw new ArgumentException("Los días de extensión deben ser positivos.", nameof(dias));
            return new PeriodoPrestamo(FechaInicioUtc, FechaFinCompromisoUtc.AddDays(dias));
        }

        public override bool Equals(object? obj)
            => obj is PeriodoPrestamo other &&
               FechaInicioUtc == other.FechaInicioUtc &&
               FechaFinCompromisoUtc == other.FechaFinCompromisoUtc;

        public override int GetHashCode() => HashCode.Combine(FechaInicioUtc, FechaFinCompromisoUtc);
    }
}