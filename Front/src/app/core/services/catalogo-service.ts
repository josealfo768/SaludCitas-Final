import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { catchError, delay, map, switchMap, tap } from 'rxjs/operators';

import { Especialidad } from '../models/especialidad';
import { Sede } from '../models/sede';
import { Medico } from '../models/medico';
import { ESPECIALIDADES, SEDES, MEDICOS } from '../data/mock-data';
import { API_URL } from './auth-service';

export interface FiltroMedicos {
  especialidadId?: number;
  sedeId?: number;
}

export interface DatosMedico {
  especialidadId: number;
  nombres: string;
  apellidos: string;
  sedeIds: number[];
}

export interface DatosEspecialidad {
  nombre: string;
}

export interface DatosSede {
  nombre: string;
  direccion: string;
}

@Injectable({ providedIn: 'root' })
export class CatalogoService {
  private readonly retraso = 600; // milisegundos, simula la espera de una API

    constructor(private http: HttpClient) {}

  // Estado local: siempre se reemplaza con una copia nueva, nunca se modifica en sitio
  private readonly especialidadesSubject = new BehaviorSubject<Especialidad[]>(ESPECIALIDADES);
  private readonly sedesSubject = new BehaviorSubject<Sede[]>(SEDES);
  private readonly medicosSubject = new BehaviorSubject<Medico[]>(MEDICOS);

  // ---------- Lecturas ----------
   getEspecialidades(): Observable<Especialidad[]> {
    return this.http
      .get<Especialidad[]>(`${API_URL}/catalogo/especialidades`)
      .pipe(tap((lista) => this.especialidadesSubject.next(lista)));
  }
    getSedes(): Observable<Sede[]> {
    return this.http
      .get<Sede[]>(`${API_URL}/catalogo/sedes`)
      .pipe(tap((lista) => this.sedesSubject.next(lista)));
  }

  getMedico(medicoId: number): Observable<Medico> {
    return this.simular(() => {
      const medico = this.medicosSubject.value.find((m) => m.medicoId === medicoId);
      if (!medico) {
        throw new Error('El médico no existe');
      }
      return medico;
    });
  }

    getMedicos(filtro: FiltroMedicos = {}, soloActivos = true): Observable<Medico[]> {
    let params = new HttpParams();
    if (filtro.especialidadId) params = params.set('especialidadId', filtro.especialidadId);
    if (filtro.sedeId) params = params.set('sedeId', filtro.sedeId);
    return this.http
      .get<Medico[]>(`${API_URL}/catalogo/medicos`, { params })
      .pipe(
        tap((lista) => this.medicosSubject.next(lista)),
        map((lista) => lista.map((m) => ({ ...m, activo: true })))
      );
  }

    getMedicosAdmin(): Observable<Medico[]> {
    return this.http.get<Medico[]>(`${API_URL}/admin/medicos`);
  }


  // ---------- Médicos ----------
    crearMedico(datos: DatosMedico): Observable<Medico> {
    return this.http
      .post<{ medicoId: number }>(`${API_URL}/admin/medicos`, datos)
      .pipe(map((resp) => ({ ...datos, medicoId: resp.medicoId, activo: true }) as Medico));
  }

    actualizarMedico(medicoId: number, datos: DatosMedico): Observable<Medico> {
    return this.http
      .put<void>(`${API_URL}/admin/medicos/${medicoId}`, datos)
      .pipe(map(() => ({ ...datos, medicoId, activo: true }) as Medico));
  }

    cambiarEstadoMedico(medicoId: number, activo: boolean): Observable<Medico> {
    return this.http.patch<void>(`${API_URL}/admin/medicos/${medicoId}/estado`, { activo }).pipe(
      switchMap(() => this.getMedicosAdmin()),
      map((lista) => lista.find((m) => m.medicoId === medicoId) as Medico)
    );
  }

  // ---------- Especialidades ----------
    crearEspecialidad(datos: DatosEspecialidad): Observable<Especialidad> {
    return this.http
      .post<{ especialidadId: number }>(`${API_URL}/admin/especialidades`, datos)
      .pipe(
        map((resp) => ({ especialidadId: resp.especialidadId, nombre: datos.nombre.trim() }) as Especialidad),
        catchError((err) => this.errorApi(err, 'No se pudo crear la especialidad'))
      );
  }

    actualizarEspecialidad(id: number, datos: DatosEspecialidad): Observable<Especialidad> {
    return this.http.put<void>(`${API_URL}/admin/especialidades/${id}`, datos).pipe(
      map(() => ({ especialidadId: id, nombre: datos.nombre.trim() }) as Especialidad),
      catchError((err) => this.errorApi(err, 'No se pudo actualizar la especialidad'))
    );
  }

    eliminarEspecialidad(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/admin/especialidades/${id}`).pipe(
      catchError((err) => this.errorApi(err, 'No se pudo eliminar la especialidad'))
    );
  }
  // ---------- Sedes ----------
    crearSede(datos: DatosSede): Observable<Sede> {
    return this.http.post<{ sedeId: number }>(`${API_URL}/admin/sedes`, datos).pipe(
      map((resp) => ({ sedeId: resp.sedeId, nombre: datos.nombre.trim(), direccion: datos.direccion.trim() }) as Sede),
      catchError((err) => this.errorApi(err, 'No se pudo crear la sede'))
    );
  }

     actualizarSede(id: number, datos: DatosSede): Observable<Sede> {
    return this.http.put<void>(`${API_URL}/admin/sedes/${id}`, datos).pipe(
      map(() => ({ sedeId: id, nombre: datos.nombre.trim(), direccion: datos.direccion.trim() }) as Sede),
      catchError((err) => this.errorApi(err, 'No se pudo actualizar la sede'))
    );
  }

    eliminarSede(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/admin/sedes/${id}`).pipe(
      catchError((err) => this.errorApi(err, 'No se pudo eliminar la sede'))
    );
  }

  // ---------- Utilidades privadas ----------
  // Ejecuta la operación después de un retraso, como si fuera una petición HTTP.
  // Si la función lanza un Error, el Observable falla igual que lo haría una API.
  private simular<T>(operacion: () => T): Observable<T> {
    return of(null).pipe(
      delay(this.retraso),
      map(() => operacion())
    );
  }

    // Convierte un error de la API en un Error con el mensaje que envía el backend
  private errorApi(err: any, mensajePorDefecto: string): Observable<never> {
    const mensaje = err?.error?.detail ?? mensajePorDefecto;
    return throwError(() => new Error(mensaje));
  }

  private siguienteId(ids: number[]): number {
    return Math.max(0, ...ids) + 1;
  }

  private validarNombreUnico(existentes: string[], nombre: string): void {
    const normalizado = nombre.trim().toLowerCase();
    if (existentes.some((n) => n.trim().toLowerCase() === normalizado)) {
      throw new Error('Ya existe un registro con ese nombre');
    }
  }
}