import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home.component').then(
        m => m.HomeComponent
      )
  },
  {
    path: 'dang-nhap',
    loadComponent: () =>
      import('./features/auth/pages/login/login.component').then(
        m => m.LoginComponent
      )
  },
  {
    path: 'dang-nhap/callback',
    loadComponent: () =>
      import('./features/auth/pages/oauth-callback/oauth-callback.component').then(
        m => m.OAuthCallbackComponent
      )
  },
  {
    path: 'tools',
    loadComponent: () =>
      import('./features/tools/tools.component').then(
        m => m.ToolsComponent
      )
  },
  {
    path: 'online-tools',
    loadComponent: () =>
      import('./features/online-tools/online-tools.component').then(
        m => m.OnlineToolsComponent
      )
  },
  {
    path: 'admin',
    loadChildren: () => 
      import('./features/admin/admin.routes').then(
        m => m.ADMIN_ROUTES
      )
  },
  {
    path: '**',
    redirectTo: '/'
  }
];
