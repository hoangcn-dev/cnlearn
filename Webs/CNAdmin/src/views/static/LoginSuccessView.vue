<script setup lang="ts">
import { CheckCircleOutlined } from '@ant-design/icons-vue';

const closeTab = () => {
  window.close();
};
</script>

<template>
  <div class="success-wrapper d-flex justify-content-center align-items-center min-vh-100">
    <div class="success-container text-center">
      <div class="success-card shadow p-5 bg-white rounded-4 border text-center">
        <!-- Success Icon -->
        <div class="icon-wrapper mb-4">
          <CheckCircleOutlined class="success-icon" />
        </div>

        <!-- Title -->
        <h2 class="fw-bold text-dark mb-3">Xác Thực Thành Công!</h2>

        <!-- Message -->
        <p class="text-secondary mb-4 description">
          Bạn đã đăng nhập hệ thống thành công.<br />
          Vui lòng <strong>quay lại tab cũ</strong> để tiếp tục thực hiện hành động của bạn.
        </p>

        <!-- Close button -->
        <a-button type="primary" size="large" @click="closeTab" class="close-btn fw-semibold px-4 rounded-3">
          Đóng Tab Này
        </a-button>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&display=swap');

.success-wrapper {
  font-family: 'Outfit', sans-serif;
  background: radial-gradient(circle at top right, rgba(34, 197, 94, 0.05) 0%, rgba(248, 250, 252, 1) 70%);
  min-height: 100vh;
  width: 100%;
  padding: 20px;
}

.success-container {
  width: 100%;
  max-width: 480px;
}

.success-card {
  border-color: rgba(34, 197, 94, 0.15) !important;
  box-shadow: 0 10px 25px -5px rgba(34, 197, 94, 0.05), 0 8px 16px -6px rgba(0, 0, 0, 0.02) !important;
  border-radius: 20px !important;
  transition: all 0.3s ease;
}

.success-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 20px 35px -5px rgba(34, 197, 94, 0.08), 0 12px 20px -8px rgba(0, 0, 0, 0.03) !important;
}

.icon-wrapper {
  display: inline-flex;
  justify-content: center;
  align-items: center;
  width: 80px;
  height: 80px;
  background-color: rgba(34, 197, 94, 0.08);
  border-radius: 50%;
  animation: pulse 2s infinite;
}

.success-icon {
  font-size: 44px;
  color: #22c55e;
}

.description {
  font-size: 1.05rem;
  line-height: 1.6;
}

.close-btn {
  height: 46px;
  background-color: #22c55e;
  border-color: #22c55e;
  transition: all 0.2s ease;
}

.close-btn:hover,
.close-btn:focus {
  background-color: #16a34a;
  border-color: #16a34a;
  box-shadow: 0 4px 12px rgba(34, 197, 94, 0.3);
}

@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(34, 197, 94, 0.4);
  }
  70% {
    box-shadow: 0 0 0 12px rgba(34, 197, 94, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(34, 197, 94, 0);
  }
}
</style>
