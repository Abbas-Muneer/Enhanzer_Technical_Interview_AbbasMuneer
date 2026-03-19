import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const appRoutes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then((module) => module.LoginComponent)
  },
  {
    path: 'purchase-bill',
    canActivate: [authGuard],
    loadComponent: () => import('./features/purchase-bill/purchase-bill.component').then((module) => module.PurchaseBillComponent)
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
