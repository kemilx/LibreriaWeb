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
            EjemplaresDisponibles > 0 && Estado == EstadoLibro.Disponible;

        public void MarcarPrestado()
        {
            if (EjemplaresDisponibles <= 0)
                throw new DomainException("No hay ejemplares disponibles para préstamo.", nameof(EjemplaresDisponibles));

            if (Estado is EstadoLibro.Dañado or EstadoLibro.Inactivo)
                throw new DomainException("El libro no puede prestarse en su estado actual.", nameof(Estado));

            if (Estado == EstadoLibro.Reservado)
                throw new DomainException("El libro está reservado y no puede prestarse a otro usuario.", nameof(Estado));

            EjemplaresDisponibles--;

            AjustarEstadoSegunDisponibilidad();
            Touch();
        }

        public void MarcarDevuelto()
        {
            if (EjemplaresDisponibles >= EjemplaresTotales)
                throw new DomainException("No hay ejemplares prestados para devolver.");

            EjemplaresDisponibles++;

            AjustarEstadoSegunDisponibilidad();
            Touch();
        }

        public void MarcarReservado()
        {
            if (Estado == EstadoLibro.Reservado)
                return;

            if (Estado is EstadoLibro.Dañado or EstadoLibro.Inactivo)
                throw new DomainException("El libro no puede reservarse en su estado actual.", nameof(Estado));

            if (EjemplaresDisponibles <= 0)
                throw new DomainException("No hay ejemplares disponibles para reservar.", nameof(EjemplaresDisponibles));

            if (Estado != EstadoLibro.Disponible)
                throw new DomainException("Solo se puede reservar un libro disponible.", nameof(Estado));

            Estado = EstadoLibro.Reservado;
            Touch();
        }

        public void MarcarDañado()
        {
            if (EjemplaresDisponibles != EjemplaresTotales)
                throw new DomainException("No es posible marcar el libro como dañado mientras existan préstamos activos.");

            Estado = EstadoLibro.Dañado;
            Touch();
        }

        public void MarcarInactivo()
        {
            if (EjemplaresDisponibles != EjemplaresTotales)
                throw new DomainException("No es posible marcar el libro como inactivo mientras existan préstamos activos.");

            Estado = EstadoLibro.Inactivo;
            Touch();
        }

        public void RestaurarDisponibilidad()
        {
            if (EjemplaresDisponibles <= 0)
                throw new DomainException("No hay ejemplares disponibles para marcar el libro como disponible.", nameof(EjemplaresDisponibles));

            Estado = EstadoLibro.Disponible;
            Touch();
        }

        public void LiberarReserva()
        {
            if (Estado != EstadoLibro.Reservado)
                throw new DomainException("El libro no está reservado.", nameof(Estado));

            if (EjemplaresDisponibles <= 0)
                throw new DomainException("No hay ejemplares disponibles para liberar la reserva.", nameof(EjemplaresDisponibles));

            Estado = EstadoLibro.Disponible;
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

        private void AjustarEstadoSegunDisponibilidad()
        {
            if (Estado is EstadoLibro.Dañado or EstadoLibro.Inactivo)
            {
                return;
            }

            if (EjemplaresDisponibles == 0)
            {
                Estado = EstadoLibro.Prestado;
            }
            else if (Estado != EstadoLibro.Reservado)
            {
                Estado = EstadoLibro.Disponible;
            }
        }
    }
}
