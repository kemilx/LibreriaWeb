using System;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Entities
{
    public sealed class Libro : AggregateRoot
    {
        private const int MaxTituloLength = 250;
        private const int MaxAutorLength = 200;
        private const int MaxIsbnLength = 40;
        private const int MaxUbicacionLength = 100;

        private Libro() { }

        public string Titulo { get; private set; } = null!;
        public string Autor { get; private set; } = null!;
        public string? Isbn { get; private set; }
        public string? Ubicacion { get; private set; }
        public int EjemplaresTotales { get; private set; }
        public int EjemplaresDisponibles { get; private set; }
        public EstadoLibro Estado { get; private set; } = EstadoLibro.Disponible;
        public DateTime? FechaPublicacion { get; private set; }

        public static Libro Create(string titulo, string autor, int ejemplares, string? isbn = null, string? ubicacion = null, DateTime? fechaPublicacion = null)
        {
            if (ejemplares <= 0)
            {
                throw new DomainException("Debe haber al menos un ejemplar.", nameof(ejemplares));
            }

            var tituloLimpio = DomainValidation.Required(titulo, MaxTituloLength, "título");
            var autorLimpio = DomainValidation.Required(autor, MaxAutorLength, "autor");
            var isbnLimpio = DomainValidation.Optional(isbn, MaxIsbnLength, "ISBN");
            var ubicacionLimpia = DomainValidation.Optional(ubicacion, MaxUbicacionLength, "ubicación");

            return new Libro
            {
                Titulo = tituloLimpio,
                Autor = autorLimpio,
                Isbn = isbnLimpio,
                Ubicacion = ubicacionLimpia,
                EjemplaresTotales = ejemplares,
                EjemplaresDisponibles = ejemplares,
                FechaPublicacion = fechaPublicacion
            };
        }

        public bool DisponibleParaPrestamo() =>
            Estado == EstadoLibro.Disponible && EjemplaresDisponibles > 0;

        public void MarcarPrestado()
        {
            if (!DisponibleParaPrestamo())
                throw new DomainException("El libro no está disponible para préstamo.");

            EjemplaresDisponibles--;
            if (EjemplaresDisponibles < EjemplaresTotales)
            {
                Estado = EstadoLibro.Prestado;
            }

            Touch();
        }

        public void MarcarDevuelto()
        {
            if (EjemplaresDisponibles >= EjemplaresTotales)
                throw new DomainException("No hay ejemplares prestados para devolver.");

            EjemplaresDisponibles++;

            if (EjemplaresDisponibles == EjemplaresTotales)
            {
                Estado = EstadoLibro.Disponible;
            }

            Touch();
        }

        public void MarcarReservado()
        {
            if (Estado != EstadoLibro.Disponible)
                throw new DomainException("Solo se puede reservar un libro disponible.");
            Estado = EstadoLibro.Reservado;
            Touch();
        }

        public void MarcarDañado()
        {
            Estado = EstadoLibro.Dañado;
            Touch();
        }

        public void MarcarInactivo()
        {
            Estado = EstadoLibro.Inactivo;
            Touch();
        }

        public void ActualizarUbicacion(string? nueva)
        {
            Ubicacion = DomainValidation.Optional(nueva, MaxUbicacionLength, "ubicación");

            Touch();
        }

        public void ActualizarDatos(string? titulo = null, string? autor = null, string? isbn = null, DateTime? fechaPublicacion = null)
        {
            if (!string.IsNullOrWhiteSpace(titulo))
            {
                Titulo = DomainValidation.Required(titulo, MaxTituloLength, "título");
            }

            if (!string.IsNullOrWhiteSpace(autor))
            {
                Autor = DomainValidation.Required(autor, MaxAutorLength, "autor");
            }

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                Isbn = DomainValidation.Required(isbn, MaxIsbnLength, "ISBN");
            }

            if (fechaPublicacion.HasValue)
            {
                FechaPublicacion = fechaPublicacion;
            }

            Touch();
        }
    }
}