import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { message } from 'ant-design-vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import("@/views/static/LoginView.vue")
    },
    {
      path: '/auth-callback',
      name: 'auth-callback',
      component: () => import("@/views/static/AuthCallbackView.vue")
    },
    {
      path: '/forbidden',
      name: 'forbidden',
      component: () => import("@/views/static/ForbiddenView.vue")
    },
    {
      path: '/',
      name: 'main-layout',
      component: () => import("@/views/layouts/MainLayout.vue"),
      redirect: '/dashboard',
      children: [
        {
          path: 'dashboard',
          name: 'dashboard',
          component: () => import("@/views/DashboardView.vue"),
          meta: { title: 'Bảng Điều Khiển', breadcrumb: 'Dashboard' }
        },
        {
          path: 'users',
          name: 'users',
          component: () => import("@/views/UserManagementView.vue"),
          meta: { title: 'Quản Lý Thành Viên', breadcrumb: 'Quản lý User' }
        },
        {
          path: 'users/:id',
          name: 'user-detail',
          component: () => import("@/views/UserDetailView.vue"),
          meta: { title: 'Chi Tiết Thành Viên', breadcrumb: 'Chi tiết User' }
        },
        {
          path: 'email-templates',
          name: 'email-templates',
          component: () => import("@/views/EmailTemplatesView.vue"),
          meta: { title: 'Quản Lý Email Template', breadcrumb: 'Mẫu Email' }
        },
        {
          path: 'email-templates/edit/:code',
          name: 'email-template-edit',
          component: () => import("@/views/EmailTemplateEditView.vue"),
          meta: { title: 'Chỉnh Sửa Email Template', breadcrumb: 'Chỉnh sửa Template' }
        },
      ]
    }
  ],
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isPublicRoute = to.name === 'login' || to.name === 'auth-callback' || to.name === 'forbidden'

  // Mặc định khi mới vào web admin, sẽ vào trang callback để lấy login info
  if (!authStore.isInitialized && !isPublicRoute) {
    next({
      name: 'auth-callback',
      query: { return_url: to.fullPath }
    })
    return
  }

  const isLoggedIn = authStore.isLoggedIn

  if (!isPublicRoute && !isLoggedIn) {
    next({ name: 'login' })
    return
  }

  if (isLoggedIn) {
    // Chỉ Admin mới được phép vào hệ thống quản lý
    const isAdmin = authStore.user?.roleName === 'Admin'
    
    if (!isAdmin && to.name !== 'forbidden') {
      message.error('Tài khoản không có quyền truy cập hệ thống quản trị!')
      next({ name: 'forbidden' })
      return
    }
    
    if (to.name === 'login' || to.name === 'forbidden') {
      if (isAdmin) {
        if (to.name === 'login' && to.query.mode === 'changepass') {
          next();
          return;
        }
        next({ name: 'dashboard' })
        return
      }
    }
  }

  next()
})

export default router

