<template>
  <div class="home-view py-3">
    <!-- Jumbotron banner -->
    <div class="banner p-5 mb-5 rounded-4 shadow-sm text-white">
      <div class="row align-items-center">
        <div class="col-lg-8">
          <span class="badge bg-indigo-light mb-3 px-3 py-2 fs-6">Học Tập Chủ Động - Trực Quan - Hiệu Quả</span>
          <h1 class="display-4 fw-extrabold mb-3">Nền Tảng Quản Lý Học Tập Thông Minh</h1>
          <p class="lead text-light-opacity mb-4">CNLearnMS hỗ trợ theo dõi bài học, ôn luyện ngân hàng đề thi trắc nghiệm đa dạng và đánh giá kết quả tức thì.</p>
          <router-link to="/categories" class="btn btn-light btn-lg px-4 py-2 fw-semibold text-indigo shadow-sm hover-up">
            Bắt đầu luyện tập ngay
          </router-link>
        </div>
      </div>
    </div>

    <!-- 3 Module Cards -->
    <h2 class="fs-3 fw-bold text-dark-blue mb-4 text-center text-lg-start">Chọn Module Học Tập</h2>
    <div class="row g-4 mb-5">
      <!-- Card 1: Khóa học -->
      <div class="col-md-4">
        <div class="card h-100 border-0 rounded-4 shadow-sm card-glowing overflow-hidden card-module disabled-module">
          <div class="gradient-secondary p-4 text-white d-flex align-items-center justify-content-between">
            <span class="badge bg-white-opacity px-2 py-1">Sắp Ra Mắt</span>
            <div class="module-icon">📚</div>
          </div>
          <div class="card-body p-4 d-flex flex-column">
            <h3 class="card-title h5 fw-bold mb-2">Khóa Học Trực Tuyến</h3>
            <p class="card-text text-secondary mb-4 small flex-grow-1">Học tập qua video bài giảng chất lượng cao, bài đọc tài liệu và thảo luận cùng giáo viên giảng dạy.</p>
            <button class="btn btn-outline-secondary w-100 rounded-3" disabled>Đang phát triển</button>
          </div>
        </div>
      </div>

      <!-- Card 2: Trắc nghiệm (ACTIVE & FOCUS) -->
      <div class="col-md-4">
        <div class="card h-100 border-0 rounded-4 shadow-sm card-glowing overflow-hidden card-module border-indigo-accent">
          <div class="gradient-primary p-4 text-white d-flex align-items-center justify-content-between">
            <span class="badge bg-success-opacity px-2 py-1">Đang Hoạt Động</span>
            <div class="module-icon">✍️</div>
          </div>
          <div class="card-body p-4 d-flex flex-column">
            <h3 class="card-title h5 fw-bold mb-2 text-indigo">Ngân Hàng Trắc Nghiệm</h3>
            <p class="card-text text-secondary mb-4 small flex-grow-1">Ôn luyện hàng nghìn câu hỏi đa dạng các cấp độ, phân chia theo danh mục bài học thông minh, chấm điểm tự động.</p>
            <router-link to="/categories" class="btn btn-primary w-100 rounded-3 fw-semibold gradient-primary border-0 hover-up">
              Truy cập Module
            </router-link>
          </div>
        </div>
      </div>

      <!-- Card 3: Khác (Kiểm tra & Báo cáo) -->
      <div class="col-md-4">
        <div class="card h-100 border-0 rounded-4 shadow-sm card-glowing overflow-hidden card-module border-indigo-accent">
          <div class="gradient-accent p-4 text-white d-flex align-items-center justify-content-between">
            <span class="badge bg-success-opacity px-2 py-1">Đang Hoạt Động</span>
            <div class="module-icon">📊</div>
          </div>
          <div class="card-body p-4 d-flex flex-column">
            <h3 class="card-title h5 fw-bold mb-2 text-indigo">Quản Lý Đề & Kỳ Thi</h3>
            <p class="card-text text-secondary mb-4 small flex-grow-1">Thiết lập đề thi từ ngân hàng câu hỏi chung/riêng, tổ chức kỳ thi thử tự động hoặc có giới hạn thời gian.</p>
            <router-link to="/exams" class="btn btn-primary w-100 rounded-3 fw-semibold gradient-accent border-0 hover-up">
              Truy cập Module
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Core screen dashboard logic
</script>

<style scoped>
.fw-extrabold {
  font-weight: 800;
}

.banner {
  background: linear-gradient(135deg, #1e1b4b 0%, #312e81 50%, #4338ca 100%);
  position: relative;
  overflow: hidden;
}

.banner::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -20%;
  width: 400px;
  height: 400px;
  border-radius: 50%;
  background: radial-gradient(circle, rgba(99, 102, 241, 0.4) 0%, rgba(99, 102, 241, 0) 70%);
}

.bg-indigo-light {
  background-color: rgba(99, 102, 241, 0.2);
  color: #c7d2fe;
}

.text-light-opacity {
  color: rgba(255, 255, 255, 0.85);
}

.text-indigo {
  color: #4f46e5;
}

.text-dark-blue {
  color: #1e1b4b;
}

.module-icon {
  font-size: 2.2rem;
}

.card-module {
  background-color: #fff;
  border: 1px solid rgba(0, 0, 0, 0.05);
}

.border-indigo-accent {
  border: 2px solid #818cf8 !important;
}

.bg-white-opacity {
  background-color: rgba(255, 255, 255, 0.2);
  color: #fff;
}

.bg-success-opacity {
  background-color: rgba(16, 185, 129, 0.2);
  color: #34d399;
}

.disabled-module {
  opacity: 85%;
}

.hover-up {
  transition: all 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-3px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.15) !important;
}
</style>
