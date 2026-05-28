<script lang="ts" setup>
import { ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { UserOutlined, SettingOutlined, DashboardOutlined, MenuUnfoldOutlined, MenuFoldOutlined, MailOutlined } from '@ant-design/icons-vue';

const router = useRouter();
const route = useRoute();
const isCollapsed = ref(false);
const emit = defineEmits(['sizechanged']);

const toggleCollapse = () => {
    isCollapsed.value = !isCollapsed.value;
    emit('sizechanged', isCollapsed.value ? 80 : 240);
};
</script>

<template>
  <div class="sidebar-container d-flex flex-column h-100 shadow-sm">
    <div class="brand d-flex align-items-center justify-content-between p-3 border-bottom bg-white">
      <div v-if="!isCollapsed" class="d-flex align-items-center gap-2">
        <div class="brand-logo bg-primary text-white rounded-3 d-flex align-items-center justify-content-center">CN</div>
        <span class="fs-5 fw-bold text-gradient">CNAdmin</span>
      </div>
      <button class="btn btn-sm btn-link text-secondary p-0 ms-auto" @click="toggleCollapse">
        <MenuFoldOutlined v-if="!isCollapsed" class="fs-5" />
        <MenuUnfoldOutlined v-else class="fs-5" />
      </button>
    </div>

    <div class="flex-grow-1 overflow-auto bg-white py-3">
      <a-menu
        mode="inline"
        :inline-collapsed="isCollapsed"
        :selectedKeys="[(route.name as string) || 'dashboard']"
        class="border-0"
      >
        <a-menu-item key="dashboard" class="menu-item-custom" @click="router.push('/dashboard')">
          <template #icon><DashboardOutlined /></template>
          <span>Dashboard</span>
        </a-menu-item>
        
        <a-menu-item key="users" class="menu-item-custom" @click="router.push('/users')">
          <template #icon><UserOutlined /></template>
          <span>Quản lý User</span>
        </a-menu-item>

        <a-menu-item key="email-templates" class="menu-item-custom" @click="router.push('/email-templates')">
          <template #icon><MailOutlined /></template>
          <span>Quản lý Email Template</span>
        </a-menu-item>

        <a-menu-item key="settings" class="menu-item-custom">
          <template #icon><SettingOutlined /></template>
          <span>Cấu hình (Demo)</span>
        </a-menu-item>
      </a-menu>
    </div>
  </div>
</template>

<style scoped>
.sidebar-container {
  background-color: #ffffff;
  border-right: 1px solid rgba(0,0,0,0.06);
}
.brand {
  height: 64px;
}
.brand-logo {
  width: 32px;
  height: 32px;
  font-weight: 700;
  font-size: 14px;
  background: linear-gradient(135deg, #0d6efd 0%, #6f42c1 100%) !important;
}
.text-gradient {
  background: linear-gradient(135deg, #0d6efd 0%, #6f42c1 100%);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}
.menu-item-custom {
  margin-bottom: 8px !important;
  border-radius: 8px !important;
  width: calc(100% - 16px) !important;
  margin-left: 8px !important;
}
</style>