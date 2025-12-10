import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { en_US, provideNzI18n } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { NzModalModule } from 'ng-zorro-antd/modal';

registerLocaleData(en);

/**
 * Application Configuration
 * 
 * Configure global providers for the application:
 * - Router with defined routes
 * - HTTP client with interceptors for authentication and error handling
 * - Zone change detection for performance optimization
 * - Client-side hydration for SSR
 */
export const appConfig: ApplicationConfig = {
  providers: [
    importProvidersFrom(NzModalModule),
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([authInterceptor, errorInterceptor])
    ),
    provideClientHydration(withEventReplay()), provideNzI18n(en_US), provideAnimationsAsync(), provideHttpClient()
  ]
};
