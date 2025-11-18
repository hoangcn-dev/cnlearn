import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-header">
      <h2>Quản lý Users</h2>
    </div>
    <div class="page-content">
      <p>Danh sách users và các chức năng quản lý...</p>
    </div>
  `,
  styles: [`
    .page-header {
      margin-bottom: 20px;
    }

    .page-header h2 {
      font-size: 24px;
      font-weight: 600;
      color: #262626;
    }

    .page-content {
      background: white;
      padding: 20px;
      border-radius: 8px;
    }
  `]
})
export class UserManagementComponent {
  constructor(private titleService: Title) {
    this.titleService.setTitle('Quản lý Users - HoangCN');
  }
}
