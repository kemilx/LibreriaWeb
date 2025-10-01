using SIGEBI.Api.Services;
using SIGEBI.Application.Interfaces;
using Domain;
using SIGEBI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly IPrestamoService _prestamoService;

    public PrestamoController(IPrestamoService prestamoService)
    {
        _prestamoService = prestamoService;
    }

    [HttpGet("usuario/{usuarioId}")]
    public IActionResult GetPrestamosByUser(int usuarioId)
    {
        var prestamos = _prestamoService.GetPrestamosByUser(usuarioId);
        return Ok(prestamos);
    }

    [HttpGet("{id}")]
    public IActionResult GetPrestamoById(int id)
    {
        var prestamo = _prestamoService.GetPrestamoById(id);
        if (prestamo == null) return NotFound();
        return Ok(prestamo);
    }

    [HttpPost]
    public IActionResult AddPrestamo([FromBody] Prestamo prestamo)
    {
        _prestamoService.AddPrestamo(prestamo);
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePrestamo(int id, [FromBody] Prestamo prestamo)
    {
        prestamo.Id = id;
        _prestamoService.UpdatePrestamo(prestamo);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePrestamo(int id)
    {
        _prestamoService.DeletePrestamo(id);
        return Ok();
    }
}