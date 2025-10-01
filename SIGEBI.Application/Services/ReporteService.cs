using System.Collections.Generic;
using SIGEBI.Domain.Entities;
using SIGEBI.Persistence.Interfaces;
{
public class ReporteService : IReporteService
{
    private readonly IReportRepository _reportRepository;

    public ReporteService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public List<Reporte> GetReportesByUser(int usuarioId) =>
        _reportRepository.GetReportesByUser(usuarioId);

    public Reporte GetReporteById(int reporteId) =>
        _reportRepository.GetReporteById(reporteId);

    public void AddReporte(Reporte reporte) =>
        _reportRepository.AddReporte(reporte);

    public void UpdateReporte(Reporte reporte) =>
        _reportRepository.UpdateReporte(reporte);

    public void DeleteReporte(int reporteId) =>
        _reportRepository.DeleteReporte(reporteId);

    public List<Reporte> GetReportesByType(string tipoReporte) =>
        _reportRepository.GetReportesByType(tipoReporte);
}