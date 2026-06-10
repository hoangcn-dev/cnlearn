<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { message } from 'ant-design-vue';
import { getLoginInfo } from '@/api/user';
import { useAuthStore } from '@/stores/auth';
import { getErrorMessage } from '@/api/config/axios';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const loading = ref(true);
const loadingText = ref('Đang xác minh phiên đăng nhập của bạn...');

onMounted(async () => {
  try {
    // Trích xuất return_url từ query - Chỉ sử dụng trường hợp chuẩn, ném ngoại lệ nếu null hoặc trống
    const returnUrl = route.query.return_url as string;
    if (!returnUrl) {
      throw new Error('return_url không được để trống.');
    }

    // Gọi API lấy thông tin đăng nhập hiện tại từ Cookie được gửi kèm tự động qua Wrapper API
    const res = await getLoginInfo();

    if (res.isSuccess && res.data) {
      const info = res.data;
      
      // Kiểm tra và ném ngoại lệ nếu các thông tin bắt buộc bị null hoặc trống (Tuân thủ Rule 1)
      if (!info.userName) {
        throw new Error('Tên tài khoản không được để trống.');
      }
      if (!info.email) {
        throw new Error('Email không được để trống.');
      }
      if (!info.roleName) {
        throw new Error('Vai trò người dùng không được để trống.');
      }
      
      // Lưu thông tin vào reactive Pinia store (tự động đồng bộ dưới localStorage)
      authStore.setLoginSession(info);
      
      loadingText.value = 'Xác minh thành công! Đang chuyển hướng...';
      
      // Đợi hiệu ứng chuyển mượt mà
      setTimeout(() => {
        loading.value = false;
        message.success('Đăng nhập hệ thống thành công.');
        if (returnUrl.startsWith('http://') || returnUrl.startsWith('https://')) {
          window.location.href = returnUrl;
        } else {
          router.push(returnUrl);
        }
      }, 800);
    } else {
      throw new Error(getErrorMessage(res, 'Không thể lấy thông tin đăng nhập.'));
    }
  } catch (error: any) {
    console.error('Lỗi xác thực Callback SSO:', error);
    const errorMsg = getErrorMessage(error, 'Phiên làm việc hết hạn hoặc không hợp lệ!');
    
    message.error(errorMsg);
    loadingText.value = 'Xác thực thất bại. Đang quay lại trang đăng nhập...';
    
    setTimeout(() => {
      loading.value = false;
      const idUrl = import.meta.env.VITE_ID_URL;
      const currentOrigin = window.location.origin;
      
      if (idUrl && currentOrigin !== idUrl) {
        // Trả ngược về trang Login của ID Server
        const fallbackUrl = `${currentOrigin}/auth-callback?return_url=${encodeURIComponent(route.query.return_url as string || '/dashboard')}`;
        window.location.href = `${idUrl}/login?return_url=${encodeURIComponent(fallbackUrl)}`;
      } else {
        router.push('/login');
      }
    }, 1500);
  }
});
</script>

<template>
  <div class="callback-wrapper d-flex align-items-center justify-content-center min-vh-100">
    <div class="callback-card text-center p-5 shadow-lg border rounded-4 bg-white">
      <!-- Glow effect decoration -->
      <div class="glow-bg"></div>

      <!-- Gorgeous Animated Ring Loader -->
      <div class="loader-box mb-4 position-relative d-inline-block">
        <div class="glow-ring"></div>
        <div class="core-spinner"></div>
        <div class="inner-icon">
          <svg viewBox="0 0 24 24" width="32" height="32" class="lock-svg">
            <path fill="currentColor" d="M18 8h-1V6c0-2.76-2.24-5-5-5S7 3.24 7 6v2H6c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2zm-6 9c-1.1 0-2-.9-2-2s.9-2 2-2 2 .9 2 2-.9 2-2 2zm3.1-9H8.9V6c0-1.71 1.39-3.1 3.1-3.1 1.71 0 3.1 1.39 3.1 3.1v2z" />
          </svg>
        </div>
      </div>

      <!-- Title & Dynamic loading text -->
      <h4 class="fw-bold text-dark mb-2">CNAdmin Auth Gateway</h4>
      <p class="text-secondary mb-0 loading-description">{{ loadingText }}</p>
      
      <!-- Progress Bar Micro-animation -->
      <div class="progress-bar-container mt-4 mx-auto rounded-pill overflow-hidden">
        <div class="progress-bar-fill rounded-pill"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&display=swap');

.callback-wrapper {
  font-family: 'Outfit', sans-serif;
  background: radial-gradient(circle at 50% 50%, #f8fafc 0%, #f1f5f9 100%);
  position: relative;
  overflow: hidden;
}

.callback-card {
  max-width: 440px;
  width: 90%;
  border-color: #f1f5f9 !important;
  z-index: 10;
  position: relative;
  overflow: hidden;
  backdrop-filter: blur(10px);
}

.glow-bg {
  position: absolute;
  top: -50%;
  left: -50%;
  width: 200%;
  height: 200%;
  background: radial-gradient(circle, rgba(13, 110, 253, 0.04) 0%, transparent 60%);
  pointer-events: none;
  z-index: 1;
}

/* Gorgeous Loader Box with layered animations */
.loader-box {
  width: 90px;
  height: 90px;
  z-index: 2;
}

.glow-ring {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  border: 4px solid rgba(13, 110, 253, 0.08);
  box-shadow: 0 0 15px rgba(13, 110, 253, 0.1);
}

.core-spinner {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  border: 4px solid transparent;
  border-top-color: #0d6efd;
  border-right-color: #6f42c1;
  animation: spin 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
}

.inner-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: #0d6efd;
  animation: pulse 2s ease-in-out infinite;
}

.lock-svg {
  filter: drop-shadow(0 2px 4px rgba(13, 110, 253, 0.2));
}

.loading-description {
  font-size: 0.95rem;
  letter-spacing: -0.1px;
}

/* Micro-animation for progress bar */
.progress-bar-container {
  width: 160px;
  height: 4px;
  background-color: #e2e8f0;
}

.progress-bar-fill {
  height: 100%;
  width: 40%;
  background: linear-gradient(90deg, #0d6efd 0%, #6f42c1 100%);
  animation: progress-slide 1.5s cubic-bezier(0.4, 0, 0.2, 1) infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes pulse {
  0%, 100% { transform: translate(-50%, -50%) scale(0.95); opacity: 0.8; }
  50% { transform: translate(-50%, -50%) scale(1.05); opacity: 1; }
}

@keyframes progress-slide {
  0% {
    margin-left: -40%;
  }
  100% {
    margin-left: 100%;
  }
}
</style>
