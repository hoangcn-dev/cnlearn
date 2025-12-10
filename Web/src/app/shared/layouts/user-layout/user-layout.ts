import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzDropdownMenuComponent, NzDropDownModule } from "ng-zorro-antd/dropdown";
import { NzIconModule } from 'ng-zorro-antd/icon';
import { ToastService } from '../../../core/services/toast.service';
import { AuthService } from '../../../features/auth/services/auth.service';
import { UtilService } from '../../../core/services/util.service';
import { LoginComponent } from '../../../features/auth/pages/login/login.component';
import { ModalService } from '../../../core/services/modal.service';
import { DashboardOverviewComponent } from '../../../features/admin/pages/dashboard-overview/dashboard-overview.component';
import { log } from 'console';
import { paths } from '../../../app.routes';

@Component({
	selector: 'app-user-layout',
	imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, NzDropDownModule, NzAvatarModule, NzIconModule],
	templateUrl: './user-layout.html',
	styleUrl: './user-layout.css',
})
export class UserLayout {
  
  authService = inject(AuthService);
  router = inject(Router);
  toastService = inject(ToastService);
  util = inject(UtilService);
  modalService = inject(ModalService);

  urls = paths

  logout(): void {
    this.modalService.showConfirm(
      "Đăng xuất khỏi hệ thống?",
      () => {
        this.authService.logout().subscribe({
          complete: () => {
            this.toastService.success("Đăng xuất thành công!");
            this.router.navigateByUrl(paths.getHomePath());
          }
        });
      }
    );
  }
}
