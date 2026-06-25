import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { PagaresListComponent } from './components/pagares-list/pagares-list.component';
import { PagareFormComponent } from './components/pagare-form/pagare-form.component';
import { PagareViewComponent } from './components/pagare-view/pagare-view.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'pagares', component: PagaresListComponent },
  { path: 'pagares/crear', component: PagareFormComponent },
  { path: 'pagares/:id/ver', component: PagareViewComponent },
  { path: 'pagares/:id/editar', component: PagareFormComponent },
  { path: 'pagares/:id', component: PagareFormComponent },
  { path: '**', redirectTo: '/login' }
];
