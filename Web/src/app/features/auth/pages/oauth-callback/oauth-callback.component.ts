import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { paths } from '../../../../app.routes';

@Component({
    selector: 'app-oauth-callback',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div style="display: flex; justify-content: center; align-items: center; height: 100vh;">
      <div style="text-align: center;">
        <img src="assets/images/logo.png" alt="Logo" class="callback-logo" />
        <div class="spinner"></div>
        <p class="h4" style="margin-top: 18px; color: #666;">{{statusMessage}}</p>
      </div>
    </div>
  `,
    styles: [`
    .callback-logo {
      height: 256px;
      display: block;
      margin: 0 auto 14px auto;
      filter: drop-shadow(0 2px 6px rgba(0,0,0,0.08));
    }

    .spinner {
      width: 48px;
      height: 48px;
      border: 4px solid #f3f3f3;
      border-top: 4px solid #3498db;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto;
    }
    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
  `]
})
export class OAuthCallbackComponent implements OnInit {
    statusMessage: string = "Đang xử lý đăng nhập...";

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authService: AuthService,
        private toastService: ToastService
    ) { }

    ngOnInit(): void {
        const success = this.route.snapshot.queryParams['success'];
        const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        if (success === 'true') {
          this.authService.getLoginInfo().subscribe({
            next: (loginInfo) => {
              // Auto redirect admin to admin panel
              const redirectUrl = loginInfo.role === 'admin' ? paths.getManageDashboardPath() : returnUrl;
              this.waitAndRedirect('Đăng nhập thành công', 3, redirectUrl);
            }
          });
        } else {
            this.waitAndRedirect('Đăng nhập thất bại', 5, paths.getAuthLoginPath());
        }
    }

    private waitAndRedirect(label: string, sec: number, redirectUrl: string) {
        this.statusMessage = `${label}, chuyển hướng sau ${sec}s`;
        const sub = setInterval(() => {
            sec -= 1;
            if (sec < 0) {
                clearInterval(sub);
                this.router.navigate([ redirectUrl ]);
                return;
            }
            this.statusMessage = `${label}, chuyển hướng sau ${sec}s`;
        }, 1000);
    }
}
