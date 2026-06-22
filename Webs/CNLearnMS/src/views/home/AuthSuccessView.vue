<template>
  <div class="auth-success-wrapper d-flex align-items-center justify-content-center min-vh-100 px-3">
    <div class="auth-success-card text-center p-5 rounded-4 border bg-white shadow-lg animate-scale-up">
      <!-- Animated Icon Container -->
      <div class="success-icon-wrapper mx-auto mb-4 d-flex align-items-center justify-content-center rounded-circle">
        <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" class="text-success checkmark-icon">
          <polyline points="20 6 9 17 4 12"></polyline>
        </svg>
      </div>

      <!-- Content -->
      <h1 class="h3 fw-bold text-dark-blue mb-3">Đăng Nhập Thành Công!</h1>
      <p class="text-secondary mb-4 fs-7 leading-relaxed mx-auto" style="max-width: 320px;">
        Phiên đăng nhập của bạn đã được khôi phục thành công. Vui lòng quay trở lại tab trước đó.
      </p>

      <!-- Action Buttons -->
      <div class="d-flex flex-column gap-2.5">
        <button class="btn btn-indigo text-white py-2.5 rounded-3 fw-semibold shadow-sm hover-up w-100" @click="closeTab">
          ✕ Đóng Tab Này
        </button>
        <button class="btn btn-outline-secondary py-2.5 rounded-3 fw-semibold hover-up w-100" @click="goToHome">
          🏠 Về Trang Chủ
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'

const router = useRouter()

const closeTab = () => {
  try {
    window.close()
  } catch (e) {
    message.info('Trình duyệt của bạn chặn tự đóng tab. Bạn có thể tự tắt tab này thủ công.')
  }
}

const goToHome = () => {
  router.push('/')
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@400;500;600;700&display=swap');

.auth-success-wrapper {
  font-family: 'Outfit', sans-serif;
  background: radial-gradient(circle at 10% 20%, rgba(99, 102, 241, 0.05) 0%, rgba(168, 85, 247, 0.05) 90.2%);
}

.auth-success-card {
  width: 100%;
  max-width: 440px;
  border-color: rgba(226, 232, 240, 0.8) !important;
  backdrop-filter: blur(8px);
  background-color: rgba(255, 255, 255, 0.9) !important;
  box-shadow: 0 20px 25px -5px rgba(99, 102, 241, 0.1), 0 10px 10px -5px rgba(99, 102, 241, 0.04) !important;
}

.success-icon-wrapper {
  width: 80px;
  height: 80px;
  background-color: rgba(16, 185, 129, 0.1);
  border: 4px solid rgba(16, 185, 129, 0.15);
  box-shadow: 0 0 20px rgba(16, 185, 129, 0.2);
}

.checkmark-icon {
  animation: stroke 0.4s cubic-bezier(0.65, 0, 0.45, 1) 0.2s forwards;
}

.text-dark-blue {
  color: #1e1b4b;
}

.leading-relaxed {
  line-height: 1.625;
}

.fs-7 {
  font-size: 0.875rem;
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  border: none;
  transition: all 0.25s ease;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(79, 70, 229, 0.2);
}

.btn-outline-secondary {
  border-color: #cbd5e1;
  color: #475569;
  transition: all 0.25s ease;
}

.btn-outline-secondary:hover {
  background-color: #f8fafc;
  border-color: #94a3b8;
  color: #1e293b;
  transform: translateY(-1px);
}

.hover-up {
  transition: all 0.25s ease;
}

/* Animations */
@keyframes scaleUp {
  0% {
    opacity: 0;
    transform: scale(0.92);
  }
  100% {
    opacity: 1;
    transform: scale(1);
  }
}

.animate-scale-up {
  animation: scaleUp 0.45s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

@keyframes stroke {
  0% {
    stroke-dashoffset: 50;
  }
  100% {
    stroke-dashoffset: 0;
  }
}
</style>
