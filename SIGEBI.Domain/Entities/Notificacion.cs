using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Notificacion : Entity
    {
        private Notificacion() { }

        public Guid UsuarioId { get; private set; }
        public string Titulo { get; private set; } = null!;
        public string Mensaje { get; private set; } = null!;
        public string Tipo { get; private set; } = null!;
        public bool Leida { get; private set; }
        public DateTime FechaLecturaUtc { get; private set; }

        public DateTime FechaCreacionUtc => CreatedAtUtc;

        public static Notificacion Crear(Guid usuarioId, string titulo, string mensaje, string tipo)
        {
            if (string.IsNullOrWhiteSpace(titulo)) throw new ArgumentException("Título requerido.", nameof(titulo));
            if (string.IsNullOrWhiteSpace(mensaje)) throw new ArgumentException("Mensaje requerido.", nameof(mensaje));
            if (string.IsNullOrWhiteSpace(tipo)) throw new ArgumentException("Tipo requerido.", nameof(tipo));

            return new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = titulo.Trim(),
                Mensaje = mensaje.Trim(),
                Tipo = tipo.Trim(),
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