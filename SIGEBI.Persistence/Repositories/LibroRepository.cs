using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.DbContext;
using SIGEBI.Persistence.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SIGEBI.Persistence.Repositories
{
    public class LibroRepository : ILibroRepository
    {
        private readonly SIGEBIDbContext _context;

        public LibroRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Libro> GetAllLibros()
        {
            return _context.Libros.ToList();
        }

        public Libro GetLibroById(int id)
        {
            return _context.Libros.Find(id);
        }

        public void AddLibro(Libro libro)
        {
            _context.Libros.Add(libro);
            _context.SaveChanges();
        }

        public void UpdateLibro(Libro libro)
        {
            _context.Libros.Update(libro);
            _context.SaveChanges();
        }

        public void DeleteLibro(int id)
        {
            var libro = _context.Libros.Find(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                _context.SaveChanges();
            }
        }
    }
}