<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { message } from 'ant-design-vue';
import { useRouter } from 'vue-router';
import {
  EditTwoTone,
  DeleteTwoTone,
} from '@ant-design/icons-vue';
import type { User } from '@/model/user/User';
import type { Role } from '@/model/user/Role';
import { getAllRoles, getAllUsers, deleteUsers } from '@/api/user';

const router = useRouter();
const loading = ref(false);

// Real Roles Data loaded from CSDL
const roles = ref<Role[]>([]);

// Real Users Data loaded from CSDL
const users = ref<User[]>([]);

const columns = [
  {
    title: 'Mã nhân viên',
    dataIndex: 'userCode',
    key: 'userCode',
    width: '130px',
  },
  {
    title: 'Tên hiển thị',
    dataIndex: 'displayName',
    key: 'displayName',
    width: '220px',
  },
  {
    title: 'Tên đăng nhập',
    dataIndex: 'userName',
    key: 'userName',
    width: '140px',
  },
  {
    title: 'Email',
    dataIndex: 'email',
    key: 'email',
    width: '200px',
  },
  {
    title: 'Vai Trò',
    dataIndex: 'roleId',
    key: 'roleId',
    width: '140px',
  },
  {
    title: 'Trạng Thái',
    dataIndex: 'isActive',
    key: 'isActive',
    width: '120px',
  },
  {
    title: 'Thao Tác',
    key: 'action',
    fixed: 'right',
    width: '80px',
  }
];

const searchText = ref('');

const filteredUsers = computed(() => {
  if (!searchText.value) return users.value;
  const q = searchText.value.toLowerCase().trim();
  return users.value.filter(u => 
    (u.userCode && u.userCode.toLowerCase().includes(q)) ||
    (u.displayName && u.displayName.toLowerCase().includes(q)) ||
    (u.userName && u.userName.toLowerCase().includes(q)) ||
    (u.email && u.email.toLowerCase().includes(q))
  );
});

// Load real users from database
const loadUsers = async () => {
  loading.value = true;
  try {
    const res = await getAllUsers();
    if (res && res.isSuccess && res.data && res.data.items) {
      users.value = res.data.items;
    } else {
      message.error(res.data?.UserMsg || 'Không thể tải danh sách người dùng.');
    }
  } catch (error) {
    console.error('Error loading users:', error);
    message.error('Có lỗi xảy ra khi kết nối máy chủ.');
  } finally {
    loading.value = false;
  }
};

// Delete user using real API
const handleDelete = async (userId: string) => {
  try {
    const res = await deleteUsers([userId]);
    if (res && res.isSuccess) {
      message.success('Xóa tài khoản người dùng thành công!');
      await loadUsers();
    } else {
      message.error(res.data?.UserMsg || 'Xóa người dùng thất bại.');
    }
  } catch (error) {
    console.error('Error deleting user:', error);
    message.error('Lỗi hệ thống khi xóa tài khoản.');
  }
};

// Navigate to User Creation mode
const handleCreateUser = () => {
  router.push('/users/new');
};

onMounted(async () => {
  loading.value = true;
  // 1. Tải danh sách vai trò
  try {
    const res = await getAllRoles();
    if (res && res.isSuccess && res.data && res.data.items) {
      roles.value = res.data.items.map((r: any) => ({
        roleId: r.roleId,
        roleName: r.roleName
      }));
    }
  } catch (error) {
    console.warn("Không thể tải vai trò từ API:", error);
  }

  // 2. Tải danh sách người dùng thực tế
  await loadUsers();
});
</script>

<template>
  <div class="d-flex flex-column">
    <div class="d-flex mb-3 align-items-center justify-content-between">
      <div class="d-flex align-items-center">
        <a-input v-model:value="searchText" placeholder="Nhập từ khóa tìm kiếm..." class="w-auto me-2" allow-clear />
      </div>
      <a-button type="primary" @click="handleCreateUser">Tạo TK</a-button>
    </div>
  </div>
  <a-table :columns="columns" :data-source="filteredUsers" size="small" class="bg-white rounded-2 shadow-sm"
    :scroll="{ x: 1200, y: 'calc(100vh - 240px)' }"
    :pagination="{ defaultPageSize: 50, showSizeChanger: true, pageSizeOptions: ['10', '20', '50', '100'] }">
    <template #bodyCell="{ column, record, text }">
      <template v-if="column.key === 'action'">
        <div class="d-flex align-items-center">
          <router-link :to="'/users/' + record.userId" class="me-2 d-flex align-items-center">
            <EditTwoTone class="hover-pointer fs-6" title="Chỉnh sửa" two-tone-color="black" />
          </router-link>
          <a-popconfirm
            title="Bạn có chắc chắn muốn xóa tài khoản này?"
            ok-text="Xóa"
            cancel-text="Hủy"
            @confirm="handleDelete(record.userId)"
          >
            <DeleteTwoTone class="hover-pointer fs-6" title="Xóa" two-tone-color="black" />
          </a-popconfirm>
        </div>
      </template>
      <template v-else-if="column.key === 'userCode'">
        <router-link :to="'/users/' + record.userId" class="text-primary fw-medium font-monospace">
          {{ record.userCode }}
        </router-link>
      </template>
      <template v-else-if="column.key === 'roleId'">
        <span class="badge bg-light text-dark border px-2 py-1.5 fw-medium">
          {{ roles.find(r => r.roleId === record.roleId)?.roleName || record.roleId }}
        </span>
      </template>
      <template v-else-if="column.key === 'isActive'">
        <span :class="record.isActive ? 'text-success' : 'text-danger'" class="fw-semibold">
          {{ record.isActive ? 'Hoạt động' : 'Đang khóa' }}
        </span>
      </template>
      <template v-else>
        {{ text }}
      </template>
    </template>
  </a-table>
</template>

<style scoped>
:deep(.ant-table-pagination.ant-pagination) {
  margin-right: 16px !important;
}
</style>
