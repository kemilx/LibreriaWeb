using SIGEBI.Domain.Base;

namespace SIGEBI.Domain.Entities
{
    public sealed class Configuracion : Entity
    {
        private const int MaxClaveLength = 120;
        private const int MaxValorLength = 500;
        private const int MaxDescripcionLength = 500;

        private Configuracion() { }

        public string Clave { get; private set; } = null!;
        public string Valor { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public bool Activo { get; private set; }

        public static Configuracion Crear(string clave, string valor, string? descripcion = null, bool activo = true)
        {
            var claveLimpia = DomainValidation.Required(clave, MaxClaveLength, nameof(clave));
            var valorLimpio = DomainValidation.Required(valor, MaxValorLength, nameof(valor));
            var descripcionLimpia = DomainValidation.Optional(descripcion, MaxDescripcionLength, nameof(descripcion));

            return new Configuracion
            {
                Clave = claveLimpia,
                Valor = valorLimpio,
                Descripcion = descripcionLimpia,
                Activo = activo
            };
        }

        public void ActualizarValor(string nuevoValor)
        {
            Valor = DomainValidation.Required(nuevoValor, MaxValorLength, nameof(nuevoValor));
            Touch();
        }

        public void ActualizarDescripcion(string? descripcion)
        {
            Descripcion = DomainValidation.Optional(descripcion, MaxDescripcionLength, nameof(descripcion));
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
