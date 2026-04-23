import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError, tap } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('token');
  
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  return next(req).pipe(
    tap(event => {
      // Optional: log successful responses if needed
    }),
    catchError((error: HttpErrorResponse) => {
      console.error(`[API Error] ${req.method} ${req.url}:`, error);
      return throwError(() => error);
    })
  );
};
