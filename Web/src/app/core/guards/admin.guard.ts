import { inject, PLATFORM_ID } from "@angular/core";
import { isPlatformBrowser } from "@angular/common";
import { CanActivate, CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../../features/auth/services/auth.service";

export const adminGuard: CanActivateFn = (route, state) => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const user = authService.getCurrentUser();
    
    if (user?.role !== 'admin') {
        router.navigate(['/dang-nhap'], { queryParams: { returnUrl: state.url } });
        return false;
    }

    console.log('✅ Admin access granted');
    return true;
}