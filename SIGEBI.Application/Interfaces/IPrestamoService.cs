using System.Collections.Generic;
using SIGEBI.Domain.Entities;

public interface IPrestamoService
{
    List<Prestamo> GetPrestamosByUser(int usuarioId);
    Prestamo GetPrestamoById(int prestamoId);
    void AddPrestamo(Prestamo prestamo);
    void UpdatePrestamo(Prestamo prestamo);
    void DeletePrestamo(int prestamoId);
}