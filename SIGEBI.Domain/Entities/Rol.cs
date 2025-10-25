using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Rol : Entity
    {
        private const int MaxNombreLength = 80;
        private const int MaxDescripcionLength = 250;

        private Rol() { }

        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }

        public static Rol Create(string nombre, string? descripcion = null)
        {
            var nombreLimpio = DomainValidation.Required(nombre, MaxNombreLength, nameof(nombre));

            string? descripcionLimpia = null;
            if (!string.IsNullOrWhiteSpace(descripcion))
            {
                descripcionLimpia = DomainValidation.Optional(descripcion, MaxDescripcionLength, nameof(descripcion));
            }

            return new Rol
            {
                Nombre = nombreLimpio,
                Descripcion = descripcionLimpia
            };
        }

        public void ActualizarDescripcion(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                Descripcion = null;
            }
            else
            {
                Descripcion = DomainValidation.Optional(descripcion, MaxDescripcionLength, nameof(descripcion));
            }
            Touch();
        }

        public override string ToString() => Nombre;
    }
}
