import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

// Endpoints publicos de auth: un 401 aqui es "credenciales invalidas", no una
// sesion expirada, asi que no debe forzar logout/redirect.
const AUTH_ENDPOINTS = ['/Users/Login', '/Users/Register'];

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: unknown) => {
      if (error instanceof HttpErrorResponse) {
        const isAuthEndpoint = AUTH_ENDPOINTS.some((endpoint) => req.url.includes(endpoint));

        if (error.status === 401 && !isAuthEndpoint) {
          authService.logout();
          router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
        }

        if (error.status === 403) {
          router.navigate(['/forbidden']);
        }
      }

      return throwError(() => error);
    }),
  );
};
