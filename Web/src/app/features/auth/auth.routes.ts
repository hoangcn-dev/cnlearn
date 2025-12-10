import { Routes } from '@angular/router';
import { paths } from '../../app.routes';

export const authRoutes: Routes = [
  { path: '', redirectTo: paths.authLogin, pathMatch: 'full' },
  { path: paths.authLogin, loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent) },
  { path: paths.authCallback, loadComponent: () => import('./pages/oauth-callback/oauth-callback.component').then(m => m.OAuthCallbackComponent)}
];
