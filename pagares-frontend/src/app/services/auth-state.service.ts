import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  private isAuthenticated = signal(
    (typeof window !== 'undefined' && typeof localStorage !== 'undefined')
      ? !!localStorage.getItem('userId')
      : false
  );

  readonly isAuthenticatedSignal = this.isAuthenticated.asReadonly();

  constructor() {
    console.log('[AuthStateService] Constructor - isAuthenticated:', this.isAuthenticated());
  }

  updateAuthState() {
    const userId = (typeof window !== 'undefined' && typeof localStorage !== 'undefined')
      ? localStorage.getItem('userId')
      : null;
    console.log('[AuthStateService] updateAuthState - userId:', userId);
    this.isAuthenticated.set(!!userId);
    console.log('[AuthStateService] updateAuthState - isAuthenticated:', this.isAuthenticated());
  }

  logout() {
    if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
      localStorage.removeItem('userId');
      localStorage.removeItem('userName');
      localStorage.removeItem('userRole');
    }
    this.isAuthenticated.set(false);
    console.log('[AuthStateService] logout - isAuthenticated:', false);
  }
}
