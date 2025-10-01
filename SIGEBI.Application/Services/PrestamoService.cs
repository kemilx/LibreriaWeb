using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Interfaces;
using System.Collections.Generic;

public class PrestamoService : IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;

    public PrestamoService(IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public List<Prestamo> GetPrestamosByUser(int usuarioId) =>
        _prestamoRepository.GetPrestamosByUser(usuarioId);

    public Prestamo GetPrestamoById(int prestamoId) =>
        _prestamoRepository.GetPrestamoById(prestamoId);

    public void AddPrestamo(Prestamo prestamo) =>
        _prestamoRepository.AddPrestamo(prestamo);

    public void UpdatePrestamo(Prestamo prestamo) =>
        _prestamoRepository.UpdatePrestamo(prestamo);

    public void DeletePrestamo(int prestamoId) =>
        _prestamoRepository.DeletePrestamo(prestamoId);
}