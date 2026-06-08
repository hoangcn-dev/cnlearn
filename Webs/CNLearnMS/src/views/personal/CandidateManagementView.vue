<template>
  <div class="candidate-management-view py-3">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-4">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/personal/exams">Quản lý đề & kỳ thi</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Quản lý phòng thi</li>
      </ol>
    </nav>

    <!-- Header info -->
    <div v-if="quiz" class="d-flex align-items-center justify-content-between mb-4 flex-wrap gap-3">
      <div class="d-flex align-items-center">
        <button class="btn btn-icon-back me-2 d-flex align-items-center justify-content-center" @click="goBack" title="Quay lại">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="feather feather-arrow-left" viewBox="0 0 24 24">
            <line x1="19" y1="12" x2="5" y2="12"></line>
            <polyline points="12 19 5 12 12 5"></polyline>
          </svg>
        </button>
        <div>
          <h1 class="fs-4 fw-bold text-dark-blue mb-1">Quản lý phòng thi: {{ quiz.title }}</h1>
          <p class="text-secondary small mb-0">Đối tượng tham gia: <span class="fw-bold">{{ quiz.targetGroup }}</span> | Mã phòng thi: <span class="font-monospace text-indigo fw-bold">{{ quiz.id }}</span></p>
        </div>
      </div>
      <div class="d-flex gap-2">
        <button class="btn btn-indigo text-white px-4 py-2 rounded-3 fw-semibold shadow-sm" @click="goToResults">
          Xem kết quả
        </button>
        <button class="btn btn-outline-indigo px-4 py-2 rounded-3 fw-semibold text-indigo" @click="exportCSV">
          Xuất báo cáo kết quả (CSV)
        </button>
      </div>
    </div>

    <!-- ROOM CONTROL BAR -->
    <div v-if="quiz" class="card border-0 rounded-4 shadow-sm p-4 bg-white mb-4">
      <div class="row align-items-center g-3">
        <div class="col-lg-5 col-md-6">
          <div class="d-flex align-items-center gap-3">
            <div class="rounded-circle bg-light d-flex align-items-center justify-content-center text-indigo" style="width: 48px; height: 48px;">
              <!-- Timer SVG Icon -->
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" viewBox="0 0 24 24">
                <circle cx="12" cy="12" r="10"></circle>
                <polyline points="12 6 12 12 16 14"></polyline>
              </svg>
            </div>
            <div>
              <div class="text-secondary small">Thời gian còn lại của phòng thi</div>
              <div class="fs-4 fw-bold" :class="isExamEnded ? 'text-danger' : 'text-dark-blue'">
                {{ countdownText }}
              </div>
            </div>
          </div>
        </div>
        
        <div class="col-lg-7 col-md-6 d-flex justify-content-md-end align-items-center gap-2 flex-wrap">
          <!-- End immediately -->
          <button 
            v-if="!isExamEnded" 
            class="btn btn-outline-danger px-3 py-2 rounded-3 fw-semibold small fs-8"
            @click="handleEndImmediately"
          >
            Kết thúc ngay
          </button>
          
          <!-- Extend custom time -->
          <button 
            v-if="!isExamEnded" 
            class="btn btn-outline-indigo px-3 py-2 rounded-3 fw-semibold small fs-8"
            @click="openExtendModal"
          >
            Gia hạn phòng thi
          </button>
          
          <!-- View results -->
          <button 
            v-if="isExamEnded" 
            class="btn btn-indigo text-white px-4 py-2 rounded-3 fw-semibold shadow-sm small fs-8"
            @click="goToResults"
          >
            Xem kết quả chi tiết
          </button>
        </div>
      </div>
    </div>

    <!-- STATS CARDS -->
    <div class="row g-3 mb-4">
      <div class="col-md-3 col-sm-6">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-3 h-100">
          <div class="text-secondary small">Tổng số thí sinh</div>
          <div class="fs-3 fw-bold text-dark-blue mt-1">{{ totalCandidatesCount }}</div>
          <div class="text-success small mt-1">● Học sinh đăng ký</div>
        </div>
      </div>
      <div class="col-md-3 col-sm-6">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-3 h-100">
          <div class="text-secondary small">Điểm số trung bình</div>
          <div class="fs-3 fw-bold text-indigo mt-1">{{ averageScore }} <span class="fs-6 text-muted">/10</span></div>
          <div class="text-secondary small mt-1">Tính trên bài đã nộp</div>
        </div>
      </div>
      <div class="col-md-3 col-sm-6">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-3 h-100">
          <div class="text-secondary small">Cảnh báo gian lận</div>
          <div class="fs-3 fw-bold text-warning mt-1">{{ totalViolationsCount }} <span class="fs-6 text-muted">lần</span></div>
          <div class="text-danger small mt-1" v-if="totalViolationsCount > 0">Cần lưu ý đặc biệt</div>
          <div class="text-secondary small mt-1" v-else>Phòng thi trung thực</div>
        </div>
      </div>
      <div class="col-md-3 col-sm-6">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-3 h-100">
          <div class="text-secondary small">Số bài bị đình chỉ (Bị loại)</div>
          <div class="fs-3 fw-bold text-danger mt-1">{{ disqualifiedCount }}</div>
          <div class="text-secondary small mt-1">Vi phạm quy chế thi</div>
        </div>
      </div>
    </div>

    <!-- TAB NAVIGATION -->
    <ul class="nav nav-tabs mb-4 border-bottom border-light">
      <li class="nav-item">
        <button 
          class="nav-link px-4 py-2.5 fw-semibold border-0" 
          :class="{ 'text-indigo bg-transparent active border-bottom border-indigo border-3': activeTab === 'candidates', 'text-secondary': activeTab !== 'candidates' }"
          @click="activeTab = 'candidates'"
        >
          Trạng thái thí sinh
        </button>
      </li>
      <li class="nav-item">
        <button 
          class="nav-link px-4 py-2.5 fw-semibold border-0 position-relative" 
          :class="{ 'text-indigo bg-transparent active border-bottom border-indigo border-3': activeTab === 'violations', 'text-secondary': activeTab !== 'violations' }"
          @click="activeTab = 'violations'"
        >
          Nhật ký vi phạm thời gian thực
          <span v-if="violationEvents.length > 0" class="badge rounded-pill bg-danger ms-2 fs-9">
            {{ violationEvents.length }}
          </span>
        </button>
      </li>
    </ul>

    <!-- TAB CONTENT: CANDIDATES -->
    <div v-if="activeTab === 'candidates'" class="card border-0 rounded-4 shadow-sm bg-white p-4">
      <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
        <h3 class="h6 fw-bold text-dark-blue mb-0">Danh sách học viên trực tiếp</h3>
        <div class="d-flex align-items-center gap-2">
          <input 
            v-model="searchQuery" 
            type="text" 
            class="form-control form-control-xs py-1.5 px-3 rounded" 
            placeholder="Tìm kiếm học viên..." 
            style="width: 240px; font-size: 0.8rem;" 
          />
        </div>
      </div>

      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead class="table-light">
            <tr>
              <th scope="col">Học viên</th>
              <th scope="col" style="width: 120px;">Mốc thời gian</th>
              <th scope="col" style="width: 150px;">Thời gian làm bài</th>
              <th scope="col" style="width: 100px;">Điểm số</th>
              <th scope="col" style="width: 140px;">Số lỗi vi phạm</th>
              <th scope="col" style="width: 120px;">Trạng thái</th>
              <th scope="col" class="text-end" style="width: 200px;">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in filteredAttempts" :key="c.attemptId">
              <td>
                <div class="d-flex align-items-center gap-2">
                  <div class="rounded-circle bg-light d-flex align-items-center justify-content-center text-secondary fw-bold" style="width: 32px; height: 32px; font-size: 0.8rem;">
                    {{ getInitials(c.candidateName) }}
                  </div>
                  <div>
                    <div class="fw-bold text-dark-blue">{{ c.candidateName }}</div>
                    <div class="text-muted fs-8">{{ c.attemptId }}</div>
                  </div>
                </div>
              </td>
              <td class="small">
                <div>Bắt đầu: <span class="text-secondary">{{ formatDate(c.startedAt) }}</span></div>
                <div v-if="c.submittedAt">Nộp bài: <span class="text-secondary">{{ formatDate(c.submittedAt) }}</span></div>
                <div v-else class="text-danger animate-pulse">Đang làm bài...</div>
              </td>
              <td class="small fw-semibold text-secondary">
                {{ getDurationText(c) }}
              </td>
              <td class="fw-bold" :class="c.status === 'disqualified' ? 'text-danger' : 'text-indigo'">
                {{ c.status === 'disqualified' ? '0.00' : (c.score !== undefined ? c.score.toFixed(2) : '--') }}
              </td>
              <td>
                <span :class="((c.antiCheatMetrics?.blurCount || 0) + (c.antiCheatMetrics?.fullscreenExitCount || 0)) > 0 ? 'text-danger fw-bold' : 'text-secondary'">
                  {{ (c.antiCheatMetrics?.blurCount || 0) + (c.antiCheatMetrics?.fullscreenExitCount || 0) }} lần
                </span>
              </td>
              <td>
                <span v-if="c.status === 'disqualified'" class="badge badge-danger-soft">Bị loại</span>
                <span v-else-if="!c.submittedAt" class="badge badge-warning-soft">Đang thi</span>
                <span v-else class="badge badge-success-soft">Đã nộp</span>
              </td>
              <td class="text-end">
                <div class="d-flex justify-content-end gap-1.5">
                  <button 
                    v-if="!c.submittedAt" 
                    class="btn btn-xs btn-outline-indigo px-2.5 py-1 rounded small" 
                    @click="forceSubmit(c.attemptId)"
                  >
                    Thu bài
                  </button>
                  <button 
                    v-if="c.status !== 'disqualified'" 
                    class="btn btn-xs btn-outline-danger px-2.5 py-1 rounded small" 
                    @click="disqualifyCandidate(c.attemptId)"
                  >
                    Đình chỉ
                  </button>
                  <span v-else class="text-muted fs-8 pe-2 select-none">Đã xử lý vi phạm</span>
                </div>
              </td>
            </tr>
            <tr v-if="filteredAttempts.length === 0">
              <td colspan="7" class="text-center py-4 text-secondary small">
                Chưa có học viên nào tham gia phòng thi này.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- TAB CONTENT: REAL-TIME VIOLATIONS -->
    <div v-if="activeTab === 'violations'" class="card border-0 rounded-4 shadow-sm bg-white p-4">
      <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
        <h3 class="h6 fw-bold text-dark-blue mb-0">Nhật ký vi phạm quy chế thi (Thời gian thực)</h3>
        <div class="d-flex align-items-center gap-2">
          <input 
            v-model="violationSearchQuery" 
            type="text" 
            class="form-control form-control-xs py-1.5 px-3 rounded" 
            placeholder="Lọc theo tên thí sinh..." 
            style="width: 240px; font-size: 0.8rem;" 
          />
        </div>
      </div>

      <div class="table-responsive">
        <table class="table table-hover align-middle">
          <thead class="table-light">
            <tr>
              <th scope="col" style="width: 180px;">Thời gian ghi nhận</th>
              <th scope="col">Thí sinh</th>
              <th scope="col" style="width: 220px;">Loại hành vi</th>
              <th scope="col" style="width: 150px;">Chi tiết</th>
              <th scope="col" class="text-end" style="width: 180px;">Hành động khẩn cấp</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(evt, idx) in filteredViolationEvents" :key="idx">
              <td class="font-monospace text-secondary small">{{ evt.timestamp }}</td>
              <td>
                <div class="fw-bold text-dark-blue">{{ evt.candidateName }}</div>
                <div class="text-muted fs-8">Mã tham gia: {{ evt.attemptId }}</div>
              </td>
              <td>
                <span :class="evt.violationType.includes('màn hình') ? 'badge badge-danger-soft fw-semibold' : 'badge badge-warning-soft fw-semibold'">
                  {{ evt.violationType }}
                </span>
              </td>
              <td class="text-secondary small">{{ evt.details }}</td>
              <td class="text-end">
                <button 
                  class="btn btn-xs btn-outline-danger px-2.5 py-1 rounded small" 
                  @click="disqualifyCandidate(evt.attemptId)"
                >
                  Đình chỉ thi
                </button>
              </td>
            </tr>
            <tr v-if="filteredViolationEvents.length === 0">
              <td colspan="5" class="text-center py-4 text-secondary small">
                Chưa ghi nhận hành vi vi phạm quy chế nào từ phòng thi này.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Gia hạn thời gian -->
    <a-modal
      v-model:visible="extendModalVisible"
      title="Gia hạn thời gian phòng thi"
      @ok="submitExtension"
      okText="Gia hạn"
      cancelText="Hủy"
    >
      <div class="py-2">
        <label class="form-label fw-bold text-dark-blue small">Nhập số phút muốn gia hạn thêm:</label>
        <a-input-number 
          v-model:value="extendMinutes" 
          :min="1" 
          :max="1440" 
          style="width: 100%;" 
          size="large"
          placeholder="Ví dụ: 15"
        />
        <div class="text-secondary small mt-2">
          Thời gian kết thúc hiện tại: <span class="fw-bold">{{ formatDateTime(quiz?.endDate) }}</span>
        </div>
        <div class="text-indigo small mt-1" v-if="extendMinutes && extendMinutes > 0">
          Thời gian kết thúc mới (dự kiến): <span class="fw-bold">{{ getNewEndDateTimeText() }}</span>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'

interface Attempt {
  attemptId: string
  quizId: string
  candidateName: string
  startedAt: string
  submittedAt?: string
  score?: number
  status: 'doing' | 'submitted' | 'disqualified'
  antiCheatMetrics: {
    blurCount: number
    fullscreenExitCount: number
  }
}

interface Quiz {
  id: string
  title: string
  targetGroup: string
  startDate: string
  endDate: string
  examId: string
}

const route = useRoute()
const router = useRouter()

const quizId = ref(route.params.id as string)
const quiz = ref<Quiz | null>(null)
const attempts = ref<Attempt[]>([])
const searchQuery = ref('')
const violationSearchQuery = ref('')

const activeTab = ref('candidates')
const now = ref(Date.now())
let timerId: any = null

const getDurationText = (c: Attempt) => {
  const start = new Date(c.startedAt).getTime()
  const end = c.submittedAt ? new Date(c.submittedAt).getTime() : now.value
  const diffMs = end - start
  if (diffMs <= 0) return '0 giây'
  const m = Math.floor(diffMs / 60000)
  const s = Math.floor((diffMs % 60000) / 1000)
  if (m === 0) return `${s} giây`
  return `${m} phút ${s} giây`
}

interface ViolationEvent {
  timestamp: string
  candidateName: string
  violationType: string
  details: string
  attemptId: string
}

const violationEvents = computed(() => {
  const list: ViolationEvent[] = []
  attempts.value.forEach(c => {
    const start = new Date(c.startedAt).getTime()
    
    // Blur violations
    for (let i = 1; i <= (c.antiCheatMetrics?.blurCount || 0); i++) {
      const eventTime = new Date(start + i * 45000)
      list.push({
        timestamp: eventTime.toLocaleString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' }),
        candidateName: c.candidateName,
        violationType: 'Mất tập trung / Chuyển tab',
        details: `Lần thứ ${i}`,
        attemptId: c.attemptId
      })
    }

    // Fullscreen exit violations
    for (let i = 1; i <= (c.antiCheatMetrics?.fullscreenExitCount || 0); i++) {
      const eventTime = new Date(start + i * 65000)
      list.push({
        timestamp: eventTime.toLocaleString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' }),
        candidateName: c.candidateName,
        violationType: 'Thoát toàn màn hình',
        details: `Lần thứ ${i}`,
        attemptId: c.attemptId
      })
    }
  })
  
  return list.sort((a, b) => b.timestamp.localeCompare(a.timestamp))
})

const filteredViolationEvents = computed(() => {
  const q = violationSearchQuery.value.trim().toLowerCase()
  if (!q) return violationEvents.value
  return violationEvents.value.filter(evt => evt.candidateName.toLowerCase().includes(q))
})

const isExamEnded = computed(() => {
  if (!quiz.value) return true
  const end = new Date(quiz.value.endDate).getTime()
  return now.value >= end
})

const countdownText = computed(() => {
  if (!quiz.value) return '00:00:00'
  const end = new Date(quiz.value.endDate).getTime()
  const diffMs = end - now.value
  if (diffMs <= 0) {
    return 'Đã kết thúc'
  }
  
  const secs = Math.floor(diffMs / 1000)
  const days = Math.floor(secs / 86400)
  const hours = Math.floor((secs % 86400) / 3600)
  const mins = Math.floor((secs % 3600) / 60)
  const remainingSecs = secs % 60
  
  const pad = (num: number) => String(num).padStart(2, '0')
  
  if (days > 0) {
    return `${days} ngày ${pad(hours)}:${pad(mins)}:${pad(remainingSecs)}`
  }
  return `${pad(hours)}:${pad(mins)}:${pad(remainingSecs)}`
})

const saveQuiz = (updatedQuiz: Quiz) => {
  const storedQuizzes = localStorage.getItem('cn_quizzes')
  const quizzesList = storedQuizzes ? JSON.parse(storedQuizzes) : []
  const idx = quizzesList.findIndex((q: any) => q.id === updatedQuiz.id)
  if (idx !== -1) {
    quizzesList[idx] = updatedQuiz
    localStorage.setItem('cn_quizzes', JSON.stringify(quizzesList))
  }
}

const handleEndImmediately = () => {
  if (!quiz.value) return
  Modal.confirm({
    title: 'Xác nhận kết thúc phòng thi ngay',
    content: 'Tất cả học viên đang làm bài chưa nộp sẽ tự động bị thu bài. Bạn có chắc chắn?',
    okText: 'Kết thúc ngay',
    okType: 'danger',
    cancelText: 'Hủy',
    onOk() {
      if (!quiz.value) return
      const updated = { ...quiz.value, endDate: new Date().toISOString() }
      quiz.value = updated
      saveQuiz(updated)
      
      attempts.value.forEach(c => {
        if (c.status === 'doing') {
          c.status = 'submitted'
          c.submittedAt = new Date().toISOString()
          c.score = c.score !== undefined ? c.score : 6.0
        }
      })
      saveAttempts()
      
      message.success('Đã kết thúc phòng thi và thu bài tất cả học sinh!')
    }
  })
}

const extendModalVisible = ref(false)
const extendMinutes = ref<number>(15)

const formatDateTime = (dateStr?: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleString('vi-VN')
}

const getNewEndDateTimeText = () => {
  if (!quiz.value || !extendMinutes.value) return ''
  const currentEnd = new Date(quiz.value.endDate).getTime()
  const newEnd = new Date(currentEnd + extendMinutes.value * 60000)
  return newEnd.toLocaleString('vi-VN')
}

const openExtendModal = () => {
  extendMinutes.value = 15
  extendModalVisible.value = true
}

const submitExtension = () => {
  if (!quiz.value) return
  if (!extendMinutes.value || extendMinutes.value <= 0) {
    message.warning('Vui lòng nhập số phút hợp lệ!')
    return
  }
  
  const currentEnd = new Date(quiz.value.endDate).getTime()
  const newEnd = new Date(currentEnd + extendMinutes.value * 60000).toISOString()
  const updated = { ...quiz.value, endDate: newEnd }
  quiz.value = updated
  saveQuiz(updated)
  
  extendModalVisible.value = false
  message.success(`Đã gia hạn thành công thêm ${extendMinutes.value} phút cho phòng thi!`)
}

const goToResults = () => {
  router.push({ name: 'quiz-result', params: { id: quizId.value } })
}

onMounted(() => {
  // Load Quiz
  const storedQuizzes = localStorage.getItem('cn_quizzes')
  const quizzesList = storedQuizzes ? JSON.parse(storedQuizzes) : []
  const found = quizzesList.find((q: any) => q.id === quizId.value)
  if (!found) {
    message.error('Không tìm thấy dữ liệu kỳ thi này!')
    router.push('/personal/exams')
    return
  }
  quiz.value = found

  // Load candidate attempts from history
  loadAttempts()

  // Start real-time timer
  timerId = window.setInterval(() => {
    now.value = Date.now()
  }, 1000)
})

onUnmounted(() => {
  if (timerId) {
    clearInterval(timerId)
  }
})

const loadAttempts = () => {
  const storedHistory = localStorage.getItem('cn_history')
  const historyList = storedHistory ? JSON.parse(storedHistory) : []
  
  // Filter attempts matching the current quiz ID
  let filtered = historyList.filter((h: any) => h.quizId === quizId.value)

  // If empty, generate some mock candidates to make it realistic for host view
  if (filtered.length === 0) {
    filtered = [
      {
        attemptId: 'attempt_mock_1',
        quizId: quizId.value,
        candidateName: 'Nguyễn Hữu Đạt',
        startedAt: new Date(Date.now() - 1200000).toISOString(),
        submittedAt: new Date(Date.now() - 200000).toISOString(),
        score: 9.33,
        status: 'submitted',
        antiCheatMetrics: { blurCount: 0, fullscreenExitCount: 0 }
      },
      {
        attemptId: 'attempt_mock_2',
        quizId: quizId.value,
        candidateName: 'Phạm Thùy Linh',
        startedAt: new Date(Date.now() - 1500000).toISOString(),
        submittedAt: new Date(Date.now() - 100000).toISOString(),
        score: 8.67,
        status: 'submitted',
        antiCheatMetrics: { blurCount: 1, fullscreenExitCount: 0 }
      },
      {
        attemptId: 'attempt_mock_3',
        quizId: quizId.value,
        candidateName: 'Lê Mai Anh',
        startedAt: new Date(Date.now() - 800000).toISOString(),
        submittedAt: new Date(Date.now() - 50000).toISOString(),
        score: 8.00,
        status: 'submitted',
        antiCheatMetrics: { blurCount: 0, fullscreenExitCount: 0 }
      },
      {
        attemptId: 'attempt_mock_4',
        quizId: quizId.value,
        candidateName: 'Vũ Đức Thịnh',
        startedAt: new Date(Date.now() - 600000).toISOString(),
        status: 'doing',
        antiCheatMetrics: { blurCount: 4, fullscreenExitCount: 2 }
      },
      {
        attemptId: 'attempt_mock_5',
        quizId: quizId.value,
        candidateName: 'Trần Quốc Bảo',
        startedAt: new Date(Date.now() - 1000000).toISOString(),
        status: 'doing',
        antiCheatMetrics: { blurCount: 1, fullscreenExitCount: 1 }
      }
    ]
    localStorage.setItem('cn_history', JSON.stringify([...filtered, ...historyList]))
  }

  attempts.value = filtered
}

const saveAttempts = () => {
  // Update attempts in global history storage
  const storedHistory = localStorage.getItem('cn_history')
  let historyList = storedHistory ? JSON.parse(storedHistory) : []
  
  // Replace matching ones
  historyList = historyList.filter((h: any) => h.quizId !== quizId.value)
  localStorage.setItem('cn_history', JSON.stringify([...attempts.value, ...historyList]))
}

// Stats computations
const totalCandidatesCount = computed(() => attempts.value.length)

const averageScore = computed(() => {
  const submitted = attempts.value.filter(a => a.status === 'submitted' && a.score !== undefined)
  if (submitted.length === 0) return '0.0'
  const sum = submitted.reduce((acc, curr) => acc + (curr.score || 0), 0)
  return (sum / submitted.length).toFixed(2)
})

const totalViolationsCount = computed(() => {
  return attempts.value.reduce((acc, curr) => {
    return acc + (curr.antiCheatMetrics?.blurCount || 0) + (curr.antiCheatMetrics?.fullscreenExitCount || 0)
  }, 0)
})

const disqualifiedCount = computed(() => {
  return attempts.value.filter(a => a.status === 'disqualified').length
})

const filteredAttempts = computed(() => {
  if (!searchQuery.value.trim()) return attempts.value
  const q = searchQuery.value.toLowerCase()
  return attempts.value.filter(a => a.candidateName.toLowerCase().includes(q))
})

// Helper methods
const getInitials = (name: string) => {
  return name.split(' ').map(n => n.charAt(0)).join('').toUpperCase().slice(0, 2)
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
}

const goBack = () => {
  router.push('/personal/exams')
}

// Operations
const forceSubmit = (attemptId: string) => {
  Modal.confirm({
    title: 'Xác nhận thu bài thi bắt buộc',
    content: 'Bạn có chắc chắn muốn buộc nộp bài làm của thí sinh này ngay lập tức?',
    okText: 'Thu bài',
    cancelText: 'Hủy',
    onOk() {
      const idx = attempts.value.findIndex(a => a.attemptId === attemptId)
      if (idx !== -1) {
        const item = attempts.value[idx]
        if (item) {
          item.status = 'submitted'
          item.submittedAt = new Date().toISOString()
          item.score = 6.5 // Assign mockup final score
          saveAttempts()
          message.success('Đã thu bài thi thành công!')
        }
      }
    }
  })
}

const disqualifyCandidate = (attemptId: string) => {
  Modal.confirm({
    title: 'Xác nhận đình chỉ thi học viên',
    content: 'Hành động này sẽ chấm dứt bài thi của thí sinh ngay lập tức với trạng thái BỊ LOẠI (0 điểm). Bạn có chắc chắn?',
    okText: 'Đình chỉ',
    okType: 'danger',
    cancelText: 'Hủy',
    onOk() {
      const idx = attempts.value.findIndex(a => a.attemptId === attemptId)
      if (idx !== -1) {
        const item = attempts.value[idx]
        if (item) {
          item.status = 'disqualified'
          item.submittedAt = new Date().toISOString()
          item.score = 0.0
          saveAttempts()
          message.success('Đã đình chỉ thí sinh thành công!')
        }
      }
    }
  })
}

const exportCSV = () => {
  if (attempts.value.length === 0) {
    message.warning('Phòng thi chưa có dữ liệu kết quả để xuất tệp!')
    return
  }

  const data = [
    ['HẠNG', 'HỌ VÀ TÊN THÍ SINH', 'MÃ THI', 'ĐIỂM SỐ', 'CHUYỂN TAB', 'THOÁT TOÀN MÀN HÌNH', 'TRẠNG THÁI']
  ]

  // Sort candidates by score
  const sorted = [...attempts.value].sort((a, b) => {
    const scoreA = a.score !== undefined ? a.score : -1
    const scoreB = b.score !== undefined ? b.score : -1
    return scoreB - scoreA
  })

  sorted.forEach((row, index) => {
    data.push([
      (index + 1).toString(),
      row.candidateName,
      row.attemptId,
      row.status === 'disqualified' ? '0.00' : (row.score !== undefined ? row.score.toFixed(2) : '--'),
      (row.antiCheatMetrics?.blurCount || 0).toString(),
      (row.antiCheatMetrics?.fullscreenExitCount || 0).toString(),
      row.status === 'disqualified' ? 'Bị loại' : (row.submittedAt ? 'Đã nộp' : 'Đang thi')
    ])
  })

  const csvContent = "\uFEFF" + data.map(e => e.join(",")).join("\n")
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement("a")
  link.setAttribute("href", url)
  link.setAttribute("download", `BaoCao_PhongThi_${quiz.value?.id || 'export'}.csv`)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  message.success('Đã xuất báo cáo CSV phòng thi thành công!')
}
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.text-indigo {
  color: #4f46e5;
}

.btn-indigo {
  background-color: #4f46e5;
  color: white !important;
  border: 1px solid #4f46e5;
  transition: all 0.15s ease-in-out;
}

.btn-indigo:hover {
  background-color: #4338ca;
  border-color: #4338ca;
  color: white !important;
}

.btn-outline-indigo {
  color: #4f46e5;
  border-color: #6366f1;
}

.btn-outline-indigo:hover {
  background-color: #4f46e5;
  color: white;
}

.badge-warning-soft {
  background-color: #fef3c7;
  color: #b45309;
}

.badge-danger-soft {
  background-color: #fee2e2;
  color: #b91c1c;
}

.badge-success-soft {
  background-color: #dcfce7;
  color: #15803d;
}

.btn-icon-back {
  background: none;
  border: none;
  color: #6b7280;
  padding: 6px;
  border-radius: 50%;
  transition: all 0.22s ease-in-out;
  cursor: pointer;
}
.btn-icon-back:hover {
  background-color: rgba(99, 102, 241, 0.08);
  color: #4f46e5;
  transform: translateX(-3px);
}

.animate-pulse {
  animation: pulse-dot 1.5s infinite;
}

@keyframes pulse-dot {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}

.fs-8 {
  font-size: 0.76rem;
}
</style>
