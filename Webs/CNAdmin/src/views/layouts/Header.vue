<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { BellOutlined, UserOutlined, KeyOutlined, LogoutOutlined } from '@ant-design/icons-vue';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const username = computed(() => {
  return authStore.user?.displayName || authStore.user?.userName || '';
});

const roleName = computed(() => {
  return authStore.user?.roleName || '';
});

const pageTitle = computed(() => {
  return (route.meta?.title as string) || 'Hệ Thống';
});

const breadcrumbText = computed(() => {
  return (route.meta?.breadcrumb as string) || 'Trang Chủ';
});

const handleLogout = () => {
  authStore.clearLoginSession();
  const idUrl = import.meta.env.VITE_ID_URL;
  const currentOrigin = window.location.origin;
  if (idUrl && currentOrigin !== idUrl) {
    window.location.href = `${idUrl}/auth`;
  } else {
    router.push('/auth');
  }
};

const showChangePasswordModal = () => {
  router.push('/auth?mode=changepass');
};
</script>

<template>
  <header class="header-container d-flex align-items-center justify-content-between px-4 bg-white shadow-sm border-bottom">
    <!-- Dynamic Page Title & Breadcrumbs in Header (Saves vertical space) -->
    <div class="d-flex flex-column text-start justify-content-center">
      <h6 class="fw-bold mb-0 text-dark" style="font-size: 16px; letter-spacing: -0.2px;">{{ pageTitle }}</h6>
      <div class="text-secondary" style="font-size: 11px;">
        <span class="text-primary-light cursor-pointer" @click="router.push('/')">CNAdmin</span> 
        <span class="mx-1">/</span> 
        <span>{{ breadcrumbText }}</span>
      </div>
    </div>

    <div class="d-flex align-items-center gap-3">
      <!-- Notification -->
      <div class="icon-badge-box p-2 bg-light rounded-circle cursor-pointer border position-relative">
        <BellOutlined class="fs-5 text-secondary" />
        <span class="position-absolute top-0 start-100 translate-middle p-1 bg-danger border border-light rounded-circle"></span>
      </div>

      <!-- User Dropdown Info -->
      <a-dropdown :trigger="['click']">
        <div class="user-profile-btn d-flex align-items-center gap-2 p-1 pe-3 bg-light rounded-pill border cursor-pointer">
          <div class="avatar-box bg-primary text-white rounded-circle d-flex align-items-center justify-content-center fw-bold text-uppercase">
            {{ username.substring(0, 2) }}
          </div>
          <div class="user-meta d-none d-sm-block text-start">
            <div class="fw-semibold text-dark small" style="line-height: 1.2;">{{ username }}</div>
            <div class="text-secondary small-extra">{{ roleName }}</div>
          </div>
        </div>
        <template #overlay>
          <a-menu>
            <a-menu-item key="0" class="py-2">
              <span class="d-flex align-items-center gap-2 text-dark small">
                <UserOutlined /> Hồ sơ cá nhân
              </span>
            </a-menu-item>
            <a-menu-item key="1" class="py-2" @click="showChangePasswordModal">
              <span class="d-flex align-items-center gap-2 text-dark small">
                <KeyOutlined /> Đổi mật khẩu
              </span>
            </a-menu-item>
            <a-menu-divider />
            <a-menu-item key="3" @click="handleLogout" class="py-2 text-danger">
              <span class="d-flex align-items-center gap-2 small">
                <LogoutOutlined /> Đăng xuất
              </span>
            </a-menu-item>
          </a-menu>
        </template>
      </a-dropdown>
    </div>
  </header>
</template>

<style scoped>
.header-container {
  height: 64px;
}
.icon-badge-box {
  width: 38px;
  height: 38px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}
.icon-badge-box:hover {
  background-color: #e9ecef !important;
}
.user-profile-btn {
  height: 38px;
  transition: all 0.2s ease;
}
.user-profile-btn:hover {
  background-color: #e9ecef !important;
}
.avatar-box {
  width: 30px;
  height: 30px;
  font-size: 12px;
  background: linear-gradient(135deg, #0d6efd 0%, #6f42c1 100%) !important;
}
.text-primary-light {
  color: #0d6efd;
}
.small-extra {
  font-size: 10px;
}
.cursor-pointer {
  cursor: pointer;
}
</style>