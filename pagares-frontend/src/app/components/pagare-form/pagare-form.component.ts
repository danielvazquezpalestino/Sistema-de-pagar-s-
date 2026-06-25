import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PagareService, PagareCreateRequest, Pagare } from '../../services/pagare.service';
import { AuthStateService } from '../../services/auth-state.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-pagare-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './pagare-form.component.html',
  styleUrl: './pagare-form.component.scss'
})
export class PagareFormComponent implements OnInit {
  pagare: Partial<PagareCreateRequest> = {
    numeroExpediente: '',
    monto: 0,
    promesaPago: '',
    beneficiario: '',
    fechaVencimiento: '',
    lugarPago: '',
    fechaElaboracion: new Date().toISOString().split('T')[0],
    lugarSuscripcion: '',
    firma: '' // Campo opcional
  };
  loading = false;
  error = '';
  isEditMode = false;
  pagareId: number | null = null;

  constructor(
    private pagareService: PagareService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private authStateService: AuthStateService
  ) {}

  ngOnInit() {
    console.log('[PagareForm] ngOnInit iniciado');

    const id = this.route.snapshot.paramMap.get('id');
    console.log('[PagareForm] ID de ruta:', id);
    if (id) {
      this.isEditMode = true;
      this.pagareId = parseInt(id);
      console.log('[PagareForm] Modo edición activado, ID:', this.pagareId);
      this.loadPagare();
    } else {
      console.log('[PagareForm] Modo creación');
    }
  }

  loadPagare() {
    console.log('[PagareForm] loadPagare iniciado, ID:', this.pagareId);
    if (this.pagareId) {
      this.loading = true;
      console.log('[PagareForm] Loading set to true');
      this.cdr.detectChanges();
      this.pagareService.getPagareById(this.pagareId).subscribe({
        next: (pagare) => {
          console.log('[PagareForm] Pagaré recibido:', pagare);
          this.pagare = {
            numeroExpediente: pagare.numeroExpediente,
            monto: pagare.monto,
            promesaPago: pagare.promesaPago,
            beneficiario: pagare.beneficiario,
            fechaVencimiento: pagare.fechaVencimiento,
            lugarPago: pagare.lugarPago,
            fechaElaboracion: pagare.fechaElaboracion,
            lugarSuscripcion: pagare.lugarSuscripcion,
            firma: pagare.firma
          };
          this.loading = false;
          console.log('[PagareForm] Loading set to false');
          this.cdr.detectChanges();
        },
        error: (err) => {
          console.error('[PagareForm] Error al cargar pagaré:', err);
          this.loading = false;
          this.cdr.detectChanges();

          if (err.status === 401) {
            this.authStateService.logout();
            this.router.navigate(['/login']);
          } else {
            this.error = 'Error al cargar pagaré';
            Swal.fire({
              title: 'Error',
              text: 'No se pudo cargar el pagaré',
              icon: 'error',
              confirmButtonColor: '#d33'
            }).then(() => {
              this.router.navigate(['/pagares']);
            });
          }
        }
      });
    }
  }

  onSubmit() {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;
    this.error = '';

    if (this.isEditMode && this.pagareId) {
      // Modo edición
      this.pagareService.updatePagare(this.pagareId, this.pagare as Pagare).subscribe({
        next: (response) => {
          this.loading = false;
          console.log('Pagaré actualizado:', response);

          Swal.fire({
            title: '¡Éxito!',
            text: 'El pagaré se actualizó correctamente',
            icon: 'success',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Aceptar',
            allowOutsideClick: false,
            allowEscapeKey: false
          }).then((result) => {
            if (result.isConfirmed) {
              this.router.navigate(['/pagares']);
            }
          });
        },
        error: (err) => {
          this.loading = false;

          if (err.status === 401) {
            this.authStateService.logout();
            this.router.navigate(['/login']);
          } else {
            const errorMessage = err.error?.message || err.message || 'Error desconocido';
            this.error = 'Error al actualizar pagaré: ' + errorMessage;
            console.error('Error al actualizar pagaré', err);

            Swal.fire({
              title: 'Error',
              text: this.error,
              icon: 'error',
              confirmButtonColor: '#d33',
              confirmButtonText: 'Aceptar'
            });
          }
        }
      });
    } else {
      // Modo creación
      this.pagareService.createPagare(this.pagare as PagareCreateRequest).subscribe({
        next: (response) => {
          this.loading = false;
          console.log('Pagaré creado:', response);

          Swal.fire({
            title: '¡Éxito!',
            text: 'El pagaré se creó correctamente',
            icon: 'success',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Aceptar',
            allowOutsideClick: false,
            allowEscapeKey: false
          }).then((result) => {
            if (result.isConfirmed) {
              this.router.navigate(['/pagares']);
            }
          });
        },
        error: (err) => {
          this.loading = false;

          if (err.status === 401) {
            this.authStateService.logout();
            this.router.navigate(['/login']);
          } else {
            const errorMessage = err.error?.message || err.message || 'Error desconocido';
            this.error = 'Error al crear pagaré: ' + errorMessage;
            console.error('Error al crear pagaré', err);

            Swal.fire({
              title: 'Error',
              text: this.error,
              icon: 'error',
              confirmButtonColor: '#d33',
              confirmButtonText: 'Aceptar'
            });
          }
        }
      });
    }
  }

  validateForm(): boolean {
    if (!this.pagare.numeroExpediente || this.pagare.numeroExpediente.trim() === '') {
      this.error = 'El número de expediente es requerido';
      return false;
    }
    if (!this.pagare.monto || this.pagare.monto <= 0) {
      this.error = 'El monto debe ser mayor a 0';
      return false;
    }
    if (!this.pagare.promesaPago || this.pagare.promesaPago.trim() === '') {
      this.error = 'La promesa de pago es requerida';
      return false;
    }
    if (!this.pagare.fechaVencimiento) {
      this.error = 'La fecha de vencimiento es requerida';
      return false;
    }
    if (!this.pagare.beneficiario || this.pagare.beneficiario.trim() === '') {
      this.error = 'El beneficiario es requerido';
      return false;
    }
    if (!this.pagare.lugarPago || this.pagare.lugarPago.trim() === '') {
      this.error = 'El lugar de pago es requerido';
      return false;
    }
    if (!this.pagare.fechaElaboracion) {
      this.error = 'La fecha de elaboración es requerida';
      return false;
    }
    if (!this.pagare.lugarSuscripcion || this.pagare.lugarSuscripcion.trim() === '') {
      this.error = 'El lugar de suscripción es requerido';
      return false;
    }
    // La firma es opcional, no se valida
    return true;
  }

  onCancel() {
    this.router.navigate(['/pagares']);
  }

  goToDashboard() {
    this.router.navigate(['/dashboard']);
  }
}
