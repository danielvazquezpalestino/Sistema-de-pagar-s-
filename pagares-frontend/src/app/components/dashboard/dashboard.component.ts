import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { PagareService } from '../../services/pagare.service';
import { AuthStateService } from '../../services/auth-state.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  currentUser: any = null;
  pagares: any[] = [];
  totalPagares = 0;
  pagaresActivos = 0;
  pagaresPagados = 0;

  constructor(
    private authService: AuthService,
    private pagareService: PagareService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authStateService: AuthStateService
  ) {}

  ngOnInit() {
    this.loadCurrentUser();
    this.loadPagares();
  }

  loadCurrentUser() {
    // Cargar desde localStorage primero para respuesta rápida
    if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
      const userName = localStorage.getItem('userName');
      const userRole = localStorage.getItem('userRole');
      const userId = localStorage.getItem('userId');

      if (userName && userRole) {
        this.currentUser = {
          nombre: userName,
          rol: userRole,
          idUsuario: userId ? parseInt(userId) : 0
        };
      } else {
        // Si no hay datos en localStorage, redirigir al login
        this.router.navigate(['/login']);
      }
    }
  }

  loadPagares() {
    this.pagareService.getPagares().subscribe({
      next: (pagares) => {
        this.pagares = pagares;
        this.totalPagares = pagares.length;
        
        // Calcular activos vs vencidos basado en fechaVencimiento
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        this.pagaresActivos = 0;
        this.pagaresPagados = 0;
        
        pagares.forEach(pagare => {
          const fechaVencimiento = new Date(pagare.fechaVencimiento);
          fechaVencimiento.setHours(0, 0, 0, 0);
          
          if (fechaVencimiento > today) {
            this.pagaresActivos++;
          } else {
            this.pagaresPagados++;
          }
        });
        
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar pagarés', err);

        if (err.status === 401) {
          this.authStateService.logout();
          this.router.navigate(['/login']);
        }
      }
    });
  }

  logout() {
    Swal.fire({
      title: '¿Cerrar Sesión?',
      text: '¿Estás seguro que deseas salir?',
      icon: 'question',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sí, salir',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        // Limpiar localStorage
        if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
          localStorage.removeItem('userId');
          localStorage.removeItem('userRole');
          localStorage.removeItem('userName');
        }

        this.authService.logout().subscribe({
          next: () => {
            this.router.navigate(['/login']);
          },
          error: (err) => {
            console.error('Error al cerrar sesión', err);
            this.router.navigate(['/login']);
          }
        });
      }
    });
  }

  goToPagares() {
    this.router.navigate(['/pagares']);
  }

  goToCreatePagare() {
    this.router.navigate(['/pagares/crear']);
  }
}
