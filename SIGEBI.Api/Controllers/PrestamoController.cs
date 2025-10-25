using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Api.Dtos;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Repository;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrestamoController : ControllerBase
{
    private const int MaxPrestamosActivosPorUsuario = 3;

    private readonly IPrestamoRepository _prestamoRepository;
    private readonly ILibroRepository _libroRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPenalizacionRepository _penalizacionRepository;

    public PrestamoController(
        IPrestamoRepository prestamoRepository,
        ILibroRepository libroRepository,
        IUsuarioRepository usuarioRepository,
        IPenalizacionRepository penalizacionRepository)
    {
        _prestamoRepository = prestamoRepository;
        _libroRepository = libroRepository;
        _usuarioRepository = usuarioRepository;
        _penalizacionRepository = penalizacionRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrestamoDto>> ObtenerPorId(Guid id, CancellationToken ct)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        return Ok(Map(prestamo));
    }

    [HttpGet("usuario/{usuarioId:guid}")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerPorUsuario(Guid usuarioId, CancellationToken ct)
    {
        var prestamos = await _prestamoRepository.ObtenerPorUsuarioAsync(usuarioId, ct);
        return Ok(prestamos.Select(Map));
    }

    [HttpGet("libro/{libroId:guid}")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerActivosPorLibro(Guid libroId, CancellationToken ct)
    {
        var prestamos = await _prestamoRepository.ObtenerActivosPorLibroAsync(libroId, ct);
        return Ok(prestamos.Select(Map));
    }

    [HttpGet("vencidos")]
    public async Task<ActionResult<IEnumerable<PrestamoDto>>> ObtenerVencidos([FromQuery] DateTime? referenciaUtc, CancellationToken ct)
    {
        var referencia = referenciaUtc ?? DateTime.UtcNow;
        var vencidos = await _prestamoRepository.ObtenerVencidosAsync(referencia, ct);
        return Ok(vencidos.Select(Map));
    }

    [HttpPost]
    public async Task<ActionResult<PrestamoDto>> Crear([FromBody] CrearPrestamoRequest request, CancellationToken ct)
    {
        var libro = await _libroRepository.GetByIdAsync(request.LibroId, ct);
        if (libro is null) return NotFound(new { message = "El libro indicado no existe." });

        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, ct);
        if (usuario is null) return NotFound(new { message = "El usuario indicado no existe." });

        var penalizacionesActivas = await _penalizacionRepository.ObtenerActivasPorUsuarioAsync(usuario.Id, ct);
        var prestamosActivos = await _prestamoRepository.ContarActivosPorUsuarioAsync(usuario.Id, ct);

        usuario.AsegurarPuedeSolicitarPrestamo(penalizacionesActivas.Any(), prestamosActivos, MaxPrestamosActivosPorUsuario);

        var periodo = PeriodoPrestamo.Create(request.FechaInicioUtc, request.FechaFinUtc);
        var prestamo = Prestamo.Solicitar(request.LibroId, request.UsuarioId, periodo);

        libro.MarcarPrestado();
        prestamo.Activar();
        usuario.RegistrarPrestamo(prestamo.Id);

        await _prestamoRepository.CrearAsync(prestamo, libro, usuario, ct);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = prestamo.Id }, Map(prestamo));
    }

    [HttpPost("{id:guid}/activar")]
    public async Task<ActionResult<PrestamoDto>> Activar(Guid id, CancellationToken ct)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        var libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct);
        if (libro is null) return NotFound(new { message = "El libro asociado al préstamo no existe." });

        var usuario = await _usuarioRepository.GetByIdAsync(prestamo.UsuarioId, ct);
        if (usuario is null) return NotFound(new { message = "El usuario asociado al préstamo no existe." });

        libro.MarcarPrestado();
        prestamo.Activar();
        usuario.RegistrarPrestamo(prestamo.Id);

        await _libroRepository.UpdateAsync(libro, ct);
        await _prestamoRepository.UpdateAsync(prestamo, ct);
        await _usuarioRepository.UpdateAsync(usuario, ct);

        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/devolver")]
    public async Task<ActionResult<PrestamoDto>> RegistrarDevolucion(Guid id, [FromBody] RegistrarDevolucionRequest request, CancellationToken ct)
    {
        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        var libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct);
        if (libro is null) return NotFound(new { message = "El libro asociado al préstamo no existe." });

        prestamo.MarcarDevuelto(request.FechaEntregaUtc, request.Observaciones);
        libro.MarcarDevuelto();

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        await _libroRepository.UpdateAsync(libro, ct);

        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/cancelar")]
    public async Task<ActionResult<PrestamoDto>> Cancelar(Guid id, [FromBody] CancelarPrestamoRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new DomainException("Debe indicar un motivo de cancelación.", nameof(request.Motivo));
        }

        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        var estabaActivo = prestamo.Estado == EstadoPrestamo.Activo;
        Libro? libro = null;

        if (estabaActivo)
        {
            libro = await _libroRepository.GetByIdAsync(prestamo.LibroId, ct);
            if (libro is null)
            {
                return NotFound(new { message = "El libro asociado al préstamo no existe." });
            }
        }

        prestamo.Cancelar(request.Motivo.Trim());

        if (estabaActivo)
        {
            libro!.MarcarDevuelto();
        }

        await _prestamoRepository.UpdateAsync(prestamo, ct);

        if (estabaActivo)
        {
            await _libroRepository.UpdateAsync(libro!, ct);
        }

        return Ok(Map(prestamo));
    }

    [HttpPost("{id:guid}/extender")]
    public async Task<ActionResult<PrestamoDto>> Extender(Guid id, [FromBody] ExtenderPrestamoRequest request, CancellationToken ct)
    {
        if (request.Dias <= 0)
        {
            throw new DomainException("Los días de extensión deben ser mayores a cero.", nameof(request.Dias));
        }

        var prestamo = await _prestamoRepository.GetByIdAsync(id, ct);
        if (prestamo is null) return NotFound();

        prestamo.Extender(request.Dias);

        await _prestamoRepository.UpdateAsync(prestamo, ct);
        return Ok(Map(prestamo));
    }

    private static PrestamoDto Map(Prestamo prestamo)
        => new(
            prestamo.Id,
            prestamo.LibroId,
            prestamo.UsuarioId,
            prestamo.Estado,
            prestamo.Periodo.FechaInicioUtc,
            prestamo.Periodo.FechaFinCompromisoUtc,
            prestamo.FechaEntregaRealUtc,
            prestamo.Observaciones,
            prestamo.CreatedAtUtc,
            prestamo.UpdatedAtUtc);
}