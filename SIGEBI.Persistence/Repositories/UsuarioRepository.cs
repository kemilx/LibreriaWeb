using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;

namespace SIGEBI.Persistence.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SIGEBIDbContext _context;

        public UsuarioRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _context.Usuarios
                             .Include("_roles")
                             .FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _context.Usuarios
                             .FirstOrDefaultAsync(u => u.Email.Value == email, ct);

        public async Task<bool> EmailExisteAsync(string email, CancellationToken ct = default)
            => await _context.Usuarios.AnyAsync(u => u.Email.Value == email, ct);

        public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
        {
            await _context.Usuarios.AddAsync(usuario, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<int> ContarActivosAsync(CancellationToken ct = default)
            => await _context.Usuarios.CountAsync(u => u.Activo, ct);
    }
}