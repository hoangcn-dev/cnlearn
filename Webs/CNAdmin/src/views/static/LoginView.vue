<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { message, notification } from 'ant-design-vue';
import { login, register, forgotPassword, changePassword } from '@/api/user';
import { useAuthStore } from '@/stores/auth';
import { getErrorMessage } from '@/api/config/axios';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const loading = ref(false);

// Dynamic titles from URL query parameters
const formTitle = computed(() => (route.query.title as string) || 'CNAdmin');
const formDesc = computed(() => (route.query.desc as string) || 'Hệ thống quản trị tài khoản thành viên');

// Modes: 'login', 'register', 'changepass'
const currentMode = ref('login');

// Smoothly transition between modes and sync the query parameter
const setMode = (mode: string) => {
  currentMode.value = mode;
  router.replace({ query: { ...route.query, mode } });
};

// Handle Forgot Password clicked
const handleForgotPassword = () => {
  setMode('forgotpass');
};

// Login form state
const formState = reactive({
  username: '',
  password: '',
  remember: true,
});

// Register form state
const registerState = reactive({
  displayName: '',
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
});

// Change Password form state
const changePasswordState = reactive({
  username: '',
  oldPassword: '',
  newPassword: '',
  confirmNewPassword: '',
});

// Forgot Password form state
const forgotPasswordState = reactive({
  email: '',
});

// Sync currentMode with URL query parameter
watch(
  () => route.query.mode,
  (newMode) => {
    if (newMode === 'register' || newMode === 'regis') {
      currentMode.value = 'register';
    } else if (newMode === 'changepass' || newMode === 'forgotpass') {
      currentMode.value = newMode;
      if (newMode === 'changepass' && authStore.user) {
        changePasswordState.username = authStore.user.userName || authStore.user.email || '';
      }
    } else {
      currentMode.value = 'login';
    }
  },
  { immediate: true }
);

// Sync all form fields with URL query parameters reactively
watch(
  () => route.query,
  (query) => {
    // 1. Login Form
    if (query.username !== undefined) {
      formState.username = (query.username as string) || '';
    } else if (query.email !== undefined) {
      formState.username = (query.email as string) || '';
    }
    if (query.password !== undefined) {
      formState.password = (query.password as string) || '';
    }

    // 2. Register Form
    if (query.displayName !== undefined) {
      registerState.displayName = (query.displayName as string) || '';
    } else if (query.name !== undefined) {
      registerState.displayName = (query.name as string) || '';
    } else if (query.fullname !== undefined) {
      registerState.displayName = (query.fullname as string) || '';
    }
    if (query.username !== undefined) {
      registerState.username = (query.username as string) || '';
    }
    if (query.email !== undefined) {
      registerState.email = (query.email as string) || '';
    }
    if (query.password !== undefined) {
      registerState.password = (query.password as string) || '';
      registerState.confirmPassword = (query.confirmPassword as string) || (query.password as string) || '';
    }

    // 3. Change Password Form
    if (query.username !== undefined) {
      changePasswordState.username = (query.username as string) || '';
    } else if (query.email !== undefined) {
      changePasswordState.username = (query.email as string) || '';
    }
    if (query.oldPassword !== undefined) {
      changePasswordState.oldPassword = (query.oldPassword as string) || '';
    } else if (query.password !== undefined) {
      changePasswordState.oldPassword = (query.password as string) || '';
    }
    if (query.newPassword !== undefined) {
      changePasswordState.newPassword = (query.newPassword as string) || '';
      changePasswordState.confirmNewPassword = (query.confirmNewPassword as string) || (query.newPassword as string) || '';
    }
  },
  { immediate: true, deep: true }
);

// Remove Vietnamese accents and special characters to auto-generate username
const removeAccents = (str: string) => {
  return str
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '') // remove accents
    .replace(/đ/g, 'd')
    .replace(/Đ/g, 'd')
    .replace(/[^a-zA-Z0-9\s]/g, '') // remove special characters
    .toLowerCase()
    .replace(/\s+/g, ''); // remove spaces
};

// Handle Auto Generate Username on demand
const handleAutoGenerateUsername = () => {
  const baseUsername = removeAccents(registerState.displayName);
  if (!baseUsername) {
    message.warning('Vui lòng nhập Họ và tên trước!');
    return;
  }
  const randomSuffix = Math.floor(100 + Math.random() * 900);
  if (!registerState.username) {
    registerState.username = baseUsername;
  } else {
    registerState.username = `${baseUsername}${randomSuffix}`;
  }
  message.success('Đã tự sinh tên đăng nhập thành công!');
};

// Handle Login submit
const handleFinish = async (values: any) => {
  loading.value = true;
  try {
    const trimmedUsername = values.username.trim();
    const res = await login({
      emailOrUserName: trimmedUsername,
      password: values.password
    });

    if (res.isSuccess) {
      message.success(`Đăng nhập thành công! Đang xác thực thông tin...`);
      const targetUrl = (route.query.return_url as string) || '/users';
      
      if (targetUrl.startsWith('http://') || targetUrl.startsWith('https://')) {
        window.location.href = targetUrl;
      } else {
        router.push({
          path: '/auth-callback',
          query: { return_url: targetUrl }
        });
      }
    } else {
      message.error(getErrorMessage(res, 'Tên đăng nhập hoặc mật khẩu không chính xác!'));
    }
  } catch (error: any) {
    console.error(error);
    message.error(getErrorMessage(error, 'Đăng nhập thất bại, vui lòng kiểm tra lại!'));
  } finally {
    loading.value = false;
  }
};

// Handle Register submit
const handleRegister = async () => {
  if (registerState.password !== registerState.confirmPassword) {
    message.error('Mật khẩu xác nhận không khớp!');
    return;
  }

  loading.value = true;
  try {
    const trimmedUsername = registerState.username.trim();
    const res = await register({
      email: registerState.email.trim(),
      password: registerState.password,
      displayName: registerState.displayName.trim(),
      userName: trimmedUsername
    });

    if (res.isSuccess) {
      message.success('Đăng ký tài khoản thành công! Bạn đã có thể đăng nhập.');

      // Reset register state
      registerState.displayName = '';
      registerState.username = '';
      registerState.email = '';
      registerState.password = '';
      registerState.confirmPassword = '';

      // Switch to login tab and pre-fill username
      setMode('login');
      formState.username = trimmedUsername;
    } else {
      message.error(getErrorMessage(res, 'Đăng ký thất bại, vui lòng thử lại!'));
    }
  } catch (error: any) {
    console.error(error);
    message.error(getErrorMessage(error, 'Tài khoản hoặc Email đã tồn tại trên hệ thống!'));
  } finally {
    loading.value = false;
  }
};

// Handle Change Password submit
const handleChangePassword = async () => {
  if (!changePasswordState.oldPassword) {
    message.error('Vui lòng nhập mật khẩu hiện tại!');
    return;
  }
  if (!changePasswordState.newPassword) {
    message.error('Vui lòng nhập mật khẩu mới!');
    return;
  }
  if (changePasswordState.newPassword.length < 6) {
    message.error('Mật khẩu mới phải có tối thiểu 6 ký tự!');
    return;
  }
  if (changePasswordState.newPassword !== changePasswordState.confirmNewPassword) {
    message.error('Xác nhận mật khẩu mới không khớp!');
    return;
  }

  loading.value = true;
  try {
    const res = await changePassword({
      currentPassword: changePasswordState.oldPassword,
      newPassword: changePasswordState.newPassword
    });

    if (res.isSuccess) {
      message.success('Đổi mật khẩu thành công! Hãy đăng nhập lại bằng mật khẩu mới.');

      // Clear the current active login session
      authStore.clearLoginSession();

      // Reset Change Password State
      changePasswordState.username = '';
      changePasswordState.oldPassword = '';
      changePasswordState.newPassword = '';
      changePasswordState.confirmNewPassword = '';

      // Switch to login mode
      setMode('login');
    } else {
      message.error(getErrorMessage(res, 'Đổi mật khẩu thất bại!'));
    }
  } catch (error: any) {
    console.error(error);
    message.error(getErrorMessage(error, 'Mật khẩu hiện tại không chính xác!'));
  } finally {
    loading.value = false;
  }
};

// Handle Forgot Password submit
const handleForgotPasswordSubmit = async () => {
  if (!forgotPasswordState.email.trim()) {
    message.error('Vui lòng nhập địa chỉ Email!');
    return;
  }

  loading.value = true;
  try {
    const res = await forgotPassword(forgotPasswordState.email.trim());
    if (res.isSuccess) {
      notification.success({
        message: 'Gửi mật khẩu tạm thời thành công',
        description: getErrorMessage(res, 'Mật khẩu tạm thời đã được gửi tới email của bạn. Vui lòng kiểm tra hộp thư.'),
        duration: 8,
        placement: 'topRight',
      });

      // Switch back to login mode and pre-fill email
      setMode('login');
      formState.username = forgotPasswordState.email.trim();
      forgotPasswordState.email = '';
    } else {
      message.error(getErrorMessage(res, 'Gửi mật khẩu tạm thời thất bại, vui lòng thử lại!'));
    }
  } catch (error: any) {
    console.error(error);
    message.error(getErrorMessage(error, 'Không tìm thấy tài khoản với Email này trên hệ thống!'));
  } finally {
    loading.value = false;
  }
};

// Handle Mock Google login
const handleGoogleLogin = () => {
  loading.value = true;
  setTimeout(() => {
    loading.value = false;
    localStorage.setItem('isLoggedIn', 'true');
    localStorage.setItem('username', 'google_user');

    // Ensure there's a google user in the users table so it doesn't break other parts
    const stored = localStorage.getItem('users');
    let userList = [];
    if (stored) {
      userList = JSON.parse(stored);
    }
    if (!userList.some((u: any) => u.userName === 'google_user')) {
      let maxId = 0;
      userList.forEach((u: any) => {
        const idNum = parseInt(u.userId);
        if (!isNaN(idNum) && idNum > maxId) {
          maxId = idNum;
        }
      });
      const newId = String(maxId + 1);
      userList.push({
        userId: newId,
        userName: 'google_user',
        userCode: `NV-${String(newId).padStart(3, '0')}`,
        displayName: 'Google User',
        email: 'google_user@gmail.com',
        password: 'google_password',
        isActive: true,
        avatarImageFileId: 'https://api.dicebear.com/7.x/adventurer/svg?seed=google',
        roleId: 'ROLE004'
      });
      localStorage.setItem('users', JSON.stringify(userList));
    }

    message.success('Đăng nhập bằng Google thành công!');
    router.push('/users');
  }, 1000);
};
</script>

<template>
  <div class="login-wrapper d-flex justify-content-center align-items-start min-vh-100">
    <div class="login-container" :style="{ maxWidth: currentMode === 'register' ? '680px' : '420px' }">
      <div class="login-card shadow-sm p-4 bg-white rounded-3 border">
        <!-- Logo / Title dynamic from URL Query -->
        <div class="text-center mb-4">
          <h2 class="fw-bold text-dark mb-1">{{ formTitle }}</h2>
          <p class="text-muted small mb-0">{{ formDesc }}</p>
        </div>

        <!-- Mode Tabs (Hidden when in Change Password and Forgot Password modes) -->
        <div v-if="currentMode !== 'changepass' && currentMode !== 'forgotpass'" class="d-flex mb-4 justify-content-center pb-2">
          <button type="button" class="btn btn-link text-decoration-none fw-semibold px-3 py-1 me-2"
            :class="currentMode === 'login' ? 'text-primary active-tab' : 'text-secondary'" @click="setMode('login')">
            Đăng nhập
          </button>
          <button type="button" class="btn btn-link text-decoration-none fw-semibold px-3 py-1"
            :class="currentMode === 'register' ? 'text-primary active-tab' : 'text-secondary'"
            @click="setMode('register')">
            Đăng ký
          </button>
        </div>

        <!-- Login Form -->
        <a-form v-if="currentMode === 'login'" :model="formState" layout="vertical" @finish="handleFinish">
          <a-form-item label="Tên đăng nhập hoặc Email" name="username"
            :rules="[{ required: true, message: 'Vui lòng nhập tên đăng nhập!' }]">
            <a-input v-model:value="formState.username" placeholder="Nhập tài khoản hoặc email..." size="large" />
          </a-form-item>

          <a-form-item label="Mật khẩu" name="password"
            :rules="[{ required: true, message: 'Vui lòng nhập mật khẩu!' }]">
            <a-input-password v-model:value="formState.password" placeholder="Nhập mật khẩu..." size="large" />
          </a-form-item>

          <div class="d-flex justify-content-between align-items-center mb-3">
            <a-checkbox v-model:checked="formState.remember" class="small">
              Ghi nhớ đăng nhập
            </a-checkbox>
            <a href="#" class="small text-primary text-decoration-none" @click.prevent="handleForgotPassword">Quên mật
              khẩu?</a>
          </div>

          <a-button type="primary" html-type="submit" size="large" block :loading="loading" class="fw-semibold mb-3">
            ĐĂNG NHẬP HỆ THỐNG
          </a-button>
        </a-form>

        <!-- Register Form -->
        <a-form v-else-if="currentMode === 'register'" :model="registerState" layout="vertical"
          @finish="handleRegister">
          <div class="row g-3 register-row">
            <!-- Cột 1: Email & Mật khẩu -->
            <div class="col-12 col-md-6 pe-md-3">
              <a-form-item label="Địa chỉ Email" name="email" :rules="[
                { required: true, message: 'Vui lòng nhập email!' },
                { type: 'email', message: 'Email không đúng định dạng!' }
              ]">
                <a-input v-model:value="registerState.email" placeholder="example@domain.com" size="large" />
              </a-form-item>

              <a-form-item label="Mật khẩu" name="password"
                :rules="[{ required: true, message: 'Vui lòng nhập mật khẩu!' }]">
                <a-input-password v-model:value="registerState.password" placeholder="Nhập mật khẩu..." size="large" />
              </a-form-item>

              <a-form-item label="Xác nhận mật khẩu" name="confirmPassword"
                :rules="[{ required: true, message: 'Vui lòng xác nhận mật khẩu!' }]">
                <a-input-password v-model:value="registerState.confirmPassword" placeholder="Nhập lại mật khẩu..."
                  size="large" />
              </a-form-item>
            </div>

            <!-- Cột 2: Họ tên và tên tài khoản tự sinh -->
            <div class="col-12 col-md-6 ps-md-3">
              <a-form-item label="Họ và tên" name="displayName"
                :rules="[{ required: true, message: 'Vui lòng nhập họ tên của bạn!' }]">
                <a-input v-model:value="registerState.displayName" placeholder="Ví dụ: Nguyễn Văn A..." size="large" />
              </a-form-item>

              <a-form-item label="Tên đăng nhập (Tự sinh)" name="username"
                :rules="[{ required: true, message: 'Vui lòng nhập tài khoản!' }]">
                <a-input v-model:value="registerState.username" readonly placeholder="Click Auto Gen để tự sinh..."
                  size="large">
                  <template #suffix>
                    <a-button type="link" size="small" @click="handleAutoGenerateUsername"
                      style="padding: 0; height: auto; font-size: 0.85rem; font-weight: 600;">
                      Auto
                    </a-button>
                  </template>
                </a-input>
              </a-form-item>
            </div>
          </div>

          <!-- Button Register aligned at the very bottom spanning full width -->
          <div class="pt-3">
            <a-button type="primary" html-type="submit" size="large" block :loading="loading" class="fw-semibold">
              ĐĂNG KÝ TÀI KHOẢN
            </a-button>
          </div>
        </a-form>

        <!-- Change Password Form (Single Column Vertical, Hidden from tabs) -->
        <a-form v-else-if="currentMode === 'changepass'" :model="changePasswordState" layout="vertical"
          @finish="handleChangePassword">
          <a-form-item label="Tên đăng nhập hoặc Email" name="username"
            :rules="[{ required: true, message: 'Vui lòng nhập tên đăng nhập!' }]">
            <a-input v-model:value="changePasswordState.username" :disabled="!!authStore.user" placeholder="Nhập tài khoản hoặc email..."
              size="large" />
          </a-form-item>

          <a-form-item label="Mật khẩu hiện tại" name="oldPassword"
            :rules="[{ required: true, message: 'Vui lòng nhập mật khẩu hiện tại!' }]">
            <a-input-password v-model:value="changePasswordState.oldPassword" placeholder="Nhập mật khẩu hiện tại..."
              size="large" />
          </a-form-item>

          <a-form-item label="Mật khẩu mới" name="newPassword"
            :rules="[{ required: true, message: 'Vui lòng nhập mật khẩu mới!' }]">
            <a-input-password v-model:value="changePasswordState.newPassword" placeholder="Nhập mật khẩu mới..."
              size="large" />
          </a-form-item>

          <a-form-item label="Xác nhận mật khẩu mới" name="confirmNewPassword"
            :rules="[{ required: true, message: 'Vui lòng xác nhận mật khẩu mới!' }]">
            <a-input-password v-model:value="changePasswordState.confirmNewPassword"
              placeholder="Nhập lại mật khẩu mới..." size="large" />
          </a-form-item>

          <!-- Action Button and Back Link -->
          <a-button type="primary" html-type="submit" size="large" block :loading="loading" class="fw-semibold mb-3">
            ĐỔI MẬT KHẨU
          </a-button>

          <div class="text-center">
            <a href="#" class="small text-primary text-decoration-none" @click.prevent="setMode('login')">
              Quay lại đăng nhập
            </a>
          </div>
        </a-form>

        <!-- Forgot Password Form (Single Column Vertical, Hidden from tabs) -->
        <a-form v-else-if="currentMode === 'forgotpass'" :model="forgotPasswordState" layout="vertical"
          @finish="handleForgotPasswordSubmit">
          <div class="text-center mb-3">
            <h4 class="fw-bold text-dark">Khôi phục mật khẩu</h4>
            <p class="text-muted small">Vui lòng nhập địa chỉ Email đăng ký của bạn. Hệ thống sẽ sinh mật khẩu tạm thời mới gửi tới email này.</p>
          </div>

          <a-form-item label="Địa chỉ Email" name="email" :rules="[
            { required: true, message: 'Vui lòng nhập địa chỉ Email!' },
            { type: 'email', message: 'Địa chỉ email không đúng định dạng!' }
          ]">
            <a-input v-model:value="forgotPasswordState.email" placeholder="example@domain.com" size="large" />
          </a-form-item>

          <!-- Action Button and Back Link -->
          <a-button type="primary" html-type="submit" size="large" block :loading="loading" class="fw-semibold mb-3">
            GỬI MẬT KHẨU TẠM THỜI
          </a-button>

          <div class="text-center">
            <a href="#" class="small text-primary text-decoration-none" @click.prevent="setMode('login')">
              Quay lại đăng nhập
            </a>
          </div>
        </a-form>

        <!-- Divider & Google (ONLY shown in Login Mode) -->
        <div v-if="currentMode === 'login'">
          <div class="position-relative text-center my-4">
            <hr class="text-muted opacity-25" />
            <span class="position-absolute top-50 start-50 translate-middle bg-white px-3 text-muted small">Hoặc tiếp
              tục với</span>
          </div>

          <!-- Google Login Button -->
          <a-button block size="large"
            class="d-flex align-items-center justify-content-center border fw-semibold bg-light hover-shadow"
            @click="handleGoogleLogin">
            <svg class="me-2" width="18" height="18" viewBox="0 0 24 24">
              <path fill="#EA4335"
                d="M12 5.04c1.63 0 3.1.56 4.25 1.66l3.18-3.18C17.5 1.7 14.95 1 12 1 7.35 1 3.39 3.67 1.4 7.56l3.8 2.95C6.1 7.39 8.81 5.04 12 5.04z" />
              <path fill="#4285F4"
                d="M23.49 12.27c0-.81-.07-1.59-.2-2.35H12v4.45h6.44c-.28 1.47-1.11 2.72-2.36 3.56l3.66 2.84c2.14-1.97 3.75-4.87 3.75-8.5z" />
              <path fill="#FBBC05"
                d="M5.2 14.51c-.24-.72-.38-1.49-.38-2.29s.14-1.57.38-2.29l-3.8-2.95C.53 8.78 0 10.33 0 12s.53 3.22 1.4 4.77l3.8-2.76z" />
              <path fill="#34A853"
                d="M12 23c3.24 0 5.97-1.07 7.96-2.92l-3.66-2.84c-1.01.68-2.31 1.08-4.3 1.08-3.19 0-5.9-2.35-6.8-5.47l-3.8 2.95C3.39 20.33 7.35 23 12 23z" />
            </svg>
            Đăng nhập với Google
          </a-button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&display=swap');

.login-wrapper {
  font-family: 'Outfit', sans-serif;
  background-color: #f8fafc;
  /* Very soft light gray-slate background */
  min-height: 100vh;
  width: 100%;
  padding-top: 10px;
}

.login-container {
  width: 100%;
  padding: 15px;
  transition: max-width 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}

.login-card {
  border-color: #e2e8f0 !important;
  /* Soft gray border */
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03) !important;
  border-radius: 12px !important;
}

.btn-link {
  font-size: 0.95rem;
  border-bottom: 2px solid transparent;
  border-radius: 0;
}

.btn-link:hover {
  color: #0d6efd;
}

.btn-link:focus {
  box-shadow: none;
}

.active-tab {
  border-bottom: 2px solid #0d6efd !important;
  color: #0d6efd !important;
}

.hover-shadow {
  transition: all 0.2s ease;
  border-radius: 6px !important;
}

.hover-shadow:hover {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08) !important;
  background-color: #f1f5f9 !important;
  border-color: #cbd5e1 !important;
}

/* Elegant Antd Form Adjustments */
:deep(.ant-form-item) {
  margin-bottom: 14px;
}

:deep(.ant-form-item:last-child) {
  margin-bottom: 0;
}

:deep(.ant-input),
:deep(.ant-input-password) {
  border-radius: 6px !important;
}

:deep(.ant-btn-primary) {
  border-radius: 6px !important;
  height: 40px;
}

.register-row {
  margin-top: 0 !important;
}

.register-row>* {
  padding-top: 0 !important;
  margin-top: 0 !important;
}
</style>