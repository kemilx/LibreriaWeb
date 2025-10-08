using System;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Entities
{
    public sealed class Libro : AggregateRoot
    {
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
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título es obligatorio.", nameof(titulo));
            if (string.IsNullOrWhiteSpace(autor))
                throw new ArgumentException("El autor es obligatorio.", nameof(autor));
            if (ejemplares <= 0)
                throw new ArgumentException("Debe haber al menos un ejemplar.", nameof(ejemplares));

            return new Libro
            {
                Titulo = titulo.Trim(),
                Autor = autor.Trim(),
                Isbn = string.IsNullOrWhiteSpace(isbn) ? null : isbn.Trim(),
                Ubicacion = string.IsNullOrWhiteSpace(ubicacion) ? null : ubicacion.Trim(),
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
                throw new InvalidOperationException("El libro no está disponible para préstamo.");

            EjemplaresDisponibles--;
            if (EjemplaresDisponibles == 0)
                Estado = EstadoLibro.Prestado;

            Touch();
        }

        public void MarcarDevuelto()
        {
            if (EjemplaresDisponibles >= EjemplaresTotales)
                throw new InvalidOperationException("No hay ejemplares prestados para devolver.");

            EjemplaresDisponibles++;
            if (Estado == EstadoLibro.Prestado)
                Estado = EstadoLibro.Disponible;

            Touch();
        }

        public void MarcarReservado()
        {
            if (Estado != EstadoLibro.Disponible)
                throw new InvalidOperationException("Solo se puede reservar un libro disponible.");
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
            Ubicacion = string.IsNullOrWhiteSpace(nueva) ? null : nueva.Trim();
            Touch();
        }

        public void ActualizarDatos(string? titulo = null, string? autor = null, string? isbn = null, DateTime? fechaPublicacion = null)
        {
            if (!string.IsNullOrWhiteSpace(titulo)) Titulo = titulo.Trim();
            if (!string.IsNullOrWhiteSpace(autor)) Autor = autor.Trim();
            if (!string.IsNullOrWhiteSpace(isbn)) Isbn = isbn.Trim();
            if (fechaPublicacion.HasValue) FechaPublicacion = fechaPublicacion;
            Touch();
        }
    }
}