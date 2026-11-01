using Microsoft.Data.Sqlite;
using ProductosApi.Models;

namespace ProductosApi.Repositories;

public class ProductoRepository(string connectionString) : IProductoRepository
{
    public void Inicializar()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Productos (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombre TEXT NOT NULL,
                Descripcion TEXT NULL,
                Precio REAL NOT NULL CHECK (Precio > 0),
                Categoria TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    public List<Producto> ObtenerTodos()
    {
        using var connection = AbrirConexion();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Nombre, Descripcion, Precio, Categoria
            FROM Productos
            ORDER BY Id;
            """;

        using var reader = command.ExecuteReader();
        var productos = new List<Producto>();
        while (reader.Read())
        {
            productos.Add(LeerProducto(reader));
        }

        return productos;
    }

    public Producto? ObtenerPorId(int id)
    {
        using var connection = AbrirConexion();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Nombre, Descripcion, Precio, Categoria
            FROM Productos
            WHERE Id = @id;
            """;
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? LeerProducto(reader) : null;
    }

    public List<Producto> Buscar(string texto)
    {
        using var connection = AbrirConexion();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Nombre, Descripcion, Precio, Categoria
            FROM Productos
            WHERE Nombre LIKE @texto COLLATE NOCASE
               OR Descripcion LIKE @texto COLLATE NOCASE
               OR Categoria LIKE @texto COLLATE NOCASE
            ORDER BY Nombre;
            """;
        command.Parameters.AddWithValue("@texto", $"%{texto}%");

        using var reader = command.ExecuteReader();
        var productos = new List<Producto>();
        while (reader.Read())
        {
            productos.Add(LeerProducto(reader));
        }

        return productos;
    }

    public int Insertar(Producto producto)
    {
        using var connection = AbrirConexion();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Productos (Nombre, Descripcion, Precio, Categoria)
            VALUES (@nombre, @descripcion, @precio, @categoria);
            SELECT last_insert_rowid();
            """;
        AgregarParametrosProducto(command, producto);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public bool Actualizar(Producto producto)
    {
        using var connection = AbrirConexion();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Productos
            SET Nombre = @nombre,
                Descripcion = @descripcion,
                Precio = @precio,
                Categoria = @categoria
            WHERE Id = @id;
            """;
        AgregarParametrosProducto(command, producto);
        command.Parameters.AddWithValue("@id", producto.Id);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Eliminar(int id)
    {
        using var connection = AbrirConexion();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Productos WHERE Id = @id;";
        command.Parameters.AddWithValue("@id", id);

        return command.ExecuteNonQuery() > 0;
    }

    private SqliteConnection AbrirConexion()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    private static void AgregarParametrosProducto(SqliteCommand command, Producto producto)
    {
        command.Parameters.AddWithValue("@nombre", producto.Nombre);
        command.Parameters.AddWithValue("@descripcion", (object?)producto.Descripcion ?? DBNull.Value);
        command.Parameters.AddWithValue("@precio", producto.Precio);
        command.Parameters.AddWithValue("@categoria", producto.Categoria);
    }

    private static Producto LeerProducto(SqliteDataReader reader)
    {
        return new Producto
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
            Precio = Convert.ToDecimal(reader.GetValue(3)),
            Categoria = reader.GetString(4)
        };
    }
}
