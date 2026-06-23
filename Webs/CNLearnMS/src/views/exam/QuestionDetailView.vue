<template>
  <div class="question-detail-view py-3 container-xl animate-fade-in">
    <!-- Breadcrumb tinh gọn -->
    <nav aria-label="breadcrumb" class="mb-3">
      <ol class="breadcrumb mb-0 fs-8">
        <li class="breadcrumb-item"><router-link to="/" class="text-secondary text-decoration-none">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/search" class="text-secondary text-decoration-none">Tìm kiếm</router-link></li>
        <li class="breadcrumb-item active text-dark" aria-current="page">Chi tiết câu hỏi</li>
      </ol>
    </nav>

    <!-- Header & Action Buttons -->
    <div class="d-flex align-items-center justify-content-between gap-3 mb-4 pb-2 border-bottom border-light">
      <div>
        <h1 class="fs-4 fw-bold text-dark-blue mb-0">Chi tiết câu hỏi</h1>
      </div>
      <div class="d-flex gap-2">
        <button 
          v-if="question"
          class="btn btn-sm px-3 py-1.5 rounded-pill fw-semibold shadow-sm d-flex align-items-center gap-2 transition-all fs-8"
          :class="isQuestionSaved ? 'btn-warning text-white border-warning' : 'btn-outline-warning'"
          @click="toggleSaveQuestionStatus"
        >
          <span>{{ isQuestionSaved ? '★ Đã lưu' : '☆ Lưu câu hỏi' }}</span>
        </button>
        <button class="btn btn-sm btn-outline-secondary px-3 py-1.5 rounded-pill fw-semibold shadow-sm d-flex align-items-center gap-1 shrink-0 fs-8 transition-all" @click="goBack">
          <span>←</span> Quay lại
        </button>
      </div>
    </div>

    <!-- Main Content -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border spinner-border-sm text-indigo" role="status">
        <span class="visually-hidden">Đang tải...</span>
      </div>
    </div>
    
    <div v-else-if="question" class="question-layout">
      
      <!-- 1. Nội dung câu hỏi (Dòng 1) -->
      <div class="question-content-section mb-3">
        <div class="section-label mb-1-5">NỘI DUNG CÂU HỎI</div>
        <div class="question-text  bg-light rounded-3 text-dark-blue fw-semibold leading-relaxed">
          <FormulaRenderer :content="question.stringContent || 'Nội dung câu hỏi trống'" />
        </div>
      </div>

      <!-- 2. Danh mục (Dòng 2) -->
      <div class="meta-row mb-2-5 d-flex align-items-center gap-2" v-if="categoryName">
        <span class="meta-label">Danh mục câu hỏi:</span>
        <span class="badge-soft bg-indigo-soft text-indigo fs-8">{{ categoryName }}</span>
      </div>

      <!-- 3. Người tạo + Nguồn + Ngày cập nhật (Dòng 3) -->
      <div class="meta-row mb-2-5 d-flex flex-wrap align-items-center gap-x-3 gap-y-1 fs-8">
        <span v-if="creatorName" class="d-inline-flex align-items-center">
          <span class="meta-label me-2">Người tạo:</span>
          <span class="text-dark fw-semibold">{{ creatorName }}</span>
        </span>
        <span v-if="sourceName" class="text-muted d-inline-flex align-items-center">
          <span class="mx-2">•</span>
          <span class="meta-label text-muted-label me-2">Nguồn:</span>
          <span class="text-secondary fw-medium">{{ sourceName }}</span>
        </span>
        <span class="text-muted d-inline-flex align-items-center">
          <span class="mx-2">•</span>
          <span>Cập nhật: {{ lastModifiedDate }}</span>
        </span>
      </div>

      <!-- 4. Mức độ (Dòng 4) -->
      <div class="meta-row mb-4 d-flex align-items-center gap-2">
        <span class="meta-label">Mức độ:</span>
        <span class="badge-soft fs-8" :class="getLevelBadgeClass(question.level)">
          {{ getLevelText(question.level) }}
        </span>
      </div>

      <!-- Đường ngăn mỏng trước phần phương án -->
      <div class="border-top border-light pt-3-5 mt-2">
        <!-- Phương án lựa chọn (Render trực tiếp) -->
        <div class="answers-section mb-4">
          <div class="section-label mb-2-5">CÁC PHƯƠNG ÁN LỰA CHỌN</div>
          <div class="row g-2">
            <div 
              v-for="(ans, idx) in question.answers" 
              :key="ans.questionAnswerId" 
              class="col-12"
            >
              <div 
                class="answer-option-card d-flex align-items-start gap-3 rounded-3 transition-all"
                :class="ans.isCorrectAnswer ? 'correct-answer border-success-subtle bg-success-light' : 'border-light-subtle bg-white'"
              >
                <div 
                  class="option-index d-flex align-items-center justify-content-center shrink-0 rounded-circle fw-bold"
                  :class="ans.isCorrectAnswer ? 'bg-success text-white' : 'bg-light text-secondary'"
                >
                  {{ getIndexChar(idx) }}
                </div>
                <div class="option-text pt-0.5 text-dark leading-relaxed">
                  <FormulaRenderer :content="ans.stringContent || ''" :inline="true" />
                </div>
                <span v-if="ans.isCorrectAnswer" class="badge bg-success-soft text-success ms-auto fs-9 py-1 px-2 rounded-2 fw-semibold align-self-center">
                  ✓ Đáp án đúng
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Giải thích chi tiết -->
        <div class="explanation-section mb-4" v-if="question.explaination">
          <div class="section-label mb-2-5">GIẢI THÍCH CHI TIẾT</div>
          <div class="explanation-box p-3 border border-indigo-accent bg-indigo-light-opacity rounded-3 text-secondary leading-relaxed fs-8-5">
            <FormulaRenderer :content="question.explaination" />
          </div>
        </div>
      </div>
    </div>

    <!-- Error state -->
    <div v-else class="text-center py-5">
      <div class="fs-2 mb-2">❓</div>
      <h3 class="fw-bold text-dark-blue fs-5">Không tìm thấy câu hỏi</h3>
      <p class="text-secondary small">Câu hỏi này có thể đã bị xóa hoặc không tồn tại.</p>
      <button class="btn btn-sm btn-indigo px-3 py-1.5 rounded-pill fw-semibold fs-8" @click="goBack">
        Quay lại trang tìm kiếm
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { getQuestionDetails, toggleSaveQuestion, getSavedQuestionIds, getQuestionAnswers, getQuestionKeys } from '@/api/questions'
import { FormulaRenderer } from '@/components/formula-editor'
import type { Question } from '@/models/questions'

const route = useRoute()
const router = useRouter()
const questionId = route.params.id as string

const isQuestionSaved = ref(false)
const categoryName = ref('')
const creatorName = ref('')
const sourceName = ref('')
const lastModifiedDate = ref('')

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
    const nextSavedState = !isQuestionSaved.value
    const res = await toggleSaveQuestion({
      TargetId: questionId,
      IsSaved: nextSavedState
    })
    if (res && res.isSuccess) {
      isQuestionSaved.value = nextSavedState
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

const loading = ref(true)
const question = ref<Question | null>(null)
const correctAnswerIds = ref<string[]>([])

const loadQuestion = async () => {
  loading.value = true
  try {
    const res = await getQuestionDetails(questionId)
    if (res && res.isSuccess && res.data) {
      const qData = res.data
      categoryName.value = qData.questionCategoryName || ''
      creatorName.value = qData.fullName || ''
      sourceName.value = qData.source || ''
      lastModifiedDate.value = qData.modifiedDate ? formatDate(new Date(qData.modifiedDate)) : formatDate(new Date())

      // Gọi tiếp API lấy keys sau khi lấy câu hỏi thành công
      const keysRes = await getQuestionKeys([questionId])

      const answersList = qData.answers || []
      const correctMap = (keysRes && keysRes.isSuccess && keysRes.data?.correctMap) ? keysRes.data.correctMap : {}
      correctAnswerIds.value = correctMap[questionId] || []

      question.value = {
        questionId: qData.questionId,
        questionSlug: qData.questionSlug,
        stringContent: qData.stringContent || 'Nội dung câu hỏi',
        explaination: qData.explaination || 'Chưa có giải thích chi tiết',
        level: qData.level || 0,
        type: qData.type || 0,
        accessType: qData.accessType || 0,
        isInBank: qData.isInBank || false,
        questionCategoryId: qData.questionCategoryId || '',
        attemptCount: qData.attemptCount || 0,
        answers: answersList.map((a: any) => ({
          questionAnswerId: a.questionAnswerId,
          stringContent: a.stringContent,
          isCorrectAnswer: correctAnswerIds.value.includes(a.questionAnswerId),
          questionId: qData.questionId,
          orderInList: a.orderInList || 0
        }))
      }
    } else {
      question.value = null
    }
  } catch (error) {
    console.error('Lỗi khi tải chi tiết câu hỏi:', error)
    question.value = null
  } finally {
    loading.value = false
  }
}

const goBack = () => {
  router.back()
}

const getLevelText = (level: number): string => {
  if (level === 0) return 'Dễ'
  if (level === 1) return 'Trung bình'
  return 'Khó'
}

const getLevelBadgeClass = (level: number): string => {
  if (level === 0) return 'bg-success-soft'
  if (level === 1) return 'bg-warning-soft'
  return 'bg-danger-soft'
}

const getIndexChar = (index: number): string => {
  return String.fromCharCode(65 + index)
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
.bg-indigo { background-color: #4f46e5; }
.text-indigo { color: #4f46e5; }
.bg-indigo-soft { background-color: rgba(99, 102, 241, 0.08); }
.fs-8 { font-size: 0.775rem !important; }
.fs-8-5 { font-size: 0.825rem !important; }
.fs-9 { font-size: 0.7rem !important; }
.shrink-0 { flex-shrink: 0; }
.transition-all { transition: all 0.2s ease-in-out; }

.mb-1-5 { margin-bottom: 6px !important; }
.mb-2-5 { margin-bottom: 10px !important; }
.pt-3-5 { padding-top: 14px !important; }

.meta-row {
  font-size: 0.775rem;
}
.meta-label {
  color: #64748b;
  font-weight: 500;
}
.text-muted-label {
  color: #64748b !important;
}

.badge-soft {
  padding: 4px 10px;
  font-size: 0.75rem;
  font-weight: 600;
  border-radius: 6px;
}
.bg-success-soft {
  background-color: rgba(16, 185, 129, 0.08);
  color: #059669;
}
.bg-warning-soft {
  background-color: rgba(245, 158, 11, 0.08);
  color: #d97706;
}
.bg-danger-soft {
  background-color: rgba(239, 68, 68, 0.08);
  color: #dc2626;
}

.section-label {
  font-size: 0.7rem;
  font-weight: 700;
  letter-spacing: 0.05em;
  color: #94a3b8;
}

.answer-option-card {
  padding: 10px 14px;
  border-width: 1px;
  border-style: solid;
}
.correct-answer {
  border-color: #a7f3d0 !important;
}
.bg-success-light {
  background-color: rgba(16, 185, 129, 0.03) !important;
}
.border-light-subtle {
  border-color: #f1f5f9 !important;
}

.option-index {
  width: 26px;
  height: 26px;
  font-size: 0.825rem;
}

.border-indigo-accent { border-color: rgba(99, 102, 241, 0.2) !important; }
.bg-indigo-light-opacity { background-color: rgba(99, 102, 241, 0.02); }
.leading-relaxed { line-height: 1.625; }

.animate-fade-in {
  animation: fadeIn 0.3s ease-out forwards;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
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
