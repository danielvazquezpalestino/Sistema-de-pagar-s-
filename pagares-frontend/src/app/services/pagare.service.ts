import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Pagare {
  idPagare?: number;
  numeroExpediente: string;
  monto: number;
  promesaPago: string;
  beneficiario: string;
  fechaVencimiento: string;
  lugarPago: string;
  fechaElaboracion: string;
  lugarSuscripcion: string;
  firma: string;
  idUsuario?: number;
  nombreUsuario?: string;
  fechaCreacion?: string;
}

export interface PagareCreateRequest {
  numeroExpediente: string;
  monto: number;
  promesaPago: string;
  beneficiario: string;
  fechaVencimiento: string;
  lugarPago: string;
  fechaElaboracion: string;
  lugarSuscripcion: string;
  firma: string;
}

@Injectable({
  providedIn: 'root'
})
export class PagareService {
  private apiUrl = 'http://localhost:5161/api';

  constructor(private http: HttpClient) {}

  getPagares(): Observable<Pagare[]> {
    if (typeof document !== 'undefined') {
      console.log('[PagareService] getPagares - Cookie:', document.cookie);
    }
    console.log('[PagareService] getPagares - withCredentials: true');
    return this.http.get<Pagare[]>(`${this.apiUrl}/pagares`,
      { withCredentials: true }
    );
  }

  getPagareById(id: number): Observable<Pagare> {
    return this.http.get<Pagare>(`${this.apiUrl}/pagares/${id}`, 
      { withCredentials: true }
    );
  }

  createPagare(pagare: PagareCreateRequest): Observable<Pagare> {
    return this.http.post<Pagare>(`${this.apiUrl}/pagares`, pagare,
      { withCredentials: true }
    );
  }

  updatePagare(id: number, pagare: Pagare): Observable<Pagare> {
    return this.http.put<Pagare>(`${this.apiUrl}/pagares/${id}`, pagare, 
      { withCredentials: true }
    );
  }

  deletePagare(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/pagares/${id}`, 
      { withCredentials: true }
    );
  }
}
