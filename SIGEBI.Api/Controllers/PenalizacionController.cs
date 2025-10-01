using Microsoft.AspNetCore.Mvc;
using SIGEBI.Application.Interfaces;
using SIGEBI.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class PenalizacionController : ControllerBase
{
    private readonly IPenalizacionService _penalizacionService;

    public PenalizacionController(IPenalizacionService penalizacionService)
    {
        _penalizacionService = penalizacionService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public IActionResult GetPenalizacionesByUser(int usuarioId)
    {
        var penalizaciones = _penalizacionService.GetPenalizacionesByUser(usuarioId);
        return Ok(penalizaciones);
    }

    [HttpGet("{id}")]
    public IActionResult GetPenalizacionById(int id)
    {
        var penalizacion = _penalizacionService.GetPenalizacionById(id);
        if (penalizacion == null) return NotFound();
        return Ok(penalizacion);
    }

    [HttpPost]
    public IActionResult AddPenalizacion([FromBody] Penalizacion penalizacion)
    {
        _penalizacionService.AddPenalizacion(penalizacion);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePenalizacion(int id, [FromBody] Penalizacion penalizacion)
    {
        penalizacion.Id = id;
        _penalizacionService.UpdatePenalizacion(penalizacion);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePenalizacion(int id)
    {
        _penalizacionService.DeletePenalizacion(id);
        return Ok();
    }
}