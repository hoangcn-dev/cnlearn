<template>
  <div class="app-container">
    <!-- Premium Header Navbar -->
    <header v-if="route.name !== 'quiz-practice' && route.name !== 'quiz-room'" class="navbar navbar-expand-lg navbar-dark bg-dark px-4 shadow-sm border-bottom border-secondary sticky-top" style="z-index: 999;">
      <div class="container-fluid">
        <router-link to="/" class="navbar-brand d-flex align-items-center gap-2">
          <span class="fs-3 fw-bold text-gradient">CN<span class="text-indigo">LearnMS</span></span>
        </router-link>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarText" aria-controls="navbarText" aria-expanded="false" aria-label="Toggle navigation">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="navbarText">
          <ul class="navbar-nav me-auto mb-2 mb-lg-0 gap-3 ms-4">
            <li class="nav-item">
              <router-link to="/" class="nav-link px-3 py-2 rounded" active-class="nav-active" exact>Trang chủ</router-link>
            </li>
            <li class="nav-item">
              <router-link to="/practice" class="nav-link px-3 py-2 rounded" active-class="nav-active">Luyện tập</router-link>
            </li>
            <li class="nav-item">
              <a href="#" class="nav-link px-3 py-2 rounded">Khóa học</a>
            </li>
            <li class="nav-item">
              <a href="#" class="nav-link px-3 py-2 rounded">Tài liệu</a>
            </li>
          </ul>
          <div class="d-flex align-items-center gap-3">
            <span class="text-white-50 small">Chào mừng, Học Viên</span>
            <a-dropdown :trigger="['click']" placement="bottomRight">
              <div class="user-avatar text-center">HV</div>
              <template #overlay>
                <a-menu class="custom-dropdown-menu">
                  <a-menu-item key="profile" @click="goTo('personal-profile')">
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="me-2 text-indigo"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>
                    Tài khoản
                  </a-menu-item>
                  <a-menu-item key="questions" @click="goTo('personal-questions')">
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="me-2 text-indigo"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path></svg>
                    Ngân hàng câu hỏi
                  </a-menu-item>
                  <a-menu-item key="exams" @click="goTo('personal-exams')">
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="me-2 text-indigo"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
                    Đề thi của tôi
                  </a-menu-item>
                  <a-menu-divider />
                  <a-menu-item key="logout" @click="handleLogout" class="text-danger-menu">
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="me-2"><path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path><polyline points="16 17 21 12 16 7"></polyline><line x1="21" y1="12" x2="9" y2="12"></line></svg>
                    Đăng xuất
                  </a-menu-item>
                </a-menu>
              </template>
            </a-dropdown>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content Body -->
    <main class="content-body" :class="route.path.startsWith('/personal') ? 'container-fluid px-0 py-0' : 'container py-4'">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </router-view>
    </main>

    <!-- Premium Footer -->
    <footer v-if="!route.path.startsWith('/personal') && route.name !== 'quiz-practice' && route.name !== 'quiz-room'" class="footer py-4 mt-auto border-top bg-white text-center text-secondary shadow-sm">
      <div class="container">
        <p class="mb-1">© 2026 CNLearnMS. Thiết kế mang phong cách hiện đại, trực quan & hiệu năng cao.</p>
        <p class="small text-muted">Hệ thống quản lý học tập và luyện trắc nghiệm tối ưu nhất.</p>
      </div>
    </footer>

    <!-- Floating Back to Top Button -->
    <button 
      class="btn btn-floating-top d-flex align-items-center justify-content-center shadow-lg"
      :class="{ 'show': showScrollTop }"
      @click="scrollToTop"
      title="Cuộn lên đầu trang"
    >
      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="19" x2="12" y2="5"></line><polyline points="5 12 12 5 19 12"></polyline></svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { message } from 'ant-design-vue'

const router = useRouter()
const route = useRoute()
const showScrollTop = ref(false)

const checkScroll = (event: Event) => {
  const target = event.target as HTMLElement | Document
  let scrollY = 0
  if (target === document || (target as any) === window) {
    scrollY = window.scrollY || document.documentElement.scrollTop
  } else if (target instanceof HTMLElement) {
    scrollY = target.scrollTop
  }
  showScrollTop.value = scrollY > 300
}

const scrollToTop = () => {
  const personalContent = document.querySelector('.personal-content')
  if (personalContent && personalContent.scrollTop > 0) {
    personalContent.scrollTo({ top: 0, behavior: 'smooth' })
  } else {
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }
}

const goTo = (name: string) => {
  router.push({ name })
}

const handleLogout = () => {
  message.success('Đăng xuất tài khoản thành công!')
  router.push('/')
}

onMounted(() => {
  window.addEventListener('scroll', checkScroll, true)
})

onUnmounted(() => {
  window.removeEventListener('scroll', checkScroll, true)
})
</script>

<style scoped>
.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.text-gradient {
  background: linear-gradient(to right, #6366f1, #a855f7);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}

.text-indigo {
  color: #818cf8 !important;
  -webkit-text-fill-color: #818cf8 !important;
}

.nav-link {
  transition: all 0.2s ease;
  font-weight: 500;
  color: rgba(255, 255, 255, 0.75) !important;
}

.nav-link:hover {
  background-color: rgba(255, 255, 255, 0.1);
  color: #fff !important;
}

.nav-active {
  background-color: rgba(99, 102, 241, 0.2) !important;
  color: #818cf8 !important;
}

.user-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background-color: #6366f1;
  color: white;
  line-height: 40px;
  font-weight: 600;
  box-shadow: 0 0 10px rgba(99, 102, 241, 0.3);
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  flex-shrink: 0;
}

.user-avatar:hover {
  transform: scale(1.05);
  box-shadow: 0 0 14px rgba(99, 102, 241, 0.5);
}

:deep(.text-danger-menu) {
  color: #ef4444 !important;
}

:deep(.text-danger-menu):hover {
  background-color: #fef2f2 !important;
}

.content-body {
  flex: 1;
}

/* Page transitions */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.25s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Floating Bottom Button */
.btn-floating-top {
  position: fixed;
  bottom: 30px;
  right: 30px;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background-color: #6366f1;
  color: white;
  border: none;
  opacity: 0;
  visibility: hidden;
  transform: translateY(20px);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 1050;
  cursor: pointer;
}

.btn-floating-top.show {
  opacity: 1;
  visibility: visible;
  transform: translateY(0);
}

.btn-floating-top:hover {
  background-color: #4f46e5;
  color: white;
  transform: translateY(-5px) scale(1.05);
  box-shadow: 0 10px 15px -3px rgba(99, 102, 241, 0.4), 0 4px 6px -2px rgba(99, 102, 241, 0.2) !important;
}
</style>
