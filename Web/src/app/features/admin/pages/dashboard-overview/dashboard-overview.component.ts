import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';

@Component({
  selector: 'app-dashboard-overview',
  standalone: true,
  imports: [CommonModule, NzCardModule, NzStatisticModule, NzGridModule, NzIconModule],
  template: `
    <div class="dashboard-container">
      <div class="header">
        <h1>Dashboard</h1>
        <p>Tổng quan hệ thống quản lý</p>
      </div>

      <!-- Stats Cards -->
      <div nz-row [nzGutter]="[16, 16]" class="stats-row">
        <div nz-col nzXs="24" nzSm="12" nzLg="6">
          <nz-card class="stat-card users">
            <div class="stat-content">
              <div class="stat-icon">
                <span nz-icon nzType="user" nzTheme="outline"></span>
              </div>
              <div class="stat-info">
                <nz-statistic 
                  nzTitle="Tổng Users" 
                  [nzValue]="stats.totalUsers"
                  [nzValueStyle]="{ color: '#3f8600' }">
                </nz-statistic>
              </div>
            </div>
          </nz-card>
        </div>
        
        <div nz-col nzXs="24" nzSm="12" nzLg="6">
          <nz-card class="stat-card orders">
            <div class="stat-content">
              <div class="stat-icon">
                <span nz-icon nzType="shopping-cart" nzTheme="outline"></span>
              </div>
              <div class="stat-info">
                <nz-statistic 
                  nzTitle="Đơn hàng" 
                  [nzValue]="stats.totalOrders"
                  [nzValueStyle]="{ color: '#1890ff' }">
                </nz-statistic>
              </div>
            </div>
          </nz-card>
        </div>

        <div nz-col nzXs="24" nzSm="12" nzLg="6">
          <nz-card class="stat-card revenue">
            <div class="stat-content">
              <div class="stat-icon">
                <span nz-icon nzType="dollar" nzTheme="outline"></span>
              </div>
              <div class="stat-info">
                <nz-statistic 
                  nzTitle="Doanh thu" 
                  [nzValue]="stats.revenue" 
                  nzPrefix="$"
                  [nzValueStyle]="{ color: '#cf1322' }">
                </nz-statistic>
              </div>
            </div>
          </nz-card>
        </div>

        <div nz-col nzXs="24" nzSm="12" nzLg="6">
          <nz-card class="stat-card products">
            <div class="stat-content">
              <div class="stat-icon">
                <span nz-icon nzType="appstore" nzTheme="outline"></span>
              </div>
              <div class="stat-info">
                <nz-statistic 
                  nzTitle="Sản phẩm" 
                  [nzValue]="stats.totalProducts"
                  [nzValueStyle]="{ color: '#722ed1' }">
                </nz-statistic>
              </div>
            </div>
          </nz-card>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="quick-actions">
        <h2>Thao tác nhanh</h2>
        <div nz-row [nzGutter]="[16, 16]">
          <div nz-col nzXs="24" nzSm="12" nzMd="8">
            <nz-card class="action-card" nzHoverable>
              <div class="action-content">
                <span nz-icon nzType="plus" class="action-icon"></span>
                <h3>Thêm sản phẩm mới</h3>
                <p>Tạo sản phẩm mới cho cửa hàng</p>
              </div>
            </nz-card>
          </div>
          
          <div nz-col nzXs="24" nzSm="12" nzMd="8">
            <nz-card class="action-card" nzHoverable>
              <div class="action-content">
                <span nz-icon nzType="team" class="action-icon"></span>
                <h3>Quản lý users</h3>
                <p>Xem và quản lý tài khoản người dùng</p>
              </div>
            </nz-card>
          </div>

          <div nz-col nzXs="24" nzSm="12" nzMd="8">
            <nz-card class="action-card" nzHoverable>
              <div class="action-content">
                <span nz-icon nzType="bar-chart" class="action-icon"></span>
                <h3>Báo cáo</h3>
                <p>Xem báo cáo chi tiết và thống kê</p>
              </div>
            </nz-card>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 24px;
      background: #f5f5f5;
      min-height: calc(100vh - 64px);
    }

    .header {
      margin-bottom: 24px;
    }

    .header h1 {
      margin: 0;
      font-size: 28px;
      font-weight: 600;
      color: #262626;
    }

    .header p {
      margin: 4px 0 0 0;
      color: #8c8c8c;
      font-size: 14px;
    }

    .stats-row {
      margin-bottom: 32px;
    }

    .stat-card {
      border-radius: 8px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24);
      transition: all 0.3s cubic-bezier(.25,.8,.25,1);
    }

    .stat-card:hover {
      box-shadow: 0 14px 28px rgba(0,0,0,0.25), 0 10px 10px rgba(0,0,0,0.22);
      transform: translateY(-2px);
    }

    .stat-content {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }

    .stat-icon {
      width: 48px;
      height: 48px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      font-size: 24px;
      color: white;
    }

    .stat-card.users .stat-icon {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }

    .stat-card.orders .stat-icon {
      background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
    }

    .stat-card.revenue .stat-icon {
      background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
    }

    .stat-card.products .stat-icon {
      background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
    }

    .stat-info {
      text-align: right;
    }

    .quick-actions {
      margin-top: 32px;
    }

    .quick-actions h2 {
      margin-bottom: 16px;
      font-size: 20px;
      font-weight: 600;
      color: #262626;
    }

    .action-card {
      height: 140px;
      border-radius: 8px;
      cursor: pointer;
      transition: all 0.3s ease;
    }

    .action-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0,0,0,0.15);
    }

    .action-content {
      height: 100%;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      text-align: center;
    }

    .action-icon {
      font-size: 32px;
      color: #1890ff;
      margin-bottom: 8px;
    }

    .action-content h3 {
      margin: 8px 0 4px 0;
      font-size: 16px;
      font-weight: 600;
      color: #262626;
    }

    .action-content p {
      margin: 0;
      font-size: 12px;
      color: #8c8c8c;
    }
  `]
})
export class DashboardOverviewComponent {
  stats = {
    totalUsers: 1247,
    totalOrders: 856,
    revenue: 125780,
    totalProducts: 423
  };

  constructor(private titleService: Title) {
    this.titleService.setTitle('Dashboard - HoangCN Admin');
  }
}
