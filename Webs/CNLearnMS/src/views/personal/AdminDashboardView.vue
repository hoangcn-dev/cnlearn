<template>
  <div class="admin-dashboard small-font">

    <!-- Alert thông báo lỗi API -->
    <div v-if="apiError" class="alert alert-warning border-0 shadow-sm rounded-3 mb-4 d-flex justify-content-between align-items-center">
      <div class="small">
        <strong>⚠️ Kết nối API:</strong> {{ apiError }} (Đã kích hoạt chế độ tự động lưu trữ Offline để thử nghiệm).
      </div>
      <button type="button" class="btn-close" @click="apiError = ''"></button>
    </div>

    <div class="row g-4">
      <div class="col-12">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4">
          <a-tabs v-model:activeKey="activeTab" class="custom-tabs">
            <!-- TAB 1: QUẢN LÝ DANH MỤC -->
            <a-tab-pane key="categories" tab="📁 Quản lý danh mục">
              <div class="row mt-3 g-4">
                <!-- Cột trái: Cây danh mục -->
                <div class="col-md-5 border-end pe-md-4">

                  <!-- Ô tìm kiếm cây -->
                  <a-input-search 
                    v-model:value="searchQuery" 
                    placeholder="Tìm kiếm danh mục..." 
                    class="mb-3"
                    allow-clear 
                  />

                  <!-- Loading state -->
                  <div v-if="loading" class="text-center py-5">
                    <div class="spinner-border text-indigo spinner-border-sm" role="status"></div>
                    <span class="text-secondary ms-2 small">Đang tải danh mục...</span>
                  </div>

                  <!-- Cây thư mục Ant-Design -->
                  <div v-else class="tree-container p-2 border rounded-3 bg-light-soft overflow-auto" style="max-height: 480px; min-height: 300px;">
                    <a-tree
                      v-if="treeData.length > 0"
                      :tree-data="filteredTreeData"
                      :default-expand-all="true"
                      v-model:selectedKeys="selectedTreeKeys"
                      @select="handleSelectNode"
                    >
                      <template #title="{ title, dataRef }">
                        <span class="d-flex align-items-center justify-content-between w-100 py-1">
                          <span class="fw-semibold text-dark">{{ title }}</span>
                          <span class="badge bg-secondary-soft text-secondary ms-2 fs-9">{{ dataRef.originalItem ? 'Entity' : 'Virtual' }}</span>
                        </span>
                      </template>
                    </a-tree>
                    <div v-else class="text-center text-muted py-5">
                      Chưa có danh mục nào được khởi tạo.
                    </div>
                  </div>
                </div>

                <!-- Cột phải: Form chi tiết -->
                <div class="col-md-7 ps-md-4">
                  <div class="card border-light-subtle rounded-3 bg-light-soft p-4">
                    <div class="d-flex justify-content-between align-items-center border-bottom pb-2 mb-3">
                      <h4 class="fw-bold text-dark-blue mb-0">
                        {{ isEditing ? 'Chi tiết danh mục' : 'Tạo danh mục mới' }}
                      </h4>
                      <span v-if="isEditing" class="badge bg-indigo-soft text-indigo">ID: {{ editingForm.id }}</span>
                    </div>

                    <a-form layout="vertical">
                      <div class="row">
                        <!-- Tên danh mục cục bộ -->
                        <div class="col-md-6">
                          <a-form-item label="Tên danh mục (nhãn cục bộ):" required>
                            <a-input 
                              v-model:value="editingForm.localName" 
                              placeholder="Ví dụ: Đại số, Vật lý hạt nhân..." 
                              @input="autoGenerateSlug"
                            />
                          </a-form-item>
                        </div>

                        <!-- Đường dẫn SEO -->
                        <div class="col-md-6">
                          <a-form-item label="Đường dẫn SEO (Slug):" required>
                            <a-input 
                              v-model:value="editingForm.slug" 
                              placeholder="Ví dụ: dai-so, vat-ly-hat-nhan" 
                            />
                          </a-form-item>
                        </div>
                      </div>

                      <div class="row">
                        <!-- Danh mục cha -->
                        <div class="col-12">
                          <a-form-item label="Danh mục cấp cha:">
                            <CategorySelect
                              v-model:value="editingForm.parentId"
                              :categories="availableParentCategories"
                              placeholder="Chọn danh mục cha (Để trống nếu là gốc)"
                              class="w-100"
                            />
                          </a-form-item>
                        </div>
                      </div>

                      <!-- Đã ẩn thẻ Preview Tên đầy đủ hệ thống để giao diện sạch sẽ hơn -->

                      <!-- Action buttons -->
                      <div class="d-flex justify-content-between align-items-center border-top pt-3">
                        <div>
                          <button 
                            v-if="isEditing" 
                            type="button" 
                            class="btn btn-outline-danger btn-sm fw-semibold" 
                            :disabled="actionLoading"
                            @click="handleDelete"
                          >
                            🗑 Xóa danh mục
                          </button>
                        </div>
                        <div class="d-flex gap-2">
                          <button 
                            type="button" 
                            class="btn btn-light btn-sm fw-semibold border" 
                            @click="resetForm"
                          >
                            Hủy bỏ
                          </button>
                          <button 
                            type="button" 
                            class="btn btn-indigo text-white btn-sm fw-semibold px-4" 
                            :disabled="actionLoading"
                            @click="handleSave"
                          >
                            <span v-if="actionLoading" class="spinner-border spinner-border-sm me-1" role="status"></span>
                            Lưu dữ liệu
                          </button>
                        </div>
                      </div>
                    </a-form>
                  </div>
                </div>
              </div>
            </a-tab-pane>

            <!-- CÁC TAB CHỜ PHÁT TRIỂN -->
            <a-tab-pane key="users" tab="👤 Quản lý người dùng" disabled>
              Tab quản lý thành viên
            </a-tab-pane>
            <a-tab-pane key="system" tab="⚙️ Cấu hình hệ thống" disabled>
              Tab cấu hình hệ thống
            </a-tab-pane>
          </a-tabs>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { message, Modal } from 'ant-design-vue'
import axios from 'axios'
import { CategorySelect } from '@/components/category'

// API configurations
const API_URL = 'http://localhost:5000/api/questioncategories'

// Interface
interface QuestionCategory {
  questionCategoryId: string
  name: string
  slug: string
}

interface TreeNode {
  key: string
  title: string
  slug?: string
  children?: TreeNode[]
  originalItem?: QuestionCategory
}

// State variables
const activeTab = ref('categories')
const loading = ref(false)
const actionLoading = ref(false)
const apiError = ref('')
const searchQuery = ref('')
const selectedTreeKeys = ref<string[]>([])

// Master flat list
const flatCategories = ref<QuestionCategory[]>([])

// Form state
const isEditing = ref(false)
const editingForm = reactive({
  id: '',
  localName: '',
  slug: '',
  parentId: ''
})

// MOCK data to fallback when API fails
const MOCK_CATEGORIES: QuestionCategory[] = [
  { questionCategoryId: "c01a92a2-a69f-4143-8589-da11688d7d01", slug: "toan-hoc", name: "Toán Học" },
  { questionCategoryId: "c02a92a2-a69f-4143-8589-da11688d7d02", slug: "toan-hoc-luyen-thi-thpt-quoc-gia", name: "Toán Học - Luyện Thi THPT Quốc Gia" },
  { questionCategoryId: "c03a92a2-a69f-4143-8589-da11688d7d03", slug: "vat-ly", name: "Vật Lý" },
  { questionCategoryId: "c04a92a2-a69f-4143-8589-da11688d7d04", slug: "vat-ly-chuyen-de-dong-dien-xoay-chieu", name: "Vật Lý - Chuyên Đề Dòng Điện Xoay Chiều" },
  { questionCategoryId: "c05a92a2-a69f-4143-8589-da11688d7d05", slug: "hoa-hoc-chuyen-de-hoa-huu-co", name: "Hóa Học - Chuyên Đề Hóa Hữu Cơ" },
  { questionCategoryId: "c06a92a2-a69f-4143-8589-da11688d7d06", slug: "tieng-anh-reading", name: "Tiếng Anh - IELTS Reading Academic" }
]

// Fetch categories from API or LocalStorage
const fetchCategories = async () => {
  loading.value = true
  apiError.value = ''
  
  try {
    const response = await axios.get(API_URL)
    if (response.data && response.data.isSuccess && response.data.data) {
      const items = response.data.data.items || []
      flatCategories.value = items
      // Sync offline data
      localStorage.setItem('offline_categories', JSON.stringify(items))
    } else {
      throw new Error('Đầu ra API không đúng định dạng')
    }
  } catch (err: any) {
    console.error('API Error:', err)
    apiError.value = 'Không thể kết nối đến API Máy chủ. Sử dụng CSDL Offline.'
    
    // Load offline storage or mock fallback
    const savedOffline = localStorage.getItem('offline_categories')
    if (savedOffline) {
      flatCategories.value = JSON.parse(savedOffline)
    } else {
      flatCategories.value = [...MOCK_CATEGORIES]
      localStorage.setItem('offline_categories', JSON.stringify(MOCK_CATEGORIES))
    }
  } finally {
    loading.value = false
  }
}

// Build Tree representation from flat list using delimiter " - "
const treeData = computed(() => {
  const rootNodes: TreeNode[] = []
  
  flatCategories.value.forEach(item => {
    const parts = item.name.split(' - ').map(p => p.trim())
    let currentLevel = rootNodes
    
    parts.forEach((part, index) => {
      let node = currentLevel.find(n => n.title === part)
      
      if (!node) {
        node = {
          key: index === parts.length - 1 ? item.questionCategoryId : `temp-${part}-${index}`,
          title: part,
          slug: index === parts.length - 1 ? item.slug : undefined,
          children: [],
          originalItem: index === parts.length - 1 ? item : undefined
        }
        currentLevel.push(node)
      } else if (index === parts.length - 1) {
        // If leaf reached, assign properties
        node.key = item.questionCategoryId
        node.slug = item.slug
        node.originalItem = item
      }
      currentLevel = node.children!
    })
  })
  
  // Remove empty children array to disable tree collapse handles
  const pruneChildren = (nodes: TreeNode[]) => {
    nodes.forEach(n => {
      if (n.children && n.children.length === 0) {
        delete n.children
      } else if (n.children) {
        pruneChildren(n.children)
      }
    })
  }
  
  pruneChildren(rootNodes)
  return rootNodes
})

// Search query filter for the tree
const filteredTreeData = computed(() => {
  if (!searchQuery.value.trim()) return treeData.value
  
  const filter = (nodes: TreeNode[]): TreeNode[] => {
    return nodes
      .map(n => ({ ...n }))
      .filter(n => {
        let matches = n.title.toLowerCase().includes(searchQuery.value.toLowerCase())
        if (n.children) {
          n.children = filter(n.children)
          matches = matches || n.children.length > 0
        }
        return matches
      })
  }
  return filter(treeData.value)
})

// Available parent categories (excludes self to avoid circular reference)
const availableParentCategories = computed(() => {
  if (!isEditing.value) return flatCategories.value
  
  // Return all flat items except the one we are editing
  return flatCategories.value.filter(cat => cat.questionCategoryId !== editingForm.id)
})

// Reconstruct Full Name
const reconstructedFullName = computed(() => {
  if (!editingForm.localName.trim()) return '*(Đang soạn thảo)*'
  
  if (editingForm.parentId) {
    const parent = flatCategories.value.find(c => c.questionCategoryId === editingForm.parentId)
    if (parent) {
      return `${parent.name} - ${editingForm.localName.trim()}`
    }
  }
  
  return editingForm.localName.trim()
})

// Auto generate slug from local name
const autoGenerateSlug = () => {
  if (!editingForm.localName) {
    editingForm.slug = ''
    return
  }
  
  let str = editingForm.localName.toLowerCase()
  // Remove accents
  str = str.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
  // Replace d -> dd
  str = str.replace(/[đĐ]/g, 'd')
  // Replace non-alphanumeric characters with spaces
  str = str.replace(/[^a-z0-9\s-]/g, '')
  // Trim and replace multiple spaces with single hyphen
  str = str.trim().replace(/\s+/g, '-')
  editingForm.slug = str
}

// Select a node in the tree to edit
const handleSelectNode = (keys: any[], info: any) => {
  if (keys.length === 0 || !info.node.originalItem) {
    resetForm()
    return
  }
  
  const original = info.node.originalItem as QuestionCategory
  isEditing.value = true
  editingForm.id = original.questionCategoryId
  editingForm.slug = original.slug
  
  // Local name is the last part of full name split by ' - '
  const parts = original.name.split(' - ')
  editingForm.localName = parts[parts.length - 1] || ''
  
  // Determine parent ID: If name has parent prefix, find matching category
  if (parts.length > 1) {
    const parentName = parts.slice(0, parts.length - 1).join(' - ')
    const parent = flatCategories.value.find(c => c.name === parentName)
    editingForm.parentId = parent ? parent.questionCategoryId : ''
  } else {
    editingForm.parentId = ''
  }
}

// Prepare to create a Root category
const prepareCreateRoot = () => {
  isEditing.value = false
  selectedTreeKeys.value = []
  editingForm.id = ''
  editingForm.localName = ''
  editingForm.slug = ''
  editingForm.parentId = ''
}

// Reset form
const resetForm = () => {
  isEditing.value = false
  selectedTreeKeys.value = []
  editingForm.id = ''
  editingForm.localName = ''
  editingForm.slug = ''
  editingForm.parentId = ''
}

// Create or update category
const handleSave = async () => {
  if (!editingForm.localName.trim()) {
    message.error('Tên danh mục không được phép để trống!')
    return
  }
  if (!editingForm.slug.trim()) {
    message.error('SEO Slug không được phép để trống!')
    return
  }

  // Double check duplicates
  const duplicate = flatCategories.value.find(c => 
    c.name.toLowerCase() === reconstructedFullName.value.toLowerCase() && 
    c.questionCategoryId !== editingForm.id
  )
  if (duplicate) {
    message.error('Tên danh mục đầy đủ này đã tồn tại trong hệ thống!')
    return
  }

  actionLoading.value = true
  
  // Create payload
  const targetId = editingForm.id || crypto.randomUUID()
  const payloadItem = {
    questionCategoryId: targetId,
    name: reconstructedFullName.value,
    slug: editingForm.slug.trim()
  }

  try {
    // API request (Post expects List<QuestionCategory>)
    await axios.post(API_URL, [payloadItem])
    message.success(isEditing.value ? 'Cập nhật danh mục thành công!' : 'Tạo danh mục mới thành công!')
    
    // Refresh
    await fetchCategories()
    resetForm()
  } catch (err) {
    console.error('Failed to save to API:', err)
    
    // Offline simulation
    if (isEditing.value) {
      const idx = flatCategories.value.findIndex(c => c.questionCategoryId === editingForm.id)
      if (idx !== -1) {
        flatCategories.value[idx] = payloadItem
      }
    } else {
      flatCategories.value.push(payloadItem)
    }
    
    localStorage.setItem('offline_categories', JSON.stringify(flatCategories.value))
    message.success('Đã lưu danh mục thành công ( Offline Simulation )!')
    resetForm()
  } finally {
    actionLoading.value = false
  }
}

// Delete category
const handleDelete = () => {
  if (!editingForm.id) return

  // Warn if this category has children
  const children = flatCategories.value.filter(c => {
    const parts = c.name.split(' - ')
    if (parts.length > 1) {
      const parentName = parts.slice(0, parts.length - 1).join(' - ')
      const currentEditing = flatCategories.value.find(curr => curr.questionCategoryId === editingForm.id)
      return currentEditing && parentName === currentEditing.name
    }
    return false
  })

  const confirmContent = children.length > 0 
    ? `Hành động này sẽ xóa danh mục này. Chú ý: Danh mục này đang có ${children.length} danh mục con cấp dưới phụ thuộc. Bạn có chắc chắn muốn xóa?`
    : `Bạn có chắc chắn muốn xóa danh mục này khỏi hệ thống?`

  Modal.confirm({
    title: 'Xác nhận xóa danh mục',
    content: confirmContent,
    okText: 'Xóa ngay',
    cancelText: 'Hủy bỏ',
    okType: 'danger',
    async onOk() {
      actionLoading.value = true
      try {
        await axios.post(`${API_URL}/delete`, { ids: [editingForm.id] })
        message.success('Xóa danh mục thành công!')
        await fetchCategories()
        resetForm()
      } catch (err) {
        console.error('API Delete error:', err)
        // Offline simulation
        flatCategories.value = flatCategories.value.filter(c => c.questionCategoryId !== editingForm.id)
        localStorage.setItem('offline_categories', JSON.stringify(flatCategories.value))
        message.success('Đã xóa danh mục thành công ( Offline Simulation )!')
        resetForm()
      } finally {
        actionLoading.value = false
      }
    }
  })
}

// Load data on start
onMounted(() => {
  fetchCategories()
})
</script>

<style scoped>
.admin-banner {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  border-radius: 12px;
}

.text-dark-blue {
  color: #1e1b4b;
}

.bg-light-soft {
  background-color: #f8fafc;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.1);
}

.bg-secondary-soft {
  background-color: rgba(100, 116, 139, 0.1);
}

.fs-9 {
  font-size: 0.72rem;
}

.tree-container {
  border: 1px solid rgba(0, 0, 0, 0.08);
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  color: white;
  border: none;
  transition: all 0.2s ease;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.2);
}

.custom-tabs :deep(.ant-tabs-nav) {
  margin-bottom: 8px;
}

.custom-tabs :deep(.ant-tabs-tab-active .ant-tabs-tab-btn) {
  color: #6366f1;
  font-weight: 600;
}

.custom-tabs :deep(.ant-tabs-ink-bar) {
  background-color: #6366f1;
}

.break-all {
  word-break: break-all;
}

.admin-dashboard :deep(.ant-tree-node-content-wrapper) {
  width: calc(100% - 24px) !important;
}
</style>
