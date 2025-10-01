using System.Collections.Generic;
using SIGEBI.Domain.Entities;

public interface IReporteService
{
    List<Reporte> GetReportesByUser(int usuarioId);
    Reporte GetReporteById(int reporteId);
    void AddReporte(Reporte reporte);
    void UpdateReporte(Reporte reporte);
    void DeleteReporte(int reporteId);
    List<Reporte> GetReportesByType(string tipoReporte);
}