# tareabasededatos.

## API de Productos

Esta API REST la desarrollé en ASP.NET Core para administrar un catálogo de productos conectándome a **SQLite**. Elegí SQLite porque guarda la base de datos directo en un archivo local (`productos.db`), así que no necesitas instalar ni configurar un servidor de base de datos pesado para probar la actividad.

---

## Requisitos y ejecución

- tener instalado **.NET SDK 10** (o superior).

Para ejecutar el proyecto, entra a la carpeta `ProductosApi` desde la terminal y corre el siguiente comando:

```powershell
dotnet run

```

Al arrancar, la aplicación lee la cadena de conexión (`ConnectionStrings:Productos`) desde el archivo `appsettings.json`. El repositorio se encarga automáticamente de abrir la conexión y crear la tabla `Productos` si aún no existe dentro del archivo `productos.db`.

Una vez iniciada la aplicación, la interfaz gráfica de **Swagger UI** estará disponible en:
`http://localhost:5289/swagger`

Desde ahí puedes probar todas las operaciones directamente sin necesidad de usar clientes externos como Postman.

---

## Flujo general del proyecto

1. **Recepción y validación:** La petición llega al `ProductosController`, el cual recibe el método HTTP y los datos desde la ruta o el cuerpo JSON. ASP.NET valida que los campos de la entidad `Producto` cumplan con las reglas definidas.
2. **Abstracción mediante interfaz:** El controlador llama a los métodos de la interfaz `IProductoRepository`. Esto desacopla la API de la capa de datos, por lo que el controlador no maneja lógica directa de SQL.
3. **Acceso a datos:** La clase `ProductoRepository` abre la conexión con SQLite en cada operación, ejecuta las consultas usando parámetros de forma segura y transforma la información devuelta por la base de datos en objetos C# (o indica si una fila existía o fue modificada).

---

## Tabla de métodos y rutas HTTP

| Operación | Método en el repositorio | Ruta HTTP | Respuesta |
| --- | --- | --- | --- |
| Consultar todos | `ObtenerTodos()` | `GET /api/productos` | Lista completa de productos |
| Buscar por ID | `ObtenerPorId(id)` | `GET /api/productos/{id}` | Objeto producto o estado `404` |
| Buscar por coincidencia | `Buscar(texto)` | `GET /api/productos/buscar?texto=...` | Lista de coincidencias |
| Insertar nuevo | `Insertar(producto)` | `POST /api/productos` | Estado `201` con el objeto creado y su ID |
| Actualizar existente | `Actualizar(producto)` | `PUT /api/productos/{id}` | Estado `204` (éxito) o `404` (no encontrado) |
| Eliminar registro | `Eliminar(id)` | `DELETE /api/productos/{id}` | Estado `204` (éxito) o `404` (no encontrado) |

---

## Formato del cuerpo JSON

Ejemplo de estructura para registrar (`POST`) o actualizar (`PUT`):

```json
{
  "nombre": "Cuaderno",
  "descripcion": "Cuaderno de 100 hojas",
  "precio": 45.5,
  "categoria": "Papelería"
}

```

### Reglas de validación aplicadas:

* **Nombre:** Entre 2 y 100 caracteres.
* **Categoría:** Entre 2 y 60 caracteres.
* **Precio:** Debe ser un valor estrictamente mayor a 0.
* **Descripción:** Opcional, con una longitud máxima de 500 caracteres.
