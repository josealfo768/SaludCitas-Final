import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  try {
    const guardada = localStorage.getItem('saludcitas_sesion');
    const token = guardada ? JSON.parse(guardada)?.token : null;
    if (token) {
      req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
    }
  } catch {
    // si la sesión guardada está dañada, se envía sin token
  }
  return next(req);
};