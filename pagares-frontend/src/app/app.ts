import { Component, signal } from '@angular/core';
import { RouterOutlet, Router } from '@angular/router';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { CommonModule } from '@angular/common';
import { AuthStateService } from './services/auth-state.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SidebarComponent, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('pagares-frontend');

  constructor(
    private router: Router,
    private authStateService: AuthStateService
  ) {
    this.authStateService.updateAuthState();
  }

  get isAuthenticated() {
    return this.authStateService.isAuthenticatedSignal();
  }
}
