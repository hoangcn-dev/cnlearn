import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/pages/login/login.component';


export const routes: Routes = [
  { path: 'dang-nhap', component: LoginComponent },
  { path: 'admin', loadChildren: () => import('./features/admin/admin.routes').then(m => m.adminRoutes) },
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
