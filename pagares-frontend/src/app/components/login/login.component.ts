import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { AuthStateService } from '../../services/auth-state.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  correo = '';
  contrasena = '';
  nombre = '';
  rol = 'abogado'; // Rol por defecto
  loading = false;
  error = '';
  showNombreField = false; // Para mostrar campo nombre cuando sea necesario
  showRolField = false; // Para mostrar campo rol cuando sea necesario

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authStateService: AuthStateService
  ) {
    // Limpiar localStorage al cargar la página de login
    if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
      console.log('[Login] Limpiando localStorage');
      localStorage.removeItem('userId');
      localStorage.removeItem('userName');
      localStorage.removeItem('userRole');
    }
  }

  onSubmit() {
    if (!this.correo || !this.contrasena) {
      this.error = 'Por favor complete todos los campos';
      return;
    }

    // Si se mostró el campo nombre, es requerido
    if (this.showNombreField && !this.nombre) {
      this.error = 'Por favor ingrese su nombre para crear la cuenta';
      return;
    }

    this.loading = true;
    this.error = '';

    // Agregar timeout de 5 segundos para respuestas lentas
    const loginRequest = this.authService.login(
      this.correo,
      this.contrasena,
      this.nombre || undefined,
      this.showRolField ? this.rol : undefined
    );

    const timeout = setTimeout(() => {
      if (this.loading) {
        this.loading = false;
        this.error = 'El servidor está tardando mucho en responder. Intente nuevamente.';
        Swal.fire({
          title: 'Tiempo de espera agotado',
          text: 'El servidor está tardando mucho en responder. Por favor intente nuevamente.',
          icon: 'warning',
          confirmButtonColor: '#3085d6',
          confirmButtonText: 'Reintentar'
        });
      }
    }, 5000);

    loginRequest.subscribe({
      next: (response) => {
        clearTimeout(timeout);
        console.log('Login exitoso', response);
        if (typeof document !== 'undefined') {
          console.log('[Login] Cookie después de login:', document.cookie);
        }
        // Después del login, obtener información del usuario
        this.authService.getCurrentUser().subscribe({
          next: (user) => {
            console.log('Usuario obtenido', user);
            console.log('Usuario rol:', user.rol);
            // Guardar información del usuario en localStorage
            if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
              if (user.idUsuario) {
                localStorage.setItem('userId', user.idUsuario.toString());
                console.log('[Login] userId guardado:', user.idUsuario);
              }
              if (user.rol) {
                localStorage.setItem('userRole', user.rol);
                console.log('[Login] userRole guardado:', user.rol);
              }
              if (user.nombre) {
                localStorage.setItem('userName', user.nombre);
                console.log('[Login] userName guardado:', user.nombre);
              }
            }
            if (typeof document !== 'undefined') {
              console.log('[Login] Cookie después de guardar localStorage:', document.cookie);
            }
            // Actualizar estado de autenticación
            this.authStateService.updateAuthState();
            this.router.navigate(['/dashboard']);
          },
          error: (err) => {
            clearTimeout(timeout);
            console.error('Error al obtener usuario', err);
            // Si falla obtener el usuario, aún así redirigir al dashboard
            this.router.navigate(['/dashboard']);
          }
        });
      },
      error: (err) => {
        clearTimeout(timeout);
        console.error('Error de login', err);
        console.error('Status:', err.status);
        console.error('Error object:', err.error);
        this.loading = false;

        // Si el error es 401 Unauthorized, mostrar campos de registro
        if (err.status === 401) {
          console.log('Detectado error 401, mostrando campos de registro');
          console.log('Antes - showNombreField:', this.showNombreField);
          console.log('Antes - showRolField:', this.showRolField);
          this.showNombreField = true;
          this.showRolField = true;
          console.log('Después - showNombreField:', this.showNombreField);
          console.log('Después - showRolField:', this.showRolField);
          this.error = 'Cuenta no existente. Ingrese su nombre y rol para crear la cuenta';

          // Forzar detección de cambios
          this.cdr.detectChanges();
          console.log('Después de detectChanges()');

          setTimeout(() => {
            console.log('En timeout - showNombreField:', this.showNombreField);
            console.log('En timeout - showRolField:', this.showRolField);
          }, 100);

          Swal.fire({
            title: 'Usuario no encontrado',
            text: 'La cuenta no existe. Por favor complete los campos para crearla.',
            icon: 'info',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Entendido'
          }).then(() => {
            // Enfocar el campo de nombre después de cerrar la alerta
            setTimeout(() => {
              const nombreInput = document.getElementById('nombre');
              if (nombreInput) {
                nombreInput.focus();
              }
            }, 100);
          });
        } else {
          const errorMsg = err.error?.error || 'Error al iniciar sesión';
          this.error = errorMsg;

          Swal.fire({
            title: 'Error',
            text: errorMsg,
            icon: 'error',
            confirmButtonColor: '#d33',
            confirmButtonText: 'Aceptar'
          });
        }
      }
    });
  }

  cancelRegister() {
    this.showNombreField = false;
    this.showRolField = false;
    this.nombre = '';
    this.rol = 'abogado';
    this.error = '';
  }
}

