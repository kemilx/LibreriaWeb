public class PenalizacionDto
{
    public int Id { get; set; }
    public int PrestamoId { get; set; }
    public decimal Monto { get; set; }
    public string Motivo { get; set; }
    public bool Pagado { get; set; }
    public DateTime FechaCreacion { get; set; }
}