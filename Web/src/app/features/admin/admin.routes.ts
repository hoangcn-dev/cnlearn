import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { AdminLayoutComponent } from './admin-layout.component';
import { adminGuard } from '../../core/guards/admin.guard';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [adminGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadComponent: () => 
          import('./pages/dashboard-overview/dashboard-overview.component').then(
            m => m.DashboardOverviewComponent
          )
      },
      {
        path: 'users',
        loadComponent: () => 
          import('./pages/user-management/user-management.component').then(
            m => m.UserManagementComponent
          )
      },
      {
        path: 'products',
        loadComponent: () => 
          import('./pages/product-management/product-management.component').then(
            m => m.ProductManagementComponent
          )
      },
      {
        path: 'orders',
        loadComponent: () => 
          import('./pages/order-management/order-management.component').then(
            m => m.OrderManagementComponent
          )
      },
      {
        path: 'settings',
        loadComponent: () => 
          import('./pages/settings/settings.component').then(
            m => m.SettingsComponent
          )
      }
    ]
  }
];
