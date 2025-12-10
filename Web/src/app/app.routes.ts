import { Routes } from '@angular/router';

export const paths = {
  getHomePath: () => "/",

  authRoutes: 'xac-thuc',
  authLogin: 'dang-nhap',
  authCallback: 'callback',
  getAuthLoginPath: () => '/' + paths.authRoutes + '/' + paths.authLogin,
  getAuthCallbackPath: () => '/' + paths.authRoutes + '/' + paths.authCallback,

  manageRoutes: 'admin',
  manageUsers: 'users',
  manageDashboard: 'dashboard',
  getManageUsersPath: () => '/' + paths.manageRoutes + '/' + paths.manageUsers,
  getManageDashboardPath: () => '/' + paths.manageRoutes + '/' + paths.manageDashboard,
}

export const routes: Routes = [
  { path: paths.authRoutes, loadChildren: () => import("./features/auth/auth.routes").then(m => m.authRoutes) },
  { path: paths.manageRoutes, loadChildren: () => import('./features/admin/admin.routes').then(m => m.adminRoutes) },
  {
    path: '',
    loadComponent: () => import('./shared/layouts/user-layout/user-layout').then(m => m.UserLayout),
    children: [
      { path: '', loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent) },
      { path: 'tools', loadComponent: () => import('./features/tools/tools.component').then(m => m.ToolsComponent) },
      { path: 'online-tools', loadComponent: () => import('./features/online-tools/online-tools.component').then(m => m.OnlineToolsComponent) },
    ]
  },
  { path: '**', redirectTo: '/' }
];
