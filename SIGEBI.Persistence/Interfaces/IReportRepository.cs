using System.Collections.Generic;
using SIGEBI.Domain.Entities;

public interface IReportRepository
{
    // Obtiene todos los reportes generados por un usuario
    List<Report> GetReportsByUser(int userId);

    // Obtiene reporte por Id
    Report GetReportById(int reportId);

    // Crea un nuevo reporte
    void AddReport(Report report);

    // Actualiza un reporte existente
    void UpdateReport(Report report);

    // Elimina (lógica o física) un reporte
    void DeleteReport(int reportId);

    // Métodos para obtener reportes filtrados, por tipo, fechas, etc.
    List<Report> GetReportsByType(string reportType);
}