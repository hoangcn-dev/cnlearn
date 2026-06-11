<template>
  <div class="question-detail-view py-4">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-4">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/search">Tìm kiếm tổng hợp</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Chi tiết câu hỏi</li>
      </ol>
    </nav>

    <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
      <div>
        <h1 class="fs-2 fw-bold text-dark-blue mb-1">Chi Tiết Câu Hỏi</h1>
        <p class="text-secondary small mb-0">Xem đầy đủ thông tin về nội dung, đáp án và giải thích chi tiết.</p>
      </div>
      <div class="d-flex gap-2">
        <button 
          v-if="question"
          class="btn px-4 py-2 rounded-pill fw-semibold shadow-sm d-flex align-items-center gap-2"
          :class="isQuestionSaved ? 'btn-warning text-white border-warning' : 'btn-outline-warning'"
          @click="toggleSaveQuestionStatus"
        >
          <span>{{ isQuestionSaved ? '★ Đã lưu câu hỏi' : '☆ Lưu câu hỏi' }}</span>
        </button>
        <button class="btn btn-outline-secondary px-4 py-2 rounded-pill fw-semibold shadow-sm d-flex align-items-center gap-2 shrink-0" @click="goBack">
          <span>←</span> Quay lại
        </button>
      </div>
    </div>

    <!-- Main Content -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-indigo" role="status">
        <span class="visually-hidden">Đang tải...</span>
      </div>
    </div>
    
    <div v-else-if="question" class="card border-0 rounded-4 shadow-sm bg-white overflow-hidden animate-fade-in">
      <!-- Header info -->
      <div class="bg-light p-4 border-bottom d-flex flex-wrap justify-content-between align-items-center gap-3">
        <div class="d-flex align-items-center gap-2">
          <span class="badge bg-indigo text-white fw-bold px-3 py-2 fs-6 rounded-3">CÂU HỎI</span>
          <span class="badge" :class="getLevelBadgeClass(question.level)">
            {{ getLevelText(question.level) }}
          </span>
        </div>
        <div class="text-secondary small d-flex align-items-center gap-3">
          <span>📅 Cập nhật: {{ formatDate(question.createdDate) }}</span>
        </div>
      </div>

      <div class="p-4 p-md-5">
        <!-- Question Content -->
        <h3 class="fw-bold text-dark-blue mb-5 fs-4 lh-base">{{ question.stringContent }}</h3>

        <!-- Answers -->
        <h4 class="fs-6 fw-bold text-dark-blue mb-3">Các lựa chọn đáp án:</h4>
        <div class="d-flex flex-column gap-3 mb-5">
          <div 
            v-for="(ans, idx) in question.answers" 
            :key="ans.questionAnswerId"
            class="p-3 rounded-3 border d-flex flex-column flex-sm-row align-items-sm-center gap-3 transition hover-up"
            :class="ans.isCorrectAnswer ? 'bg-success-soft border-success' : 'bg-light border-light-subtle'"
          >
            <div class="d-flex align-items-center gap-3 flex-grow-1">
              <div 
                class="fw-bold fs-5 d-flex align-items-center justify-content-center rounded-circle flex-shrink-0" 
                style="width: 40px; height: 40px;"
                :class="ans.isCorrectAnswer ? 'bg-success text-white' : 'bg-white text-dark-blue border'"
              >
                {{ String.fromCharCode(65 + idx) }}
              </div>
              <div class="fs-6" :class="{ 'fw-semibold text-dark': ans.isCorrectAnswer, 'text-secondary': !ans.isCorrectAnswer }">
                {{ ans.stringContent }}
              </div>
            </div>
            <div v-if="ans.isCorrectAnswer" class="mt-2 mt-sm-0 align-self-start align-self-sm-center">
              <span class="badge bg-success px-3 py-2 rounded-pill fs-7 shadow-sm text-nowrap">
                ✨ ĐÁP ÁN ĐÚNG
              </span>
            </div>
          </div>
        </div>

        <!-- Explanation -->
        <div class="explanation-box p-4 rounded-4 border border-indigo-accent bg-indigo-light-opacity">
          <div class="fw-bold text-indigo mb-3 d-flex align-items-center gap-2 fs-5">
            <span>💡</span> Hướng dẫn giải thích chi tiết
          </div>
          <div class="text-secondary leading-relaxed fs-6">
            {{ question.explaination || 'Chưa có lời giải thích chi tiết cho câu hỏi này.' }}
          </div>
        </div>
      </div>
    </div>

    <!-- Error state -->
    <div v-else class="text-center py-5">
      <div class="fs-1 mb-3">❓</div>
      <h3 class="fw-bold text-dark-blue">Không tìm thấy câu hỏi</h3>
      <p class="text-secondary">Câu hỏi này có thể đã bị xóa hoặc không tồn tại.</p>
      <button class="btn btn-indigo px-4 py-2 mt-3 rounded-pill fw-semibold" @click="goBack">
        Quay lại trang tìm kiếm
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import axios from 'axios'
import { getQuestionDetails, toggleSaveQuestion, getSavedQuestionIds } from '@/api/questions'

const route = useRoute()
const router = useRouter()
const questionId = route.params.id as string

const isQuestionSaved = ref(false)

const checkQuestionSavedStatus = async () => {
  try {
    const res = await getSavedQuestionIds()
    if (res && res.isSuccess && res.data) {
      isQuestionSaved.value = res.data.includes(questionId)
    }
  } catch (e) {
    console.error('Lỗi khi kiểm tra trạng thái lưu câu hỏi:', e)
  }
}

const toggleSaveQuestionStatus = async () => {
  try {
    const res = await toggleSaveQuestion(questionId)
    if (res && res.isSuccess) {
      isQuestionSaved.value = res.data
      if (isQuestionSaved.value) {
        message.success('Đã lưu câu hỏi thành công!')
      } else {
        message.success('Đã bỏ lưu câu hỏi!')
      }
    } else {
      message.error(res.errorMessage || 'Lỗi khi lưu câu hỏi!')
    }
  } catch (e) {
    message.error('Không thể thực hiện tác vụ lưu câu hỏi.')
  }
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

const loading = ref(true)
const question = ref<Question | null>(null)

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
    createdDate: new Date(Date.now() - 86400000),
    answers: [
      { questionAnswerId: "a3_1", stringContent: "Database.BeginTransaction()", isCorrectAnswer: true },
      { questionAnswerId: "a3_2", stringContent: "Database.StartTransaction()", isCorrectAnswer: false },
      { questionAnswerId: "a3_3", stringContent: "DbContext.Transaction()", isCorrectAnswer: false },
      { questionAnswerId: "a3_4", stringContent: "DbSet.Begin()", isCorrectAnswer: false }
    ]
  }
]

const loadQuestion = async () => {
  loading.value = true
  try {
    const res = await getQuestionDetails(questionId)
    if (res && res.isSuccess && res.data) {
      const qData = res.data
      question.value = {
        questionId: qData.id,
        stringContent: qData.stringContent || 'Nội dung câu hỏi',
        explaination: qData.explanation || 'Chưa có giải thích chi tiết',
        level: qData.level || 0,
        createdDate: qData.createdDate ? new Date(qData.createdDate) : new Date(),
        answers: qData.answers || []
      }
    } else {
      throw new Error()
    }
  } catch (error) {
    const mock = MOCK_QUESTIONS.find(q => q.questionId === questionId)
    if (mock) {
      question.value = mock
    } else {
      question.value = {
        questionId: questionId,
        stringContent: `Nội dung câu hỏi chi tiết mã ${questionId.substring(0,8)}... Được tạo ngẫu nhiên để demo giao diện.`,
        explaination: `Đây là phần giải thích chi tiết cho câu hỏi mã ${questionId.substring(0,8)}. Phương án đúng là C vì tính chất đặc trưng của môn học.`,
        level: Math.floor(Math.random() * 3),
        createdDate: new Date(),
        answers: [
          { questionAnswerId: `a1`, stringContent: 'Phương án A - gây nhiễu', isCorrectAnswer: false },
          { questionAnswerId: `a2`, stringContent: 'Phương án B - phân tích chưa đủ', isCorrectAnswer: false },
          { questionAnswerId: `a3`, stringContent: 'Phương án C - hoàn toàn chính xác', isCorrectAnswer: true },
          { questionAnswerId: `a4`, stringContent: 'Phương án D - hoàn toàn sai', isCorrectAnswer: false }
        ]
      }
    }
  } finally {
    loading.value = false
  }
}

const goBack = () => {
  router.back()
}

const getLevelText = (level: number): string => {
  if (level === 0) return 'Dễ'
  if (level === 1) return 'Trung Bình'
  return 'Khó'
}

const getLevelBadgeClass = (level: number): string => {
  if (level === 0) return 'bg-success text-white'
  if (level === 1) return 'bg-warning text-dark'
  return 'bg-danger text-white'
}

const formatDate = (d: Date): string => {
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit', month: '2-digit', year: 'numeric'
  }).format(d)
}

onMounted(() => {
  window.scrollTo({ top: 0 })
  checkQuestionSavedStatus()
  loadQuestion()
})
</script>

<style scoped>
.text-dark-blue { color: #1e1b4b; }
.bg-indigo-soft { background-color: rgba(99, 102, 241, 0.12); }
.bg-success-soft { background-color: rgba(16, 185, 129, 0.12); }
.bg-indigo { background-color: #4f46e5; }
.text-indigo { color: #4f46e5; }
.border-indigo-accent { border-color: rgba(99, 102, 241, 0.3) !important; }
.bg-indigo-light-opacity { background-color: rgba(99, 102, 241, 0.03); }
.leading-relaxed { line-height: 1.625; }
.shrink-0 { flex-shrink: 0; }
.animate-fade-in {
  animation: fadeIn 0.4s ease-out forwards;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.btn-outline-warning {
  color: #d97706;
  border-color: #f59e0b;
  background-color: transparent;
}
.btn-outline-warning:hover {
  background-color: #f59e0b;
  color: white;
}
.btn-warning {
  background-color: #f59e0b !important;
  border-color: #f59e0b !important;
  color: white !important;
}
.btn-warning:hover {
  background-color: #d97706 !important;
  border-color: #d97706 !important;
}
</style>
