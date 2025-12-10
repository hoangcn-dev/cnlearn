import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';
import { Router } from '@angular/router';
import { ToastService } from '../services/toast.service';
import { paths } from '../../app.routes';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);
  const authService = inject(AuthService);
  
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      debugger


      if (error.status == 401) {
        authService.endSession();
        router.navigateByUrl(paths.authLogin);
        toast.error("Vui lòng đăng nhập để tiếp tục!");
      }

      console.log(error);
      return throwError(() => error.error);
    })
  );
};
