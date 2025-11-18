import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-header">
      <h2>Cài đặt</h2>
    </div>
    <div class="page-content">
      <p>Các cài đặt hệ thống...</p>
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
export class SettingsComponent {
  constructor(private titleService: Title) {
    this.titleService.setTitle('Cài đặt - HoangCN');
  }
}
