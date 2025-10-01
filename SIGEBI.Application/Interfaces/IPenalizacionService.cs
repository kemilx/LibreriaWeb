using System.Collections.Generic;
using SIGEBI.Domain.Entities;

public interface IPenalizacionService
{
    List<Penalizacion> GetPenalizacionesByUser(int usuarioId);
    Penalizacion GetPenalizacionById(int penalizacionId);
    void AddPenalizacion(Penalizacion penalizacion);
    void UpdatePenalizacion(Penalizacion penalizacion);
    void DeletePenalizacion(int penalizacionId);
}