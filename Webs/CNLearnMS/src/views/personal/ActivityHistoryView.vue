<template>
  <div class="activity-history-view small-font animate-fade-in">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h1 class="fs-4 fw-bold text-dark-blue mb-1">Lịch sử làm bài</h1>
        <p class="text-secondary small mb-0">Theo dõi kết quả các bài kiểm tra, đề thi và luyện tập câu hỏi của bạn.</p>
      </div>
    </div>

    <div class="card border-0 rounded-4 shadow-sm bg-white p-4">
      <a-tabs v-model:activeKey="activeTab" class="custom-list-tabs mb-4" @change="handleTabChange">
        <a-tab-pane key="exams" tab="Đề thi / Bài thi đã làm" />
        <a-tab-pane key="questions" tab="Câu hỏi đã làm" />
      </a-tabs>
      
      <div class="history-list d-flex flex-column gap-3">
        <div 
          v-for="(item, idx) in items" 
          :key="`${item.examAttemptId}-${idx}`"
          class="history-item p-3 rounded-3 border border-light-subtle bg-light-hover transition d-flex flex-column flex-md-row gap-3 justify-content-between"
        >
          <div class="d-flex align-items-start gap-3">
            <div class="icon-box rounded-circle bg-indigo-soft text-indigo d-flex align-items-center justify-content-center flex-shrink-0" style="width: 40px; height: 40px;">
              <span class="fs-5">{{ activeTab === 'exams' ? '📝' : '❓' }}</span>
            </div>
            <div>
              <h3 class="h6 fw-bold text-dark-blue mb-1" :class="{'cursor-pointer hover-accent': activeTab === 'exams' && item.relatedId}" @click="handleItemClick(item)">
                {{ item.title || (activeTab === 'exams' ? 'Đề thi không xác định' : 'Câu hỏi không xác định') }}
              </h3>
              <div class="d-flex flex-wrap gap-2 text-secondary fs-8 mt-2">
                <span class="badge bg-light text-dark border">{{ activeTab === 'exams' ? (item.attemptType === 'quiz' ? 'Bài kiểm tra' : 'Đề thi') : 'Luyện tập' }}</span>
                <span class="d-flex align-items-center gap-1"><span class="fw-semibold">Đúng:</span> {{ item.correctCount }}/{{ item.totalQuestions }}</span>
                <span class="d-flex align-items-center gap-1"><span class="fw-semibold">Điểm:</span> <span class="text-indigo fw-bold">{{ Number(item.score).toFixed(1) }}</span></span>
                <span class="d-flex align-items-center gap-1"><span class="fw-semibold">Thời gian:</span> {{ formatDuration(item.duration) }}</span>
              </div>
            </div>
          </div>
          <div class="text-md-end d-flex flex-row flex-md-column align-items-center align-items-md-end justify-content-between">
            <div class="text-secondary small fst-italic">{{ formatDate(item.finishedDate) }}</div>
            <button v-if="activeTab === 'exams' && item.relatedId" class="btn btn-sm btn-outline-indigo mt-md-2" @click="handleItemClick(item)">
              Làm lại
            </button>
          </div>
        </div>

        <div v-if="items.length === 0 && !loading" class="text-center py-5 text-secondary">
          Bạn chưa có lịch sử làm bài nào trong mục này.
        </div>
      </div>

      <!-- Loading and Load More indicator -->
      <div ref="loadMoreRef" class="py-3 text-center">
        <a-spin v-if="loading" />
        <span v-else-if="items.length > 0 && !hasMore" class="text-secondary small">Đã tải hết lịch sử.</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { getExamHistory, getQuestionHistory } from '@/api/attempts'

const router = useRouter()
const activeTab = ref('exams')

const items = ref<any[]>([])
const page = ref(1)
const size = 10
const loading = ref(false)
const hasMore = ref(true)

const loadMoreRef = ref<HTMLElement | null>(null)
let observer: IntersectionObserver | null = null

const handleTabChange = () => {
  items.value = []
  page.value = 1
  hasMore.value = true
  loadData()
}

const handleItemClick = (item: any) => {
  if (activeTab.value === 'exams' && item.relatedId) {
    if (item.attemptType === 'quiz') {
      router.push(`/practice/${item.relatedId}?title=${encodeURIComponent(item.title)}`)
    } else {
      router.push(`/practice/${item.relatedId}?title=${encodeURIComponent(item.title)}`)
    }
  }
}

const formatDuration = (seconds: number) => {
  if (!seconds) return '0s'
  if (seconds < 60) return `${seconds}s`
  const m = Math.floor(seconds / 60)
  const s = seconds % 60
  return `${m}m${s > 0 ? ` ${s}s` : ''}`
}

const formatDate = (dateString: string) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return new Intl.DateTimeFormat('vi-VN', {
    hour: '2-digit', minute: '2-digit',
    day: '2-digit', month: '2-digit', year: 'numeric'
  }).format(date)
}

const loadData = async () => {
  if (loading.value || !hasMore.value) return
  loading.value = true
  
  try {
    let res: any
    if (activeTab.value === 'exams') {
      res = await getExamHistory(page.value, size)
    } else {
      res = await getQuestionHistory(page.value, size)
    }

    const resultData = res?.data ?? res?.Data
    if (res?.isSuccess || res?.IsSuccess || resultData) {
      const dataItems = resultData?.items ?? resultData?.Items ?? []
      if (dataItems.length > 0) {
        items.value = [...items.value, ...dataItems]
        page.value++
        
        const totalPages = resultData?.totalPages ?? resultData?.TotalPages ?? 1
        if (page.value > totalPages) {
          hasMore.value = false
        }
      } else {
        hasMore.value = false
      }
    } else {
      hasMore.value = false
    }
  } catch (error) {
    console.error(error)
    message.error('Lỗi khi tải lịch sử làm bài.')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
  
  // Setup intersection observer for infinite scrolling
  observer = new IntersectionObserver((entries) => {
    if (entries[0] && entries[0].isIntersecting) {
      loadData()
    }
  }, {
    rootMargin: '100px',
    threshold: 0.1
  })
  
  nextTick(() => {
    if (loadMoreRef.value) {
      observer?.observe(loadMoreRef.value)
    }
  })
})

onUnmounted(() => {
  if (observer) {
    observer.disconnect()
  }
})
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}
.small-font {
  font-size: 0.85rem !important;
}
.cursor-pointer {
  cursor: pointer;
}
.text-indigo {
  color: #6366f1;
}
.bg-indigo-soft {
  background-color: #e0e7ff;
}
.hover-accent:hover {
  color: #4f46e5 !important;
  text-decoration: underline;
}
.history-item {
  border: 1px solid #e2e8f0;
}
.bg-light-hover:hover {
  background-color: #f8fafc;
}
</style>
