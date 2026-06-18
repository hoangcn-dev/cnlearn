<script lang="ts" setup>
import { ref, onMounted, computed, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { message, Modal } from 'ant-design-vue';
import { 
  ArrowLeftOutlined, 
  SaveOutlined, 
  LayoutOutlined,
  CodeOutlined,
  InfoCircleOutlined
} from '@ant-design/icons-vue';
import { getTemplateById, saveTemplate, type EmailTemplate } from '@/api/emailTemplate';
import { getErrorMessage } from '@/api/config/axios';

const route = useRoute();
const router = useRouter();
const templateIdParam = route.params.id as string;

const loading = ref(false);
const isSaving = ref(false);
const originalTemplate = ref<EmailTemplate | null>(null);

// Form State
const editingTemplate = reactive<EmailTemplate>({
  emailTemplateId: '',
  templateCode: '',
  subject: '',
  content: '',
});

// Real-time Preview Mock Parameters
const previewParams = reactive({
  DisplayName: 'Hoàng Cao Nguyên',
  TemporaryPassword: 'CN' + Math.floor(100000 + Math.random() * 900000),
  ExpireTime: '15 phút',
});

// Computed Preview HTML
const livePreviewHtml = computed(() => {
  let content = editingTemplate.content || '';
  if (!content.trim()) {
    return `<div class="d-flex flex-column align-items-center justify-content-center py-5 text-muted">
      <span style="font-size: 2.5rem; opacity: 0.5;">✉️</span>
      <p class="mt-2 mb-0">Chưa có nội dung HTML. Nhập mã ở khung bên trái để xem trước.</p>
    </div>`;
  }
  // Thay thế các biến placeholder dạng {{Param}} với style premium để hiển thị dễ nhìn
  content = content.replace(/\{\{DisplayName\}\}/g, `<span class="preview-hl">${previewParams.DisplayName}</span>`);
  content = content.replace(/\{\{TemporaryPassword\}\}/g, `<span class="preview-hl-pass">${previewParams.TemporaryPassword}</span>`);
  content = content.replace(/\{\{ExpireTime\}\}/g, `<span class="preview-hl-time">${previewParams.ExpireTime}</span>`);
  return content;
});

// Helper: Format raw HTML to beautifully indented HTML with newlines
const formatHtml = (html: string): string => {
  if (!html) return '';
  let formatted = '';
  let indent = 0;
  
  // Clean up existing whitespace and newlines first to avoid double spacing
  const cleanHtml = html
    .replace(/>\s+</g, '><')
    .replace(/\s+/g, ' ')
    .trim();
    
  const reg = /(<\/?[a-zA-Z0-9]+[^>]*>)/g;
  const parts = cleanHtml.split(reg).filter(p => p.trim() !== '');
  
  parts.forEach((part) => {
    if (part.startsWith('</')) {
      indent = Math.max(0, indent - 1);
      formatted += '  '.repeat(indent) + part + '\n';
    } else if (part.startsWith('<')) {
      const isSelfClosing = part.endsWith('/>') || 
                            part.toLowerCase().startsWith('<br') || 
                            part.toLowerCase().startsWith('<hr') || 
                            part.toLowerCase().startsWith('<img');
      formatted += '  '.repeat(indent) + part + '\n';
      if (!isSelfClosing) {
        indent++;
      }
    } else {
      formatted += '  '.repeat(indent) + part + '\n';
    }
  });
  
  return formatted.trim();
};

// Load template detail
const loadTemplate = async () => {
  loading.value = true;
  try {
    const res = await getTemplateById(templateIdParam);
    if (res.isSuccess && res.data) {
      originalTemplate.value = { ...res.data };
      editingTemplate.emailTemplateId = res.data.emailTemplateId;
      editingTemplate.templateCode = res.data.templateCode;
      editingTemplate.subject = res.data.subject;
      editingTemplate.content = formatHtml(res.data.content);
    } else {
      message.error(getErrorMessage(res, 'Không thể tải thông tin mẫu email.'));
      router.push({ name: 'email-templates' });
    }
  } catch (error: any) {
    console.error('Error loading template:', error);
    message.error(getErrorMessage(error, 'Lỗi khi tải dữ liệu mẫu email.'));
    router.push({ name: 'email-templates' });
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  loadTemplate();
});

// Insert Placeholder Helper
const insertPlaceholder = (placeholder: string) => {
  const codeArea = document.getElementById('templateContentArea') as HTMLTextAreaElement;
  if (codeArea) {
    const start = codeArea.selectionStart;
    const end = codeArea.selectionEnd;
    const text = codeArea.value;
    const before = text.substring(0, start);
    const after = text.substring(end, text.length);
    editingTemplate.content = before + `{{${placeholder}}}` + after;
    
    // Tự động focus lại
    setTimeout(() => {
      codeArea.focus();
      const newPos = start + placeholder.length + 4;
      codeArea.setSelectionRange(newPos, newPos);
    }, 50);
  } else {
    editingTemplate.content += `{{${placeholder}}}`;
  }
  message.success(`Đã chèn biến {{${placeholder}}}`);
};

// Handle Save
const handleSave = async () => {
  if (!editingTemplate.subject.trim()) {
    message.error('Tiêu đề email không được để trống.');
    return;
  }
  if (!editingTemplate.content.trim()) {
    message.error('Nội dung template không được để trống.');
    return;
  }

  // Bắt buộc mẫu email quên mật khẩu phải có đủ các tham số mặc định
  if (editingTemplate.templateCode.trim().toLowerCase() === 'forgot_password') {
    if (!editingTemplate.content.includes('{{DisplayName}}') || 
        !editingTemplate.content.includes('{{TemporaryPassword}}') || 
        !editingTemplate.content.includes('{{ExpireTime}}')) {
      Modal.confirm({
        title: 'Thiếu tham số bắt buộc',
        content: 'Mẫu email quên mật khẩu không được phép thiếu các tham số mặc định: {{DisplayName}}, {{TemporaryPassword}} và {{ExpireTime}}.',
        okText: 'Reset thay đổi',
        cancelText: 'Hủy bỏ',
        okButtonProps: { type: 'primary', danger: true },
        onOk: () => {
          if (originalTemplate.value) {
            editingTemplate.content = formatHtml(originalTemplate.value.content);
            editingTemplate.subject = originalTemplate.value.subject;
            message.success('Đã khôi phục lại mẫu email gốc.');
          }
        }
      });
      return;
    }
  }

  isSaving.value = true;
  try {
    const dataToSave: EmailTemplate = {
      emailTemplateId: editingTemplate.emailTemplateId,
      templateCode: editingTemplate.templateCode.trim(),
      subject: editingTemplate.subject.trim(),
      content: editingTemplate.content.trim(),
    };

    const res = await saveTemplate(dataToSave);
    if (res.isSuccess) {
      message.success('Lưu mẫu email thành công!');
      router.push({ name: 'email-templates' });
    } else {
      message.error(getErrorMessage(res, 'Lưu thất bại.'));
    }
  } catch (error: any) {
    console.error('Error saving template:', error);
    message.error(getErrorMessage(error, 'Có lỗi xảy ra khi lưu mẫu email.'));
  } finally {
    isSaving.value = false;
  }
};

const handleCancel = () => {
  router.push({ name: 'email-templates' });
};
</script>

<template>
  <div class="email-template-edit-container d-flex flex-column">
    <!-- Top Action Bar -->
    <div class="top-action-bar d-flex justify-content-between align-items-center px-4 py-3 bg-white border-bottom shadow-sm flex-shrink-0">
      <div class="d-flex align-items-center gap-3">
        <a-button type="default" @click="handleCancel" class="d-flex align-items-center justify-content-center" style="height: 38px; width: 38px; border-radius: 8px; padding: 0;">
          <template #icon><ArrowLeftOutlined /></template>
        </a-button>
        <div class="text-start">
          <h5 class="fw-bold mb-0 text-dark d-flex align-items-center gap-2">
            Chỉnh Sửa Email Template: <span class="badge badge-code font-monospace fs-6 px-2.5 py-1 rounded">{{ templateIdParam }}</span>
          </h5>
          <p class="text-secondary small mb-0">Cấu hình thông tin mã nhận dạng, tiêu đề gửi mail và viết mã HTML cho email.</p>
        </div>
      </div>
      <div class="d-flex align-items-center gap-2">
        <a-button size="large" @click="handleCancel" :disabled="isSaving" class="px-4 rounded-3">Hủy bỏ</a-button>
        <a-button type="primary" size="large" @click="handleSave" :loading="isSaving" class="fw-semibold px-4 rounded-3 d-flex align-items-center gap-2">
          <template #icon><SaveOutlined /></template> LƯU THAY ĐỔI
        </a-button>
      </div>
    </div>

    <!-- Main Content Work Area -->
    <div class="flex-grow-1 overflow-hidden d-flex">
      <!-- Left Side: Code & Info Editor -->
      <div class="col-6 border-end bg-white d-flex flex-column h-100 p-4 overflow-auto">
        <div v-if="loading" class="d-flex flex-column align-items-center justify-content-center py-5 my-auto">
          <a-spin size="large" />
          <span class="text-secondary mt-3">Đang tải chi tiết mẫu email...</span>
        </div>

        <div v-else class="d-flex flex-column flex-grow-1 text-start">
          <!-- Metadata Fields -->
          <div class="form-section mb-4">
            <div class="row g-3">
              <div class="col-12 col-md-6">
                <div class="form-label-custom">Mã định dạng (Code)</div>
                <a-input 
                  v-model:value="editingTemplate.templateCode" 
                  disabled
                  size="large" 
                  class="rounded-3 font-monospace text-muted bg-light"
                />
              </div>
              <div class="col-12 col-md-6">
                <div class="form-label-custom">Mã tài nguyên (Guid)</div>
                <a-input 
                  :value="editingTemplate.emailTemplateId" 
                  disabled 
                  size="large" 
                  class="rounded-3 font-monospace text-muted bg-light"
                />
              </div>
            </div>
          </div>

          <!-- Subject Line -->
          <div class="form-section mb-4">
            <div class="form-label-custom">Tiêu đề gửi Email (Subject)</div>
            <a-input 
              v-model:value="editingTemplate.subject" 
              placeholder="Nhập tiêu đề thư (có thể chèn các biến)..." 
              size="large" 
              class="rounded-3"
            />
          </div>

          <!-- Placeholder Help Toolbar -->
          <div class="form-section mb-3 bg-light p-3 rounded-3 border">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <span class="small fw-bold text-dark d-flex align-items-center gap-1.5">
                <InfoCircleOutlined class="text-primary" /> Các tham số động khả dụng
              </span>
              <span class="text-secondary small font-monospace">Click để chèn nhanh</span>
            </div>
            <div class="d-flex flex-wrap gap-2">
              <button 
                type="button" 
                class="btn btn-xs btn-outline-primary rounded-pill px-2.5 py-1 font-monospace small" 
                @click="insertPlaceholder('DisplayName')"
                v-text="'{{DisplayName}}'"
              ></button>
              <button 
                type="button" 
                class="btn btn-xs btn-outline-primary rounded-pill px-2.5 py-1 font-monospace small" 
                @click="insertPlaceholder('TemporaryPassword')"
                v-text="'{{TemporaryPassword}}'"
              ></button>
              <button 
                type="button" 
                class="btn btn-xs btn-outline-primary rounded-pill px-2.5 py-1 font-monospace small" 
                @click="insertPlaceholder('ExpireTime')"
                v-text="'{{ExpireTime}}'"
              ></button>
            </div>
          </div>

          <!-- Code Editor Area -->
          <div class="form-section flex-grow-1 d-flex flex-column mb-0" style="min-height: 320px;">
            <div class="form-label-custom d-flex justify-content-between align-items-center mb-2">
              <span class="d-flex align-items-center gap-1.5"><CodeOutlined /> Nội dung thiết kế HTML</span>
              <div class="d-flex align-items-center gap-2">
                <span class="text-secondary d-none d-md-inline" style="font-size: 11px;">Hỗ trợ mã HTML & Inline Styles</span>
                <a-button type="link" size="small" class="fw-semibold p-0 text-primary d-flex align-items-center gap-1" @click="editingTemplate.content = formatHtml(editingTemplate.content)">
                  🪄 Tự động căn lề
                </a-button>
              </div>
            </div>
            <textarea 
              id="templateContentArea"
              v-model="editingTemplate.content" 
              placeholder="<html>&#10;<body>&#10;  <h3>Chào {{DisplayName}},</h3>...&#10;</body>&#10;</html>"
              class="form-control code-editor font-monospace flex-grow-1 p-3 rounded-3"
              style="resize: none;"
            ></textarea>
          </div>
        </div>
      </div>

      <!-- Right Side: Sandbox Preview -->
      <div class="col-6 bg-light d-flex flex-column h-100 p-4 overflow-auto justify-content-start">
        <div class="preview-header mb-3 flex-shrink-0 d-flex justify-content-between align-items-center">
          <h5 class="fw-bold text-dark mb-0 d-flex align-items-center gap-2">
            <LayoutOutlined class="text-primary" /> Xem trước hộp thư (Inbox Preview)
          </h5>
          <span class="badge bg-success-light text-success fw-semibold px-2.5 py-1 rounded">Real-time Rendering</span>
        </div>

        <!-- Sandbox Param Customizers -->
        <div class="bg-white p-2 rounded-3 border shadow-sm flex-shrink-0 text-start mb-4">
          <div class="d-flex flex-column gap-2">
            <div class="d-flex align-items-center gap-3">
              <label class="text-secondary mb-0 fw-semibold" style="font-size: 12px; min-width: 140px;"><span v-pre class="font-monospace text-primary bg-light px-1.5 py-0.5 rounded">{{DisplayName}}</span></label>
              <a-input v-model:value="previewParams.DisplayName" size="middle" placeholder="Ví dụ: Hoàng Cao Nguyên..." class="rounded-3 flex-grow-1" />
            </div>
            <div class="d-flex align-items-center gap-3">
              <label class="text-secondary mb-0 fw-semibold" style="font-size: 12px; min-width: 140px;"><span v-pre class="font-monospace bg-light px-1.5 py-0.5 rounded" style="color: #6f42c1;">{{TemporaryPassword}}</span></label>
              <a-input v-model:value="previewParams.TemporaryPassword" size="middle" placeholder="Ví dụ: aBcd1234..." class="rounded-3 flex-grow-1" />
            </div>
            <div class="d-flex align-items-center gap-3">
              <label class="text-secondary mb-0 fw-semibold" style="font-size: 12px; min-width: 140px;"><span v-pre class="font-monospace bg-light px-1.5 py-0.5 rounded" style="color: #f97316;">{{ExpireTime}}</span></label>
              <a-input v-model:value="previewParams.ExpireTime" size="middle" placeholder="Ví dụ: 15 phút..." class="rounded-3 flex-grow-1" />
            </div>
          </div>
        </div>

        <!-- Premium simulated email client mockup -->
        <div class="inbox-wrapper bg-white shadow rounded-3 border flex-grow-1 overflow-hidden d-flex flex-column mb-3">
          <!-- simulated email header -->
          <div class="inbox-topbar px-4 py-3 bg-light border-bottom d-flex flex-column gap-1 flex-shrink-0 text-start">
            <div class="d-flex align-items-center">
              <span class="text-secondary small" style="width: 70px;">Từ:</span>
              <span class="text-dark small fw-semibold">Hệ thống HoangCN &lt;noreply@hoangcn.dev&gt;</span>
            </div>
            <div class="d-flex align-items-center">
              <span class="text-secondary small" style="width: 70px;">Chủ đề:</span>
              <span class="text-dark small fw-bold text-truncate">{{ editingTemplate.subject || '(Chưa có tiêu đề)' }}</span>
            </div>
          </div>

          <!-- Simulated Paper Sheet Canvas for beautifully formatted HTML preview -->
          <div class="inbox-body flex-grow-1 overflow-auto p-4 d-flex justify-content-center align-items-start bg-light-gray">
            <div class="paper-canvas shadow bg-white rounded-3 border p-4 p-md-5 my-2">
              <!-- Render sandbox content safely -->
              <div v-html="livePreviewHtml"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&family=Fira+Code:wght@400;500&display=swap');

.email-template-edit-container {
  font-family: 'Outfit', sans-serif;
  background-color: #f8fafc;
  height: calc(100vh - 64px); /* Header is 64px, so this fits the remaining height perfectly */
  overflow: hidden;
}

.top-action-bar {
  height: 70px;
}

.badge-code {
  background-color: rgba(13, 110, 253, 0.08);
  color: #0d6efd;
  border: 1px solid rgba(13, 110, 253, 0.15);
  font-family: 'Fira Code', monospace;
}

.form-label-custom {
  font-weight: 600;
  font-size: 0.9rem;
  color: #334155;
  margin-bottom: 6px;
  text-align: left;
}

/* Code editor styling */
.code-editor {
  font-family: 'Fira Code', monospace !important;
  font-size: 0.9rem;
  line-height: 1.55;
  background-color: #0f172a;
  color: #e2e8f0;
  border: 1px solid #1e293b;
  tab-size: 4;
}

.code-editor:focus {
  background-color: #0f172a;
  color: #ffffff;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
}

/* Simulated Email Frame Styling */
.inbox-wrapper {
  background-color: #ffffff;
  border-radius: 12px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
  min-height: 400px;
}

.bg-light-gray {
  background-color: #f1f5f9;
}

/* Simulated Paper Canvas for modern formatted view */
.paper-canvas {
  width: 100%;
  max-width: 600px;
  min-height: 350px;
  border-color: rgba(226, 232, 240, 0.8) !important;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  text-align: left;
  word-wrap: break-word;
  overflow-wrap: break-word;
}

:deep(.preview-hl) {
  font-weight: 600;
  color: #0d6efd;
  background-color: rgba(13, 110, 253, 0.08);
  padding: 2px 6px;
  border-radius: 4px;
  border: 1px dashed rgba(13, 110, 253, 0.3);
}

:deep(.preview-hl-pass) {
  font-family: 'Fira Code', monospace;
  font-weight: 700;
  color: #6f42c1;
  background-color: rgba(111, 66, 193, 0.08);
  padding: 3px 8px;
  border-radius: 4px;
  border: 1px dashed rgba(111, 66, 193, 0.3);
}

:deep(.preview-hl-time) {
  font-weight: 600;
  color: #f97316;
  background-color: rgba(249, 115, 22, 0.08);
  padding: 2px 6px;
  border-radius: 4px;
  border: 1px dashed rgba(249, 115, 22, 0.3);
}

.bg-success-light {
  background-color: rgba(34, 197, 94, 0.08);
}

.btn-xs {
  font-size: 0.75rem;
  padding: 0.25rem 0.5rem;
  background-color: #ffffff;
  transition: all 0.2s ease;
}

.btn-xs:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}
</style>
