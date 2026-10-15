# HomeHub Backend

API REST desarrollada en .NET con arquitectura basada en Minimal APIs, Dapper y PostgreSQL para la gestión integral de estancias y tareas del hogar.

## Tecnologías

- **Lenguaje:** C# (.NET)
- **Framework:** ASP.NET Core (Minimal APIs)
- **Acceso a datos:** Dapper (Micro-ORM)
- **Base de datos:** PostgreSQL
- **Documentación:** Swagger / OpenAPI

## Estructura del Proyecto

- `Endpoints/`: Definición de las rutas HTTP agrupadas por dominios (`RoomEndpoints`, `TaskEndpoints`).
- `Repositories/`: Capa de persistencia e implementación de consultas SQL con Dapper (`PostgresRoomRepository`, `PostgresTaskRepository`).
- `Models/`: Entidades de dominio (`Room`, `TaskItem`).

## Endpoints Principales

### Habitaciones (`/api/rooms`)
- `GET /api/rooms`: Obtiene el listado completo de habitaciones.
- `POST /api/rooms`: Crea una nueva habitación.
- `DELETE /api/rooms/{id}`: Elimina una habitación y sus tareas asociadas en cascada.

### Tareas (`/api/tasks`)
- `GET /api/tasks`: Obtiene todas las tareas.
- `GET /api/tasks/room/{roomId}`: Obtiene las tareas filtradas por ID de habitación.
- `GET /api/tasks/calendar`: Obtiene las tareas ordenadas por fecha de vencimiento.
- `POST /api/tasks`: Crea una nueva tarea.
- `PUT /api/tasks`: Actualiza una tarea existente.
- `DELETE /api/tasks/{id}`: Elimina una tarea por su identificador.

## Configuración y Ejecución

1. Configurar la cadena de conexión en el archivo `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=homehub;Username=tu_usuario;Password=tu_contraseña"
   }
