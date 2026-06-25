import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { PagareService, Pagare } from '../../services/pagare.service';
import { AuthStateService } from '../../services/auth-state.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-pagare-view',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagare-view.component.html',
  styleUrl: './pagare-view.component.scss'
})
export class PagareViewComponent implements OnInit {
  pagare: Pagare | null = null;
  loading = true;
  error = '';
  nombreUsuario: string = '';

  constructor(
    private pagareService: PagareService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private authStateService: AuthStateService
  ) {
    // Obtener nombre del usuario autenticado
    if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
      this.nombreUsuario = localStorage.getItem('userName') || '';
    }
  }

  ngOnInit() {
    console.log('[PagareView] ngOnInit iniciado');
    const id = this.route.snapshot.paramMap.get('id');
    console.log('[PagareView] ID de ruta:', id);
    if (id) {
      this.loadPagare(parseInt(id));
    } else {
      console.error('[PagareView] No se encontró ID en la ruta');
      this.error = 'No se proporcionó ID del pagaré';
    }
  }

  loadPagare(id: number) {
    console.log('[PagareView] loadPagare iniciado, ID:', id);
    this.loading = true;
    console.log('[PagareView] Loading set to true');
    this.cdr.detectChanges();
    this.pagareService.getPagareById(id).subscribe({
      next: (pagare) => {
        console.log('[PagareView] Pagaré recibido:', pagare);
        this.pagare = pagare;
        this.loading = false;
        console.log('[PagareView] Loading set to false');
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('[PagareView] Error al cargar pagaré:', err);
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

  printPagare() {
    window.print();
  }

  downloadPDF() {
    // Import dinámico para evitar problemas con SSR
    import('html2pdf.js').then((html2pdfModule) => {
      const html2pdf = html2pdfModule.default;

      const element = document.querySelector('.pagare-card') as HTMLElement;
      if (!element) {
        Swal.fire({
          title: 'Error',
          text: 'No se encontró el elemento del pagaré',
          icon: 'error',
          confirmButtonColor: '#d33'
        });
        return;
      }

      const opt = {
        margin: 0.5,
        filename: `pagare-${this.pagare?.numeroExpediente || 'documento'}.pdf`,
        image: { type: 'jpeg' as const, quality: 0.98 },
        html2canvas: { scale: 2, useCORS: true },
        jsPDF: { unit: 'in' as const, format: 'letter' as const, orientation: 'portrait' as const }
      };

      html2pdf().set(opt).from(element).save().then(() => {
        Swal.fire({
          title: '¡Éxito!',
          text: 'El PDF se ha descargado correctamente',
          icon: 'success',
          confirmButtonColor: '#3085d6',
          timer: 2000,
          timerProgressBar: true
        });
      }).catch((err: any) => {
        console.error('Error al generar PDF:', err);
        Swal.fire({
          title: 'Error',
          text: 'No se pudo generar el PDF',
          icon: 'error',
          confirmButtonColor: '#d33'
        });
      });
    }).catch((err: any) => {
      console.error('Error al cargar html2pdf:', err);
      Swal.fire({
        title: 'Error',
        text: 'No se pudo cargar la librería de PDF',
        icon: 'error',
        confirmButtonColor: '#d33'
      });
    });
  }

  goBack() {
    this.router.navigate(['/pagares']);
  }

  formatDate(date: string): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('es-ES', {
      day: 'numeric',
      month: 'long',
      year: 'numeric'
    });
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('es-MX', {
      style: 'currency',
      currency: 'MXN'
    }).format(amount);
  }

  numberToWords(num: number): string {
    // Implementación básica de números a letras
    if (num === 0) return 'cero';
    const unidades = ['', 'uno', 'dos', 'tres', 'cuatro', 'cinco', 'seis', 'siete', 'ocho', 'nueve'];
    const decenas = ['', '', 'veinte', 'treinta', 'cuarenta', 'cincuenta', 'sesenta', 'setenta', 'ochenta', 'noventa'];
    const centenas = ['', 'ciento', 'doscientos', 'trescientos', 'cuatrocientos', 'quinientos', 'seiscientos', 'setecientos', 'ochocientos', 'novecientos'];

    if (num < 10) return unidades[num];
    if (num < 20) return ['diez', 'once', 'doce', 'trece', 'catorce', 'quince', 'dieciséis', 'diecisiete', 'dieciocho', 'diecinueve'][num - 10];
    if (num < 100) {
      if (num % 10 === 0) return decenas[Math.floor(num / 10)];
      return decenas[Math.floor(num / 10)] + ' y ' + unidades[num % 10];
    }
    if (num < 1000) {
      if (num % 100 === 0) return centenas[Math.floor(num / 100)];
      return centenas[Math.floor(num / 100)] + ' ' + this.numberToWords(num % 100);
    }
    // Para números mayores, simplificación
    return num.toString();
  }
}
