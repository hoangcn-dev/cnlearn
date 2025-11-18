import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  template: `
    <div class="admin-layout">
      <aside class="admin-sidebar">
        <h3>Quản lý</h3>
        <nav>
          <a routerLink="/admin/dashboard" routerLinkActive="active">Dashboard</a>
          <a routerLink="/admin/users" routerLinkActive="active">Quản lý Users</a>
          <a routerLink="/admin/products" routerLinkActive="active">Quản lý Sản phẩm</a>
          <a routerLink="/admin/orders" routerLinkActive="active">Quản lý Đơn hàng</a>
          <a routerLink="/admin/settings" routerLinkActive="active">Cài đặt</a>
        </nav>
      </aside>
      <main class="admin-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .admin-layout {
      display: flex;
      height: 100%;
    }

    .admin-sidebar {
      width: 250px;
      background: #001529;
      padding: 20px;
      color: white;
    }

    .admin-sidebar h3 {
      color: white;
      margin-bottom: 20px;
    }

    .admin-sidebar nav {
      display: flex;
      flex-direction: column;
      gap: 10px;
    }

    .admin-sidebar a {
      color: rgba(255, 255, 255, 0.65);
      text-decoration: none;
      padding: 10px 15px;
      border-radius: 4px;
      transition: all 0.3s;
    }

    .admin-sidebar a:hover {
      color: white;
      background: rgba(255, 255, 255, 0.1);
    }

    .admin-sidebar a.active {
      color: white;
      background: #1890ff;
    }

    .admin-content {
      flex: 1;
      padding: 20px;
      background: #f0f2f5;
      overflow-y: auto;
    }
  `]
})
export class AdminLayoutComponent {}
