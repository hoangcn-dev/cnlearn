<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { StopOutlined, LogoutOutlined } from '@ant-design/icons-vue';

const router = useRouter();
const authStore = useAuthStore();

const handleLogout = () => {
  authStore.clearLoginSession();
  router.push('/login');
};
</script>

<template>
  <div class="forbidden-container d-flex align-items-center justify-content-center min-vh-100 px-3">
    <!-- Premium Glassmorphism Card -->
    <div class="glass-card text-center p-5 rounded-4 shadow-lg border border-white border-opacity-10 max-width-md">
      <div class="forbidden-icon-wrapper mb-4">
        <StopOutlined class="forbidden-icon" />
      </div>
      
      <h1 class="gradient-title fw-bold mb-3">403</h1>
      <h3 class="fw-semibold text-white mb-3" style="letter-spacing: -0.5px;">Truy Cập Bị Từ Chối</h3>
      
      <p class="text-secondary mb-4 leading-relaxed" style="font-size: 14px; max-width: 320px; margin: 0 auto 1.5rem;">
        Tài khoản của bạn không quyền truy cập tính năng này. Vui lòng thử lại với tài khoản có quyền hạn cao hơn
      </p>

      <div class="d-flex flex-column gap-2 justify-content-center" style="max-width: 240px; margin: 0 auto;">
        <button class="btn btn-primary d-flex align-items-center justify-content-center gap-2 py-2 px-4 rounded-pill fw-medium transition-all" @click="handleLogout">
          <LogoutOutlined />
          Đăng xuất tài khoản
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.forbidden-container {
  background: radial-gradient(circle at center, #1a1c24 0%, #0d0e12 100%);
  font-family: 'Outfit', 'Inter', sans-serif;
}

.glass-card {
  background: rgba(255, 255, 255, 0.03);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.08);
  max-width: 420px;
  width: 100%;
}

.forbidden-icon-wrapper {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 72px;
  height: 72px;
  border-radius: 50%;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.2);
  margin: 0 auto;
}

.forbidden-icon {
  font-size: 32px;
  color: #ef4444;
  animation: pulse 2s infinite ease-in-out;
}

.gradient-title {
  font-size: 80px;
  line-height: 1;
  background: linear-gradient(135deg, #ff4d4d 0%, #f97316 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  margin-bottom: 0.5rem;
  letter-spacing: -2px;
}

.text-secondary {
  color: #9ca3af !important;
}

.btn-primary {
  background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
  border: none;
  color: white;
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.2);
}

.btn-primary:hover {
  background: linear-gradient(135deg, #f87171 0%, #ef4444 100%);
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(239, 68, 68, 0.3);
}

.btn-primary:active {
  transform: translateY(0);
}

.transition-all {
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.08);
    opacity: 0.8;
  }
}
</style>
