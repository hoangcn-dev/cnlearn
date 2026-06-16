<template>
  <div class="quiz-practice-view py-3">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-4">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/categories">Danh mục đề</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Luyện tập</li>
      </ol>
    </nav>

    <!-- STAGE 2 HEADER (Chỉ hiện khi đang làm bài) -->
    <div v-if="isStarted && !isSubmitted" class="mb-4">
      <h1 class="fs-3 fw-bold text-dark-blue mb-1">Luyện Tập: {{ quizTitle }}</h1>
    </div>

    <!-- STAGE 3: RESULTS PANEL -->
    <div v-if="isSubmitted" class="row justify-content-center animate-fade-in">
      <div class="col-lg-8">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-5 position-relative overflow-hidden mb-4">
          <!-- Fireworks / Confetti Animation -->
          <div class="confetti-animation-wrapper d-flex justify-content-center mb-4">
            <div class="confetti-container position-relative text-center">
              <span class="confetti-icon">🏆</span>
              <div class="particle p1">🎉</div>
              <div class="particle p2">✨</div>
              <div class="particle p3">🌟</div>
              <div class="particle p4">🎊</div>
              <div class="particle p5">💥</div>
              <div class="particle p6">🎉</div>
            </div>
          </div>

          <div class="text-center">
            <h2 class="fw-bold text-dark-blue mb-1">Hoàn Thành {{ isExamMode ? 'Bài Thi Thử' : 'Bài Luyện Tập' }}!</h2>
            <p class="text-secondary small mb-4">Kết quả {{ isExamMode ? 'thi thử' : 'luyện tập' }} của bạn đã được ghi nhận trên hệ thống</p>

            <div class="d-inline-block bg-indigo-light-opacity border border-indigo-accent rounded-4 px-5 py-4 mb-5">
              <div class="fs-1 fw-bold text-indigo leading-none mb-2">{{ finalScore }} <span class="fs-6 text-secondary fw-normal">/ 10 điểm</span></div>
              <div class="text-secondary fw-medium small">Đúng {{ correctCount }} trên tổng số {{ questions.length }} câu hỏi</div>
            </div>

            <div class="d-flex justify-content-center gap-3 mb-5">
              <button class="btn btn-outline-indigo px-4 py-2.5 rounded-pill fw-semibold d-flex align-items-center gap-2" @click="shareResult">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="18" cy="5" r="3"/><circle cx="6" cy="12" r="3"/><circle cx="18" cy="19" r="3"/><line x1="8.59" y1="13.51" x2="15.42" y2="17.49"/><line x1="15.41" y1="6.51" x2="8.59" y2="10.49"/></svg>
                Chia sẻ kết quả
              </button>
              <button class="btn btn-indigo px-4 py-2.5 rounded-pill fw-semibold d-flex align-items-center gap-2" @click="backToDetail">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="19" y1="12" x2="5" y2="12"/><polyline points="12 19 5 12 12 5"/></svg>
                Quay lại đề thi
              </button>
            </div>
          </div>

          <!-- Recommended Quizzes -->
          <div class="border-top border-light pt-5">
            <h3 class="h6 fw-bold text-dark-blue mb-4 d-flex align-items-center gap-2">
              <span>📚</span> Đề thi gợi ý dành cho bạn:
            </h3>
            <div class="row g-3">
              <div v-for="rec in recommendedQuizzes" :key="rec.id" class="col-md-4">
                <div class="card border border-light-subtle rounded-3 p-3 h-100 hover-up bg-light-hover hover-pointer transition" @click="viewQuizDetail(rec.id, rec.title)">
                  <div class="d-flex align-items-start gap-2 mb-2">
                    <span class="fs-5">📝</span>
                    <h4 class="fs-7 fw-bold text-dark-blue mb-0 text-line-clamp">{{ rec.title }}</h4>
                  </div>
                  <div class="d-flex align-items-center gap-3 text-secondary fs-8 mt-auto pt-2">
                    <span class="d-flex align-items-center gap-1">⏱️ {{ rec.duration }} phút</span>
                    <span class="d-flex align-items-center gap-1">❓ {{ rec.questionCount }} câu</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- STAGE 1: CONFIGURATION PANEL (Cấu hình luyện tập) -->
    <div v-else-if="!isStarted" class="row g-4 justify-content-center">
      <!-- Cột trái: Cấu hình luyện tập -->
      <div class="col-lg-7">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4 h-100">
          <div class="mb-4 pb-3 border-bottom border-light">
            <h1 class="fs-3 fw-bold text-dark-blue mb-1">Luyện Tập: {{ quizTitle }}</h1>
            <p class="text-secondary small mb-0">Thiết lập cấu hình ôn tập phù hợp với phương pháp học tập của bạn.</p>
          </div>
          
          <!-- <div class="d-flex align-items-center gap-2-5 mb-4">
            <span class="fs-4">⚙️</span>
            <h2 class="h5 fw-bold text-dark-blue mb-0">Tùy Chọn Cấu Hình</h2>
          </div> -->

          <div class="row g-3">
            <!-- 1. Chế độ làm bài (Làm theo đề / Làm theo từng câu) -->
            <div class="col-md-12 mb-2">
              <label class="form-label fw-bold text-dark-blue small">Chế độ làm bài</label>
              <select v-model="config.mode" class="form-select bg-light py-2.5">
                <option value="all">Làm toàn bộ đề (Một trang)</option>
                <option value="single">Làm theo từng câu (Chuyển tiếp)</option>
              </select>
              <div class="text-muted fs-8 mt-1.5">
                {{ config.mode === 'all' ? '💡 Hiển thị toàn bộ câu hỏi trên cùng 1 trang, cuộn để làm.' : '💡 Hiển thị mỗi lần 1 câu hỏi, có nút chuyển câu.' }}
              </div>
            </div>

            <!-- 2. Toggles -->
            <div class="col-md-6 d-flex flex-column gap-3">
              <!-- Xáo trộn đáp án -->
              <div class="form-check form-switch d-flex justify-content-between align-items-center ps-0">
                <div>
                  <label class="form-check-label fw-semibold text-dark-blue small d-block" for="shuffleAnswers">Xáo trộn đáp án</label>
                  <span class="text-secondary fs-8">Thay đổi thứ tự A, B, C, D của đáp án.</span>
                </div>
                <input class="form-check-input ms-3" type="checkbox" id="shuffleAnswers" v-model="config.shuffleAnswers">
              </div>

              <!-- Xáo trộn câu hỏi -->
              <div class="form-check form-switch d-flex justify-content-between align-items-center ps-0">
                <div>
                  <label class="form-check-label fw-semibold text-dark-blue small d-block" for="shuffleQuestions">Xáo trộn câu hỏi</label>
                  <span class="text-secondary fs-8">Thay đổi thứ tự câu hỏi ngẫu nhiên.</span>
                </div>
                <input class="form-check-input ms-3" type="checkbox" id="shuffleQuestions" v-model="config.shuffleQuestions">
              </div>
            </div>

            <div class="col-md-6 d-flex flex-column gap-3">
              <!-- Hiện kết quả ngay -->
              <div class="form-check form-switch d-flex justify-content-between align-items-center ps-0">
                <div>
                  <label class="form-check-label fw-semibold text-dark-blue small d-block" for="showResultInstant">Hiện kết quả ngay</label>
                  <span class="text-secondary fs-8">Hiển thị đúng/sai & giải thích ngay sau khi chọn.</span>
                  <span v-if="config.mode === 'all'" class="text-danger fs-9 d-block mt-1">⚠️ Không áp dụng khi làm toàn bộ đề.</span>
                </div>
                <input class="form-check-input ms-3" type="checkbox" id="showResultInstant" v-model="config.showResultInstant" :disabled="config.mode === 'all'">
              </div>

              <!-- Tự động chuyển câu sau n giây -->
              <div class="d-flex flex-column gap-1">
                <label class="fw-semibold text-dark-blue small">Tự động chuyển câu sau</label>
                <select v-model="config.autoNextSeconds" class="form-select bg-light py-2 text-indigo fw-medium" :disabled="config.mode !== 'single'">
                  <option :value="0">Tắt tự động chuyển</option>
                  <option :value="2">Chuyển sau 2 giây</option>
                  <option :value="3">Chuyển sau 3 giây</option>
                  <option :value="5">Chuyển sau 5 giây</option>
                  <option :value="10">Chuyển sau 10 giây</option>
                </select>
                <span class="text-muted fs-8">
                  {{ config.mode !== 'single' ? '⚠️ Chỉ áp dụng cho chế độ làm bài từng câu.' : '⚡ Chuyển sang câu tiếp theo sau khi chọn đáp án.' }}
                </span>
              </div>
            </div>
          </div>

          <!-- Start Practice Button -->
          <div class="text-center mt-3">
            <button class="btn btn-indigo px-5 py-3 rounded-3 fw-bold shadow hover-up text-white w-100 fs-6" @click="startPractice">
              ⚡ BẮT ĐẦU LUYỆN TẬP
            </button>
          </div>
        </div>
      </div>

      <!-- Cột phải: Hướng dẫn phím tắt -->
      <div class="col-lg-5">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4 h-100 animate-fade-in">
          <div class="d-flex align-items-center gap-2 mb-3 pb-3 border-bottom border-light">
            <span class="fs-4">⌨️</span>
            <h3 class="h5 fw-bold text-dark-blue mb-0">Hướng Dẫn Phím Tắt</h3>
          </div>
          <p class="text-secondary small mb-4">Sử dụng bàn phím giúp bạn làm bài trắc nghiệm nhanh hơn gấp 3 lần và tiết kiệm thời gian đáng kể:</p>
          <div class="d-flex flex-column gap-3">
            <div class="d-flex align-items-center gap-3 p-3 rounded-3 bg-light">
              <span class="badge bg-dark text-white px-2 py-1 rounded font-monospace fw-bold fs-7">← / A</span>
              <span class="text-secondary fs-8">Quay lại câu hỏi trước</span>
            </div>
            <div class="d-flex align-items-center gap-3 p-3 rounded-3 bg-light">
              <span class="badge bg-dark text-white px-2 py-1 rounded font-monospace fw-bold fs-7">→ / D</span>
              <span class="text-secondary fs-8">Chuyển sang câu hỏi kế tiếp</span>
            </div>
            <div class="d-flex align-items-center gap-3 p-3 rounded-3 bg-light">
              <span class="badge bg-dark text-white px-2 py-1 rounded font-monospace fw-bold fs-7">1, 2, 3, 4</span>
              <span class="text-secondary fs-8">Chọn đáp án A, B, C, D tương ứng</span>
            </div>
            <div class="d-flex align-items-center gap-3 p-3 rounded-3 bg-light">
              <span class="badge bg-success text-white px-2 py-1 rounded font-monospace fw-bold fs-7">Enter</span>
              <span class="text-secondary fs-8">Xác nhận Nộp Bài nhanh</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- STAGE 2: LIVE PRACTICE WORKSPACE (Làm bài) -->
    <div v-else class="row">
      <!-- Sidebar Navigation for all mode or info panel -->
      <div class="col-lg-3 col-md-4 mb-4 order-md-2">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4 sticky-sidebar">
          <!-- Progress bar -->
          <div class="mb-3">
            <div class="d-flex justify-content-between small text-secondary mb-1">
              <span>Đã làm:</span>
              <span class="fw-bold">{{ answeredCount }}/{{ questions.length }} câu</span>
            </div>
            <div class="progress rounded-pill" style="height: 8px;">
              <div class="progress-bar bg-indigo rounded-pill" role="progressbar" :style="{ width: `${progressPercentage}%` }" aria-valuenow="progressPercentage" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
          </div>

          <!-- Exam Timer -->
          <div v-if="isExamMode && !isSubmitted" class="mb-4 p-3 bg-danger-soft text-danger rounded-3 text-center border border-danger-subtle shadow-sm animate-fade-in">
            <div class="fs-8 fw-semibold mb-1">⏰ Thời gian còn lại</div>
            <div class="fs-2 fw-bold font-monospace tracking-wider">{{ formattedExamTime }}</div>
          </div>

          <!-- Grid Navigation to questions -->
          <div class="questions-navigation pt-2">
            <span class="fw-semibold text-dark-blue small d-block mb-2">Bản đồ câu hỏi</span>
            <div class="nav-questions-grid">
              <button 
                v-for="(q, idx) in questions" 
                :key="q.id" 
                class="btn nav-q-btn d-flex align-items-center justify-content-center fw-semibold rounded"
                :class="getNavBtnClass(idx)"
                @click="goToQuestion(idx)"
              >
                {{ idx + 1 }}
              </button>
            </div>
          </div>

          <!-- Action buttons -->
          <div class="mt-4 pt-3 border-top border-light d-flex flex-column gap-2">
            <button class="btn btn-indigo text-white w-100 rounded-3 py-2 fw-semibold shadow-sm" @click="submitPractice(false)">
              Nộp Bài
            </button>
            <button class="btn btn-outline-danger w-100 rounded-3 py-2 fw-semibold mt-1" @click="confirmExit">
              Hủy {{ isExamMode ? 'Thi Thử' : 'Luyện Tập' }}
            </button>
          </div>
        </div>
      </div>

      <!-- Live Questions Workspace -->
      <div class="col-lg-9 col-md-8 order-md-1">
        
        <!-- CHẾ ĐỘ 1: Làm từng câu (SINGLE MODE) -->
        <div v-if="config.mode === 'single' && currentQuestion">
          <QuestionCard
            :question="currentQuestion!"
            :index="currentIdx + 1"
            mode="practice"
            :show-result-instant="config.showResultInstant"
            @choose-answer="(ans: any) => chooseAnswer(currentQuestion!, ans)"
          />

          <!-- Navigation controls -->
          <div class="card border-0 rounded-4 shadow-sm bg-white p-4 mb-4 mt-n3 border-top-0 rounded-top-0">
            <div class="d-flex justify-content-between align-items-center pt-2">
              <button class="btn btn-outline-secondary px-4 py-2 rounded-3 fw-semibold" :disabled="currentIdx === 0" @click="goToPrevious">
                ⬅️ Câu trước
              </button>

              <!-- Case 1: Result Instant is true, user has selected answers, but not revealed yet -->
              <button 
                v-if="config.showResultInstant && !isResultRevealed(currentQuestion!) && currentQuestion!.chosenAnswerIds.length > 0" 
                class="btn btn-indigo text-white px-4 py-2 rounded-3 fw-semibold"
                @click="revealCurrentResult"
              >
                ✔️ Kiểm tra đáp án
              </button>

              <!-- Case 2: Result Instant is true, user has NOT selected answers, and not revealed yet -> acts as Skip -->
              <button 
                v-else-if="config.showResultInstant && !isResultRevealed(currentQuestion!) && currentQuestion!.chosenAnswerIds.length === 0" 
                class="btn btn-outline-secondary px-4 py-2 rounded-3 fw-semibold"
                :disabled="currentIdx === questions.length - 1"
                @click="skipAndNext"
              >
                Bỏ qua & Tiếp theo ➡️
              </button>

              <!-- Case 3: Result is already revealed, or Result Instant is false -> normal Next -->
              <button 
                v-else
                class="btn btn-indigo text-white px-4 py-2 rounded-3 fw-semibold"
                :disabled="currentIdx === questions.length - 1"
                @click="goToNext"
              >
                Câu tiếp theo ➡️
              </button>
            </div>
          </div>
        </div>

        <!-- CHẾ ĐỘ 2: Làm toàn bộ đề (ALL MODE) -->
        <div v-else-if="config.mode === 'all'" class="d-flex flex-column gap-4">
          <QuestionCard
            v-for="(q, idx) in questions" 
            :key="q.id" 
            :id="`q-block-${idx}`"
            :question="q"
            :index="idx + 1"
            mode="practice"
            :show-result-instant="config.showResultInstant"
            @choose-answer="(ans: any) => chooseAnswer(q, ans)"
          />
        </div>

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'
import { getExamQuestions, getExamsPaging, getExamQuestionCounts } from '@/api/exams'
import { submitAttempt } from '@/api/attempts'
import { checkAnswer } from '@/api/questions'
import QuestionCard from '@/components/QuestionCard.vue'

const route = useRoute()
const router = useRouter()

const quizId = ref(route.params.id as string)
const quizTitle = ref((route.query.title as string) || 'Đề thi trắc nghiệm')

// Configurations
const config = reactive({
  mode: 'all', // 'all': Làm theo đề, 'single': Làm từng câu
  shuffleAnswers: false,
  shuffleQuestions: false,
  showResultInstant: false, // Default is false because default mode is 'all'
  autoNextSeconds: 0 // Tự động chuyển câu sau n giây
})

watch(() => config.mode, (newMode) => {
  if (newMode === 'all') {
    config.showResultInstant = false
  }
})

// Stage status
const isStarted = ref(false)
const isSubmitted = ref(false)
const correctCount = ref(0)
const finalScore = ref('0.0')
const recommendedQuizzes = ref<any[]>([])
const currentExamCategoryId = ref<string | null>(null)
let autoNextTimeout: any = null

// Exam Mode properties
const isExamMode = computed(() => route.query.exam === 'true')
const examDurationSeconds = ref(0)
const examTimeLeft = ref(0)
let examTimer: any = null

const formattedExamTime = computed(() => {
  const m = Math.floor(examTimeLeft.value / 60)
  const s = examTimeLeft.value % 60
  return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`
})

const startExamTimer = () => {
  const durationStr = route.query.duration as string
  const durationMins = durationStr ? parseInt(durationStr) : 60
  examDurationSeconds.value = durationMins * 60
  examTimeLeft.value = examDurationSeconds.value
  
  if (examTimer) clearInterval(examTimer)
  examTimer = setInterval(() => {
    if (examTimeLeft.value > 0) {
      examTimeLeft.value--
    } else {
      clearInterval(examTimer)
      handleTimeOut()
    }
  }, 1000)
}

// Keyboard shortcuts handler
const handleKeyDown = (e: KeyboardEvent) => {
  if (!isStarted.value || isSubmitted.value) return

  // Prevent handling global shortcuts if any Ant Design Modal is open
  if (document.querySelector('.ant-modal-wrap:not([style*="display: none"])')) return

  // Prevent when focusing text inputs
  const target = e.target as HTMLElement
  const isTextInput = (target.tagName === 'INPUT' && (target as HTMLInputElement).type !== 'radio' && (target as HTMLInputElement).type !== 'checkbox') || target.tagName === 'TEXTAREA'
  if (isTextInput) return

  if (e.key === 'ArrowLeft' || e.key.toLowerCase() === 'a') {
    e.preventDefault() // Tránh trình duyệt tự cuộn hoặc tự chuyển radio
    if (currentIdx.value > 0) {
      currentIdx.value--
      if (config.mode === 'all') goToQuestion(currentIdx.value)
    }
  } else if (e.key === 'ArrowRight' || e.key.toLowerCase() === 'd') {
    e.preventDefault() // Tránh trình duyệt tự chuyển radio
    const q = currentQuestion.value
    // Đối với câu nhiều lựa chọn (multiple choice) ở chế độ từng câu: 
    // Nếu có chọn đáp án và chưa lật kết quả -> Hiện kết quả thay vì sang câu tiếp theo
    if (config.mode === 'single' && config.showResultInstant && q && isMultipleChoice(q) && !isResultRevealed(q) && q.chosenAnswerIds.length > 0) {
      revealCurrentResult()
      return
    }

    if (currentIdx.value < questions.value.length - 1) {
      currentIdx.value++
      if (config.mode === 'all') goToQuestion(currentIdx.value)
    }
  } else if (/^[1-9]$/.test(e.key)) {
    const q = currentQuestion.value
    if (q) {
      const idx = parseInt(e.key) - 1
      const ans = q.answers[idx]
      if (ans) {
        chooseAnswer(q, ans)
      }
    }
  } else if (e.key === 'Enter') {
    const q = currentQuestion.value
    if (config.mode === 'single' && config.showResultInstant && q && !isResultRevealed(q) && q.chosenAnswerIds.length > 0) {
      revealCurrentResult()
    } else {
      submitPractice(false)
    }
  }
}

// Questions list
interface Answer {
  id: string
  stringContent: string
  isCorrectAnswer: boolean
  indexChar: string
}

interface PracticeQuestion {
  id: string
  stringContent: string
  explaination: string
  level: number
  type: number
  answers: Answer[]
  chosenAnswerIds: string[]
  isConfirmed?: boolean
}

const questions = ref<PracticeQuestion[]>([])
const currentIdx = ref(0)
const currentQuestion = computed(() => questions.value[currentIdx.value] || null)

const isMultipleChoice = (q: PracticeQuestion): boolean => {
  return q.type === 1
}

const isResultRevealed = (q: PracticeQuestion) => {
  if (!config.showResultInstant) return false
  return !!q.isConfirmed
}

const answeredCount = computed(() => {
  return questions.value.filter(q => q.chosenAnswerIds.length > 0).length
})

const progressPercentage = computed(() => {
  if (questions.value.length === 0) return 0
  return Math.round((answeredCount.value / questions.value.length) * 100)
})

// Lấy danh mục của đề thi hiện tại
const fetchCurrentExamCategory = async () => {
  try {
    const res = await getExamsPaging({
      pageIndex: 1,
      pageSize: 1,
      filters: [{
        property: 'ExamId',
        operator: 0, // Equal
        value: quizId.value,
        type: 1
      }]
    })
    if (res && res.isSuccess && res.data && res.data.items && res.data.items.length > 0) {
      currentExamCategoryId.value = res.data.items[0].categoryId
    }
  } catch (error) {
    console.error('Lỗi khi lấy thông tin danh mục đề thi:', error)
  }
}

// Fetch Questions from API
const fetchQuestions = async () => {
  try {
    const res = await getExamQuestions(quizId.value)
    if (res && res.data) {
      const list: PracticeQuestion[] = res.data.map((q: any) => ({
        id: q.id,
        stringContent: q.stringContent,
        explaination: q.explanation || '',
        level: q.level,
        type: q.type || 0,
        chosenAnswerIds: [],
        isConfirmed: false,
        answers: (q.answers || []).map((a: any, idx: number) => {
          const ansId = a.questionAnswerId;
          if (!ansId) throw new Error('Dữ liệu đáp án không hợp lệ (thiếu QuestionAnswerId)')
          return {
            id: ansId,
            stringContent: a.stringContent,
            isCorrectAnswer: a.isCorrectAnswer ?? false,
            indexChar: String.fromCharCode(65 + idx)
          }
        })
      }))

      // Shuffle logic if requested
      if (config.shuffleQuestions) {
        list.sort(() => Math.random() - 0.5)
      }

      if (config.shuffleAnswers) {
        list.forEach(q => {
          q.answers.sort(() => Math.random() - 0.5)
          // Re-assign index chars A, B, C...
          q.answers.forEach((ans, idx) => {
            ans.indexChar = String.fromCharCode(65 + idx)
          })
        })
      }

      questions.value = list
      // Tải danh mục của đề thi để chuẩn bị cho phần gợi ý sau này
      fetchCurrentExamCategory()
    }
  } catch (error) {
    message.error('Lỗi khi lấy dữ liệu đề thi.')
    console.error(error)
  }
}

const startPractice = async () => {
  await fetchQuestions()
  isStarted.value = true
  currentIdx.value = 0
  message.success('Đã bắt đầu luyện tập! Chúc bạn làm bài tốt.')
}

const chooseAnswer = (q: PracticeQuestion, ans: Answer) => {
  if (isResultRevealed(q)) {
    return
  }
  if (isMultipleChoice(q)) {
    const idx = q.chosenAnswerIds.indexOf(ans.id)
    if (idx > -1) {
      q.chosenAnswerIds.splice(idx, 1)
    } else {
      q.chosenAnswerIds.push(ans.id)
    }
  } else {
    q.chosenAnswerIds = [ans.id]
    
    // Tự động hiển thị kết quả nếu bật chế độ "Hiện kết quả ngay"
    if (config.mode === 'single' && config.showResultInstant) {
      revealCurrentResult()
    }
  }
}

const goToPrevious = () => {
  if (currentIdx.value > 0) {
    currentIdx.value--
  }
}

const goToNext = () => {
  if (currentIdx.value < questions.value.length - 1) {
    currentIdx.value++
  }
}

const skipAndNext = () => {
  goToNext()
}

const revealCurrentResult = async () => {
  const q = currentQuestion.value
  if (!q) return

  if (!q.isConfirmed) {
    try {
      const res = await checkAnswer({
        questionId: q.id,
        selectedAnswerIds: q.chosenAnswerIds
      })
      if (res && res.data) {
        const checkResult = res.data
        q.explaination = checkResult.explanation || q.explaination
        // Cập nhật isCorrectAnswer từ backend
        q.answers.forEach(a => {
          a.isCorrectAnswer = checkResult.correctAnswerIds.includes(a.id)
        })
      }
    } catch (error) {
      message.error('Lỗi khi chấm đáp án.')
      console.error(error)
      return // Dừng lại nếu lỗi, không next
    }
  }

  q.isConfirmed = true

  // Auto next transition logic (n seconds)
  if (config.mode === 'single' && config.autoNextSeconds > 0) {
    if (autoNextTimeout) clearTimeout(autoNextTimeout)
    autoNextTimeout = setTimeout(() => {
      goToNext()
    }, config.autoNextSeconds * 1000)
  }
}

// Option classes for colors
const isAnswered = (q: PracticeQuestion) => q.chosenAnswerIds.length > 0
const isChosen = (q: PracticeQuestion, ans: Answer) => q.chosenAnswerIds.includes(ans.id)

const getOptionClass = (q: PracticeQuestion, ans: Answer) => {
  if (q.chosenAnswerIds.length === 0) {
    return 'border-light hover-shadow'
  }

  const chosen = isChosen(q, ans)

  if (config.showResultInstant) {
    if (isResultRevealed(q)) {
      if (ans.isCorrectAnswer) {
        return 'border-success bg-success-soft text-success font-semibold'
      }
      if (chosen && !ans.isCorrectAnswer) {
        return 'border-danger bg-danger-soft text-danger font-semibold'
      }
    } else {
      if (chosen) {
        return 'border-indigo bg-indigo-soft text-indigo font-semibold'
      }
    }
  } else {
    if (chosen) {
      return 'border-indigo bg-indigo-soft text-indigo font-semibold'
    }
  }

  return 'border-light text-muted'
}

const getNavBtnClass = (idx: number) => {
  const q = questions.value[idx]
  if (!q) return 'btn-outline-secondary'
  
  const chosen = q.chosenAnswerIds.length > 0
  const isCurrent = currentIdx.value === idx

  if (isCurrent) {
    return 'btn-indigo text-white shadow-sm'
  }
  if (chosen) {
    return 'btn-success text-white'
  }
  return 'btn-outline-secondary'
}

const goToQuestion = (idx: number) => {
  currentIdx.value = idx
  if (config.mode === 'all') {
    const el = document.getElementById(`q-block-${idx}`)
    if (el) {
      const y = el.getBoundingClientRect().top + window.scrollY - 100 // Bù trừ 100px cho header
      window.scrollTo({ top: y, behavior: 'smooth' })
    }
  }
}

const handleTimeOut = () => {
  Modal.warning({
    title: '⏱️ Hết thời gian làm bài!',
    content: 'Thời gian ôn tập cho đề thi này đã kết thúc. Hệ thống sẽ tự động chấm điểm bài làm của bạn.',
    onOk: () => {
      submitPractice(true)
    }
  })
}

const generateMockRecommendations = () => {
  const hash = quizId.value.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
  const list = []
  const topics = ['nâng cao', 'cơ bản', 'tổng hợp', 'chuyên đề', 'thử thách', 'mở rộng']
  for (let i = 1; i <= 3; i++) {
    const recIndex = (hash + i) % topics.length
    list.push({
      id: `${quizId.value}-rec-${i}`,
      title: `Đề gợi ý ${i}: Đề ôn tập ${topics[recIndex]} - ${quizTitle.value}`,
      duration: (i % 2 === 0) ? 45 : 90,
      questionCount: (i % 2 === 0) ? 30 : 50,
    })
  }
  recommendedQuizzes.value = list
}

const fetchRecommendations = async () => {
  try {
    // 1. Lấy categoryId của đề thi hiện tại nếu chưa có
    if (!currentExamCategoryId.value) {
      const examRes = await getExamsPaging({
        pageIndex: 1,
        pageSize: 1,
        filters: [{
          property: 'ExamId',
          operator: 0, // Equal
          value: quizId.value,
          type: 1
        }]
      })
      if (examRes && examRes.isSuccess && examRes.data && examRes.data.items && examRes.data.items.length > 0) {
        currentExamCategoryId.value = examRes.data.items[0].categoryId
      }
    }

    // 2. Tìm các đề thi cùng CategoryId
    const filters = []
    if (currentExamCategoryId.value) {
      filters.push({
        property: 'CategoryId',
        operator: 0, // Equal
        value: currentExamCategoryId.value,
        type: 1
      })
    }

    const [examsRes, countsRes] = await Promise.all([
      getExamsPaging({
        pageIndex: 1,
        pageSize: 10, // Lấy dư để loại trừ đề thi hiện tại
        filters: filters
      }),
      getExamQuestionCounts()
    ])

    let list: any[] = []
    if (examsRes && examsRes.isSuccess && examsRes.data && examsRes.data.items) {
      const counts = countsRes?.isSuccess ? (countsRes.data || {}) : {}
      // Loại trừ đề thi hiện tại
      const filtered = examsRes.data.items.filter((e: any) => e.examId !== quizId.value)
      
      list = filtered.map((e: any) => ({
        id: e.examId,
        title: e.name,
        duration: e.duration || 45,
        questionCount: counts[e.examId] || 0
      }))
    }

    // 3. Fallback: nếu không đủ 3 đề cùng chuyên mục, lấy thêm đề mới nhất khác
    if (list.length < 3) {
      const fallbackRes = await getExamsPaging({
        pageIndex: 1,
        pageSize: 10,
        filters: []
      })
      if (fallbackRes && fallbackRes.isSuccess && fallbackRes.data && fallbackRes.data.items) {
        const counts = countsRes?.isSuccess ? (countsRes.data || {}) : {}
        const fallbackItems = fallbackRes.data.items
          .filter((e: any) => e.examId !== quizId.value && !list.some(existing => existing.id === e.examId))
        
        const needed = 3 - list.length
        const mappedFallback = fallbackItems.slice(0, needed).map((e: any) => ({
          id: e.examId,
          title: e.name,
          duration: e.duration || 45,
          questionCount: counts[e.examId] || 0
        }))
        list = [...list, ...mappedFallback]
      }
    }

    recommendedQuizzes.value = list.slice(0, 3)
  } catch (error) {
    console.error('Lỗi khi tải đề thi gợi ý:', error)
    // Fallback sang sinh dữ liệu mock cũ nếu có lỗi API
    generateMockRecommendations()
  }
}

const shareResult = () => {
  const shareText = `Tôi đã hoàn thành bài luyện tập "${quizTitle.value}" với kết quả: ${correctCount.value}/${questions.value.length} câu đúng (Điểm: ${finalScore.value}/10). Hãy thử sức ngay tại CNLearnMS!`
  navigator.clipboard.writeText(shareText).then(() => {
    message.success('Đã sao chép kết quả luyện tập vào bộ nhớ tạm để chia sẻ!')
  }).catch(() => {
    message.error('Không thể sao chép kết quả. Vui lòng thử lại.')
  })
}

const backToDetail = () => {
  router.push({ name: 'quiz-detail', params: { id: quizId.value }, query: { title: quizTitle.value } })
}

const viewQuizDetail = (id: string, title: string) => {
  router.push({ name: 'quiz-detail', params: { id }, query: { title } })
}

let isConfirmDialogOpen = false

const submitPractice = (bypassConfirm = false) => {
  if (isSubmitted.value) return

  const performSubmit = async () => {
    if (examTimer) clearInterval(examTimer)

    const timeSpent = isExamMode.value 
      ? examDurationSeconds.value - examTimeLeft.value
      : 0; // Trong tương lai có thể track thời gian practice thực tế

    const payload = {
      examId: quizId.value,
      attemptType: isExamMode.value ? 'exam' : 'practice',
      timeSpentSeconds: timeSpent,
      answers: questions.value.map(q => ({
        questionId: q.id,
        selectedAnswerIds: q.chosenAnswerIds
      }))
    }

    try {
      const res = await submitAttempt(payload as any)
      const attemptResult = res?.data ?? (res as any)?.Data
      if (attemptResult) {
        const cCount = attemptResult.correctCount ?? attemptResult.CorrectCount ?? 0
        const score = attemptResult.score ?? attemptResult.Score ?? 0

        correctCount.value = cCount
        finalScore.value = Number(score).toFixed(1)

        // Cập nhật lại thông tin đáp án đúng sai từ Backend trả về
        const details = attemptResult.details ?? attemptResult.Details
        if (details) {
          details.forEach((det: any) => {
            const qId = det.questionId ?? det.QuestionId
            const q = questions.value.find(q => q.id === qId)
            if (q) {
              q.explaination = det.explanation ?? det.Explanation ?? q.explaination
              // Cập nhật isCorrectAnswer cho các option để UI đổi màu
              const cAnswerIds = det.correctAnswerIds ?? det.CorrectAnswerIds ?? []
              q.answers.forEach(a => {
                a.isCorrectAnswer = cAnswerIds.includes(a.id)
              })
            }
          })
        }

        fetchRecommendations()
        isSubmitted.value = true
        window.scrollTo({ top: 0, behavior: 'smooth' })
      }
    } catch (error) {
      message.error('Có lỗi xảy ra khi nộp bài. Vui lòng thử lại.')
      console.error(error)
    }
  }

  if (bypassConfirm) {
    performSubmit()
    return
  }

  if (isConfirmDialogOpen) return
  isConfirmDialogOpen = true

  Modal.confirm({
    title: `✔️ Nộp bài ${isExamMode.value ? 'thi thử' : 'luyện tập'}?`,
    content: `Bạn đã hoàn thành ${answeredCount.value}/${questions.value.length} câu. Bạn có muốn nộp bài ngay bây giờ?`,
    okText: 'Nộp bài',
    cancelText: 'Hủy',
    onOk: () => {
      isConfirmDialogOpen = false
      performSubmit()
    },
    onCancel: () => {
      isConfirmDialogOpen = false
    }
  })
}

const confirmExit = () => {
  if (isSubmitted.value) {
    router.push({ name: 'quiz-detail', params: { id: quizId.value }, query: { title: quizTitle.value } })
    return
  }
  Modal.confirm({
    title: `🚪 Thoát chế độ ${isExamMode.value ? 'thi thử' : 'luyện tập'}?`,
    content: 'Mọi tiến trình làm bài hiện tại sẽ không được lưu trữ. Bạn có chắc chắn muốn rời đi?',
    okText: 'Rời đi',
    cancelText: 'Hủy',
    onOk: () => {
      isStarted.value = false
      router.push({ name: 'quiz-detail', params: { id: quizId.value }, query: { title: quizTitle.value } })
    }
  })
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

const getLevelTextColor = (level: number): string => {
  if (level === 0) return 'text-success'
  if (level === 1) return 'text-warning'
  return 'text-danger'
}

onMounted(() => {
  window.scrollTo({ top: 0 })
  window.addEventListener('keydown', handleKeyDown)

  if (isExamMode.value) {
    config.mode = 'all'
    config.showResultInstant = false
    startPractice()
    startExamTimer()
  }
})

onUnmounted(() => {
  if (autoNextTimeout) clearTimeout(autoNextTimeout)
  if (examTimer) clearInterval(examTimer)
  window.removeEventListener('keydown', handleKeyDown)
})
</script>

<style scoped>
/* Ngăn chặn bị header che khuất khi cuộn bằng hàm scrollIntoView */
.question-block {
  scroll-margin-top: 120px;
}

.text-dark-blue {
  color: #1e1b4b;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.12);
}

.bg-success-soft {
  background-color: rgba(16, 185, 129, 0.12);
}

.bg-danger-soft {
  background-color: rgba(239, 68, 68, 0.12);
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
  border-color: #6366f1;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
  border-color: #4f46e5;
}

.bg-light-card {
  background-color: #f8fafc;
}

.hover-pointer:hover {
  cursor: pointer;
}

.hover-shadow:hover {
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  border-color: #c7d2fe !important;
}

.option-item {
  transition: all 0.2s ease;
}

.option-check {
  width: 32px;
  height: 32px;
  line-height: 30px;
  text-align: center;
  font-size: 0.9rem;
}

.border-indigo {
  border-color: #6366f1 !important;
}

.nav-questions-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 8px;
}

.nav-q-btn {
  width: 100% !important;
  aspect-ratio: 1 / 1;
  font-size: 0.85rem;
  padding: 0 !important;
  min-width: 0 !important;
}

.sticky-sidebar {
  position: -webkit-sticky;
  position: sticky;
  top: 20px; /* Closer to top since there is no sticky header navbar in this mode */
  max-height: calc(100vh - 40px); /* Adjust max-height for scrolling */
  overflow-y: auto;
  z-index: 10;
}

.quiz-icon-box {
  width: 44px;
  height: 44px;
  background-color: rgba(99, 102, 241, 0.1);
  border-radius: 10px;
  line-height: 44px;
  font-size: 1.4rem;
}

.offset-top {
  top: 88px; /* Offset to clear the sticky header */
}

.timer-display {
  font-family: monospace;
}

.hover-up {
  transition: all 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.2);
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

.gap-2-5 {
  gap: 0.625rem;
}

.mt-1-5 {
  margin-top: 0.375rem;
}

.leading-relaxed {
  line-height: 1.625;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(12px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-fade-in {
  animation: fadeIn 0.45s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.question-block {
  scroll-margin-top: 10px;
}

.option-check-item {
  border: 1px solid #f1f5f9;
  cursor: pointer;
  transition: all 0.2s ease;
}

.option-check-item:hover {
  background-color: #f8fafc;
  border-color: #cbd5e1;
}

.border-indigo-accent-solid {
  border: 1px solid #6366f1 !important;
}

.custom-input-control {
  border-color: #cbd5e1;
  width: 1.2rem;
  height: 1.2rem;
  transition: all 0.2s ease;
}

.custom-input-control:checked {
  background-color: #4f46e5;
  border-color: #4f46e5;
}

/* Confetti & Fireworks Animation */
.confetti-animation-wrapper {
  perspective: 1000px;
}

.confetti-container {
  display: inline-block;
  font-size: 4rem;
  animation: bounceConfetti 1.2s infinite alternate ease-in-out;
}

.confetti-icon {
  display: inline-block;
  filter: drop-shadow(0 4px 10px rgba(99, 102, 241, 0.2));
}

.particle {
  position: absolute;
  font-size: 1.5rem;
  opacity: 0;
  pointer-events: none;
  animation: explodeConfetti 2.2s infinite ease-out;
}

/* Coordinate directions around center */
.p1 { top: -15px; left: -15px; animation-delay: 0s; --dx: -55px; --dy: -55px; }
.p2 { top: -15px; right: -15px; animation-delay: 0.3s; --dx: 55px; --dy: -55px; }
.p3 { bottom: -15px; left: -15px; animation-delay: 0.6s; --dx: -55px; --dy: 55px; }
.p4 { bottom: -15px; right: -15px; animation-delay: 0.9s; --dx: 55px; --dy: 55px; }
.p5 { top: -40px; left: 50%; transform: translateX(-50%); animation-delay: 0.2s; --dx: 0px; --dy: -75px; }
.p6 { bottom: -40px; left: 50%; transform: translateX(-50%); animation-delay: 0.7s; --dx: 0px; --dy: 75px; }

@keyframes explodeConfetti {
  0% { transform: translate(0, 0) scale(0.4); opacity: 0; }
  30% { opacity: 1; }
  100% { transform: translate(var(--dx), var(--dy)) scale(1.3); opacity: 0; }
}

@keyframes bounceConfetti {
  from { transform: translateY(0) scale(1); }
  to { transform: translateY(-12px) scale(1.08); }
}

.btn-outline-indigo {
  color: #4f46e5;
  border: 1px solid #6366f1;
  background-color: transparent;
  transition: all 0.2s ease;
}

.btn-outline-indigo:hover {
  background-color: #4f46e5;
  color: white;
  border-color: #4f46e5;
}

.bg-light-hover {
  background-color: #fff;
  border: 1px solid #e2e8f0;
}

.bg-light-hover:hover {
  background-color: #f8fafc !important;
  border-color: #cbd5e1 !important;
}

.hover-pointer {
  cursor: pointer;
}

.text-line-clamp {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
