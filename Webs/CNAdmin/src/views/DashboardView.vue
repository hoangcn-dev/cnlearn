<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { 
  UserOutlined, 
  CheckCircleOutlined, 
  CloseCircleOutlined, 
  ArrowRightOutlined,
  DashboardOutlined,
  CalendarOutlined,
  DatabaseOutlined,
  NotificationOutlined,
  SafetyCertificateOutlined
} from '@ant-design/icons-vue';

const router = useRouter();

// Mock Statistics (synchronized with the user management database mockup)
const stats = ref({
  totalUsers: 4,
  activeUsers: 3,
  inactiveUsers: 1,
  dbSize: '24.5 MB',
  uptime: '99.98%'
});

// Mock Recent Activity Log
const recentActivities = ref([
  { id: 1, type: 'login', user: 'Nguyễn Văn A', action: 'Đăng nhập vào hệ thống', time: '5 phút trước', color: 'blue' },
  { id: 2, type: 'update', user: 'Lê Thị B', action: 'Cập nhật mã nhân viên NV-002', time: '20 phút trước', color: 'orange' },
  { id: 3, type: 'create', user: 'System', action: 'Đã sao lưu cơ sở dữ liệu tự động', time: '1 giờ trước', color: 'green' },
  { id: 4, type: 'lock', user: 'Admin', action: 'Đã khóa tài khoản Trần Văn C', time: '3 giờ trước', color: 'red' },
]);

const navigateToUsers = () => {
  router.push('/users');
};
</script>

<template>
  <div class="dashboard-view">
    <!-- Calendar bar -->
    <div class="d-flex justify-content-end mb-4">
      <div class="d-flex align-items-center gap-2 text-secondary bg-white px-3 py-2 rounded-3 border shadow-sm small">
        <CalendarOutlined />
        <span>Hôm nay: {{ new Date().toLocaleDateString('vi-VN', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }) }}</span>
      </div>
    </div>

    <!-- Quick Stats Cards (Moved from User Management) -->
    <div class="row g-3 mb-4">
      <div class="col-12 col-sm-6 col-md-4">
        <div class="stats-card p-3 rounded-4 shadow-sm border-0 d-flex align-items-center justify-content-between bg-gradient-blue text-white cursor-pointer" @click="navigateToUsers">
          <div>
            <div class="stats-label text-white-50 small fw-medium">TỔNG THÀNH VIÊN</div>
            <div class="stats-value fs-2 fw-bold mt-1">{{ stats.totalUsers }}</div>
            <div class="stats-desc small text-white-50 mt-1">Xem tất cả thành viên <ArrowRightOutlined class="ms-1 small-icon" /></div>
          </div>
          <div class="stats-icon-box bg-white-10 rounded-circle d-flex align-items-center justify-content-center">
            <UserOutlined class="fs-3" />
          </div>
        </div>
      </div>
      <div class="col-12 col-sm-6 col-md-4">
        <div class="stats-card p-3 rounded-4 shadow-sm border-0 d-flex align-items-center justify-content-between bg-gradient-green text-white cursor-pointer" @click="navigateToUsers">
          <div>
            <div class="stats-label text-white-50 small fw-medium">TÀI KHOẢN HOẠT ĐỘNG</div>
            <div class="stats-value fs-2 fw-bold mt-1">{{ stats.activeUsers }}</div>
            <div class="stats-desc small text-white-50 mt-1">Đang hoạt động tốt trên hệ thống</div>
          </div>
          <div class="stats-icon-box bg-white-10 rounded-circle d-flex align-items-center justify-content-center">
            <CheckCircleOutlined class="fs-3" />
          </div>
        </div>
      </div>
      <div class="col-12 col-md-4">
        <div class="stats-card p-3 rounded-4 shadow-sm border-0 d-flex align-items-center justify-content-between bg-gradient-purple text-white cursor-pointer" @click="navigateToUsers">
          <div>
            <div class="stats-label text-white-50 small fw-medium">TÀI KHOẢN ĐANG KHÓA</div>
            <div class="stats-value fs-2 fw-bold mt-1">{{ stats.inactiveUsers }}</div>
            <div class="stats-desc small text-white-50 mt-1">Bị đình chỉ / Khóa tạm thời</div>
          </div>
          <div class="stats-icon-box bg-white-10 rounded-circle d-flex align-items-center justify-content-center">
            <CloseCircleOutlined class="fs-3" />
          </div>
        </div>
      </div>
    </div>

    <div class="row g-4">
      <!-- Left Panel: Recent Activities & Shortcuts -->
      <div class="col-12 col-lg-8">
        <!-- Recent Activities Card -->
        <div class="card border-0 shadow-sm rounded-4 p-4 mb-4">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h6 class="mb-0 fw-bold text-dark d-flex align-items-center gap-2">
              <NotificationOutlined class="text-primary" />
              <span>Hoạt Động Gần Đây</span>
            </h6>
            <a href="#" class="text-decoration-none small text-primary">Xem tất cả</a>
          </div>
          <div class="activity-timeline">
            <div 
              v-for="act in recentActivities" 
              :key="act.id" 
              class="timeline-item d-flex gap-3 pb-3 position-relative"
            >
              <div class="timeline-badge-wrapper position-relative">
                <div 
                  class="timeline-badge rounded-circle d-flex align-items-center justify-content-center shadow-sm"
                  :class="'badge-color-' + act.color"
                >
                  <span class="dot"></span>
                </div>
              </div>
              <div class="timeline-content flex-grow-1 border-bottom pb-2">
                <div class="d-flex justify-content-between align-items-start">
                  <div class="fw-semibold text-dark small">{{ act.user }}</div>
                  <div class="text-secondary small-extra">{{ act.time }}</div>
                </div>
                <div class="text-secondary small mt-1">{{ act.action }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Shortcuts Card -->
        <div class="card border-0 shadow-sm rounded-4 p-4">
          <h6 class="mb-3 fw-bold text-dark d-flex align-items-center gap-2">
            <SafetyCertificateOutlined class="text-success" />
            <span>Phím Tắt Nhanh Quản Trị</span>
          </h6>
          <div class="row g-2">
            <div class="col-6 col-sm-4">
              <div class="shortcut-box p-3 border rounded-3 text-center cursor-pointer bg-light-hover" @click="navigateToUsers">
                <UserOutlined class="fs-4 text-primary mb-2" />
                <div class="small fw-semibold text-dark">Quản lý User</div>
              </div>
            </div>
            <div class="col-6 col-sm-4">
              <div class="shortcut-box p-3 border rounded-3 text-center cursor-pointer bg-light-hover">
                <DatabaseOutlined class="fs-4 text-success mb-2" />
                <div class="small fw-semibold text-dark">Sao lưu CSDL</div>
              </div>
            </div>
            <div class="col-6 col-sm-4">
              <div class="shortcut-box p-3 border rounded-3 text-center cursor-pointer bg-light-hover">
                <DashboardOutlined class="fs-4 text-warning mb-2" />
                <div class="small fw-semibold text-dark">Cấu hình Hệ thống</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Panel: System Health & Info -->
      <div class="col-12 col-lg-4">
        <div class="card border-0 shadow-sm rounded-4 p-4 h-100">
          <h6 class="mb-4 fw-bold text-dark d-flex align-items-center gap-2">
            <DatabaseOutlined class="text-warning" />
            <span>Thông Số Máy Chủ</span>
          </h6>
          
          <div class="server-stat-item mb-3">
            <div class="d-flex justify-content-between align-items-center mb-1">
              <span class="small text-secondary">Dung lượng Database</span>
              <span class="small fw-bold text-dark">{{ stats.dbSize }}</span>
            </div>
            <div class="progress progress-stat" style="height: 6px;">
              <div class="progress-bar bg-primary" role="progressbar" style="width: 35%" aria-valuenow="35" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
          </div>

          <div class="server-stat-item mb-3">
            <div class="d-flex justify-content-between align-items-center mb-1">
              <span class="small text-secondary">Sử dụng CPU (Server)</span>
              <span class="small fw-bold text-dark">12%</span>
            </div>
            <div class="progress progress-stat" style="height: 6px;">
              <div class="progress-bar bg-success" role="progressbar" style="width: 12%" aria-valuenow="12" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
          </div>

          <div class="server-stat-item mb-4">
            <div class="d-flex justify-content-between align-items-center mb-1">
              <span class="small text-secondary">Sử dụng RAM</span>
              <span class="small fw-bold text-dark">42%</span>
            </div>
            <div class="progress progress-stat" style="height: 6px;">
              <div class="progress-bar bg-warning" role="progressbar" style="width: 42%" aria-valuenow="42" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
          </div>

          <div class="p-3 bg-light rounded-3 border">
            <div class="d-flex align-items-center gap-3">
              <div class="p-2 bg-success-subtle text-success rounded-circle">
                <SafetyCertificateOutlined class="fs-5" />
              </div>
              <div>
                <div class="small fw-bold text-dark">Chứng chỉ SSL / Bảo mật</div>
                <div class="text-secondary small-extra">Đang hoạt động tốt (Hết hạn sau 89 ngày)</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700&display=swap');

.dashboard-view {
  font-family: 'Outfit', sans-serif;
  animation: fadeIn 0.4s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Gradients for stats cards */
.bg-gradient-blue {
  background: linear-gradient(135deg, #3a7bd5 0%, #3a6073 100%) !important;
}

.bg-gradient-green {
  background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%) !important;
}

.bg-gradient-purple {
  background: linear-gradient(135deg, #6441a5 0%, #2a0845 100%) !important;
}

.stats-card {
  transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
  overflow: hidden;
}

.stats-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.15) !important;
}

.stats-icon-box {
  width: 50px;
  height: 50px;
}

.bg-white-10 {
  background-color: rgba(255, 255, 255, 0.15);
}

.small-icon {
  font-size: 11px;
}

/* Timeline CSS */
.activity-timeline {
  padding-left: 10px;
}

.timeline-item::before {
  content: '';
  position: absolute;
  top: 24px;
  left: 11px;
  bottom: 0;
  width: 2px;
  background-color: #f1f3f5;
}

.timeline-item:last-child::before {
  display: none;
}

.timeline-badge {
  width: 24px;
  height: 24px;
  background-color: #ffffff;
  border: 2px solid;
}

.badge-color-blue {
  border-color: #0d6efd;
}
.badge-color-blue .dot {
  background-color: #0d6efd;
}

.badge-color-orange {
  border-color: #fd7e14;
}
.badge-color-orange .dot {
  background-color: #fd7e14;
}

.badge-color-green {
  border-color: #198754;
}
.badge-color-green .dot {
  background-color: #198754;
}

.badge-color-red {
  border-color: #dc3545;
}
.badge-color-red .dot {
  background-color: #dc3545;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.small-extra {
  font-size: 11px;
  color: #adb5bd;
}

.shortcut-box {
  transition: all 0.2s ease;
}

.bg-light-hover:hover {
  background-color: #f1f3f5 !important;
  border-color: #dee2e6 !important;
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.03);
}

.cursor-pointer {
  cursor: pointer;
}

.progress-stat {
  border-radius: 4px;
}
</style>
