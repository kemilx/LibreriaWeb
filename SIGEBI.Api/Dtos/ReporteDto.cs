public class ReporteDto
{
    public int Id { get; set; }
    public string TipoReporte { get; set; }
    public string Titulo { get; set; }
    public int GeneradoPor { get; set; }
    public DateTime FechaGenerado { get; set; }
    public string RutaArchivo { get; set; }
}