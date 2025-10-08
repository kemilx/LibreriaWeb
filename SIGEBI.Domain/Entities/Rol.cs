using System;
using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Rol : Entity
    {
        private Rol() { }

        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }

        public static Rol Create(string nombre, string? descripcion = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del rol es obligatorio.", nameof(nombre));

            return new Rol
            {
                Nombre = nombre.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim()
            };
        }

        public void ActualizarDescripcion(string? descripcion)
        {
            Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim();
            Touch();
        }

        public override string ToString() => Nombre;
    }
}