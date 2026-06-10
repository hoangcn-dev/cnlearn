<template>
  <div class="quiz-room-container py-4">


    <!-- STATE 1: TÌM PHÒNG (JOIN ROOM) -->
    <div v-if="state === 'join'" class="row justify-content-center animate-fade-in py-4">
      <div class="col-md-5">
        <div class="card border-0 rounded-4 shadow bg-white p-4">
          <div class="text-center mb-4">
            <h2 class="fw-bold text-dark-blue mt-2 mb-1">Tham gia phòng thi</h2>
            <p class="text-secondary small">Nhập mã phòng thi và thông tin của bạn để bắt đầu</p>
          </div>

          <a-form layout="vertical">
            <a-form-item label="Mã phòng thi (Room Code):">
              <a-input v-model:value="joinForm.roomCode" placeholder="Ví dụ: quiz_default_1" size="large" />
            </a-form-item>
            <a-form-item label="Họ và tên thí sinh:">
              <a-input v-model:value="joinForm.candidateName" placeholder="Nhập tên đầy đủ của bạn" size="large" />
            </a-form-item>

            <button 
              class="btn btn-indigo text-white w-100 py-2.5 rounded-3 fw-bold shadow-sm mt-3"
              @click="handleJoinRoom"
            >
              Vào phòng chờ
            </button>
          </a-form>
        </div>
      </div>
    </div>

    <!-- STATE 2: PHÒNG CHỜ (LOBBY / WAITING) -->
    <div v-else-if="state === 'lobby' && currentQuiz" class="row justify-content-center align-items-stretch g-4 animate-fade-in">
      <!-- Main Lobby Card -->
      <div class="col-md-6 col-lg-5">
        <div class="card border-0 rounded-4 shadow bg-white p-4 h-100">
          <div class="pb-2 border-bottom border-light mb-2 text-center">
            <span class="badge bg-warning-soft text-warning fw-bold px-3 py-1.5 rounded-pill mb-2">PHÒNG CHỜ THI</span>
            <h1 class="fs-3 fw-bold text-dark-blue mb-1">{{ currentQuiz.title }}</h1>
            <p class="text-secondary small">Thí sinh: <span class="fw-bold text-indigo">{{ candidateName }}</span></p>
          </div>

          <!-- Countdown & Start Button (Above) -->
          <div class="text-center my-2 border-bottom border-light pb-2">
            <!-- Countdown timer (if still waiting) -->
            <div v-if="lobbyTimeLeft > 0" class="mb-3">
              <div class="fs-8 text-secondary mb-2 fw-semibold">THỜI GIAN ĐẾM NGƯỢC ĐẾN GIỜ THI</div>
              <div class="countdown-circle d-inline-flex flex-column align-items-center justify-content-center bg-light rounded-circle shadow-sm" style="width: 140px; height: 140px;">
                <span class="fs-2 fw-bold text-danger font-monospace">{{ formattedLobbyTime }}</span>
                <span class="fs-9 text-secondary mt-1">còn lại</span>
              </div>
              <div class="text-danger small mt-2 fw-medium">
                Vui lòng đợi đồng hồ đếm ngược kết thúc để mở đề thi
              </div>
            </div>

            <!-- Start button -->
            <button 
              class="btn btn-indigo text-white px-5 py-3 rounded-pill fw-bold fs-6 shadow pulse-btn"
              :disabled="lobbyTimeLeft > 0"
              @click="enterExamWorkspace"
            >
              BẮT ĐẦU LÀM BÀI
            </button>
          </div>

          <!-- Regulations & Guidelines (Below) -->
          <div class="p-3 bg-light rounded-3 mb-2">
            <h5 class="fw-bold text-dark-blue fs-8 mb-2">Quy định & Chống gian lận phòng thi:</h5>
            <ul class="list-unstyled small text-secondary mb-0 d-flex flex-column gap-2" style="font-size: 0.75rem;">
              <li class="d-flex align-items-center gap-2">
                <span class="text-success">✔</span> Chế độ thi: Làm cả đề (cuộn toàn trang)
              </li>
              <li v-if="antiCheatOptions.fullscreen" class="d-flex align-items-center gap-2">
                <span class="text-danger">☒</span> Yêu cầu toàn màn hình (Fullscreen)
              </li>
              <li v-if="antiCheatOptions.lockBrowser" class="d-flex align-items-center gap-2">
                <span class="text-danger">☒</span> Khóa trình duyệt (Chống chuyển tab/cửa sổ)
              </li>
              <li v-if="antiCheatOptions.disableCopyPaste" class="d-flex align-items-center gap-2">
                <span class="text-danger">☒</span> Cấm sao chép, cắt dán đề thi & chuột phải
              </li>
            </ul>
          </div>
        </div>
      </div>

      <!-- Online Candidates Card (Side-by-Side) -->
      <div class="col-md-5 col-lg-4">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4 h-100">
          <h3 class="h6 fw-bold text-dark-blue mb-3 d-flex align-items-center gap-2">
            Thí sinh trực tuyến trong phòng chờ ({{ lobbyCandidates.length + 1 }})
          </h3>
          <div class="d-flex flex-wrap gap-2">
            <span class="badge bg-indigo-soft text-indigo px-3 py-2 rounded fw-bold border border-indigo-accent">
              {{ candidateName }} (Bạn)
            </span>
            <span v-for="name in lobbyCandidates" :key="name" class="badge bg-light text-secondary px-3 py-2 rounded border border-light-subtle">
              {{ name }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- STATE 3: LÀM BÀI THI (EXAM WORKSPACE) -->
    <div v-else-if="state === 'taking' && currentQuiz" class="row">
      <!-- WARNING INTERACTION OVERLAYS (Chống gian lận màn hình khóa) -->
      <div v-if="isLockedOut" class="fullscreen-lockout d-flex flex-column align-items-center justify-content-center text-center p-4">
        <div class="card border-0 rounded-4 shadow bg-white p-5 max-w-500">
          <h2 class="fw-bold text-danger mb-2">PHÁT HIỆN THOÁT TOÀN MÀN HÌNH!</h2>
          <p class="text-secondary small mb-4">
            Kỳ thi này yêu cầu bạn phải giữ toàn màn hình liên tục. Vi phạm quy chế thi có thể dẫn tới việc bài thi bị hủy bỏ tự động.
          </p>
          <div class="badge bg-danger text-white fs-7 py-2 px-3 rounded mb-4">
            Số lần vi phạm tự do thoát: {{ antiCheatCounters.fullscreenExitCount }} / 3 lần tối đa
          </div>
          <button class="btn btn-danger px-4 py-2.5 rounded-pill fw-semibold text-white w-100" @click="requestBackToFullscreen">
            Quay lại toàn màn hình & làm tiếp
          </button>
        </div>
      </div>

      <!-- SIDEBAR STATUS -->
      <div class="col-lg-3 col-md-4 mb-4 order-md-2">
        <div class="card border-0 rounded-4 shadow bg-white p-4 sticky-sidebar">
          <!-- Progress bar -->
          <div class="mb-3">
            <div class="d-flex justify-content-between small text-secondary mb-1">
              <span>Học viên:</span>
              <span class="fw-bold text-indigo">{{ candidateName }}</span>
            </div>
            <div class="d-flex justify-content-between small text-secondary mb-1">
              <span>Tiến độ câu:</span>
              <span class="fw-bold">{{ answeredCount }}/{{ questions.length }} câu</span>
            </div>
            <div class="progress rounded-pill" style="height: 8px;">
              <div class="progress-bar bg-indigo rounded-pill" role="progressbar" :style="{ width: `${progressPercentage}%` }"></div>
            </div>
          </div>

          <!-- Exam Timer -->
          <div class="mb-4 p-3 bg-danger-soft text-danger rounded-3 text-center border border-danger-subtle shadow-sm">
            <div class="fs-8 fw-semibold mb-1">Thời gian làm bài còn lại</div>
            <div class="fs-2 fw-bold font-monospace tracking-wider">{{ formattedExamTime }}</div>
          </div>

          <!-- Anti-cheat monitor counters -->
          <div class="mb-4 p-2.5 bg-warning-soft text-warning rounded-3 small" style="font-size: 0.72rem;">
            <div class="fw-bold text-dark-blue mb-1">Giám sát quy chế thi:</div>
            <div class="d-flex justify-content-between mb-1">
              <span>Chuyển tab/Cửa sổ:</span>
              <span class="fw-bold text-danger">{{ antiCheatCounters.blurCount }} lần</span>
            </div>
            <div class="d-flex justify-content-between">
              <span>Thoát toàn màn hình:</span>
              <span class="fw-bold text-danger">{{ antiCheatCounters.fullscreenExitCount }} lần</span>
            </div>
          </div>

          <!-- Grid Navigation -->
          <div class="questions-navigation pt-2">
            <span class="fw-semibold text-dark-blue small d-block mb-2">Bản đồ câu hỏi</span>
            <div class="nav-questions-grid">
              <button 
                v-for="(q, idx) in questions" 
                :key="q.id" 
                class="btn nav-q-btn d-flex align-items-center justify-content-center fw-semibold rounded"
                :class="getNavBtnClass(idx)"
                @click="scrollToQuestion(idx)"
              >
                {{ idx + 1 }}
              </button>
            </div>
          </div>

          <!-- Submit Buttons -->
          <div class="mt-4 pt-3 border-top border-light d-flex flex-column gap-2">
            <button class="btn btn-indigo text-white w-100 rounded-3 py-2.5 fw-bold shadow-sm" @click="submitExam(false)">
              Nộp bài thi
            </button>
          </div>
        </div>
      </div>

      <!-- EXAM QUESTIONS LIST (ALL MODE) -->
      <div class="col-lg-9 col-md-8 order-md-1">
        <div class="card border-0 rounded-4 shadow bg-white p-4">
          <div class="pb-3 border-bottom border-light mb-4">
            <h2 class="fs-4 fw-bold text-dark-blue mb-1">{{ currentQuiz.title }}</h2>
            <p class="text-secondary small mb-0">Học viên đang làm đề thi chính thức. Chế độ bảo mật chống gian lận đang kích hoạt.</p>
          </div>

          <div class="d-flex flex-column gap-4 select-none">
            <div 
              v-for="(q, idx) in questions" 
              :key="q.id" 
              :id="`q-block-${idx}`"
              class="question-block pb-4"
              :class="{ 'border-bottom border-light': idx < questions.length - 1 }"
            >
              <div class="d-flex justify-content-between align-items-center mb-2">
                <span class="fw-bold text-indigo fs-7">Câu {{ idx + 1 }} <span class="text-secondary fw-normal fs-8 opacity-75"> - {{ getLevelText(q.level).toLowerCase() }}</span></span>
              </div>

              <h3 class="h6 fw-semibold text-dark-blue leading-relaxed mb-3">
                {{ q.stringContent }}
              </h3>

              <!-- Radio Options -->
              <div class="options-container d-flex flex-column gap-2">
                <div 
                  v-for="ans in q.answers" 
                  :key="ans.id" 
                  class="option-check-item py-3 px-4 rounded-3 d-flex align-items-center gap-3 hover-pointer transition"
                  :class="{ 'bg-indigo-light-opacity border-indigo-accent-solid': isChosen(q, ans) }"
                  @click="chooseAnswer(q, ans)"
                >
                  <input 
                    :type="isMultipleChoice(q) ? 'checkbox' : 'radio'" 
                    :name="`q-radio-${q.id}`" 
                    :id="`ans-${ans.id}`" 
                    :checked="isChosen(q, ans)"
                    class="form-check-input hover-pointer custom-input-control m-0"
                    @click.stop="chooseAnswer(q, ans)"
                  >
                  <label :for="`ans-${ans.id}`" class="form-check-label text-dark small hover-pointer flex-grow-1 mb-0 ps-1">
                    <span class="fw-bold text-dark-blue me-1">{{ ans.indexChar || '' }}.</span>
                    {{ ans.stringContent }}
                  </label>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'

interface Answer {
  id: string
  stringContent: string
  isCorrectAnswer: boolean
  indexChar?: string
}

interface Question {
  id: string
  stringContent: string
  explanation: string
  level: number
  answers: Answer[]
  chosenAnswerIds: string[]
}

interface Quiz {
  id: string
  title: string
  targetGroup: string
  sourceType: string
  examId: string
  startDate: string
  endDate: string
  antiCheat?: {
    lockBrowser: boolean
    shuffleQuestions: boolean
    disableCopyPaste: boolean
    webcam: boolean
    ipLimit: boolean
    fullscreen: boolean
  }
}

const route = useRoute()
const router = useRouter()

const state = ref<'join' | 'lobby' | 'taking'>('join')

// Room Join form state
const joinForm = reactive({
  roomCode: '',
  candidateName: ''
})

// Current exam session states
const candidateName = ref('')
const currentQuiz = ref<Quiz | null>(null)
const questions = ref<Question[]>([])

// Countdown Timers
const lobbyTimeLeft = ref(0)
let lobbyTimer: any = null

const examTimeLeft = ref(0)
let examTimer: any = null

// Anti-Cheat tracking
const isLockedOut = ref(false)
const antiCheatCounters = reactive({
  blurCount: 0,
  fullscreenExitCount: 0
})

const antiCheatOptions = computed(() => {
  return currentQuiz.value?.antiCheat || {
    lockBrowser: true,
    shuffleQuestions: true,
    disableCopyPaste: true,
    webcam: false,
    ipLimit: false,
    fullscreen: true
  }
})

// MOCK PARTICIPANTS list
const lobbyCandidates = ref<string[]>([
  'Nguyễn Hữu Đạt', 'Lê Mai Anh', 'Trần Quốc Bảo', 'Phạm Thùy Linh', 'Vũ Đức Thịnh'
])

// Timer formatters
const formattedLobbyTime = computed(() => {
  const m = Math.floor(lobbyTimeLeft.value / 60)
  const s = lobbyTimeLeft.value % 60
  return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`
})

const formattedExamTime = computed(() => {
  const m = Math.floor(examTimeLeft.value / 60)
  const s = examTimeLeft.value % 60
  return `${m.toString().padStart(2, '0')}:${s.toString().padStart(2, '0')}`
})

// Answers stats
const answeredCount = computed(() => {
  return questions.value.filter(q => q.chosenAnswerIds.length > 0).length
})

const progressPercentage = computed(() => {
  if (questions.value.length === 0) return 0
  return Math.round((answeredCount.value / questions.value.length) * 100)
})

// Lifecycle
onMounted(() => {
  // Check if roomId is provided in URL path
  const paramId = route.params.id as string | undefined
  if (paramId && paramId !== 'join') {
    joinForm.roomCode = paramId
    
    // Auto populate if user logged in
    joinForm.candidateName = 'Học viên học phần'
  }
})

onUnmounted(() => {
  clearInterval(lobbyTimer)
  clearInterval(examTimer)
  removeAntiCheatListeners()
})

// Functions
const handleJoinRoom = () => {
  if (!joinForm.roomCode.trim()) {
    message.error('Vui lòng nhập Mã phòng thi!')
    return
  }
  if (!joinForm.candidateName.trim()) {
    message.error('Vui lòng nhập Họ và tên của bạn!')
    return
  }

  // Load from cn_quizzes
  const storedQuizzes = localStorage.getItem('cn_quizzes')
  const quizzesList = storedQuizzes ? JSON.parse(storedQuizzes) : []
  const found = quizzesList.find((q: any) => q.id === joinForm.roomCode.trim())

  if (!found) {
    message.error('Không tìm thấy phòng thi với mã phòng này!')
    return
  }

  currentQuiz.value = found
  candidateName.value = joinForm.candidateName.trim()
  state.value = 'lobby'

  // Calculate waiting time in seconds
  const now = new Date()
  const start = new Date(found.startDate)
  const diffMs = start.getTime() - now.getTime()
  
  if (diffMs > 0) {
    lobbyTimeLeft.value = Math.ceil(diffMs / 1000)
    startLobbyCountdown()
  } else {
    lobbyTimeLeft.value = 0
  }
}

const startLobbyCountdown = () => {
  if (lobbyTimer) clearInterval(lobbyTimer)
  lobbyTimer = setInterval(() => {
    if (lobbyTimeLeft.value > 0) {
      lobbyTimeLeft.value--
    } else {
      clearInterval(lobbyTimer)
    }
  }, 1000)
}

const enterExamWorkspace = () => {
  if (!currentQuiz.value) return

  // Load exam questions
  const storedExams = localStorage.getItem('cn_exams')
  const examsList = storedExams ? JSON.parse(storedExams) : []
  const foundExam = examsList.find((e: any) => e.id === currentQuiz.value?.examId)

  if (!foundExam || !foundExam.questions || foundExam.questions.length === 0) {
    message.error('Đề thi liên kết không có dữ liệu câu hỏi!')
    return
  }

  // Deep copy questions list
  let loadedQuestions: Question[] = JSON.parse(JSON.stringify(foundExam.questions))
  
  // Shuffle questions if configured
  if (antiCheatOptions.value.shuffleQuestions) {
    loadedQuestions.sort(() => Math.random() - 0.5)
  }

  // Map indexes char (A, B, C, D)
  const chars = ['A', 'B', 'C', 'D']
  loadedQuestions.forEach((q) => {
    q.chosenAnswerIds = []
    q.answers.forEach((ans, idx) => {
      ans.indexChar = chars[idx] || '?'
    })
  })

  questions.value = loadedQuestions
  state.value = 'taking'

  // Set duration timer
  const durationMins = foundExam.duration || 45
  examTimeLeft.value = durationMins * 60
  startExamTimer()

  // Setup security listeners
  setupAntiCheatListeners()

  // Request fullscreen if configured
  if (antiCheatOptions.value.fullscreen) {
    requestFullscreen()
  }
}

const startExamTimer = () => {
  if (examTimer) clearInterval(examTimer)
  examTimer = setInterval(() => {
    if (examTimeLeft.value > 0) {
      examTimeLeft.value--
    } else {
      clearInterval(examTimer)
      handleExamTimeOut()
    }
  }, 1000)
}

// Anti-Cheat actions
const setupAntiCheatListeners = () => {
  const options = antiCheatOptions.value

  if (options.disableCopyPaste) {
    document.addEventListener('copy', preventDefaultAction)
    document.addEventListener('cut', preventDefaultAction)
    document.addEventListener('paste', preventDefaultAction)
    document.addEventListener('contextmenu', preventDefaultAction)
  }

  if (options.lockBrowser) {
    window.addEventListener('blur', handleWindowBlur)
    document.addEventListener('visibilitychange', handleVisibilityChange)
  }

  if (options.fullscreen) {
    document.addEventListener('fullscreenchange', handleFullscreenChange)
    document.addEventListener('webkitfullscreenchange', handleFullscreenChange)
  }
}

const removeAntiCheatListeners = () => {
  document.removeEventListener('copy', preventDefaultAction)
  document.removeEventListener('cut', preventDefaultAction)
  document.removeEventListener('paste', preventDefaultAction)
  document.removeEventListener('contextmenu', preventDefaultAction)
  window.removeEventListener('blur', handleWindowBlur)
  document.removeEventListener('visibilitychange', handleVisibilityChange)
  document.removeEventListener('fullscreenchange', handleFullscreenChange)
  document.removeEventListener('webkitfullscreenchange', handleFullscreenChange)
}

const preventDefaultAction = (e: Event) => {
  e.preventDefault()
  message.warning('Hành vi sao chép/chuột phải đã bị chặn để bảo mật đề thi!')
}

const handleWindowBlur = () => {
  if (state.value !== 'taking' || isLockedOut.value) return
  antiCheatCounters.blurCount++
  message.error(`Cảnh báo gian lận: Bạn đã thoát khỏi cửa sổ làm bài (${antiCheatCounters.blurCount} lần vi phạm!)`)
}

const handleVisibilityChange = () => {
  if (state.value !== 'taking' || isLockedOut.value) return
  if (document.hidden) {
    antiCheatCounters.blurCount++
    message.error(`Cảnh báo gian lận: Vui lòng tập trung vào màn hình đề thi!`)
  }
}

const handleFullscreenChange = () => {
  if (state.value !== 'taking') return
  const isFullscreen = document.fullscreenElement || (document as any).webkitFullscreenElement
  if (!isFullscreen) {
    antiCheatCounters.fullscreenExitCount++
    isLockedOut.value = true
    
    if (antiCheatCounters.fullscreenExitCount >= 3) {
      message.error('Bạn đã thoát toàn màn hình 3 lần. Bài thi sẽ bị chấm dứt lập tức!')
      submitExam(true)
    }
  } else {
    isLockedOut.value = false
  }
}

const requestFullscreen = () => {
  const docEl = document.documentElement
  if (docEl.requestFullscreen) {
    docEl.requestFullscreen()
  } else if ((docEl as any).webkitRequestFullscreen) {
    (docEl as any).webkitRequestFullscreen()
  }
}

const requestBackToFullscreen = () => {
  requestFullscreen()
  isLockedOut.value = false
}

// Exam actions
const isMultipleChoice = (q: Question) => {
  return q.answers.filter(a => a.isCorrectAnswer).length > 1
}

const isChosen = (q: Question, ans: Answer) => {
  return q.chosenAnswerIds.includes(ans.id)
}

const chooseAnswer = (q: Question, ans: Answer) => {
  if (isLockedOut.value) return

  if (isMultipleChoice(q)) {
    const idx = q.chosenAnswerIds.indexOf(ans.id)
    if (idx > -1) {
      q.chosenAnswerIds.splice(idx, 1)
    } else {
      q.chosenAnswerIds.push(ans.id)
    }
  } else {
    q.chosenAnswerIds = [ans.id]
  }
}

const getNavBtnClass = (idx: number) => {
  const q = questions.value[idx]
  if (!q) return 'btn-outline-secondary'
  const chosen = q.chosenAnswerIds.length > 0
  return chosen ? 'btn-success text-white' : 'btn-outline-secondary'
}

const getLevelText = (level: number) => {
  if (level === 2) return 'Khó'
  if (level === 1) return 'Trung bình'
  return 'Dễ'
}

const scrollToQuestion = (idx: number) => {
  const el = document.getElementById(`q-block-${idx}`)
  if (el) {
    const y = el.getBoundingClientRect().top + window.scrollY - 80
    window.scrollTo({ top: y, behavior: 'smooth' })
  }
}

const handleExamTimeOut = () => {
  Modal.warning({
    title: 'Hết thời gian làm bài!',
    content: 'Đã hết thời gian thi quy định. Bài thi của bạn sẽ được nộp tự động.',
    onOk: () => {
      submitExam(true)
    }
  })
}

const submitExam = (bypassConfirm = false) => {
  const performSubmit = () => {
    clearInterval(examTimer)
    removeAntiCheatListeners()
    
    // Exit Fullscreen safely
    if (document.fullscreenElement) {
      document.exitFullscreen()
    }

    // Calculate score
    let correctCount = 0
    questions.value.forEach(q => {
      if (isMultipleChoice(q)) {
        const correctAnsIds = q.answers.filter(a => a.isCorrectAnswer).map(a => a.id)
        const isCorrect = correctAnsIds.length === q.chosenAnswerIds.length &&
                          correctAnsIds.every(id => q.chosenAnswerIds.includes(id))
        if (isCorrect) correctCount++
      } else {
        const chosenId = q.chosenAnswerIds[0] || null
        const chosen = q.answers.find(a => a.id === chosenId)
        if (chosen && chosen.isCorrectAnswer) correctCount++
      }
    })

    const score = parseFloat(((correctCount / questions.value.length) * 10).toFixed(2))

    // Create CandidateAttempt
    const attemptId = 'attempt_' + Date.now()
    const attempt = {
      attemptId,
      quizId: currentQuiz.value?.id || '',
      candidateName: candidateName.value,
      startedAt: new Date(Date.now() - (currentQuiz.value?.antiCheat?.fullscreen ? 10 : 0)).toISOString(),
      submittedAt: new Date().toISOString(),
      answers: questions.value.map(q => ({
        questionId: q.id,
        selectedAnswerIndexes: q.chosenAnswerIds.map(id => q.answers.findIndex(a => a.id === id))
      })),
      antiCheatMetrics: {
        blurCount: antiCheatCounters.blurCount,
        fullscreenExitCount: antiCheatCounters.fullscreenExitCount,
        violatingKeysPressed: []
      },
      score,
      status: antiCheatCounters.fullscreenExitCount >= 3 ? 'disqualified' : 'submitted'
    }

    // Save to cn_history
    const storedHistory = localStorage.getItem('cn_history')
    const historyList = storedHistory ? JSON.parse(storedHistory) : []
    historyList.unshift(attempt)
    localStorage.setItem('cn_history', JSON.stringify(historyList))

    message.success('Đã nộp bài thi thành công!')

    // Redirect to results detail
    router.push({ name: 'quiz-result', params: { id: attemptId } })
  }

  if (bypassConfirm) {
    performSubmit()
    return
  }

  Modal.confirm({
    title: 'Nộp bài thi của bạn?',
    content: `Bạn đã làm xong ${answeredCount.value}/${questions.value.length} câu hỏi. Bạn có chắc chắn muốn nộp bài thi ngay bây giờ?`,
    okText: 'Nộp bài',
    cancelText: 'Hủy',
    onOk: () => {
      performSubmit()
    }
  })
}
</script>

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

.bg-indigo {
  background-color: #4f46e5 !important;
}

.border-indigo-accent {
  border-color: rgba(99, 102, 241, 0.3) !important;
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  border: none;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
}

.bg-warning-soft {
  background-color: rgba(245, 158, 11, 0.12);
}

.bg-danger-soft {
  background-color: rgba(239, 68, 68, 0.1);
}

.option-check-item {
  border: 1.5px solid #e2e8f0;
  cursor: pointer;
}

.option-check-item:hover {
  border-color: #cbd5e1;
  background-color: #f8fafc;
}

.border-indigo-accent-solid {
  border-color: #6366f1 !important;
}

.questions-navigation {
  max-height: 250px;
  overflow-y: auto;
}

.nav-questions-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 6px;
}

.nav-q-btn {
  font-size: 0.8rem;
  padding: 8px 0;
  border: 1px solid #cbd5e1;
}

.sticky-sidebar {
  position: sticky;
  top: 88px;
  z-index: 10;
}

.pulse-btn {
  animation: pulse-animation 2s infinite;
}

@keyframes pulse-animation {
  0% { transform: scale(1); box-shadow: 0 0 0 0 rgba(99, 102, 241, 0.5); }
  70% { transform: scale(1.05); box-shadow: 0 0 0 10px rgba(99, 102, 241, 0); }
  100% { transform: scale(1); box-shadow: 0 0 0 0 rgba(99, 102, 241, 0); }
}

.countdown-circle {
  border: 4px solid #4f46e5;
  animation: border-color-pulse 2s infinite;
}

@keyframes border-color-pulse {
  0% { border-color: #4f46e5; }
  50% { border-color: #ef4444; }
  100% { border-color: #4f46e5; }
}

.fullscreen-lockout {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(15, 23, 42, 0.95);
  z-index: 9999;
}

.max-w-500 {
  max-width: 500px;
}

.fs-7 {
  font-size: 0.85rem;
}

.fs-8 {
  font-size: 0.78rem;
}

.fs-9 {
  font-size: 0.7rem;
}
</style>
