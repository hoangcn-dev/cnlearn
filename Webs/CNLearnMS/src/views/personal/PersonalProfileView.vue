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
                        <a-select-option value="User">Học viên / Giáo viên</a-select-option>
                        <a-select-option value="Admin">Quản trị viên hệ thống (Admin)</a-select-option>
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
              <div class="p-4 text-center my-4">
                <div class="mb-3">
                  <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#6366f1" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"></rect><path d="M7 11V7a5 5 0 0 1 10 0v4"></path></svg>
                </div>
                <h5 class="fw-bold text-dark mb-2">Đổi mật khẩu bảo mật</h5>
                <p class="text-secondary small mb-4">Để đảm bảo an toàn tuyệt đối cho tài khoản của bạn, chức năng thay đổi mật khẩu được thực hiện tập trung thông qua cổng xác thực bảo mật id.hoangcn.com.</p>
                <button type="button" class="btn btn-indigo text-white fw-semibold px-4 py-2" @click="redirectToChangePassword">
                  Đi tới Cổng đổi mật khẩu
                </button>
              </div>
            </a-tab-pane>
          </a-tabs>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue'
import { message } from 'ant-design-vue'
import { getLearnMsUserProfile, updateLearnMsUserProfile } from '@/api/user'

const activeTab = ref('info')
const loading = ref(false)

const userInfo = reactive({
  userId: '',
  name: 'Học Viên Cao Cấp',
  email: 'student@hoangcn.dev',
  phone: '',
  role: 'User',
  bio: ''
})

onMounted(async () => {
  // 1. Phục hồi nhanh từ LocalStorage trước
  const saved = localStorage.getItem('cn_user_profile')
  if (saved) {
    try {
      const data = JSON.parse(saved)
      userInfo.userId = data.userId || ''
      userInfo.name = data.name || ''
      userInfo.email = data.email || ''
      userInfo.role = data.role || 'User'
    } catch (e) {}
  }

  // 2. Tải thông tin thực tế từ cơ sở dữ liệu riêng của LearnMS
  try {
    const res = await getLearnMsUserProfile()
    if (res && res.isSuccess && res.data) {
      const dbUser = res.data
      userInfo.userId = dbUser.learnMsUserId
      userInfo.name = dbUser.fullName
      userInfo.email = dbUser.email
      userInfo.phone = dbUser.phoneNumber || ''
      userInfo.role = dbUser.role || 'User'
      userInfo.bio = dbUser.biography || ''
    }
  } catch (e) {
    console.error('Lỗi khi tải thông tin hồ sơ từ LearnMS:', e)
  }
})

const saveProfile = async () => {
  if (!userInfo.name.trim()) {
    message.error('Họ và tên không được để trống!')
    return
  }

  loading.value = true
  try {
    const res = await updateLearnMsUserProfile({
      learnMsUserId: userInfo.userId,
      fullName: userInfo.name,
      email: userInfo.email,
      phoneNumber: userInfo.phone,
      role: userInfo.role,
      biography: userInfo.bio
    })

    if (res && res.isSuccess) {
      // Cập nhật lại cache LocalStorage để đồng bộ header/avatar ngay lập tức
      const saved = localStorage.getItem('cn_user_profile')
      let localProfile = saved ? JSON.parse(saved) : {}
      localProfile.name = userInfo.name
      localProfile.role = userInfo.role
      localProfile.isAdmin = userInfo.role === 'Admin'
      localStorage.setItem('cn_user_profile', JSON.stringify(localProfile))

      message.success('Cập nhật thông tin tài khoản thành công!')
    } else {
      message.error(res.errorMessage || 'Lỗi cập nhật thông tin!')
    }
  } catch (e) {
    message.error('Đã xảy ra lỗi kết nối hệ thống!')
  } finally {
    loading.value = false
  }
}

const redirectToChangePassword = () => {
  const idServerUrl = import.meta.env.VITE_ID_URL || (window.location.hostname === 'localhost' ? 'http://localhost:5173' : 'https://id.hoangcn.com');
  const returnUrl = encodeURIComponent(window.location.href);
  window.location.href = `${idServerUrl}/auth?mode=changepass&return_url=${returnUrl}`;
}

watch(activeTab, (newVal) => {
  if (newVal === 'password') {
    redirectToChangePassword();
  }
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
