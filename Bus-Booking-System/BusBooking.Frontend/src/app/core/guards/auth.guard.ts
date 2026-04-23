import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth.service';

export const authGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    router.navigate(['/auth/login']);
    return false;
  }

  const requiredRole = route.data?.['role'] as string;
  if (requiredRole && authService.getRole() !== requiredRole) {
    router.navigate(['/']);
    return false;
  }

  return true;
};
