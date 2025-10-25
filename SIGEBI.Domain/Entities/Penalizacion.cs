using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Penalizacion : Entity
    {
        private const int MaxMotivoLength = 500;

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
            if (usuarioId == Guid.Empty)
                throw new DomainException("Debe indicar el usuario asociado a la penalización.", nameof(usuarioId));

            DomainValidation.NonNegative(monto, nameof(monto));
            DomainValidation.EnsureDateOrder(inicioUtc, finUtc, nameof(inicioUtc), nameof(finUtc));

            var motivoLimpio = DomainValidation.Required(motivo, MaxMotivoLength, nameof(motivo));

            return new Penalizacion
            {
                UsuarioId = usuarioId,
                PrestamoId = prestamoId,
                Monto = monto,
                FechaInicioUtc = inicioUtc,
                FechaFinUtc = finUtc,
                Motivo = motivoLimpio,
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
            if (string.IsNullOrWhiteSpace(razon))
                throw new DomainException("La razón es obligatoria para cerrar la penalización.", nameof(razon));

            var razonLimpia = razon.Trim();
            var nuevoMotivo = $"{Motivo} | Cierre anticipado: {razonLimpia}";
            if (nuevoMotivo.Length > MaxMotivoLength)
                throw new DomainException($"La razón indicada excede el máximo de {MaxMotivoLength} caracteres al combinarse con el motivo existente.", nameof(razon));

            Activa = false;
            Motivo = nuevoMotivo;
            Touch();
        }
    }
}
