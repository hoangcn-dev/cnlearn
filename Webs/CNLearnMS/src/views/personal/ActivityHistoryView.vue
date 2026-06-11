<template>
  <div class="activity-history-view small-font animate-fade-in">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h1 class="fs-4 fw-bold text-dark-blue mb-1">Lịch sử hoạt động</h1>
        <p class="text-secondary small mb-0">Theo dõi tất cả lịch sử làm bài, luyện tập và tương tác của bạn.</p>
      </div>
    </div>

    <div class="card border-0 rounded-4 shadow-sm bg-white p-4">
      <h5 class="fw-bold text-dark-blue mb-4">Lịch sử hoạt động gần đây</h5>
      
      <a-steps
        progress-dot
        direction="vertical"
        :current="0"
      >
        <a-step
          v-for="(item, idx) in activities"
          :key="idx"
        >
          <template #title>
            <span
              :class="{'cursor-pointer hover-accent text-indigo fw-bold': item.quizId}"
              @click="item.quizId && navigateToQuizResult(item.quizId)"
            >
              {{ item.title }}
            </span>
          </template>
          <template #description>
            <div
              :class="{'cursor-pointer hover-accent-desc': item.quizId}"
              @click="item.quizId && navigateToQuizResult(item.quizId)"
            >
              {{ item.description }}
            </div>
          </template>
        </a-step>
      </a-steps>
      
      <div v-if="activities.length === 0" class="text-center py-5 text-secondary">
        Chưa ghi nhận hoạt động nào gần đây.
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const navigateToQuizResult = (quizId: string) => {
  router.push(`/quiz/result/${quizId}`)
}

const activities = ref([
  {
    title: 'Hoàn thành bài kiểm tra',
    description: 'Thi thử học kỳ môn Tin Học lớp 12 - Kết quả: 9.5 điểm (Đúng 19/20 câu, 25 phút) • 10 giờ trước',
    quizId: 'quiz_default_1'
  },
  {
    title: 'Lưu đề thi',
    description: 'Đã lưu đề thi "Khảo sát Vật Lý Chuyên Đề Dòng Điện Xoay Chiều" vào kho lưu trữ cá nhân • Hôm qua'
  },
  {
    title: 'Tạo câu hỏi mới',
    description: 'Thêm câu hỏi "Lập trình hướng đối tượng trong C#" vào ngân hàng câu hỏi chung • 2 ngày trước'
  },
  {
    title: 'Đăng nhập hệ thống',
    description: 'Xác thực tài khoản thành công qua SSO và đồng bộ dữ liệu hồ sơ cá nhân • 3 ngày trước'
  }
])
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}
.small-font {
  font-size: 0.85rem !important;
}
.cursor-pointer {
  cursor: pointer;
}
.text-indigo {
  color: #6366f1;
}
.hover-accent:hover {
  color: #4f46e5 !important;
  text-decoration: underline;
}
.hover-accent-desc:hover {
  color: #3b82f6 !important;
  text-decoration: underline;
}
</style>
