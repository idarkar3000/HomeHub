# HomeHub

Aplicación full-stack para la gestión integral de estancias y tareas del hogar. Está formada por una API REST en .NET y una aplicación web en Angular.

## Estructura del Repositorio

- `HomeHub/`: API REST en .NET (Minimal APIs + Dapper + PostgreSQL).
- `ui/`: Aplicación web en Angular con Angular Material.

# Backend

API REST desarrollada en .NET con arquitectura basada en Minimal APIs, Dapper y PostgreSQL para la gestión integral de estancias y tareas del hogar.

## Tecnologías

- **Lenguaje:** C# (.NET)
- **Framework:** ASP.NET Core (Minimal APIs)
- **Acceso a datos:** Dapper (Micro-ORM)
- **Base de datos:** PostgreSQL
- **Documentación:** Swagger / OpenAPI

## Estructura del Proyecto

- `Endpoints/`: Definición de las rutas HTTP agrupadas por dominios (`RoomEndpoints`, `TaskEndpoints`, `NoteEndpoints`).
- `Repositories/`: Capa de persistencia e implementación de consultas SQL con Dapper (`PostgresRoomRepository`, `PostgresTaskRepository`, `PostgresNoteRepository`).
- `Models/`: Entidades de dominio (`Room`, `TaskItem`, `Note`) y utilidades (`Recurrence`).
- `Models/Dtos/`: Objetos de entrada para las peticiones (`RoomRequest`, `TaskRequest`, `NoteRequest`).

## Endpoints Principales

### Habitaciones (`/api/rooms`)
- `GET /api/rooms`: Obtiene el listado completo de habitaciones.
- `GET /api/rooms/{id}`: Obtiene una habitación por su identificador.
- `POST /api/rooms`: Crea una nueva habitación.
- `PUT /api/rooms/{id}`: Actualiza una habitación existente.
- `DELETE /api/rooms/{id}`: Elimina una habitación y sus tareas asociadas en cascada.

### Tareas (`/api/tasks`)
- `GET /api/tasks`: Obtiene todas las tareas.
- `GET /api/tasks/{id}`: Obtiene una tarea por su identificador.
- `GET /api/tasks/calendar`: Obtiene las tareas ordenadas por fecha de vencimiento.
- `GET /api/rooms/{roomId}/tasks`: Obtiene las tareas filtradas por habitación.
- `POST /api/rooms/{roomId}/tasks`: Crea una nueva tarea dentro de una habitación.
- `PUT /api/tasks/{id}`: Actualiza una tarea existente.
- `PATCH /api/tasks/{id}/toggle`: Cambia el estado de completado de una tarea. Si la tarea es recurrente, avanza su fecha a la siguiente repetición.
- `DELETE /api/tasks/{id}`: Elimina una tarea por su identificador.

Las tareas admiten una recurrencia (`none`, `daily`, `weekly`, `monthly`) y una fecha de vencimiento opcional. La fecha puede ser de **día completo** (solo `dueDate`) o cubrir una **franja horaria** (con `endDate`).

### Notas y lista de la compra (`/api/notes`)
- `GET /api/notes`: Obtiene las notas (permite filtrar por tipo con `?kind=note|shopping`).
- `GET /api/notes/{id}`: Obtiene una nota por su identificador.
- `POST /api/notes`: Crea una nota o elemento de la compra.
- `PUT /api/notes/{id}`: Actualiza una nota existente.
- `PATCH /api/notes/{id}/toggle`: Marca o desmarca una nota como hecha.
- `DELETE /api/notes/{id}`: Elimina una nota por su identificador.

## Configuración y Ejecución

1. Configurar la cadena de conexión en el archivo `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=homehub;Username=tu_usuario;Password=tu_contraseña"
   }
   ```

2. Ejecutar el proyecto:
   ```bash
   dotnet run --project HomeHub
   ```

3. Acceder a la documentación Swagger en `http://localhost:5092/swagger`.

# Frontend

Aplicación SPA con enfoque mobile-first construida con Angular y Angular Material. Se organiza en tres secciones: **Hogar** (habitaciones y sus tareas), **Calendario** (tareas programadas; arrastra y suelta para asignar un día completo o una franja horaria) y **Notas / Lista de la compra**. En móvil usa una **barra de navegación inferior** al estilo de una app Android, y barra lateral en escritorio.

## Tecnologías

- **Framework:** Angular 22 (standalone components, señales y zoneless).
- **UI:** Angular Material.
- **Calendario:** FullCalendar.
- **Cliente HTTP:** `provideHttpClient` con servicios basados en `Observable` y estado con señales.

## Estructura del Proyecto

- `src/app/core/models/`: Modelos de datos (`Room`, `TaskItem`, `Note`) y de peticiones.
- `src/app/core/services/`: Servicios de acceso a la API (`RoomService`, `TaskService`, `NoteService`).
- `src/app/features/rooms/`: Sección Hogar (listado de habitaciones, detalle y formularios).
- `src/app/features/calendar/`: Calendario interactivo con arrastrar y soltar.
- `src/app/features/notes/`: Notas y lista de la compra.
- `src/app/features/tasks/`: Formulario de creación y edición de tareas.
- `src/app/shared/`: Componentes reutilizables (`EmptyState`, `ConfirmDialog`).
- `src/environments/`: Configuración por entorno (URL de la API).

## Configuración y Ejecución

1. Arrancar la API (ver sección Backend) en `http://localhost:5092`.
2. Instalar las dependencias:
   ```bash
   cd ui
   npm install
   ```
3. Arrancar el servidor de desarrollo:
   ```bash
   npm start
   ```
4. Abrir `http://localhost:4200` en el navegador.

> La URL de la API se configura en `ui/src/environments/environment.ts`.

## Compilación

```bash
cd ui
npm run build
```

Los artefactos se generan en `ui/dist/`.
