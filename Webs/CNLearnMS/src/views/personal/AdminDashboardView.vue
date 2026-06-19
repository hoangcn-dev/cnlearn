<template>
  <div class="admin-dashboard small-font">

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
                      <template #title="{ title }">
                        <span class="d-flex align-items-center justify-content-between w-100 py-1">
                          <span class="fw-semibold text-dark">{{ title }}</span>
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
import { ref, reactive, computed, onMounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { CategorySelect, type CategoryNode, type QuestionCategory, buildCategoryTree, getRecursiveChildIds, generateSlug } from '@/components/category'
import { getAllCate, addCategories, updateCategories, deleteCategories } from '@/api/categories'

// State variables
const activeTab = ref('categories')
const loading = ref(false)
const actionLoading = ref(false)
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

// Fetch categories from API
const fetchCategories = async () => {
  loading.value = true
  try {
    const res = await getAllCate()
    if (res.isSuccess && res.data) {
      flatCategories.value = res.data.items || []
    }
  } catch (err: any) {
    console.error('Lỗi khi lấy danh mục:', err)
  } finally {
    loading.value = false
  }
}

// Build Tree representation from flat list
const treeData = computed(() => {
  return buildCategoryTree(flatCategories.value)
})

// Search query filter for the tree
const filteredTreeData = computed(() => {
  if (!searchQuery.value.trim()) return treeData.value
  
  const filter = (nodes: CategoryNode[]): CategoryNode[] => {
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

// Available parent categories (excludes self and its descendants to avoid circular references)
const availableParentCategories = computed(() => {
  if (!isEditing.value) return flatCategories.value
  
  const currentCategory = flatCategories.value.find(c => c.questionCategoryId === editingForm.id)
  if (!currentCategory) return flatCategories.value
  
  const childIds = getRecursiveChildIds(currentCategory, flatCategories.value)
  const excludedIds = new Set<string>([editingForm.id, ...childIds])
  
  return flatCategories.value.filter(cat => !excludedIds.has(cat.questionCategoryId))
})

// Reconstruct Full Name
const reconstructedFullName = computed(() => {
  if (!editingForm.localName.trim()) return ''
  
  if (editingForm.parentId) {
    const parent = flatCategories.value.find(c => c.questionCategoryId === editingForm.parentId)
    if (parent) {
      return `${parent.questionCategoryName} - ${editingForm.localName.trim()}`
    }
  }
  
  return editingForm.localName.trim()
})

// Auto generate slug from local name
const autoGenerateSlug = () => {
  editingForm.slug = generateSlug(editingForm.localName)
}

// Select a node in the tree to edit
const handleSelectNode = (keys: any[], info: any) => {
  if (keys.length === 0 || !info.node || !info.node.originalItem) {
    resetForm()
    return
  }
  
  const original = info.node.originalItem as QuestionCategory
  isEditing.value = true
  editingForm.id = original.questionCategoryId
  editingForm.slug = original.questionCategorySlug || ''
  
  // Local name is the last part of full name split by ' - '
  const parts = original.questionCategoryName.split(' - ')
  editingForm.localName = parts[parts.length - 1] || ''
  editingForm.parentId = original.parentId || ''
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
  const fullName = reconstructedFullName.value
  const duplicate = flatCategories.value.find(c => 
    c.questionCategoryName.toLowerCase() === fullName.toLowerCase() && 
    c.questionCategoryId !== editingForm.id
  )
  if (duplicate) {
    message.error('Tên danh mục đầy đủ này đã tồn tại trong hệ thống!')
    return
  }

  actionLoading.value = true
  
  // Create payload
  const payloadItem = {
    questionCategoryId: editingForm.id || undefined,
    questionCategoryName: fullName,
    questionCategorySlug: editingForm.slug.trim(),
    parentId: editingForm.parentId || null
  }

  try {
    const res = isEditing.value ? await updateCategories([payloadItem]) : await addCategories([payloadItem])
    if (res.isSuccess) {
      message.success(isEditing.value ? 'Cập nhật danh mục thành công!' : 'Tạo danh mục mới thành công!')
      await fetchCategories()
      resetForm()
    } else {
      message.error(res.errorMessage || 'Lưu danh mục thất bại')
    }
  } catch (err) {
    console.error('Failed to save to API:', err)
  } finally {
    actionLoading.value = false
  }
}

// Delete category
const handleDelete = () => {
  if (!editingForm.id) return

  // Warn if this category has children
  const children = flatCategories.value.filter(c => c.parentId === editingForm.id)

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
        const res = await deleteCategories([editingForm.id])
        if (res.isSuccess) {
          message.success('Xóa danh mục thành công!')
          await fetchCategories()
          resetForm()
        } else {
          message.error(res.errorMessage || 'Xóa danh mục thất bại')
        }
      } catch (err) {
        console.error('API Delete error:', err)
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
