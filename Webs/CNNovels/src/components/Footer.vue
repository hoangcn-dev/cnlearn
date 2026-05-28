<template>
  <footer class="footer glass-panel">
    <div class="footer-container container">
      <div class="footer-brand">
        <div class="logo">
          <span class="logo-icon">📖</span>
          <span class="logo-text">CN<span class="accent">Novels</span></span>
        </div>
        <p class="brand-desc">
          Mạng đọc truyện chữ trực tuyến cập nhật nhanh nhất, tối ưu giao diện đọc tốt nhất trên điện thoại và máy tính. Đọc truyện online hoàn toàn miễn phí.
        </p>
      </div>

      <div class="footer-links-grid">
        <div class="link-column">
          <h4>Khám Phá</h4>
          <ul>
            <li><RouterLink to="/">Truyện Hot</RouterLink></li>
            <li><RouterLink to="/">Truyện Mới</RouterLink></li>
            <li><RouterLink to="/">Truyện Hoàn Thành</RouterLink></li>
          </ul>
        </div>
        <div class="link-column">
          <h4>Chính Sách</h4>
          <ul>
            <li><a href="#">Điều Khoản Dịch Vụ</a></li>
            <li><a href="#">Chính Sách Quyền Riêng Tư</a></li>
            <li><a href="#">Khiếu Nại Bản Quyền</a></li>
          </ul>
        </div>
        <div class="link-column">
          <h4>Liên Hệ</h4>
          <p class="contact-info">📬 contact@cnnovels.com</p>
          <div class="social-icons">
            <span class="icon">📘</span>
            <span class="icon">🐦</span>
            <span class="icon">✉️</span>
          </div>
        </div>
      </div>
    </div>
    <div class="footer-bottom">
      <p class="copyright">&copy; 2026 CNNovels. Clone of ETruyen. All rights reserved.</p>
    </div>
  </footer>
</template>

<style scoped>
.footer {
  border-radius: var(--border-radius-md) var(--border-radius-md) 0 0;
  border-bottom: none;
  border-left: none;
  border-right: none;
  padding: 50px 0 20px 0;
  margin-top: auto;
}

.footer-container {
  display: grid;
  grid-template-columns: 1.5fr repeat(3, 1fr);
  gap: 40px;
  margin-bottom: 40px;
}

.footer-brand {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.logo {
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: var(--font-heading);
  font-size: 22px;
  font-weight: 900;
}

.logo-text .accent {
  color: var(--accent-cyan);
  text-shadow: var(--glow-cyan);
}

.brand-desc {
  font-size: 13.5px;
  color: var(--text-secondary);
  line-height: 1.6;
}

.footer-links-grid {
  display: contents;
}

.link-column h4 {
  font-family: var(--font-heading);
  font-size: 16px;
  font-weight: 700;
  margin-bottom: 16px;
  color: var(--text-primary);
}

.link-column ul {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.link-column ul a {
  font-size: 13.5px;
  color: var(--text-secondary);
}

.link-column ul a:hover {
  color: var(--accent-cyan);
  padding-left: 4px;
}

.contact-info {
  font-size: 13.5px;
  color: var(--text-secondary);
  margin-bottom: 12px;
}

.social-icons {
  display: flex;
  gap: 12px;
}

.social-icons .icon {
  font-size: 20px;
  cursor: pointer;
  transition: var(--transition-smooth);
}

.social-icons .icon:hover {
  transform: scale(1.2);
}

.footer-bottom {
  border-top: 1px solid var(--border-color);
  padding-top: 20px;
  text-align: center;
}

.copyright {
  font-size: 12px;
  color: var(--text-muted);
}

@media (max-width: 768px) {
  .footer-container {
    grid-template-columns: 1fr;
    gap: 30px;
  }
}
</style>
