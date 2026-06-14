<template>
  <div class="question-bank-view py-3 small-font">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-3">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Ngân hàng câu hỏi</li>
      </ol>
    </nav>

    <!-- Header -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
      <div>
        <h1 class="fs-4 fw-bold text-dark-blue mb-1">Ngân Hàng Câu Hỏi</h1>
        <p class="text-secondary small mb-0">Quản lý, tạo mới, chỉnh sửa và đóng góp câu hỏi trắc nghiệm cho hệ thống.</p>
      </div>
    </div>

    <!-- Separate Action Bar Card -->
    <div class="card bg-white mb-3 p-3">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-center gap-3">
        <!-- Search & Filter -->
        <div class="d-flex gap-2 align-items-center flex-grow-1" style="max-width: 500px;">
          <div class="flex-grow-1" style="max-width: 320px;">
            <a-input v-model:value="filters.search" placeholder="Tìm kiếm nội dung..." allow-clear>
              <template #prefix>
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="text-muted"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
              </template>
            </a-input>
          </div>

          <button type="button" class="btn btn-outline-indigo btn-sm d-flex align-items-center gap-1.5 px-3 py-1.5 rounded" @click="filterModalOpen = true">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon></svg>
            <span>Bộ lọc</span>
            <span v-if="hasActiveFilters" class="badge bg-indigo text-white rounded-pill ms-1" style="font-size: 0.7rem;">{{ activeFiltersCount }}</span>
          </button>
        </div>

        <!-- Add Question Button -->
        <button class="btn btn-indigo text-white fw-semibold hover-up d-flex align-items-center gap-2 btn-sm flex-shrink-0" @click="goToCreatePage">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="me-1"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
          Thêm câu hỏi mới
        </button>
      </div>
    </div>

    <!-- Separate Table Card -->
    <div class="card bg-white mb-5 p-3 pt-0">
      <!-- List Tabs -->
      <a-tabs v-model:activeKey="listTab" class="custom-list-tabs mb-2">
        <a-tab-pane key="mine" tab="Của tôi" />
        <a-tab-pane key="saved" tab="Đã lưu" />
      </a-tabs>

      <!-- Table list -->
      <div class="table-responsive py-1">
        <a-table 
          :columns="columns" 
          :data-source="questions" 
          row-key="id"
          :pagination="{
            current: currentPage,
            pageSize: pageSize,
            total: totalItems,
            showSizeChanger: false,
            size: 'small'
          }"
          :loading="loading"
          @change="handleTableChange"
          class="custom-table"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'stringContent'">
              <div class="fw-semibold text-dark-blue text-truncate-2" style="max-width: 380px;">
                {{ record.stringContent }}
              </div>
              <div class="text-secondary small mt-1">
                {{ record.answers.length }} đáp án • Giải thích: <span class="fst-italic text-truncate-1 d-inline-block align-middle" style="max-width: 250px;">{{ record.explanation || 'Không có' }}</span>
              </div>
            </template>

            <template v-else-if="column.key === 'category'">
              <span class="badge bg-light text-dark border">{{ getCategoryName(record.categoryIds[0]) }}</span>
            </template>

            <template v-else-if="column.key === 'level'">
              <span :class="getLevelBadgeClass(record.level)">
                {{ getLevelText(record.level) }}
              </span>
            </template>

            <template v-else-if="column.key === 'actions'">
              <div class="d-flex justify-content-end pe-2">
                <a-dropdown trigger="click" placement="bottomRight">
                  <button class="btn btn-xs btn-outline-secondary p-1 rounded-circle d-flex align-items-center justify-content-center" style="width: 26px; height: 26px; border-color: #cbd5e1;">
                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>
                  </button>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item key="view" @click="viewQuestionDetail(record.id)">
                        <span class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                          Xem chi tiết
                        </span>
                      </a-menu-item>
                      <a-menu-item key="save-toggle" @click="handleToggleSave(record.id)">
                        <span v-if="listTab === 'saved' || savedQuestionIdsList.includes(record.id)" class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon></svg>
                          Bỏ lưu câu hỏi
                        </span>
                        <span v-else class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon></svg>
                          Lưu câu hỏi
                        </span>
                      </a-menu-item>
                      <a-menu-item key="edit" @click="goToEditPage(record.id)">
                        <span class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4z"></path></svg>
                          Chỉnh sửa
                        </span>
                      </a-menu-item>
                      <a-menu-item key="delete" @click="confirmDelete(record.id)">
                        <span class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
                          Xóa câu hỏi
                        </span>
                      </a-menu-item>
                    </a-menu>
                  </template>
                </a-dropdown>
              </div>
            </template>
          </template>
        </a-table>
      </div>
    </div>

    <!-- Centered Filter Modal -->
    <a-modal
      v-model:open="filterModalOpen"
      title="Bộ lọc câu hỏi"
      :footer="null"
      centered
      width="380px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label small fw-semibold">Danh mục môn học:</label>
          <a-select v-model:value="filters.categoryId" style="width: 100%" placeholder="Tất cả môn học" allow-clear>
            <a-select-option v-for="cat in categories" :key="cat.questionCategoryId" :value="cat.questionCategoryId">
              {{ cat.name }}
            </a-select-option>
          </a-select>
        </div>
        <div class="mb-3">
          <label class="form-label small fw-semibold">Độ khó:</label>
          <a-select v-model:value="filters.level" style="width: 100%" placeholder="Tất cả độ khó" allow-clear>
            <a-select-option :value="0">Dễ</a-select-option>
            <a-select-option :value="1">Trung bình</a-select-option>
            <a-select-option :value="2">Khó</a-select-option>
          </a-select>
        </div>
        <div class="mb-3">
          <label class="form-label small fw-semibold">Phạm vi:</label>
          <a-select v-model:value="filters.accessType" style="width: 100%" placeholder="Tất cả phạm vi" allow-clear>
            <a-select-option :value="0">🔒 Cá nhân</a-select-option>
            <a-select-option :value="1">🌐 Công khai (Public)</a-select-option>
          </a-select>
        </div>
        <div class="mb-2">
          <label class="form-label small fw-semibold d-block">Tác giả:</label>
          <a-radio-group v-model:value="filters.scope" button-style="solid" size="small" class="w-100 d-flex">
            <a-radio-button value="all" class="flex-grow-1 text-center">Tất cả</a-radio-button>
            <a-radio-button value="mine" class="flex-grow-1 text-center">Của tôi</a-radio-button>
          </a-radio-group>
        </div>
        <div class="border-top pt-2 mt-4 d-flex justify-content-between align-items-center">
          <button class="btn btn-link btn-xs text-secondary p-0 text-decoration-none small" @click="resetFilters">Đặt lại bộ lọc</button>
          <button class="btn btn-indigo text-white btn-sm px-4" @click="filterModalOpen = false">Áp dụng</button>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'
import { getAllCate } from '@/api/categories'
import { getQuestionsPaging, deleteQuestions, getSavedQuestionsPaging, getDoneQuestionsPaging, toggleSaveQuestion, getSavedQuestionIds } from '@/api/questions'

const router = useRouter()

interface Answer {
  stringContent: string
  isCorrectAnswer: boolean
}

interface Question {
  id: string
  slug: string
  stringContent: string
  explanation: string
  level: number
  type: number
  accessType: number
  categoryIds: string[]
  answers: Answer[]
  isMyCreated?: boolean
}

interface Category {
  questionCategoryId: string
  name: string
}

// Categories list loaded from API
const categories = ref<Category[]>([])
const questions = ref<Question[]>([])

const listTab = ref('mine')
const filterModalOpen = ref(false)

// Pagination state
const currentPage = ref(1)
const pageSize = ref(8)
const totalItems = ref(0)
const loading = ref(false)

// Filters state
const filters = reactive({
  search: '',
  categoryId: undefined as string | undefined,
  level: undefined as number | undefined,
  accessType: undefined as number | undefined,
  scope: 'all' // all | mine
})

const activeFiltersCount = computed(() => {
  let count = 0
  if (filters.categoryId !== undefined) count++
  if (filters.level !== undefined) count++
  if (filters.accessType !== undefined) count++
  if (filters.scope !== 'all') count++
  return count
})

const hasActiveFilters = computed(() => activeFiltersCount.value > 0)

const resetFilters = () => {
  filters.categoryId = undefined
  filters.level = undefined
  filters.accessType = undefined
  filters.scope = 'all'
}

// Columns config
const columns = [
  { title: 'Nội dung câu hỏi', dataIndex: 'stringContent', key: 'stringContent' },
  { title: 'Danh mục', dataIndex: 'category', key: 'category', width: '260px' },
  { title: 'Độ khó', dataIndex: 'level', key: 'level', width: '120px' },
  { title: 'Thao tác', key: 'actions', width: '90px' }
]

const getCategoryName = (catId?: string) => {
  if (!catId) return 'Chuyên đề chung'
  const found = categories.value.find(c => c.questionCategoryId === catId)
  return found ? found.name : 'Chuyên đề chung'
}

const getLevelText = (lvl: number) => {
  if (lvl === 2) return 'Khó'
  if (lvl === 1) return 'Trung bình'
  return 'Dễ'
}

const getLevelBadgeClass = (lvl: number) => {
  if (lvl === 2) return 'badge bg-danger text-white fs-9 fw-normal'
  if (lvl === 1) return 'badge bg-warning text-dark fs-9 fw-normal'
  return 'badge bg-success text-white fs-9 fw-normal'
}

// API fetch questions
const fetchQuestions = async () => {
  loading.value = true
  try {
    const apiFilters: any[] = []
    let fetchFunc = getQuestionsPaging

    // 1. Tab filtering
    if (listTab.value === 'mine') {
      apiFilters.push({
        property: 'IsMyCreated',
        operator: 'Equal',
        value: true,
        type: 'Bool'
      })
    } else if (listTab.value === 'saved') {
      fetchFunc = getSavedQuestionsPaging
    } else if (listTab.value === 'done') {
      fetchFunc = getDoneQuestionsPaging
    }

    // 2. Extra Filters from modal
    if (filters.categoryId) {
      apiFilters.push({
        property: 'CategoryId',
        operator: 'Equal',
        value: filters.categoryId,
        type: 'String'
      })
    }
    if (filters.level !== undefined) {
      apiFilters.push({
        property: 'Level',
        operator: 'Equal',
        value: filters.level,
        type: 'Number'
      })
    }
    if (filters.accessType !== undefined) {
      apiFilters.push({
        property: 'AccessType',
        operator: 'Equal',
        value: filters.accessType,
        type: 'Number'
      })
    }
    // (Đã loại bỏ bộ lọc scope === 'mine' vì backend không cho phép truyền userId kiểu text)

    const payload = {
      page: currentPage.value,
      size: pageSize.value,
      key: filters.search || undefined,
      filters: apiFilters,
      sortBy: 'CreatedDate',
      isAsc: false,
      isPaging: true
    }

    const res = await fetchFunc(payload)
    if (res && res.isSuccess && res.data) {
      questions.value = res.data.items || []
      totalItems.value = res.data.total || 0
    }
  } catch (error) {
    console.error('Lỗi tải danh sách câu hỏi:', error)
  } finally {
    loading.value = false
  }
}

// Table change handler for paging
const handleTableChange = (pagination: any) => {
  currentPage.value = pagination.current
  fetchQuestions()
}

// Debounce helper
function debounce<T extends (...args: any[]) => any>(fn: T, delay: number) {
  let timeoutId: ReturnType<typeof setTimeout> | null = null
  return function (this: any, ...args: Parameters<T>) {
    if (timeoutId) clearTimeout(timeoutId)
    timeoutId = setTimeout(() => {
      fn.apply(this, args)
    }, delay)
  }
}

const debouncedFetch = debounce(() => {
  currentPage.value = 1
  fetchQuestions()
}, 300)

// Watchers for search & filters
watch(() => filters.search, () => {
  debouncedFetch()
})

watch(() => [filters.categoryId, filters.level, filters.accessType, filters.scope], () => {
  currentPage.value = 1
  fetchQuestions()
})

watch(listTab, () => {
  currentPage.value = 1
  fetchQuestions()
})

// Routing
const goToCreatePage = () => {
  router.push('/personal/questions/create')
}

const goToEditPage = (id: string) => {
  router.push(`/personal/questions/edit/${id}`)
}

const viewQuestionDetail = (id: string) => {
  router.push(`/question/${id}`)
}

// Actions
const confirmDelete = (id: string) => {
  Modal.confirm({
    title: 'Xác nhận xóa câu hỏi',
    content: 'Bạn có chắc chắn muốn xóa câu hỏi này khỏi Ngân hàng câu hỏi?',
    okText: 'Xóa câu hỏi',
    okType: 'danger',
    cancelText: 'Hủy bỏ',
    async onOk() {
      try {
        const res = await deleteQuestions([id])
        if (res && res.isSuccess) {
          message.success('Đã xóa câu hỏi thành công!')
          fetchQuestions()
        } else {
          message.error(res.errorMessage || 'Xóa câu hỏi thất bại')
        }
      } catch (error) {
        message.error('Đã xảy ra lỗi khi xóa câu hỏi')
      }
    }
  })
}

// Categories load
const fetchCategories = async () => {
  try {
    const res = await getAllCate()
    if (res && res.isSuccess && res.data) {
      categories.value = res.data.items || []
    }
  } catch (error) {
    console.error('Lỗi tải danh mục:', error)
  }
}

const savedQuestionIdsList = ref<string[]>([])

const fetchSavedQuestionIds = async () => {
  try {
    const res = await getSavedQuestionIds()
    if (res && res.isSuccess && res.data) {
      savedQuestionIdsList.value = res.data
    }
  } catch (e) {
    console.error('Lỗi tải danh sách ID đã lưu:', e)
  }
}

const handleToggleSave = async (id: string) => {
  try {
    const res = await toggleSaveQuestion(id)
    if (res && res.isSuccess) {
      const isSaved = res.data
      if (isSaved) {
        message.success('Đã lưu câu hỏi thành công!')
        if (!savedQuestionIdsList.value.includes(id)) {
          savedQuestionIdsList.value.push(id)
        }
      } else {
        message.success('Đã bỏ lưu câu hỏi thành công!')
        savedQuestionIdsList.value = savedQuestionIdsList.value.filter(savedId => savedId !== id)
        if (listTab.value === 'saved') {
          questions.value = questions.value.filter(q => q.id !== id)
          totalItems.value = Math.max(0, totalItems.value - 1)
        }
      }
    } else {
      message.error(res.errorMessage || 'Lỗi khi thực hiện thao tác')
    }
  } catch (error) {
    message.error('Không thể thực hiện tác vụ lưu câu hỏi')
  }
}

onMounted(async () => {
  await fetchCategories()
  await fetchSavedQuestionIds()
  await fetchQuestions()
})
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.text-indigo {
  color: #6366f1;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.1);
}

.bg-light-soft {
  background-color: #f8fafc;
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  color: white;
  transition: all 0.2s ease;
  border: none;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.25);
}

.btn-outline-indigo {
  color: #6366f1;
  border-color: #6366f1;
  transition: all 0.2s ease;
}

.btn-outline-indigo:hover {
  background-color: rgba(99, 102, 241, 0.05);
  color: #4f46e5;
}

.hover-up {
  transition: all 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.2);
}

.badge-public {
  background-color: #dcfce7;
  color: #15803d;
  font-size: 0.8rem;
  padding: 3px 8px;
  border-radius: 6px;
  font-weight: 500;
}

.badge-private {
  background-color: #f1f5f9;
  color: #475569;
  font-size: 0.8rem;
  padding: 3px 8px;
  border-radius: 6px;
  font-weight: 500;
}

.text-truncate-1 {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.text-truncate-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.fs-8 {
  font-size: 0.75rem;
}

.fs-9 {
  font-size: 0.7rem;
}

.custom-table :deep(.ant-table-thead > tr > th) {
  background-color: #f8fafc;
  color: #1e1b4b;
  font-weight: 600;
}

.custom-table :deep(.ant-table-row:hover) {
  background-color: rgba(99, 102, 241, 0.02);
}

.custom-list-tabs :deep(.ant-tabs-tab) {
  user-select: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
}
</style>
