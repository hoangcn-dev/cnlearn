import { Routes } from '@angular/router';

export const authRoutes: Routes = [
  {
    path: '',
    loadComponent: () => 
      import('./pages/login/login.component').then(
        m => m.LoginComponent
      )
  },
  {
    path: 'callback',
    loadComponent: () => 
      import('./pages/oauth-callback/oauth-callback.component').then(
        m => m.OAuthCallbackComponent
      )
  }
];
