import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from "@angular/router";
import { NzDividerModule } from "ng-zorro-antd/divider";
import { AuthService } from '../../../features/auth/services/auth.service';
import { ModalService } from '../../../core/services/modal.service';
import { ToastService } from '../../../core/services/toast.service';
import { UtilService } from '../../../core/services/util.service';
import { paths } from '../../../app.routes';
import { NzIconModule } from "ng-zorro-antd/icon";

@Component({
  selector: 'app-admin-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, NzDividerModule, NzIconModule],
  templateUrl: './admin-layout.html',
  styleUrl: './admin-layout.css',
})
export class AdminLayout {
  authService = inject(AuthService);
  modalService = inject(ModalService);
  toastService = inject(ToastService);
  router = inject(Router)
  util = inject(UtilService);

  urls = paths;

  logout(): void {
    this.modalService.showConfirm(
      "Đăng xuất khỏi hệ thống?",
      () => {
        this.authService.logout().subscribe({
          complete: () => {
            this.toastService.success("Đăng xuất thành công!");
            this.router.navigateByUrl(paths.getAuthLoginPath());
          }
        });
      }
    );
  }
}
