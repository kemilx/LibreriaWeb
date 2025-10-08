using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Penalizacion : Entity
    {
        private Penalizacion() { }

        public Guid UsuarioId { get; private set; }
        public Guid? PrestamoId { get; private set; }
        public decimal Monto { get; private set; }
        public DateTime FechaInicioUtc { get; private set; }
        public DateTime FechaFinUtc { get; private set; }
        public string Motivo { get; private set; } = null!;
        public bool Activa { get; private set; }

        public static Penalizacion Generar(Guid usuarioId, Guid? prestamoId, decimal monto, DateTime inicioUtc, DateTime finUtc, string motivo)
        {
            if (monto < 0) throw new ArgumentException("El monto no puede ser negativo.", nameof(monto));
            if (finUtc <= inicioUtc) throw new ArgumentException("La fecha fin debe ser posterior a la fecha inicio.", nameof(finUtc));
            if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("Motivo requerido.", nameof(motivo));

            return new Penalizacion
            {
                UsuarioId = usuarioId,
                PrestamoId = prestamoId,
                Monto = monto,
                FechaInicioUtc = inicioUtc,
                FechaFinUtc = finUtc,
                Motivo = motivo.Trim(),
                Activa = true
            };
        }

        public void VerificarEstado(DateTime ahoraUtc)
        {
            if (Activa && ahoraUtc >= FechaFinUtc)
            {
                Activa = false;
                Touch();
            }
        }

        public void CerrarAnticipadamente(string razon)
        {
            if (!Activa) return;
            Activa = false;
            Motivo = $"{Motivo} | Cierre anticipado: {razon}";
            Touch();
        }
    }
}