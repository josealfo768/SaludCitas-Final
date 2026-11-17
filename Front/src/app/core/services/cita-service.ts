import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { Cita } from '../models/cita';
import { API_URL } from './auth-service';

export interface DatosCita {
  pacienteId: number;
  medicoId: number;
  sedeId: number;
  fechaHora: string; // 'YYYY-MM-DDTHH:mm:00'
}

@Injectable({ providedIn: 'root' })
export class CitaService {
  constructor(private http: HttpClient) {}

  getHorariosDisponibles(medicoId: number, fecha: string): Observable<string[]> {
    // Sin atención sábados ni domingos
    const dia = new Date(`${fecha}T00:00:00`).getDay();
    if (dia === 0 || dia === 6) {
      return new Observable<string[]>((o) => {
        o.next([]);
        o.complete();
      });
    }

    const params = new HttpParams().set('medicoId', medicoId).set('fecha', fecha);
    return this.http.get<string[]>(`${API_URL}/citas/horarios`, { params }).pipe(
      map((lista) =>
        lista
          .map((f) => f.substring(11, 16))
          // Hora de almuerzo: sin atención de 12:00 a 14:00
          .filter((h) => h < '12:00' || h >= '14:00')
      )
    );
  }

  getCitasPaciente(pacienteId: number): Observable<Cita[]> {
    return this.http
      .get<Cita[]>(`${API_URL}/citas/mias`)
      .pipe(map((lista) => [...lista].sort((a, b) => a.fechaHora.localeCompare(b.fechaHora))));
  }

  agendar(datos: DatosCita): Observable<Cita> {
    const cuerpo = {
      medicoId: datos.medicoId,
      sedeId: datos.sedeId,
      fechaHora: datos.fechaHora,
    };
    return this.http.post<{ citaId: number }>(`${API_URL}/citas`, cuerpo).pipe(
      map((resp) => ({ ...datos, citaId: resp.citaId, estado: 'programada' }) as Cita),
      catchError((err) => this.manejarError(err, 'No se pudo agendar la cita'))
    );
  }

  cancelar(citaId: number): Observable<Cita> {
    return this.http.post<void>(`${API_URL}/citas/${citaId}/cancelar`, {}).pipe(
      map(() => ({ citaId, estado: 'cancelada' }) as Cita),
      catchError((err) => this.manejarError(err, 'No se pudo cancelar la cita'))
    );
  }

  private manejarError(err: any, mensajePorDefecto: string): Observable<never> {
    const mensaje = err?.error?.detail ?? mensajePorDefecto;
    return throwError(() => new Error(mensaje));
  }
}