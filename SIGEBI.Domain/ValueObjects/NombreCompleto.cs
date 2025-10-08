using System;

namespace SIGEBI.Domain.ValueObjects
{
    public sealed class NombreCompleto
    {
        public string Nombres { get; }
        public string Apellidos { get; }

        private NombreCompleto(string nombres, string apellidos)
        {
            Nombres = nombres;
            Apellidos = apellidos;
        }

        public static NombreCompleto Create(string nombres, string apellidos)
        {
            if (string.IsNullOrWhiteSpace(nombres))
                throw new ArgumentException("Nombres requeridos.", nameof(nombres));
            if (string.IsNullOrWhiteSpace(apellidos))
                throw new ArgumentException("Apellidos requeridos.", nameof(apellidos));

            return new NombreCompleto(nombres.Trim(), apellidos.Trim());
        }

        public string Completo => $"{Nombres} {Apellidos}";

        public override string ToString() => Completo;

        public override bool Equals(object? obj)
        {
            if (obj is not NombreCompleto other) return false;
            return Nombres.Equals(other.Nombres, StringComparison.OrdinalIgnoreCase)
                   && Apellidos.Equals(other.Apellidos, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
            => HashCode.Combine(Nombres.ToLowerInvariant(), Apellidos.ToLowerInvariant());
    }
}