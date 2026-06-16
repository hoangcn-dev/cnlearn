import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('@/views/home/HomeView.vue')
    },
    {
      path: '/auth-success',
      name: 'auth-success',
      component: () => import('@/views/home/AuthSuccessView.vue')
    },
    {
      path: '/practice',
      name: 'practice',
      component: () => import('@/views/exam/PracticeView.vue')
    },
    {
      path: '/categories',
      redirect: '/practice'
    },
    {
      path: '/category/:id',
      name: 'quiz-list',
      component: () => import('@/views/exam/QuizListView.vue')
    },
    {
      path: '/search',
      name: 'search',
      component: () => import('@/views/search/SearchView.vue')
    },
    {
      path: '/quiz/:id',
      name: 'quiz-detail',
      component: () => import('@/views/exam/QuizDetailView.vue')
    },
    {
      path: '/practice/:id',
      name: 'quiz-practice',
      component: () => import('@/views/exam/QuizPracticeView.vue')
    },
    {
      path: '/question/:id',
      name: 'question-detail',
      component: () => import('@/views/exam/QuestionDetailView.vue')
    },
    {
      path: '/questions',
      redirect: '/personal/questions'
    },
    {
      path: '/exams',
      redirect: '/personal/exams'
    },
    {
      path: '/exams/create',
      redirect: '/personal/exams/create'
    },
    {
      path: '/quiz/room/:id',
      name: 'quiz-room',
      component: () => import('@/views/exam/QuizRoomView.vue')
    },
    {
      path: '/quiz/result/:id',
      name: 'quiz-result',
      component: () => import('@/views/exam/QuizResultView.vue')
    },
    {
      path: '/personal',
      component: () => import('@/views/personal/PersonalLayout.vue'),
      children: [
        {
          path: '',
          redirect: '/personal/profile'
        },
        {
          path: 'profile',
          name: 'personal-profile',
          component: () => import('@/views/personal/PersonalProfileView.vue')
        },
        {
          path: 'activity',
          name: 'personal-activity',
          component: () => import('@/views/personal/ActivityHistoryView.vue')
        },
        {
          path: 'questions',
          name: 'personal-questions',
          component: () => import('@/views/personal/QuestionBankView.vue')
        },
        {
          path: 'questions/create',
          name: 'personal-question-create',
          component: () => import('@/views/personal/QuestionFormView.vue')
        },
        {
          path: 'questions/edit/:id',
          name: 'personal-question-edit',
          component: () => import('@/views/personal/QuestionFormView.vue')
        },
        {
          path: 'exams',
          name: 'personal-exams',
          component: () => import('@/views/personal/ExamManagementView.vue')
        },
        {
          path: 'exams/create',
          name: 'personal-exam-create',
          component: () => import('@/views/personal/ExamFormView.vue')
        },
        {
          path: 'exams/edit/:id',
          name: 'personal-exam-edit',
          component: () => import('@/views/personal/ExamFormView.vue')
        },
        {
          path: 'exams/candidates/:id',
          name: 'personal-exam-candidates',
          component: () => import('@/views/personal/CandidateManagementView.vue')
        },
        {
          path: 'admin',
          name: 'personal-admin',
          component: () => import('@/views/personal/AdminDashboardView.vue'),
          meta: { requiresAdmin: true }
        }
      ]
    },
    {
      path: '/tests',
      name: 'formula-test',
      component: () => import('@/views/test/FormulaTestView.vue')
    }
  ],
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition
    } else {
      return { top: 0 }
    }
  }
})

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAdmin) {
    const saved = localStorage.getItem('cn_user_profile')
    let isAdmin = true // Tạm thời bypass để review giao diện
    if (saved) {
      try {
        const data = JSON.parse(saved)
        // Bypass
      } catch (e) { }
    }
    if (!isAdmin) {
      next('/personal/profile')
      return
    }
  }
  next()
})

export default router
