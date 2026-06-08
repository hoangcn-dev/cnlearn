<template>
  <div class="search-view py-3">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-4">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
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
          <label class="form-label fw-semibold text-dark-blue small">Từ khóa tìm kiếm</label>
          <div class="input-group">
            <span class="input-group-text bg-light border-end-0 text-muted">🔍</span>
            <input 
              type="text" 
              v-model="filters.q" 
              class="form-control bg-light border-start-0 py-2" 
              placeholder="Nhập từ khóa cần tìm..." 
              @input="handleFilterChange"
            />
          </div>
        </div>

        <!-- 2. Lọc theo danh mục -->
        <div class="col-lg-4 col-md-6">
          <label class="form-label fw-semibold text-dark-blue small d-block">Lọc theo danh mục đề</label>
          <CategorySelect 
            v-model:value="filters.category" 
            :categories="availableCategories"
            placeholder="Tất cả danh mục..."
            class="w-100"
            @change="handleFilterChange"
          />
        </div>

        <!-- 3. Tiêu chí sắp xếp -->
        <div class="col-lg-4 col-md-12">
          <label class="form-label fw-semibold text-dark-blue small">Sắp xếp theo tiêu chí</label>
          <select 
            v-model="filters.sortBy" 
            class="form-select bg-light py-2" 
            @change="handleFilterChange"
          >
            <option value="newest">Mới nhất (Ngày đăng tải)</option>
            <option value="duration-asc">Thời gian: Tăng dần ⏱️</option>
            <option value="duration-desc">Thời gian: Giảm dần ⏱️</option>
            <option value="questions-asc">Câu hỏi: Tăng dần ❓</option>
            <option value="questions-desc">Câu hỏi: Giảm dần ❓</option>
          </select>
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
                      <span class="category-badge px-2.5 py-1 rounded-3 fs-7" :style="getBadgeStyle(category.name)">
                        {{ getSubjectCategory(category.name) }}
                      </span>
                      <div class="quiz-count-bubble d-flex align-items-center gap-1">
                        <span>📝</span>
                        <span class="fw-bold">{{ getQuizCount(category.questionCategoryId) }} đề</span>
                      </div>
                    </div>
                    <h3 class="card-title h5 fw-bold text-dark-blue mb-3 flex-grow-1 line-clamp-2">
                      {{ category.name }}
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
                    <span class="text-muted small text-nowrap">
                      Số lượt làm: <span class="fw-bold text-dark-blue">{{ getAttemptsCount(question.questionId).toLocaleString('vi-VN') }}</span>
                    </span>
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

// Mock categories
const MOCK_CATEGORIES: QuestionCategory[] = [
  { questionCategoryId: "c01a92a2-a69f-4143-8589-da11688d7d01", slug: "toan-hoc", name: "Toán Học" },
  { questionCategoryId: "c02a92a2-a69f-4143-8589-da11688d7d02", slug: "toan-hoc-luyen-thi-thpt-quoc-gia", name: "Toán Học - Luyện Thi THPT Quốc Gia" },
  { questionCategoryId: "c03a92a2-a69f-4143-8589-da11688d7d03", slug: "vat-ly", name: "Vật Lý" },
  { questionCategoryId: "c04a92a2-a69f-4143-8589-da11688d7d04", slug: "vat-ly-chuyen-de-dong-dien-xoay-chieu", name: "Vật Lý - Chuyên Đề Dòng Điện Xoay Chiều" },
  { questionCategoryId: "c05a92a2-a69f-4143-8589-da11688d7d05", slug: "hoa-hoc-chuyen-de-hoa-huu-co", name: "Hóa Học - Chuyên Đề Hóa Hữu Cơ" },
  { questionCategoryId: "c06a92a2-a69f-4143-8589-da11688d7d06", slug: "tieng-anh-reading", name: "Tiếng Anh - IELTS Reading Academic" }
]

// Mock questions for Tab 3
const MOCK_QUESTIONS: Question[] = [
  {
    questionId: "q1",
    stringContent: "Điểm khác biệt lớn nhất giữa IEnumerable và IQueryable là gì?",
    explaination: "IEnumerable thực hiện lọc dữ liệu trên Client (In-Memory), còn IQueryable thực hiện lọc phía Server (Database) trước khi kéo về Client.",
    level: 1,
    createdDate: new Date(),
    answers: [
      { questionAnswerId: "a1_1", stringContent: "IEnumerable lọc ở Client, IQueryable lọc ở Server Database", isCorrectAnswer: true },
      { questionAnswerId: "a1_2", stringContent: "IEnumerable lọc ở Server, IQueryable lọc ở Client", isCorrectAnswer: false }
    ]
  },
  {
    questionId: "q2",
    stringContent: "Cú pháp khai báo biến hằng trong C# là gì?",
    explaination: "Biến hằng trong C# được khai báo bằng từ khóa const đứng trước kiểu dữ liệu và gán giá trị trực tiếp.",
    level: 0,
    createdDate: new Date(Date.now() - 3600000),
    answers: [
      { questionAnswerId: "a2_1", stringContent: "const <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;", isCorrectAnswer: true },
      { questionAnswerId: "a2_2", stringContent: "readonly <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;", isCorrectAnswer: false },
      { questionAnswerId: "a2_3", stringContent: "static <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;", isCorrectAnswer: false }
    ]
  },
  {
    questionId: "q3",
    stringContent: "Phương thức nào dùng để khởi tạo một Transaction trong DbContext của EF Core?",
    explaination: "Phương thức Database.BeginTransaction() khởi tạo một giao dịch mới để đảm bảo tính toàn vẹn dữ liệu.",
    level: 2,
    createdDate: new Date(Date.now() - 7200000),
    answers: [
      { questionAnswerId: "a3_1", stringContent: "Database.BeginTransaction()", isCorrectAnswer: true },
      { questionAnswerId: "a3_2", stringContent: "Database.StartTransaction()", isCorrectAnswer: false },
      { questionAnswerId: "a3_3", stringContent: "Database.Commit()", isCorrectAnswer: false }
    ]
  },
  {
    questionId: "q4",
    stringContent: "Một lớp (Class) kế thừa trong C# có thể có tối đa bao nhiêu lớp cơ sở?",
    explaination: "C# chỉ hỗ trợ đơn kế thừa lớp (Single Inheritance), nghĩa là một lớp con chỉ được phép kế thừa từ duy nhất 1 lớp cha.",
    level: 0,
    createdDate: new Date(Date.now() - 86400000),
    answers: [
      { questionAnswerId: "a4_1", stringContent: "1", isCorrectAnswer: true },
      { questionAnswerId: "a4_2", stringContent: "Không giới hạn", isCorrectAnswer: false },
      { questionAnswerId: "a4_3", stringContent: "2", isCorrectAnswer: false }
    ]
  }
]

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

const getQuizCount = (id: string): number => {
  const hash = id.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
  return (hash % 12) + 4
}

const getSubjectCategory = (name: string): string => {
  const lowercase = name.toLowerCase()
  if (lowercase.includes("toán")) return "Toán học"
  if (lowercase.includes("lý")) return "Vật lý"
  if (lowercase.includes("hóa")) return "Hóa học"
  if (lowercase.includes("anh")) return "Tiếng anh"
  if (lowercase.includes("sử")) return "Lịch sử"
  if (lowercase.includes("sinh")) return "Sinh học"
  if (lowercase.includes("tin")) return "Tin học"
  if (lowercase.includes("địa")) return "Địa lý"
  return "Chuyên đề"
}

const getBadgeStyle = (name: string) => {
  const subject = getSubjectCategory(name)
  switch (subject) {
    case "Toán học": return { backgroundColor: '#e0f2fe', color: '#0369a1' }
    case "Vật lý": return { backgroundColor: '#fef3c7', color: '#b45309' }
    case "Hóa học": return { backgroundColor: '#dcfce7', color: '#15803d' }
    case "Tiếng anh": return { backgroundColor: '#f3e8ff', color: '#6b21a8' }
    case "Lịch sử": return { backgroundColor: '#fee2e2', color: '#b91c1c' }
    case "Sinh học": return { backgroundColor: '#e2f0d9', color: '#385723' }
    case "Tin học": return { backgroundColor: '#e0f7fa', color: '#006064' }
    case "Địa lý": return { backgroundColor: '#fff3e0', color: '#e65100' }
    default: return { backgroundColor: '#f3f4f6', color: '#374151' }
  }
}

const getLevelText = (level: number): string => {
  if (level === 0) return 'Dễ'
  if (level === 1) return 'Trung Bính'
  return 'Khó'
}

const getLevelBadgeClass = (level: number): string => {
  if (level === 0) return 'bg-success text-white'
  if (level === 1) return 'bg-warning text-dark'
  return 'bg-danger text-white'
}

const getAttemptsCount = (id: string) => {
  const hash = id.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
  return 100 + (hash * 13) % 5000
}

// Load Categories
const loadCategories = async () => {
  try {
    const response = await axios.post('http://localhost:5000/api/questioncategories/paging', {
      page: 1,
      size: 100,
      search: "",
      filters: []
    })
    if (response.data && response.data.isSuccess && response.data.data) {
      availableCategories.value = response.data.data.items || []
    } else {
      throw new Error()
    }
  } catch {
    availableCategories.value = MOCK_CATEGORIES
  }
}

// Load Questions from Backend API or fall back to mock data
const loadQuestions = async () => {
  try {
    const response = await axios.post('http://localhost:5000/api/questions/paging', {
      page: 1,
      size: 100,
      search: filters.q,
      filters: []
    })
    
    if (response.data && response.data.isSuccess && response.data.data && response.data.data.items) {
      const items = response.data.data.items
      const list: Question[] = []
      for (const item of items) {
        let answersList: Answer[] = []
        try {
          const ansResponse = await axios.get(`http://localhost:5000/api/questionsanswers/question/${item.questionId}`)
          if (ansResponse.data && ansResponse.data.isSuccess) {
            answersList = ansResponse.data.data || []
          }
        } catch {
          answersList = [
            { questionAnswerId: `${item.questionId}_a1`, stringContent: 'Đáp án mẫu A (Đúng)', isCorrectAnswer: true },
            { questionAnswerId: `${item.questionId}_a2`, stringContent: 'Đáp án mẫu B (Sai)', isCorrectAnswer: false }
          ]
        }

        list.push({
          questionId: item.questionId,
          stringContent: item.stringContent || 'Câu hỏi trắc nghiệm ôn tập',
          explaination: item.explaination || 'Chưa có giải thích chi tiết cho câu hỏi này.',
          level: item.level || 0,
          createdDate: item.createdDate ? new Date(item.createdDate) : new Date(),
          answers: answersList
        })
      }
      
      allQuestions.value = list
    } else {
      throw new Error()
    }
  } catch {
    allQuestions.value = MOCK_QUESTIONS
  }
}

// Generate Quizzes pool
const generateQuizPool = () => {
  const list: Quiz[] = []
  availableCategories.value.forEach((cat, index) => {
    const quizCount = 5 + (index % 4)
    for (let i = 1; i <= quizCount; i++) {
      const createdAt = new Date()
      createdAt.setHours(createdAt.getHours() - (index * 12 + i * 4))

      list.push({
        id: `${cat.questionCategoryId}-quiz-${i}`,
        title: `Đề thi thử ${cat.name} - Đề số ${i} (Khảo sát chất lượng hè)`,
        categoryName: cat.name,
        categoryId: cat.questionCategoryId,
        duration: (i % 2 === 0) ? 45 : 90,
        questionCount: (i % 2 === 0) ? 30 : 50,
        level: i % 3,
        createdAt
      })
    }
  })
  allQuizzes.value = list
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
    result = result.filter(c => c.name.toLowerCase().includes(query))
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
    query: { name: category.name }
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
  generateQuizPool()
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
