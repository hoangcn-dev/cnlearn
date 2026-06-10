<template>
  <div class="quiz-detail-view py-3">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-4">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/categories">Danh mục đề</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Chi tiết đề thi</li>
      </ol>
    </nav>

    <div class="row g-4">
      <!-- COLUMN 1: Comments Section (Replaced Preview Image Card) -->
      <div class="col-lg-4 col-md-5 align-self-start sticky-sidebar">
        <div class="card border-0 rounded-4 shadow-sm bg-white px-4 pt-3 pb-4 d-flex flex-column" style="height: calc(100vh - 160px);">
          <div class="d-flex align-items-center justify-content-between mb-2 pb-1 border-bottom border-light flex-wrap gap-2">
            <div class="d-flex align-items-center gap-2">
              <span class="fs-4">💬</span>
              <h3 class="h6 fw-bold text-dark-blue mb-0">Thảo Luận ({{ comments.length }})</h3>
              <span 
                class="fs-9 text-muted hover-pointer text-decoration-none select-none ms-1 opacity-75" 
                @click="isLoggedIn = !isLoggedIn"
                title="Click để đổi trạng thái đăng nhập test"
              >
                ({{ isLoggedIn ? 'Học Viên' : 'Khách' }})
              </span>
            </div>
            <span class="badge bg-light text-secondary fs-8">25 đánh giá</span>
          </div>

          <!-- Comment trigger button -->
          <div class="mb-3">
            <button 
              v-if="isLoggedIn" 
              class="btn btn-indigo text-white w-100 py-2 rounded-3 fw-semibold fs-8 hover-up shadow-sm"
              @click="openCommentModal"
            >
              💬 Viết Bình Luận
            </button>
            <button 
              v-else 
              class="btn btn-outline-indigo w-100 py-2 rounded-3 fw-semibold fs-8 hover-up shadow-sm"
              @click="handleLoginAndComment"
            >
              🔑 Đăng Nhập Để Bình Luận
            </button>
          </div>

          <!-- Comments List (scrollable) -->
          <div class="d-flex flex-column flex-grow-1" style="padding-right: 4px; min-height: 0; overflow-y: auto;">
              <div v-for="c in comments" :key="c.id" class="comment-item p-3 rounded-3 bg-light-card border-0 d-flex gap-3">
                <!-- Left Column: Avatar & Thread Line -->
                <div class="d-flex flex-column align-items-center flex-shrink-0">
                  <div class="avatar-ring rounded-circle d-flex align-items-center justify-content-center" style="width: 36px; height: 36px;">
                    <div class="rounded-circle bg-indigo text-white fw-bold d-flex align-items-center justify-content-center w-100 h-100" style="font-size: 0.75rem;">
                      {{ c.avatar }}
                    </div>
                  </div>
                  <div class="comment-thread-line flex-grow-1 my-2"></div>
                </div>

                <!-- Right Column: Metadata + Content + Actions -->
                <div class="flex-grow-1">
                  <!-- Author metadata -->
                  <div class="d-flex align-items-center gap-1.5 flex-wrap mb-1.5">
                    <span class="fw-bold text-dark-blue fs-8">{{ c.userName }}</span>
                    <span class="mx-1 text-secondary opacity-50 fs-9">•</span>
                    <span class="text-muted fs-9">{{ getRelativeTime(c.createdAt) }}</span>
                  </div>

                  <!-- Content text -->
                  <p class="text-secondary fs-8 mb-1 leading-relaxed">
                    <span v-if="!expandedComments[c.id] && c.content.length > 95">
                      {{ c.content.slice(0, 95) }}...
                      <span 
                        class="fs-9 text-muted hover-pointer text-decoration-none select-none fw-semibold text-nowrap ms-1" 
                        @click="toggleExpandComment(c.id)"
                      >Xem thêm</span>
                    </span>
                    <span v-else>
                      {{ c.content }}
                      <span 
                        v-if="c.content.length > 95"
                        class="fs-9 text-muted hover-pointer text-decoration-none select-none fw-semibold text-nowrap ms-2" 
                        @click="toggleExpandComment(c.id)"
                      >Thu gọn</span>
                    </span>
                  </p>

                  <!-- Action buttons -->
                  <div class="d-flex gap-3 small text-secondary">
                    <span class="hover-pointer" @click="likeComment(c)">👍 {{ c.likes }}</span>
                    <span class="hover-pointer">💬 Phản hồi</span>
                  </div>
                </div>
              </div>
            </div>
        </div>
      </div>

      <!-- COLUMN 2: Basic Info Card & Actions -->
      <div class="col-lg-8 col-md-7">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4 h-100 d-flex flex-column justify-content-between">
          <div>
            <div class="d-flex align-items-center mb-3">
              <button class="btn btn-icon-back me-2 d-flex align-items-center justify-content-center" @click="router.back()" title="Quay lại">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="feather feather-arrow-left" viewBox="0 0 24 24">
                  <line x1="19" y1="12" x2="5" y2="12"></line>
                  <polyline points="12 19 5 12 12 5"></polyline>
                </svg>
              </button>
              <div class="d-flex flex-wrap gap-2">
                <span 
                  v-for="tag in quizInfo.tags" 
                  :key="tag" 
                  class="badge bg-indigo-soft text-indigo px-2.5 py-1.5 rounded-pill fs-8 fw-semibold"
                >
                  {{ tag }}
                </span>
              </div>
            </div>
            <h1 class="fs-3 fw-bold text-dark-blue mb-3">{{ quizTitle }}</h1>
            
            <!-- Metadata details -->
            <div class="row g-4 py-3 border-top border-bottom border-light my-3">
              <div class="col-6 col-sm-4">
                <div class="text-secondary small">Số câu hỏi</div>
                <div class="fs-5 fw-bold text-dark-blue">{{ quizInfo.questionCount }} câu</div>
              </div>
              <div class="col-6 col-sm-4">
                <div class="text-secondary small">Thời gian làm bài</div>
                <div class="fs-5 fw-bold text-dark-blue">{{ quizInfo.duration }} phút</div>
              </div>
              <div class="col-6 col-sm-4">
                <div class="text-secondary small">Lượt làm đề</div>
                <div class="fs-5 fw-bold text-success">{{ quizInfo.attemptsCount.toLocaleString('vi-VN') }} lượt</div>
              </div>
              <div class="col-6 col-sm-4">
                <div class="text-secondary small">Người cập nhật</div>
                <div class="fs-6 fw-semibold text-dark-blue">{{ quizInfo.createdBy }}</div>
              </div>
              <div class="col-6 col-sm-4">
                <div class="text-secondary small">Ngày cập nhật</div>
                <div class="fs-6 fw-semibold text-dark-blue">{{ formatDate(quizInfo.modifiedDate) }}</div>
              </div>
              <div class="col-6 col-sm-4">
                <div class="text-secondary small">Độ khó</div>
                <div>
                  <span class="badge" :class="getLevelBadgeClass(quizInfo.level)">
                    {{ getLevelText(quizInfo.level) }}
                  </span>
                </div>
              </div>
            </div>

            <!-- Detailed Description -->
            <div class="mb-4">
              <h5 class="fw-bold text-dark-blue h6 mb-2">Mô tả chi tiết:</h5>
              <p class="text-secondary small leading-relaxed">
                Bộ đề ôn tập này được tuyển chọn kỹ lưỡng bám sát theo chương trình chuẩn hóa. Bộ câu hỏi bao gồm đầy đủ các khía cạnh từ lý thuyết căn bản tới bài tập vận dụng cao, kèm lời giải chi tiết giúp học viên hiểu rõ bản chất vấn đề sau khi hoàn thành.
              </p>
            </div>
          </div>

          <!-- Horizontal Action Buttons -->
          <div class="actions-wrapper pt-3 border-top border-light">
            <div class="d-flex flex-wrap gap-3">
              <!-- Action: Lưu đề thi -->
              <button 
                class="btn px-4 py-2.5 rounded-3 fw-semibold flex-grow-1 flex-sm-grow-0 hover-up d-flex align-items-center gap-1.5"
                :class="isQuizSaved ? 'btn-warning text-white' : 'btn-outline-warning'"
                @click="toggleSaveQuiz"
              >
                <span>{{ isQuizSaved ? '★' : '☆' }}</span>
                <span>{{ isQuizSaved ? 'Đã lưu đề' : 'Lưu đề thi' }}</span>
              </button>

              <!-- Action 1: Tải xuống PDF/Word -->
              <button class="btn btn-outline-secondary px-4 py-2.5 rounded-3 fw-semibold flex-grow-1 flex-sm-grow-0 hover-up" @click="showDownloadModal = true">
                📥 Tải PDF / Word
              </button>

              <!-- Action 2: Thi thử -->
              <button class="btn btn-outline-indigo px-4 py-2.5 rounded-3 fw-semibold flex-grow-1 flex-sm-grow-0 hover-up" @click="startExamMode">
                ⏱️ Thi Thử (Hẹn Giờ)
              </button>

              <!-- Action 3: Luyện tập (Chính) -->
              <button class="btn btn-indigo px-5 py-2.5 rounded-3 fw-semibold flex-grow-1 flex-sm-grow-0 shadow hover-up text-white" @click="goToPractice">
                ✍️ Luyện Tập
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- DOWNLOAD SELECT MODAL -->
    <a-modal
      v-model:visible="showDownloadModal"
      title="📥 Tải Đề Thi Chất Lượng Cao"
      :footer="null"
      centered
      class="premium-modal"
    >
      <div class="text-center py-3">
        <p class="text-secondary small mb-4">Lựa chọn định dạng văn bản bạn muốn tải xuống máy tính của mình:</p>
        <div class="d-flex gap-3 justify-content-center">
          <button class="btn btn-indigo px-4 py-3 rounded-3 d-flex flex-column align-items-center gap-2 flex-grow-1 text-white" @click="triggerDownload('docx')">
            <span class="fs-3">📝</span>
            <span class="fw-bold text-white">Tải Bản Word (.docx)</span>
            <span class="fs-8 opacity-75">Thích hợp chỉnh sửa</span>
          </button>
          <button class="btn btn-danger px-4 py-3 rounded-3 d-flex flex-column align-items-center gap-2 flex-grow-1 text-white" @click="triggerDownload('pdf')">
            <span class="fs-3">📄</span>
            <span class="fw-bold text-white">Tải Bản PDF (.pdf)</span>
            <span class="fs-8 opacity-75">Thích hợp in ấn</span>
          </button>
        </div>
      </div>
    </a-modal>

    <!-- COMMENT POPUP MODAL -->
    <a-modal
      v-model:visible="showCommentModal"
      title="💬 Gửi Bình Luận Của Bạn"
      :footer="null"
      centered
      class="premium-modal"
    >
      <div class="py-3">
        <div class="d-flex gap-3 mb-3 align-items-start">
          <div class="avatar-ring rounded-circle d-flex align-items-center justify-content-center flex-shrink-0" style="width: 38px; height: 38px;">
            <div class="rounded-circle bg-indigo text-white fw-bold d-flex align-items-center justify-content-center w-100 h-100" style="font-size: 0.8rem;">
              ME
            </div>
          </div>
          <div class="flex-grow-1">
            <div class="fw-bold text-dark-blue fs-7 mb-1">Học viên (Bạn)</div>
            <textarea 
              v-model="newCommentText" 
              class="form-control border-light rounded-3 bg-light p-2.5 fs-8" 
              rows="3" 
              placeholder="Chia sẻ cảm nghĩ hoặc thắc mắc của bạn về đề thi này..."
            ></textarea>
          </div>
        </div>
        <div class="d-flex justify-content-end gap-2 pt-2 border-top border-light">
          <button class="btn btn-outline-secondary px-3 py-1.5 rounded-3 fw-semibold small" @click="showCommentModal = false">
            Hủy
          </button>
          <button class="btn btn-indigo text-white px-4 py-1.5 rounded-3 fw-semibold small" @click="submitCommentFromModal">
            Gửi Bình Luận
          </button>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'

const route = useRoute()
const router = useRouter()

const quizId = ref(route.params.id as string)
const quizTitle = ref((route.query.title as string) || 'Đề thi trắc nghiệm ôn tập')

const isQuizSaved = ref(false)

const checkQuizSavedStatus = () => {
  const saved = localStorage.getItem('cn_saved_quizzes')
  const savedIds = saved ? JSON.parse(saved) : []
  isQuizSaved.value = savedIds.includes(quizId.value)
}

const toggleSaveQuiz = () => {
  const saved = localStorage.getItem('cn_saved_quizzes')
  let savedIds = saved ? JSON.parse(saved) : []
  if (savedIds.includes(quizId.value)) {
    savedIds = savedIds.filter((id: string) => id !== quizId.value)
    message.success('Đã bỏ lưu đề thi!')
  } else {
    savedIds.push(quizId.value)
    message.success('Đã lưu đề thi thành công!')
  }
  localStorage.setItem('cn_saved_quizzes', JSON.stringify(savedIds))
  isQuizSaved.value = savedIds.includes(quizId.value)
}

// Comments and Download Modal state
const showDownloadModal = ref(false)
const showCommentModal = ref(false)
const newCommentText = ref('')
const isLoggedIn = ref(true)
const expandedComments = ref<Record<string, boolean>>({})

const toggleExpandComment = (id: string) => {
  expandedComments.value[id] = !expandedComments.value[id]
}

const openCommentModal = () => {
  showCommentModal.value = true
}

const handleLoginAndComment = () => {
  isLoggedIn.value = true
  message.success('Đăng nhập thành công với tài khoản Học Viên!')
  showCommentModal.value = true
}

const submitCommentFromModal = () => {
  if (newCommentText.value.trim() === '') {
    message.warning('Vui lòng nhập nội dung bình luận!')
    return
  }

  comments.value.unshift({
    id: `c-new-${Date.now()}`,
    avatar: 'ME',
    userName: 'Học viên (Bạn)',
    createdAt: new Date(),
    content: newCommentText.value.trim(),
    likes: 0
  })

  newCommentText.value = ''
  showCommentModal.value = false
  message.success('Bình luận của bạn đã được gửi thành công!')
}

interface CommentItem {
  id: string
  avatar: string
  userName: string
  createdAt: Date
  content: string
  likes: number
}

const comments = ref<CommentItem[]>([
  {
    id: 'c-1',
    avatar: 'HN',
    userName: 'Hoàng Nguyễn',
    createdAt: new Date(Date.now() - 3600000 * 2),
    content: 'Đề thi cực kỳ bám sát cấu trúc đề thi khảo sát của Sở GD&ĐT. Các câu hỏi phân hóa từ câu 35 trở đi rất hay!',
    likes: 12
  },
  {
    id: 'c-2',
    avatar: 'AT',
    userName: 'Anh Tuấn',
    createdAt: new Date(Date.now() - 3600000 * 8),
    content: 'Lần đầu tiên mình làm thử được 8.5 điểm. Hướng dẫn giải thích của câu số 7 siêu chi tiết và dễ hiểu, cảm ơn ad!',
    likes: 7
  },
  {
    id: 'c-3',
    avatar: 'ML',
    userName: 'Mai Lan',
    createdAt: new Date(Date.now() - 86400000 * 1.5),
    content: 'Có ai bị vướng ở câu cuối cùng giống mình không? Mình tính mãi không ra đáp án đúng nhưng đọc phần giải thích mới vỡ lẽ ra công thức nhanh.',
    likes: 4
  },
  {
    id: 'c-4',
    avatar: 'QH',
    userName: 'Quốc Huy',
    createdAt: new Date(Date.now() - 86400000 * 2),
    content: 'Đề thi này rất thích hợp để luyện tốc độ làm bài. Các câu hỏi lý thuyết không quá đánh đố nhưng cần sự cẩn thận cao độ để không bị mất điểm oan.',
    likes: 9
  },
  {
    id: 'c-5',
    avatar: 'VT',
    userName: 'Văn Thanh',
    createdAt: new Date(Date.now() - 86400000 * 3),
    content: 'Admin có thể đăng thêm nhiều đề khảo sát chất lượng từ các trường chuyên khác được không ạ? Mình thấy học theo dạng này tiến bộ rất nhanh.',
    likes: 15
  },
  {
    id: 'c-6',
    avatar: 'TH',
    userName: 'Thu Hà',
    createdAt: new Date(Date.now() - 86400000 * 4),
    content: 'Đề thi bám sát chương trình học mới. Lời giải rất dễ hiểu đối với học sinh trung bình khá như mình, cảm ơn đội ngũ biên soạn!',
    likes: 6
  },
  {
    id: 'c-7',
    avatar: 'DK',
    userName: 'Duy Khánh',
    createdAt: new Date(Date.now() - 86400000 * 5),
    content: 'Phần giải thích chi tiết của đề này làm rất có tâm. Hy vọng hệ thống sẽ cập nhật thêm tính năng thi thử bấm giờ thực tế nữa.',
    likes: 11
  },
  {
    id: 'c-8',
    avatar: 'MP',
    userName: 'Minh Phương',
    createdAt: new Date(Date.now() - 86400000 * 6),
    content: 'Có một số câu về phương trình mũ ở cuối đòi hỏi kỹ năng biến đổi tốt. Rất đáng thử sức cho bạn nào muốn lấy điểm 9, 10!',
    likes: 8
  },
  {
    id: 'c-9',
    avatar: 'BL',
    userName: 'Bảo Lâm',
    createdAt: new Date(Date.now() - 86400000 * 7),
    content: 'Đề khảo sát đợt hè này vừa sức, hệ thống lại load rất mượt mà. Sẽ giới thiệu cho các bạn cùng lớp vào ôn tập cùng luôn.',
    likes: 13
  },
  {
    id: 'c-10',
    avatar: 'AN',
    userName: 'Ánh Ngọc',
    createdAt: new Date(Date.now() - 86400000 * 8),
    content: 'Một trải nghiệm học tập tuyệt vời. Giao diện trực quan, giải thích rõ ràng và có nhiều đề thi chất lượng. Cảm ơn website rất nhiều!',
    likes: 20
  }
])

const likeComment = (c: CommentItem) => {
  c.likes++
  message.success('Đã thích bình luận!')
}

const triggerDownload = (format: string) => {
  showDownloadModal.value = false
  const key = 'downloading'
  message.loading({ content: `Đang kết xuất tệp tài liệu ${format.toUpperCase()}...`, key })
  setTimeout(() => {
    message.success({ content: `Tải xuống đề thi bản ${format.toUpperCase()} thành công!`, key, duration: 2 })
  }, 1500)
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



interface QuizDetails {
  questionCount: number
  duration: number
  attemptsCount: number
  createdBy: string
  modifiedDate: Date
  level: number
  tags: string[]
}

const quizInfo = ref<QuizDetails>({
  questionCount: 40,
  duration: 60,
  attemptsCount: 1420,
  createdBy: 'Hoàng Cao Nguyên',
  modifiedDate: new Date(),
  level: 1,
  tags: []
})

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

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  }).format(date)
}



const startExamMode = () => {
  router.push({
    name: 'quiz-practice',
    params: { id: quizId.value },
    query: { title: quizTitle.value, exam: 'true', duration: quizInfo.value.duration }
  })
}

const goToPractice = () => {
  router.push({
    name: 'quiz-practice',
    params: { id: quizId.value },
    query: { title: quizTitle.value }
  })
}

onMounted(() => {
  checkQuizSavedStatus()
  // Populate random details based on ID to keep it realistic
  const hash = quizId.value.split('').reduce((acc, char) => acc + char.charCodeAt(0), 0)
  quizInfo.value.questionCount = (hash % 2 === 0) ? 30 : 50
  quizInfo.value.duration = (hash % 2 === 0) ? 45 : 90
  quizInfo.value.attemptsCount = 820 + (hash * 7) % 5000
  quizInfo.value.level = hash % 3

  // Populate dynamic category tags based on title
  const titleLower = quizTitle.value.toLowerCase()
  const tagsList = ['Luyện thi']
  if (titleLower.includes('toán')) tagsList.push('Toán học')
  if (titleLower.includes('lý') || titleLower.includes('vật lý')) tagsList.push('Vật lý')
  if (titleLower.includes('hóa')) tagsList.push('Hóa học')
  if (titleLower.includes('anh') || titleLower.includes('tiếng anh')) tagsList.push('Tiếng Anh')
  if (titleLower.includes('thpt') || titleLower.includes('quốc gia')) tagsList.push('THPT Quốc Gia')
  if (titleLower.includes('khảo sát') || titleLower.includes('hè')) tagsList.push('Khảo sát hè')
  
  if (tagsList.length === 1) {
    tagsList.push('Trắc Nghiệm', 'Tổng Hợp')
  }
  quizInfo.value.tags = tagsList
})
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

.btn-outline-indigo {
  color: #4f46e5;
  border-color: #6366f1;
}

.btn-outline-indigo:hover {
  background-color: #4f46e5;
  color: white;
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
}



.hover-up {
  transition: all 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08) !important;
}

.tracking-wider {
  letter-spacing: 0.05em;
}

.fs-7 {
  font-size: 0.825rem;
}

.fs-8 {
  font-size: 0.75rem;
}

.fs-9 {
  font-size: 0.7rem;
}

.overflow-y-auto::-webkit-scrollbar {
  width: 4px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: transparent;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: rgba(99, 102, 241, 0.2);
  border-radius: 4px;
}

.overflow-y-auto::-webkit-scrollbar-thumb:hover {
  background: rgba(99, 102, 241, 0.4);
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

.avatar-placeholder {
  transition: all 0.2s ease;
}

.comment-item {
  transition: all 0.2s ease;
}

.comment-item:hover {
  background-color: rgba(99, 102, 241, 0.03);
}

.hover-pointer {
  cursor: pointer;
}

.hover-pointer:hover {
  color: #4f46e5 !important;
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

.avatar-ring {
  padding: 2px;
  border: 2px solid rgba(99, 102, 241, 0.65);
  background-color: #ffffff;
}

.comment-text-collapsed {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
}

.comment-thread-line {
  width: 2px;
  background-color: rgba(99, 102, 241, 0.25);
  border-radius: 1px;
}

.sticky-sidebar {
  position: -webkit-sticky;
  position: sticky;
  top: 88px; /* Clear the sticky header navbar */
  z-index: 10;
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
