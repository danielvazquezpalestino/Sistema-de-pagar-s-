import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { PagareService, Pagare } from '../../services/pagare.service';
import { AuthStateService } from '../../services/auth-state.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-pagares-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagares-list.component.html',
  styleUrl: './pagares-list.component.scss'
})
export class PagaresListComponent implements OnInit {
  pagares: Pagare[] = [];
  loading = true;

  constructor(
    private pagareService: PagareService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authStateService: AuthStateService
  ) {}

  ngOnInit() {
    this.loadPagares();
  }

  loadPagares() {
    this.pagareService.getPagares().subscribe({
      next: (pagares) => {
        this.pagares = pagares;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error al cargar pagarés', err);
        this.loading = false;
        this.cdr.detectChanges();

        if (err.status === 401) {
          this.authStateService.logout();
          this.router.navigate(['/login']);
        }
      }
    });
  }

  goToCreate() {
    this.router.navigate(['/pagares/crear']);
  }

  goToDashboard() {
    this.router.navigate(['/dashboard']);
  }

  viewPagare(id: number | undefined) {
    if (id) {
      this.router.navigate(['/pagares', id, 'ver']);
    }
  }

  editPagare(id: number | undefined) {
    if (id) {
      this.router.navigate(['/pagares', id, 'editar']);
    }
  }

  deletePagare(id: number | undefined) {
    if (id) {
      Swal.fire({
        title: '¿Eliminar Pagaré?',
        text: '¿Estás seguro de eliminar este pagaré? Esta acción no se puede deshacer.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
      }).then((result) => {
        if (result.isConfirmed) {
          this.pagareService.deletePagare(id).subscribe({
            next: () => {
              Swal.fire({
                title: '¡Eliminado!',
                text: 'El pagaré ha sido eliminado exitosamente.',
                icon: 'success',
                confirmButtonColor: '#3085d6'
              });
              this.loadPagares();
            },
            error: (err) => {
              console.error('Error al eliminar pagaré', err);
              Swal.fire({
                title: 'Error',
                text: 'No se pudo eliminar el pagaré',
                icon: 'error',
                confirmButtonColor: '#d33'
              });
            }
          });
        }
      });
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('es-ES');
  }
}
