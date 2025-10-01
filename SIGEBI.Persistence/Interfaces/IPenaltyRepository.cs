using System.Collections.Generic;
using SIGEBI.Domain.Entities;

public interface IPenaltyRepository
{
    // Obtiene todas las penalizaciones de un usuario
    List<Penalty> GetPenaltiesByUser(int userId);

    // Obtiene penalización por Id
    Penalty GetPenaltyById(int penaltyId);

    // Crea una nueva penalización
    void AddPenalty(Penalty penalty);

    // Actualiza una penalización existente
    void UpdatePenalty(Penalty penalty);

    // Elimina (lógica o física) una penalización
    void DeletePenalty(int penaltyId);


}