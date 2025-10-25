using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Application.Services;

public sealed class LibroService : ILibroService
{
    private readonly ILibroRepository _libroRepository;

    public LibroService(ILibroRepository libroRepository)
    {
        _libroRepository = libroRepository;
    }

    public Task<Libro?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default)
        => _libroRepository.GetByIdAsync(id, ct);

    public async Task<IReadOnlyList<Libro>> BuscarAsync(string? titulo, string? autor, CancellationToken ct = default)
    {
        var tituloLimpio = titulo?.Trim();
        var autorLimpio = autor?.Trim();

        if (string.IsNullOrWhiteSpace(tituloLimpio) && string.IsNullOrWhiteSpace(autorLimpio))
        {
            throw new DomainException("Debe indicar un texto de búsqueda por título o autor.");
        }

        if (!string.IsNullOrWhiteSpace(tituloLimpio))
        {
            return await _libroRepository.BuscarPorTituloAsync(tituloLimpio, ct);
        }

        return await _libroRepository.BuscarPorAutorAsync(autorLimpio!, ct);
    }

    public async Task<Libro> CrearAsync(string titulo, string autor, int ejemplaresTotales, string? isbn, string? ubicacion, DateTime? fechaPublicacionUtc, CancellationToken ct = default)
    {
        var libro = Libro.Create(titulo, autor, ejemplaresTotales, isbn, ubicacion, fechaPublicacionUtc);
        await _libroRepository.AddAsync(libro, ct);
        return libro;
    }

    public async Task<Libro?> ActualizarAsync(Guid id, string? titulo, string? autor, string? isbn, DateTime? fechaPublicacionUtc, CancellationToken ct = default)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct);
        if (libro is null)
        {
            return null;
        }

        libro.ActualizarDatos(titulo, autor, isbn, fechaPublicacionUtc);
        await _libroRepository.UpdateAsync(libro, ct);
        return libro;
    }

    public async Task<Libro?> ActualizarUbicacionAsync(Guid id, string? ubicacion, CancellationToken ct = default)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct);
        if (libro is null)
        {
            return null;
        }

        libro.ActualizarUbicacion(ubicacion);
        await _libroRepository.UpdateAsync(libro, ct);
        return libro;
    }

    public async Task<Libro?> CambiarEstadoAsync(Guid id, EstadoLibro estado, CancellationToken ct = default)
    {
        var libro = await _libroRepository.GetByIdAsync(id, ct);
        if (libro is null)
        {
            return null;
        }

        switch (estado)
        {
            case EstadoLibro.Reservado:
                libro.MarcarReservado();
                break;
            case EstadoLibro.Dañado:
                libro.MarcarDañado();
                break;
            case EstadoLibro.Inactivo:
                libro.MarcarInactivo();
                break;
            case EstadoLibro.Prestado:
                libro.MarcarPrestado();
                break;
            case EstadoLibro.Disponible:
                if (libro.EjemplaresDisponibles < libro.EjemplaresTotales)
                {
                    libro.MarcarDevuelto();
                }
                else if (libro.Estado != EstadoLibro.Disponible)
                {
                    throw new DomainException("No es posible marcar como disponible sin ejemplares prestados.");
                }
                break;
            default:
                throw new DomainException("Estado no soportado para el libro.");
        }

        await _libroRepository.UpdateAsync(libro, ct);
        return libro;
    }
}
