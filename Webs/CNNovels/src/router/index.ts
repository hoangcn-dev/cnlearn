import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import StoryDetailView from '../views/StoryDetailView.vue'
import ChapterReaderView from '../views/ChapterReaderView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/truyen/:id',
      name: 'story-detail',
      component: StoryDetailView,
    },
    {
      path: '/truyen/:id/chuong-:chapterId',
      name: 'chapter-reader',
      component: ChapterReaderView,
    }
  ],
})

export default router
