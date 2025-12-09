import { Routes } from '@angular/router';
import { adminGuard } from '../../core/guards/admin.guard';

export const adminRoutes: Routes = [
	{
		path: '',
		loadComponent: () => import('../../shared/layouts/admin-layout/admin-layout').then(m => m.AdminLayout),
		canActivate: [adminGuard],
		children: [
			{ path: '', redirectTo: 'dashboard', pathMatch: 'full' },
			{ path: 'dashboard', loadComponent: () => import('./pages/dashboard-overview/dashboard-overview.component').then(m => m.DashboardOverviewComponent) },
			{ path: 'users', loadComponent: () => import('./pages/user-management/user-management.component').then(m => m.UserManagementComponent) },
		]
	}
];