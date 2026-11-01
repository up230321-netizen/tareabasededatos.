using Microsoft.AspNetCore.Mvc;
using ProductosApi.Models;
using ProductosApi.Repositories;

namespace ProductosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(IProductoRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Producto>> ObtenerTodos()
    {
        return Ok(repository.ObtenerTodos());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Producto> ObtenerPorId(int id)
    {
        var producto = repository.ObtenerPorId(id);
        return producto is null ? NotFound() : Ok(producto);
    }

    [HttpGet("buscar")]
    public ActionResult<List<Producto>> Buscar([FromQuery] string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return BadRequest("El parámetro 'texto' es obligatorio.");
        }

        return Ok(repository.Buscar(texto.Trim()));
    }

    [HttpPost]
    public ActionResult<Producto> Insertar(Producto producto)
    {
        producto.Id = repository.Insertar(producto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, Producto producto)
    {
        producto.Id = id;
        return repository.Actualizar(producto) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id)
    {
        return repository.Eliminar(id) ? NoContent() : NotFound();
    }
}
