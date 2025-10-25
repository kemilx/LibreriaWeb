using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Notificacion : Entity
    {
        private const int MaxTituloLength = 150;
        private const int MaxMensajeLength = 1000;
        private const int MaxTipoLength = 50;

        private Notificacion() { }

        public Guid UsuarioId { get; private set; }
        public string Titulo { get; private set; } = null!;
        public string Mensaje { get; private set; } = null!;
        public string Tipo { get; private set; } = null!;
        public bool Leida { get; private set; }
        public DateTime? FechaLecturaUtc { get; private set; }

        public DateTime FechaCreacionUtc => CreatedAtUtc;

        public static Notificacion Crear(Guid usuarioId, string titulo, string mensaje, string tipo)
        {
            if (usuarioId == Guid.Empty)
                throw new DomainException("Debe indicar el destinatario de la notificación.", nameof(usuarioId));

            var tituloLimpio = DomainValidation.Required(titulo, MaxTituloLength, nameof(titulo));
            var mensajeLimpio = DomainValidation.Required(mensaje, MaxMensajeLength, nameof(mensaje));
            var tipoLimpio = DomainValidation.Required(tipo, MaxTipoLength, nameof(tipo));

            return new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = tituloLimpio,
                Mensaje = mensajeLimpio,
                Tipo = tipoLimpio,
                Leida = false
            };
        }

        public void MarcarComoLeida()
        {
            if (Leida) return;
            Leida = true;
            FechaLecturaUtc = DateTime.UtcNow;
            Touch();
        }
    }
}
