<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { message } from 'ant-design-vue';
import { useRouter } from 'vue-router';
import {
  EditTwoTone,
  DeleteTwoTone,
} from '@ant-design/icons-vue';
import type { User } from '@/model/user/User';
import type { Role } from '@/model/user/Role';
import { getAllRoles, getUserPaging, deleteUsers } from '@/api/user';

const router = useRouter();
const loading = ref(false);

// Real Roles Data loaded from CSDL
const roles = ref<Role[]>([]);

// Real Users Data loaded from CSDL
const users = ref<User[]>([]);

// Pagination, sorting and filtering state
const sortBy = ref<string>('');
const isAsc = ref<boolean>(true);
const activeFilters = ref<Record<string, any>>({});

const pagination = ref({
  current: 1,
  pageSize: 10,
  total: 0,
  showSizeChanger: true,
  pageSizeOptions: ['10', '20', '50', '100']
});

const mapFieldToPascalCase = (field: string) => {
  if (!field) return '';
  if (field === 'roleId') return 'RoleId';
  if (field === 'userCode') return 'UserCode';
  if (field === 'displayName') return 'DisplayName';
  if (field === 'userName') return 'UserName';
  if (field === 'email') return 'Email';
  if (field === 'isActive') return 'IsActive';
  return field.charAt(0).toUpperCase() + field.slice(1);
};

const columns = computed(() => [
  {
    title: 'Mã nhân viên',
    dataIndex: 'userCode',
    key: 'userCode',
    width: '130px',
    sorter: true,
  },
  {
    title: 'Tên hiển thị',
    dataIndex: 'displayName',
    key: 'displayName',
    width: '220px',
    sorter: true,
  },
  {
    title: 'Tên đăng nhập',
    dataIndex: 'userName',
    key: 'userName',
    width: '140px',
    sorter: true,
  },
  {
    title: 'Email',
    dataIndex: 'email',
    key: 'email',
    width: '200px',
    sorter: true,
  },
  {
    title: 'Vai Trò',
    dataIndex: 'roleId',
    key: 'roleId',
    width: '140px',
    filters: roles.value.map(r => ({ text: r.roleName, value: r.roleId })),
    filterMultiple: false,
  },
  {
    title: 'Trạng Thái',
    dataIndex: 'isActive',
    key: 'isActive',
    width: '120px',
    filters: [
      { text: 'Hoạt động', value: 'true' },
      { text: 'Đang khóa', value: 'false' }
    ],
    filterMultiple: false,
  },
  {
    title: 'Thao Tác',
    key: 'action',
    fixed: 'right',
    width: '80px',
  }
]);

const searchText = ref('');

const filteredUsers = computed(() => {
  return users.value;
});

// Watch searchText with 400ms debounce to prevent request flooding
let searchTimeout: any = null;
watch(searchText, () => {
  if (searchTimeout) clearTimeout(searchTimeout);
  searchTimeout = setTimeout(() => {
    pagination.value.current = 1;
    loadUsers();
  }, 400);
});

// Load real users from database using backend paging
const loadUsers = async () => {
  loading.value = true;
  try {
    const filtersList: any[] = [];

    // Filter roleId
    if (activeFilters.value.roleId && activeFilters.value.roleId.length > 0) {
      filtersList.push({
        Property: 'RoleId',
        Operator: 0, // Equal
        Value: activeFilters.value.roleId[0],
        Type: 'String'
      });
    }

    // Filter isActive
    if (activeFilters.value.isActive && activeFilters.value.isActive.length > 0) {
      filtersList.push({
        Property: 'IsActive',
        Operator: 0, // Equal
        Value: activeFilters.value.isActive[0] === 'true',
        Type: 'Bool'
      });
    }

    const requestData = {
      page: pagination.value.current,
      size: pagination.value.pageSize,
      sortBy: sortBy.value ? mapFieldToPascalCase(sortBy.value) : undefined,
      isAsc: sortBy.value ? isAsc.value : undefined,
      isPaging: true,
      key: searchText.value.trim() || undefined,
      filters: filtersList,
      filterGroupType: 0 // And = 0
    };

    const res = await getUserPaging(requestData);
    if (res && res.isSuccess && res.data && res.data.items) {
      users.value = res.data.items;
      pagination.value.total = res.data.total || 0;
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

// Handle table page, size, sorting, and filter changes
const handleTableChange = (pag: any, filters: any, sorter: any) => {
  pagination.value.current = pag.current;
  pagination.value.pageSize = pag.pageSize;
  activeFilters.value = filters;

  if (sorter && sorter.field && sorter.order) {
    sortBy.value = sorter.field;
    isAsc.value = sorter.order === 'ascend';
  } else {
    sortBy.value = '';
    isAsc.value = true;
  }

  loadUsers();
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
    :pagination="pagination"
    :loading="loading"
    @change="handleTableChange">
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
