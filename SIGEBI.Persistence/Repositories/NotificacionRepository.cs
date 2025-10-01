using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.DbContext;
using SIGEBI.Persistence.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SIGEBI.Persistence.Repositories
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly SIGEBIDbContext _context;

        public NotificacionRepository(SIGEBIDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Notificacion> GetAllNotificaciones()
        {
            return _context.Notificaciones.ToList();
        }

        public Notificacion GetNotificacionById(int id)
        {
            return _context.Notificaciones.Find(id);
        }

        public void AddNotificacion(Notificacion notificacion)
        {
            _context.Notificaciones.Add(notificacion);
            _context.SaveChanges();
        }

        public void UpdateNotificacion(Notificacion notificacion)
        {
            _context.Notificaciones.Update(notificacion);
            _context.SaveChanges();
        }

        public void DeleteNotificacion(int id)
        {
            var notificacion = _context.Notificaciones.Find(id);
            if (notificacion != null)
            {
                _context.Notificaciones.Remove(notificacion);
                _context.SaveChanges();
            }
        }
    }
}