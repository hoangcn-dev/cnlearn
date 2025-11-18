import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="home-container">
      <!-- Hero Section -->
      <section class="hero-section">
        <div class="hero-content">
          <div class="profile-image">
            <img src="/assets/profile-avatar.jpg" alt="HoangCN Profile" />
          </div>
          <h1>Chào mừng đến với HoangCN</h1>
          <p class="subtitle">Full-stack Developer & Software Engineer</p>
          <div class="cta-buttons">
            <a routerLink="/online-tools" class="btn btn-primary">Công cụ Online</a>
            <a routerLink="/tools" class="btn btn-secondary">Download</a>
            <a routerLink="/dang-nhap" class="btn btn-outline">Đăng nhập</a>
          </div>
        </div>
      </section>

      <!-- About Section -->
      <section class="about-section">
        <div class="container">
          <h2>Giới thiệu bản thân</h2>
          <div class="about-grid">
            <div class="about-text">
              <p>
                Xin chào! Tôi là <strong>HoangCN</strong>, một Full-stack Developer với đam mê 
                phát triển các ứng dụng web hiện đại và hiệu quả.
              </p>
              <p>
                Với kinh nghiệm trong việc xây dựng các hệ thống từ frontend đến backend, 
                tôi chuyên về các công nghệ như Angular, .NET Core, và các giải pháp cloud.
              </p>
              <div class="skills">
                <h3>Kỹ năng chính:</h3>
                <div class="skill-tags">
                  <span class="skill-tag">Angular</span>
                  <span class="skill-tag">.NET Core</span>
                  <span class="skill-tag">TypeScript</span>
                  <span class="skill-tag">C#</span>
                  <span class="skill-tag">SQL Server</span>
                  <span class="skill-tag">Azure</span>
                </div>
              </div>
            </div>
            <div class="about-stats">
              <div class="stat-item">
                <h3>5+</h3>
                <p>Năm kinh nghiệm</p>
              </div>
              <div class="stat-item">
                <h3>50+</h3>
                <p>Dự án hoàn thành</p>
              </div>
              <div class="stat-item">
                <h3>100+</h3>
                <p>Khách hàng hài lòng</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Services Section -->
      <section class="services-section">
        <div class="container">
          <h2>Dịch vụ</h2>
          <div class="services-grid">
            <div class="service-card">
              <div class="service-icon">🌐</div>
              <h3>Phát triển Web</h3>
              <p>Xây dựng các ứng dụng web hiện đại với Angular và .NET Core</p>
            </div>
            <div class="service-card">
              <div class="service-icon">📱</div>
              <h3>Ứng dụng di động</h3>
              <p>Phát triển ứng dụng mobile đa nền tảng với các công nghệ mới nhất</p>
            </div>
            <div class="service-card">
              <div class="service-icon">☁️</div>
              <h3>Giải pháp Cloud</h3>
              <p>Triển khai và quản lý hệ thống trên các nền tảng cloud như Azure</p>
            </div>
            <div class="service-card">
              <div class="service-icon">🔧</div>
              <h3>Công cụ & Tiện ích</h3>
              <p>Tạo ra các công cụ hữu ích cho developers và end-users</p>
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .home-container {
      background: white;
      min-height: 100vh;
    }

    .hero-section {
      padding: 80px 20px;
      text-align: center;
      background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
    }

    .hero-content {
      max-width: 800px;
      margin: 0 auto;
    }

    .profile-image {
      margin-bottom: 32px;
    }

    .profile-image img {
      width: 150px;
      height: 150px;
      border-radius: 50%;
      object-fit: cover;
      border: 6px solid white;
      box-shadow: 0 10px 30px rgba(0,0,0,0.1);
    }

    .hero-section h1 {
      font-size: 3.5rem;
      margin-bottom: 1rem;
      font-weight: 700;
      color: #2c3e50;
    }

    .subtitle {
      font-size: 1.5rem;
      margin-bottom: 2rem;
      color: #7f8c8d;
      font-weight: 300;
    }

    .cta-buttons {
      display: flex;
      gap: 1rem;
      justify-content: center;
      flex-wrap: wrap;
    }

    .btn {
      padding: 14px 32px;
      border-radius: 50px;
      text-decoration: none;
      font-weight: 600;
      transition: all 0.3s ease;
      display: inline-block;
      font-size: 16px;
    }

    .btn-primary {
      background: linear-gradient(45deg, #667eea, #764ba2);
      color: white;
      box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
    }

    .btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 25px rgba(102, 126, 234, 0.4);
    }

    .btn-secondary {
      background: transparent;
      color: #667eea;
      border: 2px solid #667eea;
    }

    .btn-secondary:hover {
      background: #667eea;
      color: white;
      transform: translateY(-2px);
    }

    .btn-outline {
      background: transparent;
      color: #2c3e50;
      border: 2px solid #2c3e50;
    }

    .btn-outline:hover {
      background: #2c3e50;
      color: white;
      transform: translateY(-2px);
    }

    .container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 20px;
    }

    .about-section, .services-section {
      padding: 80px 0;
    }

    .about-section {
      background: #f8f9fa;
    }

    .about-section h2, .services-section h2 {
      text-align: center;
      font-size: 2.5rem;
      margin-bottom: 3rem;
      color: #2c3e50;
      font-weight: 600;
    }

    .about-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 4rem;
      align-items: center;
    }

    .about-text p {
      font-size: 1.1rem;
      line-height: 1.8;
      color: #5a6c7d;
      margin-bottom: 1.5rem;
    }

    .skills {
      margin-top: 2rem;
    }

    .skills h3 {
      color: #2c3e50;
      margin-bottom: 1rem;
      font-weight: 600;
    }

    .skill-tags {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .skill-tag {
      background: linear-gradient(45deg, #667eea, #764ba2);
      color: white;
      padding: 8px 16px;
      border-radius: 20px;
      font-size: 14px;
      font-weight: 500;
    }

    .about-stats {
      display: flex;
      flex-direction: column;
      gap: 2rem;
    }

    .stat-item {
      text-align: center;
      padding: 2rem;
      background: white;
      border-radius: 10px;
      box-shadow: 0 5px 15px rgba(0,0,0,0.08);
    }

    .stat-item h3 {
      font-size: 2.5rem;
      color: #667eea;
      margin-bottom: 0.5rem;
      font-weight: 700;
    }

    .stat-item p {
      color: #7f8c8d;
      font-weight: 500;
    }

    .services-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 2rem;
    }

    .service-card {
      background: white;
      padding: 2.5rem 2rem;
      border-radius: 15px;
      text-align: center;
      box-shadow: 0 10px 30px rgba(0,0,0,0.1);
      transition: all 0.3s ease;
    }

    .service-card:hover {
      transform: translateY(-10px);
      box-shadow: 0 20px 40px rgba(0,0,0,0.15);
    }

    .service-icon {
      font-size: 3rem;
      margin-bottom: 1.5rem;
    }

    .service-card h3 {
      color: #2c3e50;
      margin-bottom: 1rem;
      font-weight: 600;
      font-size: 1.3rem;
    }

    .service-card p {
      color: #7f8c8d;
      line-height: 1.6;
    }

    @media (max-width: 768px) {
      .hero-section h1 {
        font-size: 2.5rem;
      }

      .about-grid {
        grid-template-columns: 1fr;
        gap: 2rem;
      }

      .about-stats {
        flex-direction: row;
        justify-content: space-around;
      }

      .stat-item {
        padding: 1.5rem 1rem;
      }

      .stat-item h3 {
        font-size: 2rem;
      }
    }
  `]
})
export class HomeComponent {}
