<script setup lang="ts">
import { ref, onMounted, reactive, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import { ArrowLeftOutlined } from '@ant-design/icons-vue';
import type { User } from '@/model/user/User';
import type { Role } from '@/model/user/Role';
import { getAllRoles, getUserById, saveUsers } from '@/api/user';
import { getErrorMessage } from '@/api/config/axios';

const route = useRoute();
const router = useRouter();

const roles = ref<Role[]>([]);

const formState = reactive<User>({
  userId: '',
  userName: '',
  userCode: '',
  displayName: '',
  email: '',
  password: '',
  isActive: true,
  avatarImageFileId: undefined,
  roleId: ''
});

const rules = computed(() => {
  const isCreate = route.params.id === 'new' || route.params.id === 'create';
  return {
    userCode: isCreate ? [] : [{ required: true, message: 'Vui lòng nhập mã nhân viên!', trigger: 'blur' }],
    displayName: [{ required: true, message: 'Vui lòng nhập tên hiển thị!', trigger: 'blur' }],
    userName: [{ required: true, message: 'Vui lòng nhập tài khoản!', trigger: 'blur' }],
    email: [
      { required: true, message: 'Vui lòng nhập email!', trigger: 'blur' },
      { type: 'email', message: 'Email không đúng định dạng!', trigger: 'blur' }
    ],
    roleId: [{ required: true, message: 'Vui lòng chọn vai trò!', trigger: 'change' }]
  };
});

const formRef = ref();

onMounted(async () => {
  // Tải danh sách vai trò thực tế từ API cơ sở dữ liệu
  try {
    const res = await getAllRoles();
    if (res && res.isSuccess && res.data && res.data.items) {
      roles.value = res.data.items.map((r: any) => ({
        roleId: r.roleId,
        roleName: r.roleName
      }));
    }
  } catch (error) {
    console.error("DEBUG: Failed to load roles from API:", error);
  }

  const userId = route.params.id as string;
  if (userId === 'new' || userId === 'create') {
    // Khởi tạo thông tin cho tài khoản mới (tự động sinh mã nhân viên ngẫu nhiên ban đầu)
    Object.assign(formState, {
      userId: '',
      userName: '',
      userCode: 'U_' + Math.random().toString(36).substring(2, 10).toUpperCase(),
      displayName: '',
      email: '',
      password: '',
      isActive: true,
      avatarImageFileId: undefined,
      roleId: roles.value.length > 0 ? (roles.value[0]?.roleId || '') : ''
    });
  } else {
    // Tải thông tin chi tiết người dùng từ API
    try {
      const res = await getUserById(userId);
      if (res && res.isSuccess && res.data) {
        Object.assign(formState, res.data);
      } else {
        message.error(getErrorMessage(res, 'Không tìm thấy người dùng!'));
        router.push('/users');
      }
    } catch (error: any) {
      console.error('Lỗi khi tải chi tiết người dùng:', error);
      message.error(getErrorMessage(error, 'Có lỗi xảy ra khi tải dữ liệu người dùng.'));
      router.push('/users');
    }
  }
});

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
  const baseUsername = removeAccents(formState.displayName || '');
  if (!baseUsername) {
    message.warning('Vui lòng nhập Tên hiển thị trước!');
    return;
  }
  const randomSuffix = Math.floor(100 + Math.random() * 900);
  formState.userName = `${baseUsername}${randomSuffix}`;
  
  // Tự động đồng bộ/sinh mã nhân viên (userCode) nếu chưa có hoặc rỗng
  if (!formState.userCode) {
    formState.userCode = 'U_' + Math.random().toString(36).substring(2, 10).toUpperCase();
  }
  
  message.success('Đã tự sinh tên đăng nhập thành công!');
};

const handleSave = () => {
  formRef.value
    .validate()
    .then(async () => {
      try {
        const payload = { ...formState };
        // Nếu tạo mới, loại bỏ hoàn toàn trường userId để Backend tự sinh tự động
        if (route.params.id === 'new' || route.params.id === 'create') {
          delete payload.userId;
        }
        // Loại bỏ avatarImageFileId nếu rỗng để Backend gán null/mặc định hợp lệ
        if (!payload.avatarImageFileId) {
          delete payload.avatarImageFileId;
        }
        const res = await saveUsers([payload]);
        if (res && res.isSuccess) {
          message.success('Lưu thông tin thành viên thành công!');
          router.push('/users');
        } else {
          message.error(getErrorMessage(res, 'Lưu thông tin thất bại.'));
        }
      } catch (error: any) {
        console.error('Lỗi khi lưu người dùng:', error);
        message.error(getErrorMessage(error, 'Có lỗi xảy ra khi lưu thông tin thành viên.'));
      }
    })
    .catch((error: any) => {
      console.log('Validation failed:', error);
      message.error('Vui lòng điền đầy đủ các thông tin bắt buộc!');
    });
};

const handleCancel = () => {
  router.push('/users');
};
</script>

<template>
  <div class="user-detail-view">
    <!-- Top Action bar -->
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div class="d-flex align-items-center gap-3">
        <a-button type="default" size="small" class="d-flex align-items-center justify-content-center p-2 rounded-circle" @click="handleCancel">
          <template #icon><ArrowLeftOutlined /></template>
        </a-button>
        <h4 class="mb-0 fw-bold text-dark"><span class="text-primary">{{ formState.userCode || 'Thêm Mới Thành Viên' }}</span></h4>
      </div>
      <div class="d-flex gap-2">
        <a-button size="middle" @click="handleCancel">
          Hủy bỏ
        </a-button>
        <a-button type="primary" size="middle" @click="handleSave">
          Lưu thay đổi
        </a-button>
      </div>
    </div>

    <!-- Main Detail Card Form -->
    <div class="card border-0 shadow-sm rounded-3 p-4 bg-white">
      <a-form
        ref="formRef"
        :model="formState"
        :rules="rules"
        layout="vertical"
      >
        <div class="row g-3">
          <!-- User Code (Mã) - Chỉ hiển thị khi chỉnh sửa -->
          <div class="col-12 col-md-6" v-if="route.params.id !== 'new' && route.params.id !== 'create'">
            <a-form-item label="Mã nhân viên" name="userCode">
              <a-input v-model:value="formState.userCode" placeholder="Nhập mã nhân viên..." :disabled="route.params.id !== 'new' && route.params.id !== 'create'" autocomplete="off" />
            </a-form-item>
          </div>

          <!-- Display Name (Tên hiển thị) -->
          <div class="col-12 col-md-6">
            <a-form-item label="Tên hiển thị" name="displayName">
              <a-input v-model:value="formState.displayName" placeholder="Nhập tên hiển thị..." autocomplete="off" />
            </a-form-item>
          </div>

          <!-- Username (Tài khoản) -->
          <div class="col-12 col-md-6">
            <a-form-item label="Tên đăng nhập / Tài khoản" name="userName">
              <a-input v-model:value="formState.userName" placeholder="Nhập tài khoản đăng nhập..." :disabled="route.params.id !== 'new' && route.params.id !== 'create'" autocomplete="off">
                <template #suffix v-if="route.params.id === 'new' || route.params.id === 'create'">
                  <a-button type="link" size="small" @click="handleAutoGenerateUsername"
                    style="padding: 0; height: auto; font-size: 0.85rem; font-weight: 600;">
                    Auto
                  </a-button>
                </template>
              </a-input>
            </a-form-item>
          </div>

          <!-- Email -->
          <div class="col-12 col-md-6">
            <a-form-item label="Địa chỉ Email" name="email">
              <a-input v-model:value="formState.email" placeholder="example@domain.com" autocomplete="off" />
            </a-form-item>
          </div>

          <!-- Mật khẩu (Chỉ hiển thị khi tạo mới) -->
          <div class="col-12 col-md-6" v-if="route.params.id === 'new' || route.params.id === 'create'">
            <a-form-item label="Mật khẩu khởi tạo" name="password" :rules="[{ required: true, message: 'Vui lòng nhập mật khẩu khởi tạo!', trigger: 'blur' }]">
              <a-input-password v-model:value="formState.password" placeholder="Nhập mật khẩu..." autocomplete="new-password" />
            </a-form-item>
          </div>

          <!-- Role selection (Vai trò) -->
          <div class="col-12 col-md-6">
            <a-form-item label="Vai trò / Phân quyền" name="roleId">
              <a-select v-model:value="formState.roleId" placeholder="Chọn một vai trò" class="w-100">
                <a-select-option v-for="r in roles" :key="r.roleId" :value="r.roleId">
                  {{ r.roleName }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </div>

          <!-- Account Status (Trạng thái) -->
          <div class="col-12 col-md-6">
            <a-form-item label="Trạng thái tài khoản" name="isActive">
              <div class="d-flex align-items-center gap-2 mt-1">
                <a-switch v-model:checked="formState.isActive" />
                <span class="fw-semibold small" :class="formState.isActive ? 'text-success' : 'text-danger'">
                  {{ formState.isActive ? 'Đang hoạt động' : 'Đang khóa' }}
                </span>
              </div>
            </a-form-item>
          </div>
        </div>
      </a-form>
    </div>
  </div>
</template>

<style scoped>
.user-detail-view {
  animation: fadeIn 0.3s ease-out;
}

:deep(.ant-form-item) {
  margin-bottom: 0px;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
