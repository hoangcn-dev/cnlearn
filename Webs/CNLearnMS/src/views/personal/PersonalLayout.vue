<template>
  <div class="personal-layout d-flex gap-0 pt-0 pb-0 px-0 mx-0 w-100 align-items-stretch">
    <!-- Left Sidebar -->
    <div class="personal-sidebar">
      <div class="card p-3 bg-white sidebar-card d-flex flex-column">
        <!-- Sidebar profile teaser -->
        <div class="d-flex align-items-center gap-3 p-2 mb-3 border-bottom pb-3 flex-shrink-0">
          <div class="profile-avatar shadow-sm">HV</div>
          <div class="overflow-hidden">
            <h4 class="fs-6 fw-bold text-dark-blue mb-0 text-truncate">{{ userName }}</h4>
            <span class="badge bg-indigo-soft text-indigo fs-8">{{ isAdmin ? 'Quản trị viên' : 'Học viên' }}</span>
          </div>
        </div>
        
        <!-- Navigation Menu -->
        <div class="sidebar-menu flex-grow-1">
          <ul class="nav flex-column gap-1">
            <li class="nav-item">
              <router-link to="/personal/profile" class="nav-link px-3 py-2 rounded-3 d-flex align-items-center gap-2 sidebar-item" active-class="active-item">
                <span class="d-flex align-items-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="sidebar-icon"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>
                </span>
                <span class="label fw-semibold">Tài khoản</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/personal/activity" class="nav-link px-3 py-2 rounded-3 d-flex align-items-center gap-2 sidebar-item" active-class="active-item">
                <span class="d-flex align-items-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="sidebar-icon"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
                </span>
                <span class="label fw-semibold">Lịch sử hoạt động</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/personal/questions" class="nav-link px-3 py-2 rounded-3 d-flex align-items-center gap-2 sidebar-item" active-class="active-item">
                <span class="d-flex align-items-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="sidebar-icon"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path></svg>
                </span>
                <span class="label fw-semibold">Ngân hàng câu hỏi</span>
              </router-link>
            </li>
            <li class="nav-item">
              <router-link to="/personal/exams" class="nav-link px-3 py-2 rounded-3 d-flex align-items-center gap-2 sidebar-item" active-class="active-item">
                <span class="d-flex align-items-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="sidebar-icon"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
                </span>
                <span class="label fw-semibold">Đề thi của tôi</span>
              </router-link>
            </li>
            <li v-if="isAdmin" class="nav-item">
              <router-link to="/personal/admin" class="nav-link px-3 py-2 rounded-3 d-flex align-items-center gap-2 sidebar-item" active-class="active-item">
                <span class="d-flex align-items-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="sidebar-icon"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>
                </span>
                <span class="label fw-semibold">Quản trị hệ thống</span>
              </router-link>
            </li>
          </ul>
        </div>
        
        <!-- Sidebar Footer Action -->
        <div class="mt-auto border-top pt-2 flex-shrink-0">
          <a href="#" @click.prevent="logout" class="nav-link px-3 py-2 rounded-3 text-danger d-flex align-items-center gap-2 logout-btn">
            <span class="d-flex align-items-center">
              <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="sidebar-icon"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path><polyline points="16 17 21 12 16 7"></polyline><line x1="21" y1="12" x2="9" y2="12"></line></svg>
            </span>
            <span class="label fw-semibold">Đăng xuất</span>
          </a>
        </div>
      </div>
    </div>
    
    <!-- Right Content Area -->
    <div class="personal-content flex-grow-1 overflow-hidden">
      <router-view v-slot="{ Component }">
        <transition name="slide-fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'

const router = useRouter()
const userName = ref('Học Viên Cao Cấp')
const isAdmin = ref(true)

onMounted(() => {
  const saved = localStorage.getItem('cn_user_profile')
  if (saved) {
    try {
      const data = JSON.parse(saved)
      if (data.name) {
        userName.value = data.name
      }
      isAdmin.value = data.role === 'admin' || data.email?.includes('admin') || data.isAdmin === true
    } catch (e) {
      // Ignore
    }
  }
})

const logout = () => {
  message.success('Đăng xuất thành công!')
  router.push('/')
}
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.1);
}

.sidebar-icon {
  color: #64748b;
  transition: color 0.2s ease;
}

.active-item .sidebar-icon {
  color: #6366f1;
}

.logout-btn .sidebar-icon {
  color: #dc2626;
}

.profile-avatar {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background-color: #6366f1;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 1.1rem;
  flex-shrink: 0;
}

.personal-sidebar {
  width: 220px;
  flex-shrink: 0;
  height: 100% !important;
}

.sidebar-card {
  height: 100% !important;
  display: flex;
  flex-direction: column;
  border-radius: 0 !important;
  border-top: none !important;
  border-left: none !important;
  border-bottom: none !important;
  border-right: 1px solid rgba(0, 0, 0, 0.08) !important;
  box-shadow: none !important;
}

.sidebar-menu {
  flex-grow: 1;
  overflow-y: auto;
  margin-bottom: 0.5rem;
}

.sidebar-menu::-webkit-scrollbar {
  width: 4px;
}

.sidebar-menu::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 4px;
}

.sidebar-item {
  color: #475569 !important;
  transition: all 0.2s ease;
  border: 1px solid transparent;
}

.sidebar-item:hover {
  background-color: #f8fafc;
  color: #1e293b !important;
}

.active-item {
  background-color: rgba(99, 102, 241, 0.08) !important;
  color: #6366f1 !important;
  border-color: rgba(99, 102, 241, 0.1);
}

.logout-btn {
  transition: all 0.2s ease;
}

.logout-btn:hover {
  background-color: rgba(239, 68, 68, 0.05);
  color: #dc2626 !important;
}

/* Sub-page sliding transition */
.slide-fade-enter-active,
.slide-fade-leave-active {
  transition: all 0.25s ease;
}

.slide-fade-enter-from {
  opacity: 0;
  transform: translateX(15px);
}

.slide-fade-leave-to {
  opacity: 0;
  transform: translateX(-15px);
}

.personal-layout {
  font-size: 0.85rem !important;
  height: calc(100vh - 58px) !important;
  overflow: hidden !important;
}

.personal-content {
  height: 100% !important;
  overflow-y: auto !important;
  padding: 24px !important;
  background-color: #f8fafc;
}

.personal-layout :deep(.custom-table),
.personal-layout :deep(.ant-table),
.personal-layout :deep(.ant-form),
.personal-layout :deep(.btn),
.personal-layout :deep(input),
.personal-layout :deep(select),
.personal-layout :deep(.ant-select),
.personal-layout :deep(textarea),
.personal-layout :deep(.badge) {
  font-size: 0.85rem !important;
}

.personal-layout :deep(.ant-table-thead > tr > th),
.personal-layout :deep(.ant-table-tbody > tr > td) {
  font-size: 0.83rem !important;
  padding: 6px 10px !important;
}

.personal-layout :deep(.card) {
  border: 1px solid rgba(0, 0, 0, 0.08) !important;
  box-shadow: 0 4px 12px -2px rgba(0, 0, 0, 0.03), 0 2px 6px -1px rgba(0, 0, 0, 0.02) !important;
}

.personal-layout :deep(h1), 
.personal-layout :deep(.fs-2) {
  font-size: 1.35rem !important;
}

.personal-layout :deep(h2), 
.personal-layout :deep(.fs-3) {
  font-size: 1.2rem !important;
}

.personal-layout :deep(h3), 
.personal-layout :deep(.fs-4) {
  font-size: 1.05rem !important;
}

.personal-layout :deep(h5) {
  font-size: 0.9rem !important;
}
</style>
