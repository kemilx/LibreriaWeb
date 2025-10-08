using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Configuracion : Entity
    {
        private Configuracion() { }

        public string Clave { get; private set; } = null!;
        public string Valor { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public bool Activo { get; private set; }

        public static Configuracion Crear(string clave, string valor, string? descripcion = null, bool activo = true)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Clave requerida.", nameof(clave));
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("Valor requerido.", nameof(valor));

            return new Configuracion
            {
                Clave = clave.Trim(),
                Valor = valor.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                Activo = activo
            };
        }

        public void ActualizarValor(string nuevoValor)
        {
            if (string.IsNullOrWhiteSpace(nuevoValor))
                throw new ArgumentException("Valor requerido.", nameof(nuevoValor));

            Valor = nuevoValor.Trim();
            Touch();
        }

        public void ActualizarDescripcion(string? descripcion)
        {
            Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
            Touch();
        }

        public void CambiarEstado(bool activo)
        {
            if (Activo == activo) return;
            Activo = activo;
            Touch();
        }
    }
}