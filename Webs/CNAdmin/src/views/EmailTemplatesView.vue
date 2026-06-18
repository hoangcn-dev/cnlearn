F<script lang="ts" setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { EditOutlined, ReloadOutlined, AppstoreOutlined, TableOutlined } from '@ant-design/icons-vue';
import { getAllTemplates, type EmailTemplate } from '@/api/emailTemplate';
import { getErrorMessage } from '@/api/config/axios';

const router = useRouter();
const loading = ref(false);
const templatesList = ref<EmailTemplate[]>([]);
const viewMode = ref<'grid' | 'table'>('grid'); // Grid or Table toggle

// Filtered Templates (direct map)
const filteredTemplates = computed(() => {
  return templatesList.value;
});

// Load templates from API
const loadTemplates = async () => {
  loading.value = true;
  try {
    const res = await getAllTemplates();
    if (res.isSuccess) {
      templatesList.value = res.data?.items || res.data || [];
    } else {
      message.error(getErrorMessage(res, 'Không thể tải danh sách mẫu email.'));
    }
  } catch (error: any) {
    console.error('Error loading templates:', error);
    message.error(getErrorMessage(error, 'Có lỗi xảy ra khi kết nối máy chủ.'));
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadTemplates();
});

// Navigate to dedicated edit page
const handleEdit = (record: EmailTemplate) => {
  router.push({ name: 'email-template-edit', params: { id: record.emailTemplateId } });
};
</script>

<template>
  <div class="template-management-container container-fluid py-3 px-4">
    <!-- Header Controls Area -->
    <div class="d-flex justify-content-end mb-4">
      <div class="d-flex align-items-center gap-2">
        <!-- Toggle View Mode Buttons -->
        <div class="btn-group shadow-sm bg-white p-1 rounded border gap-1" style="height: 38px;">
          <button type="button" class="btn btn-sm px-2.5 rounded d-flex align-items-center justify-content-center"
            :class="viewMode === 'grid' ? 'btn-primary text-white' : 'btn-light text-secondary'" @click="viewMode = 'grid'" style="border: none;">
            <AppstoreOutlined class="fs-6" />
          </button>
          <button type="button" class="btn btn-sm px-2.5 rounded d-flex align-items-center justify-content-center"
            :class="viewMode === 'table' ? 'btn-primary text-white' : 'btn-light text-secondary'" @click="viewMode = 'table'" style="border: none;">
            <TableOutlined class="fs-6" />
          </button>
        </div>

        <!-- Corrected Reload Button UI (Perfect Alignment) -->
        <a-button type="default" @click="loadTemplates" :loading="loading" class="d-flex align-items-center justify-content-center shadow-sm" style="height: 38px; width: 38px; border-radius: 8px; padding: 0;">
          <template #icon><ReloadOutlined style="font-size: 16px;" /></template>
        </a-button>
      </div>
    </div>

    <!-- Main Content Area -->
    <div v-if="loading && templatesList.length === 0" class="d-flex flex-column align-items-center justify-content-center py-5">
      <a-spin size="large" />
      <span class="text-secondary mt-3">Đang tải danh sách mẫu email...</span>
    </div>

    <div v-else>
      <!-- Mode 1: Grid Cards (Premium Visual) -->
      <div v-if="viewMode === 'grid'" class="row g-4">
        <div v-if="filteredTemplates.length === 0" class="col-12 text-center py-5 text-muted bg-white rounded shadow-sm border">
          <div class="fs-1">📂</div>
          <h5 class="mt-2 fw-semibold">Không tìm thấy mẫu email phù hợp</h5>
          <p class="text-secondary small">Hệ thống chưa thiết lập mẫu email nào.</p>
        </div>

        <div v-for="item in filteredTemplates" :key="item.emailTemplateId" class="col-12 col-md-6 col-lg-4 text-start">
          <div class="glass-card h-100 p-4 border rounded shadow-sm d-flex flex-column justify-content-between position-relative overflow-hidden">
            <!-- Background Accent Blob -->
            <div class="accent-blob"></div>

            <div>
              <div class="d-flex justify-content-between align-items-center mb-3">
                <span class="badge badge-code px-2.5 py-1.5 rounded-pill font-monospace fw-semibold">{{ item.templateCode }}</span>
                <span class="small text-muted font-monospace text-truncate ms-2" style="max-width: 140px;">ID: {{ item.emailTemplateId }}...</span>
              </div>

              <h5 class="fw-bold text-dark mb-2 text-truncate">{{ item.subject }}</h5>
              
              <!-- Quick Preview Info -->
              <div class="content-preview text-muted small p-2 bg-light rounded border mb-3">
                {{ item.content.replace(/<[^>]*>/g, '').substring(0, 100) }}...
              </div>
            </div>

            <!-- Card Bottom Metrics -->
            <div class="d-flex justify-content-end align-items-center border-top pt-3 mt-auto">
              <div class="d-flex gap-2">
                <a-button type="default" size="middle" class="d-flex align-items-center rounded-3 px-3" @click="handleEdit(item)">
                  <template #icon><EditOutlined class="text-primary" /></template> Sửa
                </a-button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Mode 2: Standard Clean Table -->
      <div v-else class="bg-white rounded-3 border shadow-sm p-3">
        <a-table :dataSource="filteredTemplates" :rowKey="(r: any) => r.emailTemplateId" :loading="loading" :pagination="{ pageSize: 8 }">
          <a-table-column title="Mã mẫu (Code)" dataIndex="templateCode" key="templateCode">
            <template #default="{ text }">
              <span class="badge badge-code px-2.5 py-1.5 rounded font-monospace">{{ text }}</span>
            </template>
          </a-table-column>
          <a-table-column title="Tiêu đề Email (Subject)" dataIndex="subject" key="subject">
            <template #default="{ text }">
              <strong class="text-dark">{{ text }}</strong>
            </template>
          </a-table-column>
          <a-table-column title="Người tạo" dataIndex="createdBy" key="createdBy">
            <template #default="{ text }">
              <span>{{ text || 'Hệ thống' }}</span>
            </template>
          </a-table-column>
          <a-table-column title="Ngày chỉnh sửa" dataIndex="modifiedDate" key="modifiedDate">
            <template #default="{ text, record }">
              <span class="small text-muted">{{ record.modifiedDate ? new Date(record.modifiedDate).toLocaleString('vi-VN') : new Date(record.createdDate || '').toLocaleString('vi-VN') }}</span>
            </template>
          </a-table-column>
          <a-table-column title="Thao tác" key="action" width="120">
            <template #default="{ record }">
              <a-button type="link" size="small" class="fw-semibold px-0 d-flex align-items-center gap-1" @click="handleEdit(record)">
                <EditOutlined /> Chỉnh sửa
              </a-button>
            </template>
          </a-table-column>
        </a-table>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&family=Fira+Code:wght@400;500&display=swap');

.template-management-container {
  font-family: 'Outfit', sans-serif;
  background-color: #f8fafc;
  min-height: 100%;
}

/* Glassmorphism premium card styling */
.glass-card {
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  border: 1px solid rgba(226, 232, 240, 0.8) !important;
  border-radius: 16px !important;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.glass-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.05), 0 2px 6px rgba(0, 0, 0, 0.02) !important;
  border-color: #0d6efd !important;
}

.accent-blob {
  position: absolute;
  top: -40px;
  right: -40px;
  width: 100px;
  height: 100px;
  background: radial-gradient(circle, rgba(13,110,253,0.06) 0%, rgba(111,66,193,0.03) 70%);
  border-radius: 50%;
  pointer-events: none;
  z-index: 0;
}

.badge-code {
  background-color: rgba(13, 110, 253, 0.08);
  color: #0d6efd;
  border: 1px solid rgba(13, 110, 253, 0.15);
  font-family: 'Fira Code', monospace;
  font-size: 0.8rem;
}

.content-preview {
  height: 60px;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
  line-height: 1.4;
  border-color: #f1f5f9 !important;
}
</style>
