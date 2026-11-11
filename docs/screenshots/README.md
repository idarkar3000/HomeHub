# Capturas del README

Imágenes usadas por el `README.md` principal. Todas están en esta carpeta en formato PNG,
con nombres en minúsculas, sin acentos ni espacios.

| Archivo | Pantalla | Resolución | Peso |
|---|---|---|---|
| `hogar.png` | Hogar (`/rooms`) | 2557×1437 | ~316 KB |
| `calendario.png` | Calendario (`/calendar`) | 2557×1437 | ~129 KB |
| `arrastrar-calendario.png` | Calendario con arrastrar y soltar | 2047×1151 | ~135 KB |
| `compra.png` | Notas → Compra (`/notes`) | 2557×1437 | ~104 KB |
| `notas.png` | Notas (`/notes`) | 2557×1437 | ~78 KB |
| `modo-oscuro.png` | Hogar en modo oscuro | 2557×1437 | ~276 KB |
| `login.png` | Inicio de sesión (`/login`) | 2557×1377 | ~820 KB |

## Cómo se han capturado

- Navegador al **100 % de zoom** y sin barra de marcadores.
- `F11` para ocultar la interfaz del navegador y capturar solo la app.
- `Win + Shift + S` para el recorte rectangular.
- Sin correos, nombres ni datos personales.
- `arrastrar-calendario.png` muestra una tarea de la bandeja **Sin fecha** siendo
  arrastrada a un día del calendario.

## Checklist antes del commit

```powershell
Get-ChildItem docs/screenshots

Test-Path docs/screenshots/hogar.png
Test-Path docs/screenshots/calendario.png
Test-Path docs/screenshots/arrastrar-calendario.png
Test-Path docs/screenshots/compra.png
Test-Path docs/screenshots/notas.png
Test-Path docs/screenshots/modo-oscuro.png
Test-Path docs/screenshots/login.png
```

Después:

```powershell
git add docs/screenshots README.md
git commit -m "docs: añade capturas de la aplicación"
```

> Si falta alguna imagen, GitHub mostrará un icono de imagen rota en el README. Añade todos los
> archivos enlazados antes de hacer el commit.
