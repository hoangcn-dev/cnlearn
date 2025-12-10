import { Routes } from '@angular/router';
import { adminGuard } from '../../core/guards/admin.guard';
import { paths } from '../../app.routes';

export const adminRoutes: Routes = [
	{
		path: '',
		loadComponent: () => import('../../shared/layouts/admin-layout/admin-layout').then(m => m.AdminLayout),
		canActivate: [adminGuard],
		children: [
			{ path: '', redirectTo: paths.manageDashboard, pathMatch: 'full' },
			{ path: paths.manageDashboard, loadComponent: () => import('./pages/dashboard-overview/dashboard-overview.component').then(m => m.DashboardOverviewComponent) },
			{ path: paths.manageUsers, loadComponent: () => import('./pages/user-management/user-management.component').then(m => m.UserManagementComponent) },
		]
	}
];