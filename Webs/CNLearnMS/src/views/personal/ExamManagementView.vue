<template>
  <div class="exam-management-view py-3 small-font">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-3">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Quản lý đề thi</li>
      </ol>
    </nav>

    <!-- Header -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
      <div>
        <h1 class="fs-4 fw-bold text-dark-blue mb-1">Quản Lý Đề Thi Cá Nhân</h1>
        <p class="text-secondary small mb-0">Tạo mới, chỉnh sửa, xóa và quản lý danh sách đề thi của bạn.</p>
      </div>
    </div>

    <!-- Action Bar -->
    <div class="card bg-white mb-3 p-3">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-center gap-3">
        <!-- Search & Filter -->
        <div class="d-flex gap-2 align-items-center flex-grow-1" style="max-width: 500px;">
          <div class="flex-grow-1" style="max-width: 320px;">
            <a-input v-model:value="request.key" placeholder="Tìm kiếm tên đề thi..." allow-clear>
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

        <!-- Add Exam Button -->
        <button class="btn btn-indigo text-white fw-semibold hover-up d-flex align-items-center gap-2 btn-sm flex-shrink-0" @click="goToCreatePage">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="me-1"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
          Thêm đề thi mới
        </button>
      </div>
    </div>

    <!-- Table Card -->
    <div class="card bg-white mb-5 p-3">
      <div class="table-responsive py-1">
        <a-table 
          :columns="columns" 
          :data-source="exams" 
          row-key="examId"
          :pagination="{
            current: request.page,
            pageSize: request.size,
            total: totalItems,
            showSizeChanger: false,
            size: 'small'
          }"
          :loading="loading"
          @change="handleTableChange"
          class="custom-table"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'name'">
              <div class="fw-semibold text-dark-blue exam-title-link" style="cursor: pointer;" @click="goToEditPage(record.examId)">
                {{ record.name }}
                <span v-if="record.isDraft" class="badge bg-secondary text-white ms-1 fw-bold fs-9">Nháp</span>
              </div>
              <div class="text-secondary small mt-1">
                <span class="fst-italic text-truncate-1 d-inline-block align-middle me-2">
                  {{ record.durationMin }}m - {{ record.questionCategoryName || 'Chưa phân loại' }}
                </span>
                <span v-if="record.description" class="text-muted d-inline-block align-middle border-start ps-2 text-truncate-1">
                  {{ record.description }}
                </span>
              </div>
            </template>

            <template v-else-if="column.key === 'actions'">
              <div class="d-flex justify-content-end pe-2">
                <a-dropdown trigger="click" placement="bottomRight">
                  <button class="btn btn-xs btn-outline-secondary p-1 rounded-circle d-flex align-items-center justify-content-center" style="width: 26px; height: 26px; border-color: #cbd5e1;">
                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>
                  </button>
                  <template #overlay>
                    <a-menu>
                      <a-menu-item key="view" @click="viewExamDetail(record)">
                        <span class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                          Xem câu hỏi
                        </span>
                      </a-menu-item>
                      <a-menu-item key="edit" @click="goToEditPage(record.examId)">
                        <span class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4z"></path></svg>
                          Chỉnh sửa đề
                        </span>
                      </a-menu-item>
                      <a-menu-item key="delete" @click="confirmDelete(record.examId)">
                        <span class="d-flex align-items-center gap-2 text-secondary small">
                          <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
                          Xóa đề thi
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

    <!-- Filter Modal -->
    <a-modal
      v-model:open="filterModalOpen"
      title="Bộ lọc đề thi"
      :footer="null"
      centered
      width="380px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label small fw-semibold">Danh mục môn học:</label>
          <CategorySelect
            v-model:value="filters.categoryId"
            :categories="categories"
            :disable-parents="true"
            placeholder="Tất cả môn học"
            class="w-100"
            show-all-option
          />
        </div>
        <div class="mb-3">
          <label class="form-label small fw-semibold">Phạm vi:</label>
          <a-select v-model:value="filters.accessType" style="width: 100%" placeholder="Tất cả phạm vi" allow-clear>
            <a-select-option :value="0">🌐 Công khai (Public)</a-select-option>
            <a-select-option :value="1">🔒 Riêng tư</a-select-option>
          </a-select>
        </div>

        <div class="border-top pt-2 mt-4 d-flex justify-content-between align-items-center">
          <button class="btn btn-link btn-xs text-secondary p-0 text-decoration-none small" @click="resetFilters">Đặt lại bộ lọc</button>
          <button class="btn btn-indigo text-white btn-sm px-4" @click="applyFilters">Áp dụng</button>
        </div>
      </div>
    </a-modal>

    <!-- Exam Detail (Questions View) Modal -->
    <a-modal 
      v-model:open="detailModalOpen" 
      :title="selectedExam ? 'Chi tiết đề thi: ' + selectedExam.name : 'Chi tiết đề thi'"
      :footer="null"
      width="750px"
    >
      <div class="py-3" v-if="selectedExam">
        <div class="d-flex justify-content-between align-items-center pb-3 border-bottom mb-3">
          <div>
            <span class="badge bg-indigo-soft text-indigo me-2">{{ selectedExam.questionCategoryName || 'Chuyên đề chung' }}</span>
            <span class="text-secondary small">{{ selectedExam.durationMin }} phút làm bài</span>
          </div>
          <span class="fw-bold text-dark-blue fs-6">Tổng số câu hỏi: {{ detailQuestions.length }} câu</span>
        </div>

        <div class="exam-questions-list" style="max-height: 450px; overflow-y: auto;">
          <div v-for="(q, index) in detailQuestions" :key="q.questionId" class="card card-body p-3 mb-3 border-light shadow-sm bg-light-soft">
            <p class="fw-bold text-dark-blue mb-2">Câu {{ index + 1 }}: <span class="fw-normal">{{ q.stringContent }}</span></p>
            
            <div class="row g-2 mb-2">
              <div v-for="(ans, aIdx) in q.answers" :key="aIdx" class="col-md-6">
                <div class="p-2 border rounded-3 bg-white small d-flex align-items-center gap-2">
                  <span class="badge" :class="ans.isCorrectAnswer ? 'bg-success text-white' : 'bg-light text-dark border'">
                    {{ String.fromCharCode(65 + Number(aIdx)) }}
                  </span>
                  <span>{{ ans.stringContent }}</span>
                </div>
              </div>
            </div>
            
            <div v-if="q.explaination" class="text-muted small mt-2 p-2 bg-white rounded border-start border-indigo border-3">
              <span class="fw-bold text-indigo">Giải thích:</span> <span class="fst-italic">{{ q.explaination }}</span>
            </div>
          </div>
          
          <div v-if="detailQuestions.length === 0" class="text-center py-5 text-muted">
            Đề thi này chưa có câu hỏi nào.
          </div>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch, getCurrentInstance } from 'vue'
import { useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { getAllCate } from '@/api/categories'
import { CategorySelect } from '@/components/category'
import { getExamsPaging, deleteExams, getExamQuestions } from '@/api/exams'
import type { GetRequest, Filter } from '@/models/get-request'
import { FilterGroupType, FilterOperator, FilterType } from '@/models/get-request'
import type { ExamDto } from '@/models/exams'
import type { QuestionCategory } from '@/models/category'
import { showDialog } from '@/utils/dialog'

const router = useRouter()

const categories = ref<QuestionCategory[]>([])
const exams = ref<ExamDto[]>([])
const totalItems = ref(0)
const loading = ref(false)
const filterModalOpen = ref(false)
const detailModalOpen = ref(false)
const selectedExam = ref<ExamDto | null>(null)
const detailQuestions = ref<any[]>([])

const request = ref<GetRequest>({
  page: 1,
  size: 8,
  isPaging: true,
  key: "",
  filters: [],
  filterGroupType: FilterGroupType.And,
})

const filters = reactive({
  categoryId: undefined as string | undefined,
  accessType: undefined as number | undefined
})

const activeFiltersCount = computed(() => {
  let count = 0
  if (filters.categoryId !== undefined) count++
  if (filters.accessType !== undefined) count++
  return count
})

const hasActiveFilters = computed(() => activeFiltersCount.value > 0)

const resetFilters = () => {
  filters.categoryId = undefined
  filters.accessType = undefined
  applyFilters()
}

const applyFilters = () => {
  const apiFilters: Filter[] = []
  if (filters.categoryId) {
    apiFilters.push({
      property: 'QuestionCategoryId',
      operator: FilterOperator.Equal,
      value: filters.categoryId,
      type: FilterType.String
    })
  }
  if (filters.accessType !== undefined) {
    apiFilters.push({
      property: 'AccessType',
      operator: FilterOperator.Equal,
      value: filters.accessType,
      type: FilterType.Number
    })
  }
  request.value.filters = apiFilters
  request.value.page = 1
  filterModalOpen.value = false
  fetchExams()
}

// Columns configuration
const columns = [
  { title: 'Tên đề thi', dataIndex: 'name', key: 'name' },
  { title: 'Thao tác', key: 'actions', width: '90px' }
]

const fetchExams = async () => {
  loading.value = true
  try {
    const res = await getExamsPaging(request.value)
    if (res && res.isSuccess && res.data) {
      exams.value = res.data.items || []
      totalItems.value = res.data.total || 0
    }
  } catch (error) {
    console.error('Lỗi tải danh sách đề thi:', error)
    message.error('Không thể tải danh sách đề thi.')
  } finally {
    loading.value = false
  }
}

const handleTableChange = (pagination: any) => {
  request.value.page = pagination.current
  fetchExams()
}

const fetchCategories = async () => {
  try {
    const res = await getAllCate()
    if (res && res.isSuccess && res.data) {
      categories.value = res.data.items || []
    }
  } catch (err) {
    console.error(err)
  }
}

const viewExamDetail = async (exam: ExamDto) => {
  selectedExam.value = exam
  detailQuestions.value = []
  try {
    const res = await getExamQuestions(exam.examId)
    if (res && res.isSuccess && res.data) {
      detailQuestions.value = res.data
    }
  } catch (err) {
    console.error('Lỗi tải câu hỏi đề thi:', err)
    message.error('Không thể tải danh sách câu hỏi của đề thi này.')
  }
  detailModalOpen.value = true
}

let timeoutId: any = null
watch(() => request.value.key, () => {
  if (timeoutId) clearTimeout(timeoutId)
  timeoutId = setTimeout(() => {
    request.value.page = 1
    fetchExams()
  }, 300)
})

const goToCreatePage = () => {
  router.push('/personal/exams/create')
}

const goToEditPage = (id: string) => {
  router.push(`/personal/exams/edit/${id}`)
}

const instance = getCurrentInstance()
const appContext = instance?.appContext

const confirmDelete = (id: string) => {
  showDialog({
    title: 'Xác nhận xóa đề thi',
    content: 'Bạn có chắc chắn muốn xóa đề thi này không? Thao tác này không thể hoàn tác.',
    okText: 'Xóa đề',
    cancelText: 'Hủy bỏ',
    buttonType: 'danger',
    appContext,
    onOk: async () => {
      try {
        const res = await deleteExams([id])
        if (res && res.isSuccess) {
          message.success('Đã xóa đề thi thành công!')
          await fetchExams()
        } else {
          message.error(res.errorMessage || 'Xóa đề thi thất bại.')
        }
      } catch (error) {
        message.error('Đã xảy ra lỗi khi xóa đề thi.')
      }
    }
  })
}

onMounted(async () => {
  fetchCategories()
  fetchExams()
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
  background-color: rgba(99, 102, 241, 0.08);
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

.text-truncate-1 {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.custom-table :deep(.ant-table-thead > tr > th) {
  background-color: #f8fafc;
  color: #1e1b4b;
  font-weight: 600;
}

.custom-table :deep(.ant-table-row:hover) {
  background-color: rgba(99, 102, 241, 0.02);
}

.exam-title-link {
  cursor: pointer;
  transition: color 0.15s ease;
}

.exam-title-link:hover {
  color: #4f46e5;
  text-decoration: underline;
}
</style>
