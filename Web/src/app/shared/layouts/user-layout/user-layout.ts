import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDropdownMenuComponent, NzDropDownModule } from "ng-zorro-antd/dropdown";
import { NzIconModule } from 'ng-zorro-antd/icon';
import { ToastService } from '../../../core/services/toast.service';
import { AuthService } from '../../../features/auth/services/auth.service';

@Component({
	selector: 'app-user-layout',
	imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, NzDropDownModule, NzAvatarModule, NzIconModule],
	templateUrl: './user-layout.html',
	styleUrl: './user-layout.css',
})
export class UserLayout {
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
