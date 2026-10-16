# HomeHub UI

Aplicación web (mobile-first) para gestionar el hogar. Construida con Angular 22 (standalone components, señales y zoneless), Angular Material y FullCalendar. Incluye tres secciones: **Hogar** (habitaciones y tareas), **Calendario** (arrastra para programar en un día o en una franja horaria) y **Notas / Lista de la compra**. En móvil usa barra de navegación inferior estilo Android; en escritorio, barra lateral.

## Requisitos

- Node.js y npm.
- La API de HomeHub en ejecución (`dotnet run --project ../HomeHub`, `http://localhost:5092`).

## Servidor de desarrollo

```bash
npm install
npm start
```

Abre `http://localhost:4200`. La aplicación se recarga automáticamente al modificar los archivos fuente.

## Compilación

```bash
npm run build
```

Los artefactos se generan en `dist/`.

## Estructura

- `src/app/core/models/`: Modelos de datos (`Room`, `TaskItem`, `Note`) y de peticiones.
- `src/app/core/services/`: Servicios de acceso a la API (`RoomService`, `TaskService`, `NoteService`).
- `src/app/features/rooms/`: Sección Hogar (listado de habitaciones, detalle y formularios).
- `src/app/features/calendar/`: Calendario interactivo con arrastrar y soltar.
- `src/app/features/notes/`: Notas y lista de la compra.
- `src/app/features/tasks/`: Formulario de creación y edición de tareas.
- `src/app/shared/`: Componentes reutilizables (`EmptyState`, `ConfirmDialog`).
- `src/environments/`: Configuración por entorno (URL de la API).
