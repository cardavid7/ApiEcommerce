import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// Uso en rutas: canActivate: [roleGuard(['Admin'])]
export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return (_route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isAuthenticated()) {
      authService.logout();
      return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
    }

    if (allowedRoles.some((role) => authService.hasRole(role))) {
      return true;
    }

    return router.createUrlTree(['/forbidden']);
  };
};
