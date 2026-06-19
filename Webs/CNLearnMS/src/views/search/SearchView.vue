<template>
  <div class="search-view py-3">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-4">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/practice">Luyện tập</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Tìm kiếm tổng hợp</li>
      </ol>
    </nav>

    <div class="mb-4">
      <h1 class="fs-2 fw-bold text-dark-blue mb-1">Tìm Kiếm Tổng Hợp</h1>
      <p class="text-secondary small mb-0">Tìm kiếm nhanh chóng, phân loại chi tiết theo đề thi, danh mục chuyên môn và các câu hỏi ôn tập.</p>
    </div>

    <!-- Search & Filter Controls -->
    <div class="card border-0 rounded-4 shadow-sm p-4 bg-white mb-4">
      <div class="row g-3">
        <!-- 1. Từ khóa -->
        <div class="col-lg-4 col-md-6">
          <label class="form-label fw-semibold text-dark-blue small d-block mb-2">Từ khóa tìm kiếm</label>
          <a-input 
            v-model:value="filters.q" 
            placeholder="Nhập từ khóa cần tìm..." 
            allow-clear
            @input="handleSearchInput"
            class="w-100"
          >
            <template #prefix>
              <span class="text-muted me-1">🔍</span>
            </template>
          </a-input>
        </div>

        <!-- 2. Lọc theo danh mục -->
        <div class="col-lg-4 col-md-6">
          <label class="form-label fw-semibold text-dark-blue small d-block mb-2">Lọc theo danh mục đề</label>
          <CategorySelect 
            v-model:value="filters.category" 
            :categories="availableCategories"
            placeholder="Tất cả danh mục..."
            class="w-100"
            show-all-option
            @change="handleFilterChange"
          />
        </div>

        <!-- 3. Tiêu chí sắp xếp -->
        <div class="col-lg-4 col-md-12">
          <label class="form-label fw-semibold text-dark-blue small d-block mb-2">Sắp xếp theo tiêu chí</label>
          <BaseSelect 
            v-model:value="filters.sortBy" 
            :options="sortByOptions"
            class="w-100"
            @change="handleFilterChange"
          />
        </div>
      </div>
      
      <!-- Reset button and count summary -->
      <div class="d-flex justify-content-between align-items-center mt-3 pt-3 border-top border-light">
        <span class="text-secondary small fw-medium">
          Tìm thấy <span class="text-indigo fw-bold">{{ totalItemsCount }}</span> kết quả phù hợp.
        </span>
        <button class="btn btn-link text-indigo text-decoration-none small p-0" @click="resetFilters">
          🔄 Đặt lại bộ lọc
        </button>
      </div>
    </div>

    <!-- 3 Collapsible Groups ("dropdown dạng text") -->
    <div class="search-accordion d-flex flex-column gap-4">
      
      <!-- GROUP 1: Đề Thi -->
      <div class="accordion-group bg-white rounded-4 shadow-sm border p-4">
        <div class="accordion-header d-flex align-items-center justify-content-between hover-pointer pb-3" @click="toggles.quizzes = !toggles.quizzes">
          <div class="d-flex align-items-center gap-2">
            <span class="accordion-arrow fs-5 text-indigo">{{ toggles.quizzes ? '▼' : '▶' }}</span>
            <span class="fs-5 fw-bold text-dark-blue">Đề thi ({{ filteredQuizzes.length }})</span>
          </div>
          <span class="text-muted small fs-8">Nhấp để {{ toggles.quizzes ? 'thu gọn' : 'mở rộng' }}</span>
        </div>
        
        <transition name="collapse">
          <div v-show="toggles.quizzes" class="accordion-content border-top pt-3">
            <div class="row g-3">
              <div v-for="quiz in filteredQuizzes" :key="quiz.id" class="col-12">
                <div class="card border-0 rounded-3 shadow-sm hover-up p-4 d-flex flex-md-row justify-content-between align-items-md-center gap-3 bg-light-card">
                  <div class="d-flex align-items-start gap-3">
                    <div class="quiz-icon-box text-center">📝</div>
                    <div>
                      <div class="d-flex align-items-center gap-2 mb-2 flex-wrap">
                        <h3 class="h6 fw-bold text-dark-blue mb-0">{{ quiz.title }}</h3>
                        <span class="badge bg-indigo-soft text-indigo fs-9 px-2 py-0.5 rounded">
                          {{ quiz.categoryName }}
                        </span>
                      </div>
                      <div class="d-flex flex-wrap align-items-center gap-3 text-secondary fs-8">
                        <span class="d-flex align-items-center gap-1">
                          <span>⏱️</span>
                          <span>{{ quiz.duration }} phút</span>
                        </span>
                        <span class="d-flex align-items-center gap-1">
                          <span>❓</span>
                          <span>{{ quiz.questionCount }} câu hỏi</span>
                        </span>
                        <span class="d-flex align-items-center gap-1 bg-white px-2 py-0.5 rounded text-dark border">
                          <span>📅</span>
                          <span>Đăng tải: {{ getRelativeTime(quiz.createdAt) }}</span>
                        </span>
                      </div>
                    </div>
                  </div>
                  <div class="d-flex align-items-center gap-3 ms-md-auto">
                    <button class="btn btn-indigo px-4 py-2 rounded-pill fw-semibold shadow-sm" @click="startQuiz(quiz)">
                      Chi Tiết
                    </button>
                  </div>
                </div>
              </div>
              
              <!-- Empty state inside quizzes group -->
              <div v-if="filteredQuizzes.length === 0" class="text-center py-4 bg-light rounded-3 text-muted small">
                Không tìm thấy đề thi phù hợp.
              </div>
            </div>
          </div>
        </transition>
      </div>

      <!-- GROUP 2: Danh Mục -->
      <div class="accordion-group bg-white rounded-4 shadow-sm border p-4">
        <div class="accordion-header d-flex align-items-center justify-content-between hover-pointer pb-3" @click="toggles.categories = !toggles.categories">
          <div class="d-flex align-items-center gap-2">
            <span class="accordion-arrow fs-5 text-indigo">{{ toggles.categories ? '▼' : '▶' }}</span>
            <span class="fs-5 fw-bold text-dark-blue">Danh mục ({{ filteredCategories.length }})</span>
          </div>
          <span class="text-muted small fs-8">Nhấp để {{ toggles.categories ? 'thu gọn' : 'mở rộng' }}</span>
        </div>
        
        <transition name="collapse">
          <div v-show="toggles.categories" class="accordion-content border-top pt-3">
            <div class="row g-4">
              <div v-for="category in filteredCategories" :key="category.questionCategoryId" class="col-md-6 col-lg-4">
                <div class="card h-100 border-0 rounded-4 shadow-sm card-glowing overflow-hidden d-flex flex-column bg-light-card" @click="goToCategoryDetail(category)">
                  <div class="p-4 d-flex flex-column h-100 hover-pointer">
                    <div class="d-flex justify-content-between align-items-start mb-3">
                      <span class="category-badge px-2.5 py-1 rounded-3 fs-7 bg-indigo-soft text-indigo fw-semibold">
                        Danh Mục
                      </span>
                      <div class="quiz-count-bubble d-flex align-items-center gap-1">
                        <span>📝</span>
                        <span class="fw-bold">{{ allQuizzes.filter(q => q.categoryId === category.questionCategoryId).length }} đề</span>
                      </div>
                    </div>
                    <h3 class="card-title h5 fw-bold text-dark-blue mb-3 flex-grow-1 line-clamp-2">
                      {{ category.questionCategoryName }}
                    </h3>
                    <div class="d-flex align-items-center justify-content-between text-indigo mt-auto pt-3 border-top border-light">
                      <span class="fs-7 fw-semibold">Xem các đề thi</span>
                      <span>➔</span>
                    </div>
                  </div>
                </div>
              </div>
              
              <!-- Empty state inside categories group -->
              <div v-if="filteredCategories.length === 0" class="text-center py-4 bg-light rounded-3 text-muted small w-100 mx-3">
                Không tìm thấy danh mục nào phù hợp.
              </div>
            </div>
          </div>
        </transition>
      </div>

      <!-- GROUP 3: Câu Hỏi -->
      <div class="accordion-group bg-white rounded-4 shadow-sm border p-4">
        <div class="accordion-header d-flex align-items-center justify-content-between hover-pointer pb-3" @click="toggles.questions = !toggles.questions">
          <div class="d-flex align-items-center gap-2">
            <span class="accordion-arrow fs-5 text-indigo">{{ toggles.questions ? '▼' : '▶' }}</span>
            <span class="fs-5 fw-bold text-dark-blue">Câu hỏi ({{ filteredQuestions.length }})</span>
          </div>
          <span class="text-muted small fs-8">Nhấp để {{ toggles.questions ? 'thu gọn' : 'mở rộng' }}</span>
        </div>
        
        <transition name="collapse">
          <div v-show="toggles.questions" class="accordion-content border-top pt-3">
            <div class="row g-3">
              <div v-for="(question, index) in filteredQuestions" :key="question.questionId" class="col-12">
                <div class="card border-0 rounded-3 shadow-sm p-4 bg-light-card d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-4 hover-up">
                  <!-- Left side: Badge and Question -->
                  <div class="flex-grow-1">
                    <div class="mb-2">
                      <span class="badge bg-indigo-soft text-indigo fw-bold px-2 py-1">Câu hỏi {{ index + 1 }}</span>
                    </div>
                    <div class="question-text fw-semibold text-dark-blue fs-6 mb-0">
                      {{ question.stringContent }}
                    </div>
                  </div>
                  
                  <!-- Right side: Stats and Button -->
                  <div class="d-flex align-items-center gap-4 ms-md-auto mt-3 mt-md-0 flex-shrink-0">
                      Số lượt làm: <span class="fw-bold text-dark-blue">0</span>
                    <button class="btn bg-white border border-light-subtle px-4 py-2 rounded-pill fw-semibold text-dark shadow-sm hover-up" @click="viewQuestionDetail(question.questionId)">
                      Chi Tiết
                    </button>
                  </div>
                </div>
              </div>
              
              <!-- Empty state inside questions group -->
              <div v-if="filteredQuestions.length === 0" class="text-center py-4 bg-light rounded-3 text-muted small">
                Không tìm thấy câu hỏi nào phù hợp.
              </div>
            </div>
          </div>
        </transition>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import axios from 'axios'
import { CategorySelect, getRecursiveChildIds, type QuestionCategory } from '@/components/category'
import BaseSelect, { type SelectOption } from '@/components/BaseSelect.vue'
import { getAllCate } from '@/api/categories'
import { getQuestionsPaging } from '@/api/questions'
import { getExamsPaging, getExamQuestionCounts } from '@/api/exams'
const route = useRoute()
const router = useRouter()

// Collapsible Group Toggles
const toggles = reactive({
  quizzes: true,
  categories: true,
  questions: true
})

interface Quiz {
  id: string
  title: string
  categoryName: string
  categoryId: string
  duration: number
  questionCount: number
  level: number
  createdAt: Date
}

interface Answer {
  questionAnswerId: string
  stringContent: string
  isCorrectAnswer: boolean
}

interface Question {
  questionId: string
  stringContent: string
  explaination: string
  level: number
  createdDate: Date
  answers: Answer[]
}

// State data lists
const availableCategories = ref<QuestionCategory[]>([])
const allQuizzes = ref<Quiz[]>([])
const allQuestions = ref<Question[]>([])

const filters = reactive({
  q: '',
  category: '',
  sortBy: 'newest'
})

const sortByOptions: SelectOption[] = [
  { value: 'newest', label: 'Mới nhất (Ngày đăng tải)', icon: '📅' },
  { value: 'duration-asc', label: 'Thời gian: Tăng dần', icon: '⏱️' },
  { value: 'duration-desc', label: 'Thời gian: Giảm dần', icon: '⏱️' },
  { value: 'questions-asc', label: 'Câu hỏi: Tăng dần', icon: '❓' },
  { value: 'questions-desc', label: 'Câu hỏi: Giảm dần', icon: '❓' }
]

// No mock data

// Utilities
const getRelativeTime = (date: Date): string => {
  if (!date) return 'Vừa xong'
  const parsedDate = typeof date === 'string' ? new Date(date) : date
  const now = new Date()
  const diffMs = now.getTime() - parsedDate.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)

  if (diffMins < 60) {
    return `${Math.max(1, diffMins)} phút trước`
  } else if (diffHours < 24) {
    return `${diffHours} giờ trước`
  } else {
    return `${diffDays} ngày trước`
  }
}

// Load Categories
const loadCategories = async () => {
  try {
    const res = await getAllCate()
    if (res.isSuccess && res.data) {
      availableCategories.value = res.data.items || []
    } else {
      throw new Error()
    }
  } catch (error) {
    message.error('Lỗi tải danh mục từ API.')
    availableCategories.value = []
  }
}

// Load Questions from Backend API or fall back to mock data
const loadQuestions = async () => {
  try {
    const apiFilters = []
    if (filters.category) {
      apiFilters.push({
        property: 'CategoryId',
        operator: 0,
        value: filters.category,
        type: 1
      })
    }

    const res = await getQuestionsPaging({
      page: 1,
      size: 100,
      key: filters.q || undefined,
      filters: apiFilters,
      isPaging: true
    })
    
    if (res && res.isSuccess && res.data && res.data.items) {
      const items = res.data.items
      allQuestions.value = items.map((item: any) => ({
        questionId: item.id,
        stringContent: item.stringContent || 'Câu hỏi trắc nghiệm ôn tập',
        explaination: item.explanation || 'Chưa có giải thích chi tiết cho câu hỏi này.',
        level: item.level || 0,
        createdDate: item.createdDate ? new Date(item.createdDate) : new Date(),
        answers: item.answers || []
      }))
    } else {
      throw new Error()
    }
  } catch (error) {
    console.error('Lỗi tải câu hỏi từ API:', error)
    message.error('Lỗi tải danh sách câu hỏi.')
    allQuestions.value = []
  }
}

// Load Quizzes from Backend API
const loadQuizzes = async () => {
  try {
    const [examsRes, countsRes] = await Promise.all([
      getExamsPaging({
        pageIndex: 1,
        pageSize: 1000,
        filters: []
      }),
      getExamQuestionCounts()
    ])

    if (examsRes && examsRes.isSuccess && examsRes.data) {
      const counts = countsRes?.isSuccess ? (countsRes.data || {}) : {}
      
      allQuizzes.value = (examsRes.data.items || []).map((e: any) => {
        // Tìm category name
        const cat = availableCategories.value.find(c => c.questionCategoryId === e.categoryId)
        return {
          id: e.examId,
          title: e.name,
          categoryName: cat ? cat.questionCategoryName : 'Danh mục chung',
          categoryId: e.categoryId,
          duration: e.duration,
          questionCount: counts[e.examId] || 0,
          level: 1,
          createdAt: new Date(e.createdDate || Date.now())
        }
      })
    } else {
      allQuizzes.value = []
    }
  } catch (error) {
    console.error('Lỗi tải đề thi từ API:', error)
    allQuizzes.value = []
  }
}

// Computed total results
const totalItemsCount = computed(() => {
  return filteredQuizzes.value.length + filteredCategories.value.length + filteredQuestions.value.length
})

// Filtered Lists
const filteredQuizzes = computed(() => {
  let result = [...allQuizzes.value]

  // Filter keyword
  if (filters.q.trim() !== '') {
    const query = filters.q.toLowerCase().trim()
    result = result.filter(q => q.title.toLowerCase().includes(query))
  }

  // Filter category
  if (filters.category !== '') {
    const parentCat = availableCategories.value.find(c => c.questionCategoryId === filters.category)
    if (parentCat) {
      const targetIds = [filters.category, ...getRecursiveChildIds(parentCat, availableCategories.value)]
      result = result.filter(q => targetIds.includes(q.categoryId))
    } else {
      result = result.filter(q => q.categoryId === filters.category)
    }
  }

  // Sort
  if (filters.sortBy === 'newest') {
    result.sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime())
  } else if (filters.sortBy === 'duration-asc') {
    result.sort((a, b) => a.duration - b.duration)
  } else if (filters.sortBy === 'duration-desc') {
    result.sort((a, b) => b.duration - a.duration)
  } else if (filters.sortBy === 'questions-asc') {
    result.sort((a, b) => a.questionCount - b.questionCount)
  } else if (filters.sortBy === 'questions-desc') {
    result.sort((a, b) => b.questionCount - a.questionCount)
  }

  return result
})

const filteredCategories = computed(() => {
  let result = [...availableCategories.value]
  if (filters.q.trim() !== '') {
    const query = filters.q.toLowerCase().trim()
    result = result.filter(c => c.questionCategoryName.toLowerCase().includes(query))
  }
  return result
})

const filteredQuestions = computed(() => {
  let result = [...allQuestions.value]
  if (filters.q.trim() !== '') {
    const query = filters.q.toLowerCase().trim()
    result = result.filter(q => q.stringContent.toLowerCase().includes(query))
  }
  return result
})

let debounceTimer: any = null
const debounce = (fn: Function, delay: number) => {
  return (...args: any[]) => {
    if (debounceTimer) clearTimeout(debounceTimer)
    debounceTimer = setTimeout(() => {
      fn(...args)
    }, delay)
  }
}

const debouncedLoadQuestions = debounce(() => {
  loadQuestions()
}, 300)

const handleSearchInput = () => {
  debouncedLoadQuestions()
}

const handleFilterChange = () => {
  loadQuestions()
}

const resetFilters = () => {
  filters.q = ''
  filters.category = ''
  filters.sortBy = 'newest'
  message.success('Đã đặt lại bộ lọc về mặc định!')
  loadQuestions()
}

const goToCategoryDetail = (category: QuestionCategory) => {
  router.push({
    name: 'quiz-list',
    params: { id: category.questionCategoryId },
    query: { name: category.questionCategoryName }
  })
}

const startQuiz = (quiz: Quiz) => {
  router.push({
    name: 'quiz-detail',
    params: { id: quiz.id },
    query: { title: quiz.title }
  })
}

const viewQuestionDetail = (questionId: string) => {
  router.push({
    name: 'question-detail',
    params: { id: questionId }
  })
}

onMounted(async () => {
  if (route.query.q) {
    filters.q = route.query.q as string
  }
  if (route.query.category) {
    filters.category = route.query.category as string
  }

  await loadCategories()
  await loadQuizzes()
  await loadQuestions()
})
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.12);
}

.bg-success-soft {
  background-color: rgba(16, 185, 129, 0.12);
}

.bg-indigo-light-opacity {
  background-color: rgba(99, 102, 241, 0.04);
}

.border-indigo-accent {
  border: 1px dashed rgba(99, 102, 241, 0.4) !important;
}

.text-indigo {
  color: #4f46e5;
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  color: white;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.2);
}

.quiz-icon-box {
  width: 48px;
  height: 48px;
  background-color: rgba(99, 102, 241, 0.1);
  border-radius: 12px;
  line-height: 48px;
  font-size: 1.5rem;
}

.hover-up {
  transition: all 0.2s ease;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.hover-up:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 15px rgba(0, 0, 0, 0.08) !important;
  border-color: #c7d2fe;
}

.category-badge {
  font-weight: 600;
  letter-spacing: 0.5px;
}

.quiz-count-bubble {
  background-color: #f3f4f6;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 0.8rem;
  color: #4b5563;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.accordion-header {
  user-select: none;
}

.accordion-arrow {
  transition: transform 0.2s ease;
  display: inline-block;
}

.bg-light-card {
  background-color: #f8fafc;
}

/* Collapse Transition */
.collapse-enter-active,
.collapse-leave-active {
  transition: max-height 0.3s ease-out, opacity 0.3s ease-out;
  max-height: 2000px;
  overflow: hidden;
}

.collapse-enter-from,
.collapse-leave-to {
  max-height: 0;
  opacity: 0;
}

.fs-7 {
  font-size: 0.825rem;
}

.fs-8 {
  font-size: 0.75rem;
}

.fs-9 {
  font-size: 0.675rem;
}
</style>
