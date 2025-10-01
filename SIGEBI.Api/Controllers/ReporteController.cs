using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class ReporteController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReporteController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public IActionResult GetReportesByUser(int usuarioId)
    {
        var reportes = _reporteService.GetReportesByUser(usuarioId);
        return Ok(reportes);
    }

    [HttpGet("{id}")]
    public IActionResult GetReporteById(int id)
    {
        var reporte = _reporteService.GetReporteById(id);
        if (reporte == null) return NotFound();
        return Ok(reporte);
    }

    [HttpPost]
    public IActionResult AddReporte([FromBody] Reporte reporte)
    {
        _reporteService.AddReporte(reporte);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateReporte(int id, [FromBody] Reporte reporte)
    {
        reporte.Id = id;
        _reporteService.UpdateReporte(reporte);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteReporte(int id)
    {
        _reporteService.DeleteReporte(id);
        return Ok();
    }

    [HttpGet("tipo/{tipoReporte}")]
    public IActionResult GetReportesByType(string tipoReporte)
    {
        var reportes = _reporteService.GetReportesByType(tipoReporte);
        return Ok(reportes);
    }
}