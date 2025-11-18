import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzTagModule } from 'ng-zorro-antd/tag';

interface Tool {
  id: string;
  name: string;
  description: string;
  category: string;
  downloadUrl: string;
  version: string;
  size: string;
  platform: string[];
  icon: string;
  tags: string[];
}

@Component({
  selector: 'app-tools',
  standalone: true,
  imports: [CommonModule, NzCardModule, NzButtonModule, NzIconModule, NzGridModule, NzTagModule],
  template: `
    <div class="tools-container">
      <div class="header">
        <h1>Công cụ & Tiện ích</h1>
        <p>Tổng hợp các công cụ hữu ích cho developers và end-users</p>
      </div>

      <div class="categories">
        <button 
          *ngFor="let category of categories"
          class="category-btn"
          [class.active]="selectedCategory === category"
          (click)="filterByCategory(category)">
          {{ category }}
        </button>
      </div>

      <div nz-row [nzGutter]="[16, 16]" class="tools-grid">
        <div nz-col nzXs="24" nzSm="12" nzLg="8" *ngFor="let tool of filteredTools">
          <nz-card class="tool-card" nzHoverable>
            <div class="tool-header">
              <div class="tool-icon">
                <span nz-icon [nzType]="tool.icon" nzTheme="outline"></span>
              </div>
              <div class="tool-info">
                <h3>{{ tool.name }}</h3>
                <p>{{ tool.description }}</p>
              </div>
            </div>
            
            <div class="tool-details">
              <div class="detail-row">
                <span class="label">Version:</span>
                <span class="value">{{ tool.version }}</span>
              </div>
              <div class="detail-row">
                <span class="label">Size:</span>
                <span class="value">{{ tool.size }}</span>
              </div>
              <div class="detail-row">
                <span class="label">Platform:</span>
                <div class="platforms">
                  <nz-tag *ngFor="let platform of tool.platform" nzColor="blue">
                    {{ platform }}
                  </nz-tag>
                </div>
              </div>
            </div>

            <div class="tool-tags">
              <nz-tag *ngFor="let tag of tool.tags" nzColor="default">
                {{ tag }}
              </nz-tag>
            </div>

            <div class="tool-actions">
              <button 
                nz-button 
                nzType="primary" 
                nzSize="large"
                (click)="downloadTool(tool)">
                <span nz-icon nzType="download" nzTheme="outline"></span>
                Tải xuống
              </button>
              <button 
                nz-button 
                nzType="default"
                (click)="viewDetails(tool)">
                Chi tiết
              </button>
            </div>
          </nz-card>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .tools-container {
      padding: 24px;
      background: #f5f5f5;
      min-height: calc(100vh - 64px);
    }

    .header {
      text-align: center;
      margin-bottom: 32px;
    }

    .header h1 {
      font-size: 2.5rem;
      color: #2c3e50;
      margin-bottom: 0.5rem;
      font-weight: 600;
    }

    .header p {
      color: #7f8c8d;
      font-size: 1.1rem;
    }

    .categories {
      display: flex;
      justify-content: center;
      gap: 12px;
      margin-bottom: 32px;
      flex-wrap: wrap;
    }

    .category-btn {
      padding: 10px 20px;
      border: 2px solid #e9ecef;
      background: white;
      border-radius: 25px;
      cursor: pointer;
      transition: all 0.3s ease;
      font-weight: 500;
      color: #6c757d;
    }

    .category-btn:hover {
      border-color: #667eea;
      color: #667eea;
    }

    .category-btn.active {
      background: linear-gradient(45deg, #667eea, #764ba2);
      color: white;
      border-color: transparent;
    }

    .tools-grid {
      max-width: 1200px;
      margin: 0 auto;
    }

    .tool-card {
      height: 100%;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      transition: all 0.3s ease;
    }

    .tool-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0,0,0,0.15);
    }

    .tool-header {
      display: flex;
      align-items: flex-start;
      gap: 16px;
      margin-bottom: 16px;
    }

    .tool-icon {
      width: 50px;
      height: 50px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(45deg, #667eea, #764ba2);
      border-radius: 10px;
      color: white;
      font-size: 24px;
    }

    .tool-info h3 {
      margin: 0 0 8px 0;
      color: #2c3e50;
      font-size: 1.3rem;
      font-weight: 600;
    }

    .tool-info p {
      margin: 0;
      color: #7f8c8d;
      font-size: 14px;
      line-height: 1.5;
    }

    .tool-details {
      margin-bottom: 16px;
      padding: 16px;
      background: #f8f9fa;
      border-radius: 8px;
    }

    .detail-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 8px;
    }

    .detail-row:last-child {
      margin-bottom: 0;
    }

    .label {
      font-weight: 600;
      color: #495057;
      font-size: 14px;
    }

    .value {
      color: #6c757d;
      font-size: 14px;
    }

    .platforms {
      display: flex;
      gap: 4px;
      flex-wrap: wrap;
    }

    .tool-tags {
      margin-bottom: 20px;
      display: flex;
      gap: 4px;
      flex-wrap: wrap;
    }

    .tool-actions {
      display: flex;
      gap: 8px;
      justify-content: space-between;
    }

    .tool-actions button {
      flex: 1;
    }

    @media (max-width: 768px) {
      .tools-container {
        padding: 16px;
      }

      .header h1 {
        font-size: 2rem;
      }

      .tool-header {
        flex-direction: column;
        text-align: center;
      }

      .tool-actions {
        flex-direction: column;
      }
    }
  `]
})
export class ToolsComponent {
  selectedCategory = 'Tất cả';
  
  categories = [
    'Tất cả',
    'Development',
    'Design',
    'Productivity',
    'Utilities'
  ];

  tools: Tool[] = [
    {
      id: 'vscode',
      name: 'Visual Studio Code',
      description: 'Code editor mạnh mẽ và miễn phí từ Microsoft',
      category: 'Development',
      downloadUrl: 'https://code.visualstudio.com/download',
      version: '1.84.2',
      size: '95 MB',
      platform: ['Windows', 'macOS', 'Linux'],
      icon: 'code',
      tags: ['Editor', 'Free', 'Extensions']
    },
    {
      id: 'postman',
      name: 'Postman',
      description: 'Platform để test và phát triển APIs',
      category: 'Development',
      downloadUrl: 'https://www.postman.com/downloads/',
      version: '10.18.9',
      size: '156 MB',
      platform: ['Windows', 'macOS', 'Linux'],
      icon: 'api',
      tags: ['API', 'Testing', 'Collaboration']
    },
    {
      id: 'figma',
      name: 'Figma Desktop',
      description: 'Công cụ thiết kế UI/UX collaborative',
      category: 'Design',
      downloadUrl: 'https://www.figma.com/downloads/',
      version: '116.15.2',
      size: '78 MB',
      platform: ['Windows', 'macOS'],
      icon: 'bg-colors',
      tags: ['Design', 'UI/UX', 'Collaborative']
    },
    {
      id: 'notion',
      name: 'Notion',
      description: 'Workspace tổng hợp cho ghi chú và quản lý dự án',
      category: 'Productivity',
      downloadUrl: 'https://www.notion.so/desktop',
      version: '2.0.30',
      size: '120 MB',
      platform: ['Windows', 'macOS'],
      icon: 'file-text',
      tags: ['Notes', 'Planning', 'Database']
    },
    {
      id: 'git',
      name: 'Git for Windows',
      description: 'Hệ thống quản lý version control phân tán',
      category: 'Development',
      downloadUrl: 'https://git-scm.com/downloads',
      version: '2.42.0',
      size: '47 MB',
      platform: ['Windows'],
      icon: 'branches',
      tags: ['Version Control', 'CLI', 'Essential']
    },
    {
      id: 'winrar',
      name: 'WinRAR',
      description: 'Công cụ nén và giải nén file mạnh mẽ',
      category: 'Utilities',
      downloadUrl: 'https://www.win-rar.com/download.html',
      version: '6.24',
      size: '3.2 MB',
      platform: ['Windows'],
      icon: 'file-zip',
      tags: ['Compression', 'Archive', 'Utility']
    }
  ];

  filteredTools: Tool[] = [];

  constructor(private titleService: Title) {
    this.titleService.setTitle('Công cụ & Tiện ích - HoangCN');
    this.filteredTools = this.tools;
  }

  filterByCategory(category: string): void {
    this.selectedCategory = category;
    if (category === 'Tất cả') {
      this.filteredTools = this.tools;
    } else {
      this.filteredTools = this.tools.filter(tool => tool.category === category);
    }
  }

  downloadTool(tool: Tool): void {
    window.open(tool.downloadUrl, '_blank');
  }

  viewDetails(tool: Tool): void {
    alert(`Chi tiết công cụ: ${tool.name}\n\nMô tả: ${tool.description}\nVersion: ${tool.version}\nSize: ${tool.size}`);
  }
}