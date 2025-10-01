using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Interfaces;
using System.Collections.Generic;

public class PenalizacionService : IPenalizacionService
{
    private readonly IPenalizacionRepository _penalizacionRepository;

    public PenalizacionService(IPenalizacionRepository penalizacionRepository)
    {
        _penalizacionRepository = penalizacionRepository;
    }

    public List<Penalizacion> GetPenalizacionesByUser(int usuarioId) =>
        _penalizacionRepository.GetPenalizacionesByUser(usuarioId);

    public Penalizacion GetPenalizacionById(int penalizacionId) =>
        _penalizacionRepository.GetPenalizacionById(penalizacionId);

    public void AddPenalizacion(Penalizacion penalizacion) =>
        _penalizacionRepository.AddPenalizacion(penalizacion);

    public void UpdatePenalizacion(Penalizacion penalizacion) =>
        _penalizacionRepository.UpdatePenalizacion(penalizacion);

    public void DeletePenalizacion(int penalizacionId) =>
        _penalizacionRepository.DeletePenalizacion(penalizacionId);
}