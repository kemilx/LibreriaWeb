using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

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
                             .Include("_roles") // si tienes la navegación fuerte, usa .Include(u => u.Roles)
                             .FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            // Normaliza/valida con tu VO (si es inválido, lanzará ArgumentException)
            var normalizedEmail = EmailAddress.Create(email).Value;

            return await _context.Usuarios
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(u => u.Email.Value == normalizedEmail, ct);
        }

        public async Task<bool> EmailExisteAsync(string email, CancellationToken ct = default)
        {
            var normalizedEmail = EmailAddress.Create(email).Value;

            return await _context.Usuarios
                                 .AsNoTracking()
                                 .AnyAsync(u => u.Email.Value == normalizedEmail, ct);
        }

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
