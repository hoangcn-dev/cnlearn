import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Core Module
 * 
 * This module should be imported only once in the AppModule.
 * It contains singleton services, guards, and interceptors that are used throughout the application.
 */
@NgModule({
  imports: [CommonModule],
  providers: [
    // Services are provided in 'root' using @Injectable({ providedIn: 'root' })
    // Guards and Interceptors are registered in app.config.ts for standalone components
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it in the AppModule only.');
    }
  }
}
