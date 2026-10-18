import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let message = 'Ha ocurrido un error, inténtalo de nuevo';

      if (error.status === 0) {
        message = 'No se pudo conectar con el servidor';
      } else if (error.status === 400) {
        message = 'Datos no válidos';
      } else if (error.status === 401 && req.url.includes('/auth/login')) {
        message = 'Email o contraseña incorrectos';
      } else if (error.status === 401) {
        message = 'Tu sesión ha caducado, vuelve a entrar';
      } else if (error.status === 409) {
        message = 'Ese email ya está registrado';
      }

      snackBar.open(message, 'Cerrar', { duration: 4000 });
      return throwError(() => error);
    }),
  );
};
