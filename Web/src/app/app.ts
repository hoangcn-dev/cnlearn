import { Component, signal, inject } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { AuthService } from './features/auth/services/auth.service';
import { ToastService } from './core/services/toast.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, NzDropDownModule, NzAvatarModule, NzIconModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('HoangCN');
  protected authService = inject(AuthService);
  protected router = inject(Router);
  private toastService = inject(ToastService);

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.toastService.success('Đăng xuất thành công!');
        this.router.navigate(['/dang-nhap']);
      },
      error: (error) => {
        this.toastService.error(error.message || 'Đăng xuất thất bại!');
      }
    });
  }
}
