import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

// Components
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { ButtonComponent } from './components/button/button.component';

// Directives
import { HighlightDirective } from './directives/highlight.directive';
import { HasPermissionDirective } from './directives/has-permission.directive';

// Pipes
import { TruncatePipe } from './pipes/truncate.pipe';
import { TimeAgoPipe } from './pipes/time-ago.pipe';

/**
 * Shared Module
 * 
 * This module contains reusable components, directives, and pipes
 * that can be used across different feature modules.
 * 
 * Note: For standalone components, you can import them directly.
 * This module is useful if you're using traditional NgModules.
 */
@NgModule({
  imports: [
    CommonModule,
    // Import standalone components
    LoadingSpinnerComponent,
    ButtonComponent,
    HighlightDirective,
    HasPermissionDirective,
    TruncatePipe,
    TimeAgoPipe
  ],
  exports: [
    // Export for use in other modules
    LoadingSpinnerComponent,
    ButtonComponent,
    HighlightDirective,
    HasPermissionDirective,
    TruncatePipe,
    TimeAgoPipe
  ]
})
export class SharedModule {}
