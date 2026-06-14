<template>
  <div class="quiz-list-view py-3">
    <!-- Breadcrumb điều hướng động -->
    <nav aria-label="breadcrumb" class="mb-4">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/categories">Danh mục đề</router-link></li>
        <template v-for="(item, index) in breadcrumbItems" :key="index">
          <li 
            v-if="index === breadcrumbItems.length - 1" 
            class="breadcrumb-item active" 
            aria-current="page"
          >
            {{ item.name }}
          </li>
          <li v-else class="breadcrumb-item">
            <router-link 
              v-if="item.id" 
              :to="{ name: 'quiz-list', params: { id: item.id }, query: { name: item.fullName } }"
            >
              {{ item.name }}
            </router-link>
            <span v-else>{{ item.name }}</span>
          </li>
        </template>
      </ol>
    </nav>

    <!-- Navigation & Search Bar -->
    <div class="d-flex justify-content-between align-items-center flex-wrap gap-3 mb-3">
      <div>
        <button class="btn btn-back-custom d-inline-flex align-items-center gap-2 px-3 py-2 rounded-3 fw-semibold fs-8 bg-white border border-light-subtle shadow-sm hover-up" @click="router.back()">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="feather feather-arrow-left" viewBox="0 0 24 24">
            <line x1="19" y1="12" x2="5" y2="12"></line>
            <polyline points="12 19 5 12 12 5"></polyline>
          </svg>
          <span>Quay lại</span>
        </button>
      </div>
      
      <div class="search-container" style="width: 100%; max-width: 380px;">
        <form @submit.prevent="handleSearchRedirect">
          <div class="input-group search-box rounded-3 overflow-hidden border border-secondary-subtle bg-white shadow-sm">
            <input 
              type="text" 
              v-model="searchQuery" 
              class="form-control border-0 py-2 ps-3 fs-8" 
              placeholder="Tìm đề thi trong danh mục..."
            />
            <button class="btn btn-indigo border-0 px-4 fs-8 fw-semibold" type="submit">
              Tìm Kiếm
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Category Info Card -->
    <div class="card border-0 rounded-4 shadow-sm p-4 bg-white mb-4">
      <h1 class="fs-2 fw-bold text-dark-blue mb-1">{{ getLocalName(categoryName) }}</h1>
      <p class="text-secondary small mb-0">Luyện tập ngay các bộ đề ôn tập chất lượng cao để củng cố kiến thức môn học.</p>
    </div>

    <!-- Loading State -->
    <div v-if="loadingCategories" class="text-center py-5">
      <div class="spinner-border text-indigo spinner-border-sm" role="status"></div>
      <span class="text-secondary ms-2 small">Đang phân tích danh mục chuyên môn...</span>
    </div>

    <div v-else>
      <!-- NẾU LÀ DANH MỤC ẢO (CHỨA CÁC DANH MỤC CON) -->
      <div v-if="!isLeaf" class="category-group-section">
        <div class="mb-3 px-2">
          <span class="text-secondary small fw-bold uppercase-title">📁 Hướng dẫn chuyên đề con</span>
        </div>
        <CategoryChildGrid :children="childCategories" />
      </div>

      <!-- NẾU LÀ DANH MỤC LÁ (CHỨA ĐỀ THI) -->
      <div v-else class="quiz-list-section">
        <div class="mb-3 px-2">
          <span class="text-muted small fw-semibold">Tìm thấy {{ quizzes.length }} đề thi phù hợp</span>
        </div>

        <!-- Quiz List -->
        <div class="row g-3 mb-4">
          <div v-for="quiz in paginatedQuizzes" :key="quiz.id" class="col-12">
            <div class="card border-0 rounded-3 shadow-sm hover-up p-4 d-flex flex-md-row justify-content-between align-items-md-center gap-3 bg-white">
              <div class="d-flex align-items-start gap-3">
                <div class="quiz-icon-box text-center">📝</div>
                <div>
                  <h3 class="h6 fw-bold text-dark-blue mb-2 hover-indigo">{{ quiz.title }}</h3>
                  <div class="d-flex flex-wrap align-items-center gap-3 text-secondary fs-8">
                    <span class="d-flex align-items-center gap-1">
                      <span>⏱️</span>
                      <span>{{ quiz.duration }} phút</span>
                    </span>
                    <span class="d-flex align-items-center gap-1">
                      <span>❓</span>
                      <span>{{ quiz.questionCount }} câu hỏi</span>
                    </span>
                    <span class="d-flex align-items-center gap-1 bg-light px-2 py-0.5 rounded text-dark">
                      <span>📅</span>
                      <span>Đăng tải: {{ getRelativeTime(quiz.createdAt) }}</span>
                    </span>
                  </div>
                </div>
              </div>
              <div class="d-flex align-items-center gap-3 ms-md-auto">
                <button class="btn btn-indigo px-4 py-2 rounded-pill fw-semibold shadow-sm" @click="startQuiz(quiz)">
                  Chi tiết
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty state -->
        <div v-if="paginatedQuizzes.length === 0" class="text-center py-5 bg-white rounded-4 shadow-sm border mb-4">
          <div class="fs-1 mb-3">📭</div>
          <h3 class="fw-bold text-dark-blue">Chưa có đề thi nào trong danh mục</h3>
          <p class="text-secondary small">Vui lòng quay lại danh mục đề thi khác hoặc tạo câu hỏi mới.</p>
        </div>

        <!-- Paging Controls -->
        <nav v-if="totalPages > 1" aria-label="Page navigation" class="mt-4">
          <ul class="pagination justify-content-center gap-2">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
              <a class="page-link rounded-circle d-flex align-items-center justify-content-center" href="#" @click.prevent="goToPage(currentPage - 1)">
                ◀
              </a>
            </li>
            <li v-for="p in totalPages" :key="p" class="page-item" :class="{ active: p === currentPage }">
              <a class="page-link rounded-circle d-flex align-items-center justify-content-center" href="#" @click.prevent="goToPage(p)">
                {{ p }}
              </a>
            </li>
            <li class="page-item" :class="{ disabled: currentPage === totalPages }">
              <a class="page-link rounded-circle d-flex align-items-center justify-content-center" href="#" @click.prevent="goToPage(currentPage + 1)">
                ▶
              </a>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import axios from 'axios'
import { getDirectChildren, type QuestionCategory, CategoryChildGrid } from '@/components/category'
import { getAllCate } from '@/api/categories'
import { getExamsPaging, getExamQuestionCounts } from '@/api/exams'

const route = useRoute()
const router = useRouter()

const categoryId = computed(() => route.params.id as string)
const categoryName = ref<string>('Danh Mục Đề Thi')

const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = ref(5)
const loadingCategories = ref(false)

interface Quiz {
  id: string
  title: string
  duration: number
  questionCount: number
  level: number // 0: Dễ, 1: Trung bình, 2: Khó
  createdAt: Date
}

const quizzes = ref<Quiz[]>([])
const flatCategories = ref<QuestionCategory[]>([])
const currentCategory = ref<QuestionCategory | null>(null)

// Tải danh sách danh mục để phục vụ phân tách cấp bậc
const loadCategories = async () => {
  loadingCategories.value = true
  try {
    const res = await getAllCate()
    if (res.isSuccess && res.data) {
      flatCategories.value = res.data.items || []
    } else {
      throw new Error()
    }
  } catch (error) {
    message.error('Lỗi khi tải danh mục. Vui lòng thử lại sau.')
    flatCategories.value = []
  } finally {
    loadingCategories.value = false
  }

  // Tìm danh mục hiện tại trong list phẳng
  const found = flatCategories.value.find(c => c.questionCategoryId === categoryId.value)
  if (found) {
    currentCategory.value = found
    categoryName.value = found.name
  } else {
    const defaultName = (route.query.name as string) || 'Danh Mục Đề Thi'
    currentCategory.value = {
      questionCategoryId: categoryId.value,
      parentId: null,
      name: defaultName,
      slug: ''
    }
    categoryName.value = defaultName
  }
}

// Kiểm tra xem danh mục hiện tại là danh mục lá (Leaf - chứa đề thi) hay danh mục ảo (Virtual - chứa các con)
const isLeaf = computed(() => {
  if (!currentCategory.value) return true
  const prefix = currentCategory.value.name + ' - '
  return !flatCategories.value.some(c => c.name.startsWith(prefix))
})

// Lấy danh mục con trực tiếp
const childCategories = computed(() => {
  if (!currentCategory.value) return []
  return getDirectChildren(currentCategory.value, flatCategories.value)
})

// Phân tách breadcrumb động theo tên đầy đủ phân cấp
interface BreadcrumbItem {
  name: string
  id: string | null
  fullName: string
}

const breadcrumbItems = computed<BreadcrumbItem[]>(() => {
  if (!currentCategory.value) {
    return [{ name: categoryName.value, id: null, fullName: categoryName.value }]
  }

  const parts = currentCategory.value.name.split(' - ')
  const list: BreadcrumbItem[] = []
  let accumulatedName: string = ''

  for (let i = 0; i < parts.length; i++) {
    const partName = parts[i] || ''
    accumulatedName = accumulatedName ? `${accumulatedName} - ${partName}` : partName
    const match = flatCategories.value.find(c => c.name === accumulatedName)
    
    list.push({
      name: partName,
      id: match ? match.parentId : null,
      fullName: accumulatedName
    })
  }

  return list
})

const getLocalName = (fullName: string) => {
  const parts = fullName.split(' - ')
  return parts[parts.length - 1] || fullName
}

const getRelativeTime = (date: Date): string => {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)

  if (diffMins < 60) {
    return `${diffMins} phút trước`
  } else if (diffHours < 24) {
    return `${diffHours} giờ trước`
  } else {
    return `${diffDays} ngày trước`
  }
}

const fetchExams = async () => {
  try {
    const filters = []
    if (categoryId.value) {
      filters.push({
        property: 'CategoryId',
        value: categoryId.value,
        operator: 0, // Equal
        type: 1 // String
      })
    }

    const [examsRes, countsRes] = await Promise.all([
      getExamsPaging({
        pageIndex: 1,
        pageSize: 500,
        filters: filters
      }),
      getExamQuestionCounts()
    ])

    if (examsRes && examsRes.isSuccess && examsRes.data) {
      const counts = countsRes?.isSuccess ? (countsRes.data || {}) : {}
      
      quizzes.value = (examsRes.data.items || []).map((e: any) => ({
        id: e.examId,
        title: e.name,
        duration: e.duration,
        questionCount: counts[e.examId] || 0,
        level: 1, // Default mapping if no level property exists
        createdAt: new Date(e.createdDate || Date.now())
      }))
    } else {
      quizzes.value = []
    }
  } catch (err) {
    console.error('Lỗi khi tải đề thi theo danh mục:', err)
    message.error('Lỗi khi tải đề thi theo danh mục.')
    quizzes.value = []
  }
}

const handleSearchRedirect = () => {
  router.push({
    name: 'search',
    query: {
      q: searchQuery.value,
      category: categoryId.value
    }
  })
}

const totalPages = computed(() => Math.ceil(quizzes.value.length / itemsPerPage.value))

const paginatedQuizzes = computed(() => {
  const startIdx = (currentPage.value - 1) * itemsPerPage.value
  const endIdx = startIdx + itemsPerPage.value
  return quizzes.value.slice(startIdx, endIdx)
})

const goToPage = (p: number) => {
  if (p < 1 || p > totalPages.value) return
  currentPage.value = p
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const startQuiz = (quiz: Quiz) => {
  router.push({
    name: 'quiz-detail',
    params: { id: quiz.id },
    query: { title: quiz.title }
  })
}

// Thực hiện load lại mỗi khi id category thay đổi trên URL
watch(() => route.params.id, async (newId) => {
  if (newId) {
    currentPage.value = 1
    await loadCategories()
    fetchExams()
  }
})

onMounted(async () => {
  await loadCategories()
  fetchExams()
})
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
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

.hover-indigo:hover {
  color: #4f46e5;
  cursor: pointer;
}

.hover-up {
  transition: all 0.2s ease;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.hover-up:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 15px rgba(0, 0, 0, 0.08) !important;
  border-color: #c7d2fe !important;
}

.page-link {
  width: 40px;
  height: 40px;
  color: #4f46e5;
  border: 1px solid #e5e7eb;
}

.page-item.active .page-link {
  background-color: #4f46e5;
  border-color: #4f46e5;
  color: white;
}

.page-link:hover {
  background-color: #f3f4f6;
  color: #3730a3;
}

.fs-8 {
  font-size: 0.75rem;
}

.btn-back-custom {
  color: #4b5563;
  transition: all 0.2s ease;
}
.btn-back-custom:hover {
  background-color: #f9fafb !important;
  color: #4f46e5;
  border-color: #c7d2fe !important;
}

.uppercase-title {
  text-transform: uppercase;
  letter-spacing: 0.05em;
  font-weight: 700;
}
</style>


<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.12);
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

.hover-indigo:hover {
  color: #4f46e5;
  cursor: pointer;
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

/* Pagination Customizing */
.page-link {
  width: 40px;
  height: 40px;
  color: #4f46e5;
  border: 1px solid #e5e7eb;
}

.page-item.active .page-link {
  background-color: #4f46e5;
  border-color: #4f46e5;
  color: white;
}

.page-link:hover {
  background-color: #f3f4f6;
  color: #3730a3;
}

.fs-7 {
  font-size: 0.825rem;
}
.fs-8 {
  font-size: 0.75rem;
}

.btn-icon-back {
  background: none;
  border: none;
  color: #6b7280;
  padding: 6px;
  border-radius: 50%;
  transition: all 0.22s ease-in-out;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}
.btn-icon-back:hover {
  background-color: rgba(99, 102, 241, 0.08);
  color: #4f46e5;
  transform: translateX(-3px);
}

.btn-back-custom {
  color: #4b5563;
  transition: all 0.2s ease;
}
.btn-back-custom:hover {
  background-color: #f9fafb !important;
  color: #4f46e5;
  border-color: #c7d2fe !important;
}
</style>
