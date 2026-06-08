<template>
  <div class="profile-view small-font">
    <div class="card border-0 rounded-4 shadow-sm bg-white overflow-hidden mb-4">
      <!-- Profile Banner -->
      <div class="profile-banner"></div>
      
      <!-- Profile Header Info -->
      <div class="p-4 pt-0 position-relative">
        <div class="profile-header-details d-flex flex-column flex-md-row align-items-center align-items-md-end gap-3">
          <div class="large-avatar shadow-lg">HV</div>
          <div class="flex-grow-1 text-center text-md-start mt-2">
            <h2 class="fs-5 fw-bold text-dark-blue mb-1">{{ userInfo.name }}</h2>
            <p class="text-secondary small mb-0">Tham gia từ: 15/05/2026 • Trạng thái: Hoạt động</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Details Form -->
    <div class="row g-4">
      <div class="col-12">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4">
          <a-tabs v-model:activeKey="activeTab" class="custom-tabs">
            <a-tab-pane key="info" tab="Thông tin cá nhân">
              <a-form layout="vertical" class="mt-3">
                <div class="row">
                  <div class="col-md-6">
                    <a-form-item label="Họ và tên:" required>
                      <a-input v-model:value="userInfo.name" placeholder="Nhập họ tên..." />
                    </a-form-item>
                  </div>
                  <div class="col-md-6">
                    <a-form-item label="Email liên hệ:" required>
                      <a-input v-model:value="userInfo.email" placeholder="Nhập email..." disabled />
                    </a-form-item>
                  </div>
                </div>
                <div class="row">
                  <div class="col-md-6">
                    <a-form-item label="Số điện thoại:">
                      <a-input v-model:value="userInfo.phone" placeholder="Nhập số điện thoại..." />
                    </a-form-item>
                  </div>
                  <div class="col-md-6">
                    <a-form-item label="Vai trò người dùng (Thay đổi để thử nghiệm phân quyền):">
                      <a-select v-model:value="userInfo.role" placeholder="Chọn vai trò">
                        <a-select-option value="student">Học viên / Giáo viên</a-select-option>
                        <a-select-option value="admin">Quản trị viên hệ thống (Admin)</a-select-option>
                      </a-select>
                    </a-form-item>
                  </div>
                </div>
                <a-form-item label="Giới thiệu bản thân:">
                  <a-textarea v-model:value="userInfo.bio" placeholder="Mô tả ngắn gọn về bạn..." :rows="3" />
                </a-form-item>
                
                <div class="text-end border-top pt-3 mt-3">
                  <button type="button" class="btn btn-indigo text-white fw-semibold px-4 hover-up" @click="saveProfile" :disabled="loading">
                    <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Lưu thay đổi
                  </button>
                </div>
              </a-form>
            </a-tab-pane>

            <a-tab-pane key="password" tab="Đổi mật khẩu">
              <a-form layout="vertical" class="mt-3">
                <a-form-item label="Mật khẩu hiện tại:" required>
                  <a-input-password v-model:value="passwordState.current" placeholder="Nhập mật khẩu hiện tại..." />
                </a-form-item>
                <a-form-item label="Mật khẩu mới:" required>
                  <a-input-password v-model:value="passwordState.new" placeholder="Nhập mật khẩu mới..." />
                </a-form-item>
                <a-form-item label="Xác nhận mật khẩu mới:" required>
                  <a-input-password v-model:value="passwordState.confirm" placeholder="Xác nhận mật khẩu mới..." />
                </a-form-item>
                
                <div class="text-end border-top pt-3 mt-3">
                  <button type="button" class="btn btn-indigo text-white fw-semibold px-4 hover-up" @click="changePassword" :disabled="loadingPassword">
                    <span v-if="loadingPassword" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                    Đổi mật khẩu
                  </button>
                </div>
              </a-form>
            </a-tab-pane>
          </a-tabs>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { message } from 'ant-design-vue'

const activeTab = ref('info')
const loading = ref(false)
const loadingPassword = ref(false)

const userInfo = reactive({
  name: 'Học Viên Cao Cấp',
  email: 'student@hoangcn.dev',
  phone: '0987654321',
  role: 'student',
  bio: 'Đam mê học hỏi công nghệ và luyện thi trắc nghiệm trực tuyến.'
})

const passwordState = reactive({
  current: '',
  new: '',
  confirm: ''
})

onMounted(() => {
  const saved = localStorage.getItem('cn_user_profile')
  if (saved) {
    try {
      const data = JSON.parse(saved)
      Object.assign(userInfo, data)
    } catch (e) {
      // Ignore
    }
  }
})

const saveProfile = () => {
  if (!userInfo.name.trim()) {
    message.error('Họ và tên không được để trống!')
    return
  }

  loading.value = true
  setTimeout(() => {
    loading.value = false
    localStorage.setItem('cn_user_profile', JSON.stringify(userInfo))
    message.success('Cập nhật thông tin tài khoản thành công!')
  }, 1000)
}

const changePassword = () => {
  if (!passwordState.current || !passwordState.new || !passwordState.confirm) {
    message.error('Vui lòng điền đầy đủ thông tin đổi mật khẩu!')
    return
  }

  if (passwordState.new !== passwordState.confirm) {
    message.error('Mật khẩu mới và xác nhận mật khẩu không khớp!')
    return
  }

  loadingPassword.value = true
  setTimeout(() => {
    loadingPassword.value = false
    passwordState.current = ''
    passwordState.new = ''
    passwordState.confirm = ''
    message.success('Đổi mật khẩu tài khoản thành công!')
  }, 1000)
}
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

.profile-banner {
  height: 140px;
  background: linear-gradient(135deg, #6366f1 0%, #a855f7 100%);
}

.large-avatar {
  width: 90px;
  height: 90px;
  border-radius: 50%;
  background-color: #4f46e5;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.2rem;
  font-weight: 700;
  border: 4px solid white;
  margin-top: -45px;
  z-index: 2;
}

.profile-header-details {
  margin-top: 10px;
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  color: white;
  border: none;
  transition: all 0.2s ease;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.25);
}

.hover-up {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-1px);
}

.custom-tabs :deep(.ant-tabs-nav) {
  margin-bottom: 16px;
}

.custom-tabs :deep(.ant-tabs-tab-active .ant-tabs-tab-btn) {
  color: #6366f1;
  font-weight: 600;
}

.custom-tabs :deep(.ant-tabs-ink-bar) {
  background-color: #6366f1;
}

.small-font {
  font-size: 0.85rem !important;
}
.small-font :deep(.ant-form-item-label > label) {
  font-size: 0.85rem !important;
}
.small-font :deep(.ant-input),
.small-font :deep(.ant-input-password input) {
  font-size: 0.85rem !important;
}
.small-font :deep(.ant-tabs-tab-btn) {
  font-size: 0.85rem !important;
}
.small-font :deep(.ant-form-item) {
  margin-bottom: 12px;
}
</style>
