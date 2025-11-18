import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { FormsModule } from '@angular/forms';

interface OnlineTool {
  id: string;
  name: string;
  description: string;
  category: string;
  url: string;
  icon: string;
  tags: string[];
  features: string[];
  isPopular?: boolean;
}

@Component({
  selector: 'app-online-tools',
  standalone: true,
  imports: [
    CommonModule, 
    NzCardModule, 
    NzButtonModule, 
    NzIconModule, 
    NzGridModule, 
    NzTagModule,
    NzInputModule,
    NzUploadModule,
    FormsModule
  ],
  template: `
    <div class="online-tools-container">
      <div class="header">
        <h1>Công cụ Online & Convert</h1>
        <p>Tổng hợp các công cụ trực tuyến hữu ích cho convert, edit và xử lý file</p>
      </div>

      <!-- Quick Convert Section -->
      <div class="quick-convert-section">
        <h2>🚀 Convert nhanh</h2>
        <div nz-row [nzGutter]="[16, 16]">
          <div nz-col nzXs="24" nzSm="12" nzLg="8">
            <nz-card class="convert-card" title="PDF to Word" nzHoverable>
              <div class="convert-actions">
                <button nz-button nzType="primary" nzSize="large" (click)="openTool('pdf-to-word')">
                  <span nz-icon nzType="file-pdf" nzTheme="outline"></span>
                  Chọn file PDF
                </button>
                <p>Chuyển đổi PDF sang DOCX miễn phí</p>
              </div>
            </nz-card>
          </div>
          
          <div nz-col nzXs="24" nzSm="12" nzLg="8">
            <nz-card class="convert-card" title="Image Convert" nzHoverable>
              <div class="convert-actions">
                <button nz-button nzType="primary" nzSize="large" (click)="openTool('image-convert')">
                  <span nz-icon nzType="picture" nzTheme="outline"></span>
                  Chọn ảnh
                </button>
                <p>JPG, PNG, WebP, AVIF convert</p>
              </div>
            </nz-card>
          </div>

          <div nz-col nzXs="24" nzSm="12" nzLg="8">
            <nz-card class="convert-card" title="Video Convert" nzHoverable>
              <div class="convert-actions">
                <button nz-button nzType="primary" nzSize="large" (click)="openTool('video-convert')">
                  <span nz-icon nzType="video-camera" nzTheme="outline"></span>
                  Chọn video
                </button>
                <p>MP4, AVI, MOV, WebM convert</p>
              </div>
            </nz-card>
          </div>
        </div>
      </div>

      <!-- Categories -->
      <div class="categories">
        <button 
          *ngFor="let category of categories"
          class="category-btn"
          [class.active]="selectedCategory === category"
          (click)="filterByCategory(category)">
          {{ category }}
        </button>
      </div>

      <!-- Tools Grid -->
      <div nz-row [nzGutter]="[16, 16]" class="tools-grid">
        <div nz-col nzXs="24" nzSm="12" nzLg="8" *ngFor="let tool of filteredTools">
          <nz-card class="tool-card" [class.popular]="tool.isPopular" nzHoverable>
            <div class="popular-badge" *ngIf="tool.isPopular">
              <span nz-icon nzType="fire" nzTheme="fill"></span>
              Popular
            </div>
            
            <div class="tool-header">
              <div class="tool-icon">
                <span nz-icon [nzType]="tool.icon" nzTheme="outline"></span>
              </div>
              <div class="tool-info">
                <h3>{{ tool.name }}</h3>
                <p>{{ tool.description }}</p>
              </div>
            </div>
            
            <div class="tool-features">
              <h4>Tính năng:</h4>
              <ul>
                <li *ngFor="let feature of tool.features">{{ feature }}</li>
              </ul>
            </div>

            <div class="tool-tags">
              <nz-tag *ngFor="let tag of tool.tags" nzColor="blue">
                {{ tag }}
              </nz-tag>
            </div>

            <div class="tool-actions">
              <button 
                nz-button 
                nzType="primary" 
                nzSize="large"
                nzBlock
                (click)="openTool(tool.id)">
                <span nz-icon nzType="link" nzTheme="outline"></span>
                Sử dụng ngay
              </button>
            </div>
          </nz-card>
        </div>
      </div>

      <!-- Custom Tools Section -->
      <div class="custom-tools-section">
        <h2>🛠️ Công cụ tùy chỉnh</h2>
        <div nz-row [nzGutter]="[16, 16]">
          <div nz-col nzXs="24" nzSm="12">
            <nz-card title="JSON Formatter" nzHoverable>
              <textarea 
                nz-input 
                [(ngModel)]="jsonInput"
                placeholder="Paste JSON here..."
                rows="6">
              </textarea>
              <div class="custom-actions">
                <button nz-button nzType="primary" (click)="formatJSON()">Format</button>
                <button nz-button nzType="default" (click)="minifyJSON()">Minify</button>
                <button nz-button nzType="default" (click)="validateJSON()">Validate</button>
              </div>
            </nz-card>
          </div>

          <div nz-col nzXs="24" nzSm="12">
            <nz-card title="Base64 Encoder/Decoder" nzHoverable>
              <input 
                nz-input 
                [(ngModel)]="base64Input"
                placeholder="Enter text or base64..."
                class="base64-input">
              <div class="custom-actions">
                <button nz-button nzType="primary" (click)="encodeBase64()">Encode</button>
                <button nz-button nzType="default" (click)="decodeBase64()">Decode</button>
                <button nz-button nzType="default" (click)="copyResult()">Copy</button>
              </div>
              <div class="result-area">
                <textarea 
                  nz-input 
                  [value]="base64Result"
                  readonly
                  rows="3"
                  placeholder="Kết quả sẽ hiển thị ở đây...">
                </textarea>
              </div>
            </nz-card>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .online-tools-container {
      padding: 24px;
      background: #f5f5f5;
      min-height: calc(100vh - 64px);
    }

    .header {
      text-align: center;
      margin-bottom: 40px;
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

    .quick-convert-section {
      margin-bottom: 40px;
      background: white;
      padding: 24px;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }

    .quick-convert-section h2 {
      margin-bottom: 20px;
      color: #2c3e50;
      font-weight: 600;
    }

    .convert-card {
      height: 150px;
      border-radius: 8px;
    }

    .convert-actions {
      text-align: center;
      height: 100%;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }

    .convert-actions button {
      margin-bottom: 12px;
      width: 100%;
    }

    .convert-actions p {
      margin: 0;
      color: #6c757d;
      font-size: 12px;
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
      margin: 0 auto 40px auto;
    }

    .tool-card {
      height: 100%;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      transition: all 0.3s ease;
      position: relative;
      overflow: hidden;
    }

    .tool-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0,0,0,0.15);
    }

    .tool-card.popular {
      border: 2px solid #ff7875;
    }

    .popular-badge {
      position: absolute;
      top: 12px;
      right: 12px;
      background: linear-gradient(45deg, #ff7875, #ff4d4f);
      color: white;
      padding: 4px 8px;
      border-radius: 12px;
      font-size: 11px;
      font-weight: 600;
      z-index: 1;
    }

    .popular-badge span {
      margin-right: 4px;
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

    .tool-features {
      margin-bottom: 16px;
    }

    .tool-features h4 {
      margin: 0 0 8px 0;
      color: #495057;
      font-size: 14px;
      font-weight: 600;
    }

    .tool-features ul {
      margin: 0;
      padding-left: 16px;
      color: #6c757d;
      font-size: 13px;
    }

    .tool-features li {
      margin-bottom: 4px;
    }

    .tool-tags {
      margin-bottom: 20px;
      display: flex;
      gap: 4px;
      flex-wrap: wrap;
    }

    .tool-actions button {
      font-weight: 600;
    }

    .custom-tools-section {
      background: white;
      padding: 24px;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }

    .custom-tools-section h2 {
      margin-bottom: 20px;
      color: #2c3e50;
      font-weight: 600;
    }

    .custom-actions {
      margin-top: 12px;
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
    }

    .base64-input {
      margin-bottom: 12px;
    }

    .result-area {
      margin-top: 12px;
    }

    @media (max-width: 768px) {
      .online-tools-container {
        padding: 16px;
      }

      .header h1 {
        font-size: 2rem;
      }

      .custom-actions {
        flex-direction: column;
      }

      .custom-actions button {
        width: 100%;
      }
    }
  `]
})
export class OnlineToolsComponent {
  selectedCategory = 'Tất cả';
  jsonInput = '';
  base64Input = '';
  base64Result = '';
  
  categories = [
    'Tất cả',
    'Convert',
    'PDF Tools',
    'Image Tools',
    'Text Tools',
    'Developer Tools',
    'Compress'
  ];

  tools: OnlineTool[] = [
    {
      id: 'pdf-to-word',
      name: 'PDF to Word Converter',
      description: 'Chuyển đổi PDF sang Word document online',
      category: 'PDF Tools',
      url: 'https://smallpdf.com/pdf-to-word',
      icon: 'file-pdf',
      tags: ['PDF', 'Word', 'Convert'],
      features: ['Free', 'No watermark', 'OCR support', 'Batch convert'],
      isPopular: true
    },
    {
      id: 'image-converter',
      name: 'Image Converter',
      description: 'Convert between JPG, PNG, WebP, AVIF formats',
      category: 'Image Tools',
      url: 'https://convertio.co/image-converter/',
      icon: 'picture',
      tags: ['Image', 'JPG', 'PNG', 'WebP'],
      features: ['Multiple formats', 'Batch convert', 'Quality control', 'Resize option']
    },
    {
      id: 'compress-pdf',
      name: 'PDF Compressor',
      description: 'Nén file PDF giữ nguyên chất lượng',
      category: 'Compress',
      url: 'https://www.ilovepdf.com/compress_pdf',
      icon: 'file-zip',
      tags: ['PDF', 'Compress', 'Size'],
      features: ['Smart compression', 'Quality preserved', 'Password protect', 'OCR']
    },
    {
      id: 'youtube-downloader',
      name: 'YouTube Downloader',
      description: 'Tải video và audio từ YouTube',
      category: 'Convert',
      url: 'https://yt1s.com/',
      icon: 'youtube',
      tags: ['YouTube', 'Download', 'MP3', 'MP4'],
      features: ['HD quality', 'MP3/MP4 formats', 'No software needed', 'Fast download'],
      isPopular: true
    },
    {
      id: 'json-formatter',
      name: 'JSON Formatter',
      description: 'Format, validate và minify JSON online',
      category: 'Developer Tools',
      url: 'https://jsonformatter.org/',
      icon: 'code',
      tags: ['JSON', 'Format', 'Validate'],
      features: ['Syntax highlighting', 'Error detection', 'Tree view', 'Copy to clipboard']
    },
    {
      id: 'color-palette',
      name: 'Color Palette Generator',
      description: 'Tạo bảng màu đẹp cho design',
      category: 'Image Tools',
      url: 'https://coolors.co/',
      icon: 'bg-colors',
      tags: ['Color', 'Palette', 'Design'],
      features: ['AI generated', 'Export formats', 'Color blind safe', 'Trending colors']
    },
    {
      id: 'qr-generator',
      name: 'QR Code Generator',
      description: 'Tạo mã QR cho text, URL, WiFi',
      category: 'Text Tools',
      url: 'https://www.qr-code-generator.com/',
      icon: 'qrcode',
      tags: ['QR Code', 'Generate', 'URL'],
      features: ['Multiple types', 'Customizable', 'High resolution', 'Batch generate']
    },
    {
      id: 'html-to-pdf',
      name: 'HTML to PDF',
      description: 'Convert webpage HTML thành PDF',
      category: 'Convert',
      url: 'https://pdfshift.io/',
      icon: 'file-text',
      tags: ['HTML', 'PDF', 'Webpage'],
      features: ['Full page capture', 'Custom CSS', 'API available', 'High quality']
    }
  ];

  filteredTools: OnlineTool[] = [];

  constructor(private titleService: Title) {
    this.titleService.setTitle('Công cụ Online - HoangCN');
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

  openTool(toolId: string): void {
    const tool = this.tools.find(t => t.id === toolId);
    if (tool) {
      window.open(tool.url, '_blank');
    }
  }

  formatJSON(): void {
    try {
      const parsed = JSON.parse(this.jsonInput);
      this.jsonInput = JSON.stringify(parsed, null, 2);
    } catch (error) {
      alert('Invalid JSON format!');
    }
  }

  minifyJSON(): void {
    try {
      const parsed = JSON.parse(this.jsonInput);
      this.jsonInput = JSON.stringify(parsed);
    } catch (error) {
      alert('Invalid JSON format!');
    }
  }

  validateJSON(): void {
    try {
      JSON.parse(this.jsonInput);
      alert('✅ Valid JSON!');
    } catch (error) {
      alert('❌ Invalid JSON format!');
    }
  }

  encodeBase64(): void {
    try {
      this.base64Result = btoa(this.base64Input);
    } catch (error) {
      alert('Error encoding to Base64!');
    }
  }

  decodeBase64(): void {
    try {
      this.base64Result = atob(this.base64Input);
    } catch (error) {
      alert('Error decoding from Base64!');
    }
  }

  copyResult(): void {
    navigator.clipboard.writeText(this.base64Result).then(() => {
      alert('Copied to clipboard!');
    });
  }
}