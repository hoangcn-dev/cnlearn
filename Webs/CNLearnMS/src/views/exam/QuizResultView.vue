<template>
  <div class="quiz-result-container py-4">


    <div v-if="attempt" class="row g-4 justify-content-center">
      <!-- Cột trái: Thẻ điểm cá nhân & Phân tích gian lận -->
      <div class="col-lg-4 col-md-5">
        <div class="card border-0 rounded-4 shadow bg-white p-4 text-center position-relative overflow-hidden mb-4">
          <div class="score-badge-decor"></div>
          <h2 class="fw-bold text-dark-blue mt-2 mb-1">Kết quả bài thi</h2>
          <p class="text-secondary small mb-4">Thí sinh: <span class="fw-bold text-indigo">{{ attempt.candidateName }}</span></p>

          <div class="d-inline-block bg-indigo-soft border border-indigo-accent rounded-4 px-5 py-4 mb-4">
            <div class="fs-1 fw-bold text-indigo leading-none mb-1">{{ attempt.score }}</div>
            <div class="text-secondary fw-semibold small">Thang điểm 10.0</div>
          </div>

          <div class="p-3 bg-light rounded-3 text-start small mb-4">
            <div class="d-flex justify-content-between mb-2 pb-2 border-bottom border-light-subtle">
              <span class="text-secondary">Trạng thái bài nộp:</span>
              <span v-if="attempt.status === 'disqualified'" class="badge bg-danger text-white fw-bold">Đình chỉ thi</span>
              <span v-else class="badge bg-success text-white fw-bold">Đã nộp bài</span>
            </div>
            <div class="d-flex justify-content-between mb-2">
              <span class="text-secondary">Thời gian nộp:</span>
              <span class="fw-semibold text-dark-blue">{{ formatDate(attempt.submittedAt) }}</span>
            </div>
            <div class="d-flex justify-content-between">
              <span class="text-secondary">Thời gian làm bài:</span>
              <span class="fw-semibold text-dark-blue">{{ timeDuration }}</span>
            </div>
          </div>

          <!-- Anti-cheat compliance summary -->
          <div class="p-3 bg-warning-soft text-warning rounded-3 text-start small mb-4">
            <div class="fw-bold text-dark-blue mb-2 d-flex align-items-center gap-1.5">
              Lịch sử chống gian lận:
            </div>
            <div class="d-flex justify-content-between mb-1.5">
              <span class="text-secondary-dark">Thoát tab/cửa sổ:</span>
              <span class="fw-bold text-danger">{{ attempt.antiCheatMetrics?.blurCount || 0 }} lần</span>
            </div>
            <div class="d-flex justify-content-between">
              <span class="text-secondary-dark">Thoát toàn màn hình:</span>
              <span class="fw-bold text-danger">{{ attempt.antiCheatMetrics?.fullscreenExitCount || 0 }} lần</span>
            </div>
            <div v-if="(attempt.antiCheatMetrics?.fullscreenExitCount ?? 0) >= 3 || attempt.status === 'disqualified'" class="mt-2 pt-2 border-top border-warning-subtle text-danger fw-semibold">
              Vi phạm quy chế thi vượt hạn mức!
            </div>
          </div>

          <!-- Actions -->
          <div class="d-flex flex-column gap-2">
            <button class="btn btn-indigo text-white w-100 py-2.5 rounded-pill fw-bold" @click="backToHome">
              Quay lại trang chủ
            </button>
          </div>
        </div>
      </div>

      <!-- Cột phải: Bảng xếp hạng bục vinh danh và Danh sách xếp hạng -->
      <div class="col-lg-8 col-md-7">
        <!-- PODIUM LAYOUT FOR TOP 3 -->
        <div class="card border-0 rounded-4 shadow bg-white p-4 mb-4">
          <h3 class="h5 fw-bold text-dark-blue mb-4 d-flex align-items-center gap-2">
            Top 3 thí sinh xuất sắc nhất
          </h3>

          <div class="d-flex justify-content-center align-items-end gap-3 py-3" style="min-height: 200px;">
            <!-- 2nd Place -->
            <div v-if="podium[1]" class="podium-column d-flex flex-column align-items-center">
              <div class="avatar-ring rounded-circle bg-light d-flex align-items-center justify-content-center mb-2" style="width: 48px; height: 48px; border: 2.5px solid #cbd5e1;">
                <span class="fw-bold text-secondary">{{ podium[1].avatar }}</span>
              </div>
              <div class="fw-bold text-dark-blue small text-center px-1">{{ podium[1].name }}</div>
              <div class="text-indigo fw-bold fs-7 mb-2">{{ podium[1].score }} đ</div>
              <div class="podium-step bg-secondary-subtle d-flex align-items-center justify-content-center" style="width: 80px; height: 80px;">
                <span class="fs-1 fw-bold text-secondary-dark opacity-50">2</span>
              </div>
            </div>

            <!-- 1st Place -->
            <div v-if="podium[0]" class="podium-column d-flex flex-column align-items-center">
              <div class="avatar-ring rounded-circle bg-warning-soft d-flex align-items-center justify-content-center mb-2" style="width: 58px; height: 58px; border: 3px solid #eab308;">
                <span class="fw-bold text-warning-dark">{{ podium[0].avatar }}</span>
              </div>
              <div class="fw-bold text-dark-blue small text-center px-1">{{ podium[0].name }}</div>
              <div class="text-indigo fw-bold fs-6 mb-2">{{ podium[0].score }} đ</div>
              <div class="podium-step bg-warning-subtle d-flex align-items-center justify-content-center" style="width: 90px; height: 110px;">
                <span class="fs-1 fw-bold text-warning-dark opacity-50">1</span>
              </div>
            </div>

            <!-- 3rd Place -->
            <div v-if="podium[2]" class="podium-column d-flex flex-column align-items-center">
              <div class="avatar-ring rounded-circle bg-orange-soft d-flex align-items-center justify-content-center mb-2" style="width: 44px; height: 44px; border: 2.5px solid #f97316;">
                <span class="fw-bold text-orange-dark">{{ podium[2].avatar }}</span>
              </div>
              <div class="fw-bold text-dark-blue small text-center px-1">{{ podium[2].name }}</div>
              <div class="text-indigo fw-bold fs-7 mb-2">{{ podium[2].score }} đ</div>
              <div class="podium-step bg-orange-subtle d-flex align-items-center justify-content-center" style="width: 80px; height: 60px;">
                <span class="fs-1 fw-bold text-orange-dark opacity-50">3</span>
              </div>
            </div>
          </div>
        </div>

        <!-- LEADERBOARD TABLE -->
        <div class="card border-0 rounded-4 shadow bg-white p-4">
          <h3 class="h5 fw-bold text-dark-blue mb-4 d-flex align-items-center justify-content-between flex-wrap gap-2">
            Bảng xếp hạng điểm phòng thi
            <button v-if="isQuizOwner" class="btn btn-sm btn-outline-indigo px-3 py-1 rounded" @click="exportAllCSV">
              Xuất kết quả ra file excel
            </button>
          </h3>

          <div class="table-responsive">
            <table class="table table-hover align-middle">
              <thead class="table-light">
                <tr>
                  <th scope="col" style="width: 60px;">Hạng</th>
                  <th scope="col">Họ và tên</th>
                  <th scope="col" style="width: 100px;">Điểm số</th>
                  <th scope="col" style="width: 120px;">Lượt vi phạm</th>
                  <th scope="col" style="width: 140px;">Trạng thái</th>
                </tr>
              </thead>
              <tbody>
                <tr 
                  v-for="(row, idx) in leaderboard" 
                  :key="row.name"
                  :class="{ 'table-indigo-highlight': row.name === attempt.candidateName }"
                >
                  <td class="fw-bold text-center">
                    <span v-if="idx === 0">1</span>
                    <span v-else-if="idx === 1">2</span>
                    <span v-else-if="idx === 2">3</span>
                    <span v-else>{{ idx + 1 }}</span>
                  </td>
                  <td>
                    <div class="d-flex align-items-center gap-2">
                      <div class="rounded-circle bg-light d-flex align-items-center justify-content-center text-secondary-dark fw-bold fs-8" style="width: 32px; height: 32px;">
                        {{ row.avatar }}
                      </div>
                      <div>
                        <div class="fw-bold text-dark-blue">{{ row.name }} <span v-if="row.name === attempt.candidateName" class="badge bg-indigo-soft text-indigo rounded fs-9">Bạn</span></div>
                        <div class="text-secondary fs-8">Thời gian nộp: {{ row.submittedAt }}</div>
                      </div>
                    </div>
                  </td>
                  <td class="fw-bold text-indigo">{{ row.score.toFixed(2) }}</td>
                  <td>
                    <span :class="row.violations > 0 ? 'text-danger fw-semibold' : 'text-secondary'">
                      {{ row.violations }} lần
                    </span>
                  </td>
                  <td>
                    <span :class="row.status === 'Bị loại' ? 'badge bg-danger-soft text-danger' : 'badge bg-success-soft text-success'">
                      {{ row.status }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Detailed Review Card -->
      <div v-if="attempt && quizQuestions.length > 0" class="col-12 mt-4">
        <div class="card border-0 rounded-4 shadow bg-white p-4 p-md-5">
          <h3 class="h5 fw-bold text-dark-blue mb-4 d-flex align-items-center gap-2">
            <span>📝</span> Xem chi tiết đáp án & Lời giải bài làm
          </h3>

          <div class="d-flex flex-column gap-4">
            <QuestionCard
              v-for="(q, idx) in quizQuestions" 
              :key="q.id || q.questionId"
              :question="q"
              :index="idx + 1"
              mode="result"
              :chosen-answer-indexes="getChosenAnswerIndexes(q)"
              :correct-answer-ids="correctAnswerMap[q.id || q.questionId]"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import QuestionCard from '@/components/QuestionCard.vue'
import { getQuestionAnswers, getQuestionKeys } from '@/api/questions'

interface Attempt {
  attemptId: string
  quizId: string
  candidateName: string
  startedAt: string
  submittedAt: string
  score: number
  status: 'doing' | 'submitted' | 'disqualified'
  antiCheatMetrics?: {
    blurCount: number
    fullscreenExitCount: number
  }
}

interface LeaderboardRow {
  name: string
  avatar: string
  score: number
  violations: number
  status: string
  submittedAt: string
}

const route = useRoute()
const router = useRouter()

const attempt = ref<any | null>(null)
const leaderboard = ref<LeaderboardRow[]>([])
const isQuizOwner = ref(false)

const quizQuestions = ref<any[]>([])
const correctAnswerMap = ref<Record<string, string[]>>({})

const getAttemptAnswerForQuestion = (questionId: string) => {
  if (!attempt.value || !attempt.value.answers) return null
  return attempt.value.answers.find((a: any) => a.questionId === questionId)
}

const getChosenAnswerIndexes = (q: any): number[] => {
  const ans = getAttemptAnswerForQuestion(q.id || q.questionId)
  return ans ? ans.selectedAnswerIndexes : []
}

// Compute Duration
const timeDuration = computed(() => {
  if (!attempt.value) return '--:--'
  const start = new Date(attempt.value.startedAt)
  const end = new Date(attempt.value.submittedAt)
  const diffMs = end.getTime() - start.getTime()
  if (diffMs <= 0) return '0 giây'
  const m = Math.floor(diffMs / 60000)
  const s = Math.floor((diffMs % 60000) / 1000)
  return `${m} phút ${s} giây`
})

// Podium Top 3 computed
const podium = computed(() => {
  return leaderboard.value.slice(0, 3)
})

onMounted(() => {
  const attemptId = route.params.id as string | undefined
  if (!attemptId) {
    message.error('Không xác định được mã kết quả!')
    router.push('/')
    return
  }

  // Load attempt from cn_history
  const storedHistory = localStorage.getItem('cn_history')
  const historyList = storedHistory ? JSON.parse(storedHistory) : []
  let found = historyList.find((h: any) => h.attemptId === attemptId)

  if (!found) {
    // Check if route.params.id is actually a quizId (meaning the user wants to view the general scoreboard of this room)
    const quizAttempts = historyList.filter((h: any) => h.quizId === attemptId)
    if (quizAttempts.length > 0) {
      found = quizAttempts.sort((a: any, b: any) => (b.score || 0) - (a.score || 0))[0]
    } else {
      const storedQuizzes = localStorage.getItem('cn_quizzes')
      const quizzesList = storedQuizzes ? JSON.parse(storedQuizzes) : []
      const quizExists = quizzesList.find((q: any) => q.id === attemptId)
      if (quizExists) {
        found = {
          attemptId: 'dummy_showcase',
          quizId: attemptId,
          candidateName: 'Nguyễn Hữu Đạt',
          startedAt: new Date(Date.now() - 3600000).toISOString(),
          submittedAt: new Date().toISOString(),
          score: 9.33,
          status: 'submitted',
          antiCheatMetrics: { blurCount: 0, fullscreenExitCount: 0 }
        }
      }
    }
  }

  if (!found) {
    message.error('Không tìm thấy dữ liệu kết quả thi!')
    router.push('/')
    return
  }

  attempt.value = found

  // Check if current user is owner of the quiz
  const storedQuizzes = localStorage.getItem('cn_quizzes')
  const quizzesList = storedQuizzes ? JSON.parse(storedQuizzes) : []
  const foundQuiz = quizzesList.find((q: any) => q.id === found.quizId)
  if (foundQuiz) {
    isQuizOwner.value = foundQuiz.isMyCreated === true
    const storedExams = localStorage.getItem('cn_exams')
    const examsList = storedExams ? JSON.parse(storedExams) : []
    const foundExam = examsList.find((e: any) => e.id === foundQuiz.examId)
    if (foundExam && foundExam.questions) {
      quizQuestions.value = foundExam.questions
    }
  }

  if (quizQuestions.value.length === 0) {
    // Fallback: seed some mock questions matching the general domain
    const chars = ['A', 'B', 'C', 'D']
    quizQuestions.value = [
      {
        id: 'q001',
        stringContent: 'Điểm khác biệt lớn nhất giữa IEnumerable và IQueryable là gì?',
        explanation: 'IEnumerable thực hiện lọc dữ liệu trên Client (In-Memory), còn IQueryable thực hiện lọc phía Server (Database).',
        answers: [
          { stringContent: 'IEnumerable lọc ở Client, IQueryable lọc ở Server Database', isCorrectAnswer: true },
          { stringContent: 'IEnumerable lọc ở Server, IQueryable lọc ở Client', isCorrectAnswer: false },
          { stringContent: 'IEnumerable thuộc .NET Framework cũ, IQueryable thuộc .NET Core', isCorrectAnswer: false }
        ].map((ans, idx) => ({ ...ans, indexChar: chars[idx] }))
      },
      {
        id: 'q002',
        stringContent: 'Cú pháp khai báo biến hằng trong C# là gì?',
        explanation: 'Biến hằng trong C# được khai báo bằng từ khóa const đứng trước kiểu dữ liệu.',
        answers: [
          { stringContent: 'const <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;', isCorrectAnswer: true },
          { stringContent: 'readonly <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;', isCorrectAnswer: false },
          { stringContent: 'static <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;', isCorrectAnswer: false }
        ].map((ans, idx) => ({ ...ans, indexChar: chars[idx] }))
      },
      {
        id: 'q003',
        stringContent: 'Một sóng âm truyền từ không khí vào nước thì:',
        explanation: 'Khi truyền qua các môi trường khác nhau, tần số của sóng âm không đổi, nhưng vận tốc truyền sóng đổi dẫn đến bước sóng đổi.',
        answers: [
          { stringContent: 'tần số không đổi, bước sóng tăng.', isCorrectAnswer: true },
          { stringContent: 'chu kỳ tăng, vận tốc giảm.', isCorrectAnswer: false },
          { stringContent: 'bước sóng không đổi, chu kỳ giảm.', isCorrectAnswer: false },
          { stringContent: 'tần số tăng, bước sóng giảm.', isCorrectAnswer: false }
        ].map((ans, idx) => ({ ...ans, indexChar: chars[idx] }))
      }
    ]
  }

  // Load answers and keys from APIs dynamically
  const fetchResultAnswersAndKeys = async () => {
    if (quizQuestions.value.length > 0) {
      try {
        const questionIds = quizQuestions.value.map((q: any) => q.id || q.questionId)
        const [answersRes, keysRes] = await Promise.all([
          getQuestionAnswers(questionIds),
          getQuestionKeys(questionIds)
        ])

        if (answersRes && answersRes.isSuccess && answersRes.data) {
          const allAnswers = answersRes.data
          quizQuestions.value.forEach((q: any) => {
            const qId = q.id || q.questionId
            const qAnswers = allAnswers.filter((a: any) => a.questionId === qId)
            if (qAnswers.length > 0) {
              q.answers = qAnswers.map((a: any, idx: number) => ({
                id: a.questionAnswerId || a.id,
                questionAnswerId: a.questionAnswerId || a.id,
                stringContent: a.stringContent,
                indexChar: String.fromCharCode(65 + idx)
              }))
            }
          })
        }

        if (keysRes && keysRes.isSuccess && keysRes.data?.correctMap) {
          correctAnswerMap.value = keysRes.data.correctMap
        }
      } catch (err) {
        console.error('Lỗi khi tải chi tiết đáp án và key từ API:', err)
      }
    }
  }

  fetchResultAnswersAndKeys().then(() => {
    if (found && !found.answers) {
      const total = quizQuestions.value.length
      const scoreVal = found.score || 9.0
      const correctCount = Math.round((scoreVal / 10.0) * total)
      
      found.answers = quizQuestions.value.map((q: any, qIdx: number) => {
        const isCorrect = qIdx < correctCount
        const correctIds = correctAnswerMap.value[q.id || q.questionId] || []
        
        let selected: number[] = []
        if (q.answers && q.answers.length > 0) {
          if (isCorrect) {
            selected = q.answers
              .map((a: any, i: number) => correctIds.includes(a.id || a.questionAnswerId) ? i : -1)
              .filter((i: number) => i !== -1)
          } else {
            const incorrectIdx = q.answers.findIndex((a: any) => !correctIds.includes(a.id || a.questionAnswerId))
            selected = incorrectIdx !== -1 ? [incorrectIdx] : [0]
          }
        }
        return {
          questionId: q.id || q.questionId,
          selectedAnswerIndexes: selected
        }
      })
    }
    generateLeaderboard(found)
  })
})

const generateLeaderboard = (myAttempt: Attempt) => {
  const storedHistory = localStorage.getItem('cn_history')
  const historyList = storedHistory ? JSON.parse(storedHistory) : []
  
  // Get all attempts for this quiz
  const quizAttempts = historyList.filter((h: any) => h.quizId === myAttempt.quizId)
  
  const list: LeaderboardRow[] = []
  
  if (quizAttempts.length > 0) {
    quizAttempts.forEach((att: any) => {
      const initials = att.candidateName.split(' ').map((n: string) => n.charAt(0)).join('').toUpperCase().slice(0, 2)
      list.push({
        name: att.candidateName,
        avatar: initials || 'ST',
        score: att.score !== undefined ? att.score : 0.0,
        violations: (att.antiCheatMetrics?.blurCount || 0) + (att.antiCheatMetrics?.fullscreenExitCount || 0),
        status: att.status === 'disqualified' ? 'Bị loại' : 'Hoàn thành',
        submittedAt: att.submittedAt ? new Date(att.submittedAt).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }) : '--:--'
      })
    })
  } else {
    // Fallback to static mock candidates if none in history
    list.push(
      { name: 'Nguyễn Hữu Đạt', avatar: 'HD', score: 9.33, violations: 0, status: 'Hoàn thành', submittedAt: '17:42:00' },
      { name: 'Phạm Thùy Linh', avatar: 'TL', score: 8.67, violations: 1, status: 'Hoàn thành', submittedAt: '17:45:15' },
      { name: 'Lê Mai Anh', avatar: 'MA', score: 8.00, violations: 0, status: 'Hoàn thành', submittedAt: '17:44:05' },
      { name: 'Vũ Đức Thịnh', avatar: 'DT', score: 7.33, violations: 4, status: 'Bị loại', submittedAt: '17:43:20' },
      { name: 'Trần Quốc Bảo', avatar: 'QB', score: 6.00, violations: 2, status: 'Hoàn thành', submittedAt: '17:47:00' }
    )
  }

  // Ensure the main showcase candidate is in the list (if not already added)
  const exists = list.some(item => item.name === myAttempt.candidateName)
  if (!exists) {
    const initials = myAttempt.candidateName.split(' ').map((n: string) => n.charAt(0)).join('').toUpperCase().slice(0, 2)
    list.push({
      name: myAttempt.candidateName,
      avatar: initials || 'ME',
      score: myAttempt.score,
      violations: (myAttempt.antiCheatMetrics?.blurCount || 0) + (myAttempt.antiCheatMetrics?.fullscreenExitCount || 0),
      status: myAttempt.status === 'disqualified' ? 'Bị loại' : 'Hoàn thành',
      submittedAt: myAttempt.submittedAt ? new Date(myAttempt.submittedAt).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }) : '--:--'
    })
  }

  // Sort by score descending
  list.sort((a, b) => b.score - a.score)
  leaderboard.value = list
}

const formatDate = (dateStr: string) => {
  if (!dateStr) return '--:--'
  return new Date(dateStr).toLocaleString('vi-VN', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric' })
}

const backToHome = () => {
  router.push('/')
}

const exportCSV = () => {
  if (!attempt.value) return
  const data = [
    ['HỌ VÀ TÊN THÍ SINH', 'ĐIỂM SỐ', 'THỜI GIAN LÀM BÀI', 'SỐ LẦN CHUYỂN TAB', 'THOÁT TOÀN MÀN HÌNH', 'TRẠNG THÁI'],
    [
      attempt.value.candidateName,
      attempt.value.score,
      timeDuration.value,
      attempt.value.antiCheatMetrics?.blurCount || 0,
      attempt.value.antiCheatMetrics?.fullscreenExitCount || 0,
      attempt.value.status === 'disqualified' ? 'Bị loại' : 'Đã nộp bài'
    ]
  ]

  const csvContent = "\uFEFF" + data.map(e => e.join(",")).join("\n")
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement("a")
  link.setAttribute("href", url)
  link.setAttribute("download", `KetQua_${attempt.value.candidateName.replace(/\s+/g, '')}.csv`)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  message.success('Đã tải xuống báo cáo kết quả thi cá nhân!')
}

const exportAllCSV = () => {
  const data = [
    ['HẠNG', 'HỌ VÀ TÊN', 'ĐIỂM SỐ', 'SỐ LẦN VI PHẠM', 'TRẠNG THÁI', 'THỜI GIAN NỘP']
  ]

  leaderboard.value.forEach((row, index) => {
    data.push([
      (index + 1).toString(),
      row.name,
      row.score.toFixed(2),
      row.violations.toString(),
      row.status,
      row.submittedAt
    ])
  })

  const csvContent = "\uFEFF" + data.map(e => e.join(",")).join("\n")
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement("a")
  link.setAttribute("href", url)
  link.setAttribute("download", `BangXepHang_PhongThi.csv`)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
  message.success('Đã tải xuống bảng xếp hạng kết quả phòng thi dạng CSV!')
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

.bg-orange-soft {
  background-color: rgba(249, 115, 22, 0.12);
}

.text-orange-dark {
  color: #c2410c;
}

.secondary-dark {
  color: #475569;
}

.text-secondary-dark {
  color: #334155;
}

.score-badge-decor {
  position: absolute;
  top: -40px;
  right: -40px;
  width: 100px;
  height: 100px;
  background-color: rgba(99, 102, 241, 0.05);
  border-radius: 50%;
}

.podium-column {
  width: 130px;
}

.podium-step {
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.05);
}

.table-indigo-highlight {
  background-color: rgba(99, 102, 241, 0.05) !important;
}

.table-indigo-highlight td {
  border-color: rgba(99, 102, 241, 0.1) !important;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-5px); }
}

.animate-bounce {
  animation: bounce 2s infinite;
}

.max-w-100 {
  max-width: 100px;
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

.bg-success-soft {
  background-color: rgba(16, 185, 129, 0.08) !important;
}
.bg-danger-soft {
  background-color: rgba(239, 68, 68, 0.08) !important;
}
.border-success-subtle {
  border-color: rgba(16, 185, 129, 0.3) !important;
}
.border-danger-subtle {
  border-color: rgba(239, 68, 68, 0.3) !important;
}
.text-success {
  color: #10b981 !important;
}
.text-danger {
  color: #ef4444 !important;
}
.border-success {
  border-color: #10b981 !important;
}
.border-danger {
  border-color: #ef4444 !important;
}
</style>
