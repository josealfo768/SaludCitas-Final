import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

import { Usuario } from '../models/usuario';
import { Paciente } from '../models/paciente';

export interface Sesion {
  token: string;
  usuario: Omit<Usuario, 'clave'>;
  paciente?: Paciente;
}

export interface DatosRegistro {
  nombres: string;
  apellidos: string;
  documento: string;
  telefono: string;
  correo: string;
  clave: string;
}

export const API_URL = 'https://localhost:7101/api';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly claveStorage = 'saludcitas_sesion';

  private readonly sesionSubject = new BehaviorSubject<Sesion | null>(this.leerSesion());
  readonly sesion$ = this.sesionSubject.asObservable();

  constructor(private http: HttpClient) {}

  get sesionActual(): Sesion | null {
    return this.sesionSubject.value;
  }

  get estaAutenticado(): boolean {
    return this.sesionActual !== null;
  }

  get esAdmin(): boolean {
    return this.sesionActual?.usuario.rol === 'admin';
  }

  login(correo: string, clave: string): Observable<Sesion> {
    return this.http.post<Sesion>(`${API_URL}/auth/login`, { correo, clave }).pipe(
      tap((sesion) => this.guardarSesion(sesion)),
      catchError(() => throwError(() => new Error('Correo o contraseña incorrectos')))
    );
  }

  // Pendiente: el backend aún no tiene el endpoint de registro
  registrar(datos: DatosRegistro): Observable<Sesion> {
    return this.http.post<Sesion>(`${API_URL}/auth/registro`, datos).pipe(
      tap((sesion) => this.guardarSesion(sesion)),
      catchError((err) =>
        throwError(() => new Error(err?.error?.detail ?? 'No se pudo crear la cuenta'))
      )
    );
  }

  logout(): void {
    localStorage.removeItem(this.claveStorage);
    this.sesionSubject.next(null);
  }

  private guardarSesion(sesion: Sesion): void {
    localStorage.setItem(this.claveStorage, JSON.stringify(sesion));
    this.sesionSubject.next(sesion);
  }

  private leerSesion(): Sesion | null {
    try {
      const guardada = localStorage.getItem(this.claveStorage);
      return guardada ? (JSON.parse(guardada) as Sesion) : null;
    } catch {
      return null;
    }
  }
}