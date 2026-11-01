using System.ComponentModel.DataAnnotations;

namespace ProductosApi.Models;

public class Producto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descripcion { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Precio { get; set; }

    [Required]
    [StringLength(60, MinimumLength = 2)]
    public string Categoria { get; set; } = string.Empty;
}
