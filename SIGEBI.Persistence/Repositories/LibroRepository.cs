using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Persistence.Repositories
{
    public class LibroRepository : ILibroRepository
    {
        private readonly SIGEBIDbContext _context;

        public LibroRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task<Libro?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Libros.FirstOrDefaultAsync(l => l.Id == id, ct);

        public async Task AddAsync(Libro libro, CancellationToken ct = default)
        {
            await _context.Libros.AddAsync(libro, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Libro libro, CancellationToken ct = default)
        {
            if (_context.Entry(libro).State == EntityState.Detached)
            {
                _context.Libros.Attach(libro);
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Libro>> BuscarPorTituloAsync(string texto, CancellationToken ct = default)
            => await _context.Libros
                             .AsNoTracking()
                             .Where(l => EF.Functions.Like(l.Titulo, $"%{texto}%"))
                             .OrderBy(l => l.Titulo)
                             .ToListAsync(ct);

        public async Task<IReadOnlyList<Libro>> BuscarPorAutorAsync(string autor, CancellationToken ct = default)
            => await _context.Libros
                             .AsNoTracking()
                             .Where(l => EF.Functions.Like(l.Autor, $"%{autor}%"))
                             .OrderBy(l => l.Autor)
                             .ToListAsync(ct);

        public async Task<int> ContarDisponiblesAsync(CancellationToken ct = default)
            => await _context.Libros.CountAsync(l => l.Estado == EstadoLibro.Disponible && l.EjemplaresDisponibles > 0, ct);

        public async Task<int> ContarPorEstadoAsync(EstadoLibro estado, CancellationToken ct = default)
            => await _context.Libros.CountAsync(l => l.Estado == estado, ct);
    }
}