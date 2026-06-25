import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5161/api';

  constructor(private http: HttpClient) {}

  login(correo: string, contrasena: string, nombre?: string, rol?: string): Observable<any> {
    const body: any = { correo, contrasena };
    if (nombre) {
      body.nombre = nombre;
    }
    if (rol) {
      body.rol = rol;
    }
    if (typeof document !== 'undefined') {
      console.log('[AuthService] login - Cookie antes:', document.cookie);
    }
    console.log('[AuthService] login - withCredentials: true');
    return this.http.post(`${this.apiUrl}/auth/login`,
      body,
      { withCredentials: true }
    );
  }

  getCurrentUser(): Observable<any> {
    return this.http.get(`${this.apiUrl}/auth/me`, 
      { withCredentials: true }
    );
  }

  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/logout`, {}, 
      { withCredentials: true }
    );
  }

  isLoggedIn(): boolean {
    if (typeof document === 'undefined') {
      return false;
    }
    return document.cookie.includes('pagares_session');
  }
}
