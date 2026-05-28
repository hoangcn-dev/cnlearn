<script setup lang="ts">
import { ref, onMounted } from 'vue';
import type { Review } from '../data/mockNovels';

const props = defineProps<{
  novelId: string;
  initialReviews: Review[];
}>();

const reviews = ref<Review[]>([]);
const commentText = ref('');
const userRating = ref(5);
const showSuccess = ref(false);

const loadReviews = () => {
  const localKey = `cnnovels_reviews_${props.novelId}`;
  const saved = localStorage.getItem(localKey);
  if (saved) {
    try {
      reviews.value = JSON.parse(saved);
    } catch (e) {
      reviews.value = [...props.initialReviews];
    }
  } else {
    reviews.value = [...props.initialReviews];
  }
};

const submitComment = () => {
  if (!commentText.value.trim()) return;

  // Retrieve user details from localStorage if logged in
  let username = 'Đạo Hữu Vô Danh';
  let level = 'Phàm Nhân';
  const savedUser = localStorage.getItem('cnnovels_user');
  if (savedUser) {
    try {
      const u = JSON.parse(savedUser);
      username = u.username || 'Đạo Hữu Vô Danh';
      level = u.rank || 'Phàm Nhân';
    } catch (e) {}
  } else {
    // Generate funny cultivation levels for anonymous users
    const ranks = ['Luyện Khí Tầng 3', 'Trúc Cơ Kỳ', 'Kim Đan Sơ Kỳ', 'Nguyên Anh Lão Tổ'];
    level = ranks[Math.floor(Math.random() * ranks.length)] || 'Luyện Khí Tầng 3';
  }

  const newReview: Review = {
    id: Date.now(),
    username,
    avatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&q=80&w=100',
    rating: userRating.value,
    content: commentText.value.trim(),
    level,
    createdAt: 'Vừa xong'
  };

  reviews.value.unshift(newReview);
  
  // Save to localStorage
  const localKey = `cnnovels_reviews_${props.novelId}`;
  localStorage.setItem(localKey, JSON.stringify(reviews.value));

  // Reset fields
  commentText.value = '';
  userRating.value = 5;
  
  showSuccess.value = true;
  setTimeout(() => {
    showSuccess.value = false;
  }, 3000);
};

onMounted(() => {
  loadReviews();
});
</script>

<template>
  <div class="comment-section glass-panel">
    <h3 class="section-title">💬 Bình luận & Đánh giá ({{ reviews.length }})</h3>

    <!-- Success banner -->
    <Transition name="fade">
      <div class="success-banner" v-if="showSuccess">
        ✨ Tăng 10 điểm Tu Vi! Đánh giá đạo tâm thành công!
      </div>
    </Transition>

    <!-- Review Form -->
    <form @submit.prevent="submitComment" class="comment-form">
      <div class="rating-picker">
        <span class="label">Đánh giá linh lực:</span>
        <div class="stars">
          <button 
            type="button" 
            v-for="star in 5" 
            :key="star" 
            class="star-btn"
            :class="{ active: star <= userRating }"
            @click="userRating = star"
          >
            ★
          </button>
        </div>
        <span class="rating-desc">({{ userRating }}/5 Sao)</span>
      </div>

      <div class="input-wrapper">
        <textarea 
          placeholder="Chia sẻ cảm nhận của đạo hữu về bộ truyện này... (Ví dụ: Main thông minh, tình tiết logic...)" 
          v-model="commentText" 
          rows="3"
          required
        ></textarea>
        <button type="submit" class="btn-submit">
          <span>Gửi Đánh Giá</span>
          <span>⚡</span>
        </button>
      </div>
    </form>

    <!-- Review List -->
    <div class="comment-list" v-if="reviews.length > 0">
      <div v-for="review in reviews" :key="review.id" class="comment-card">
        <div class="comment-user">
          <img :src="review.avatar" alt="" class="user-avatar" />
          <div class="user-info">
            <div class="user-row">
              <span class="username">{{ review.username }}</span>
              <span class="level-badge">{{ review.level }}</span>
            </div>
            <div class="rating-row">
              <span class="stars">
                <span v-for="s in 5" :key="s" class="star" :class="{ filled: s <= review.rating }">★</span>
              </span>
              <span class="date">{{ review.createdAt }}</span>
            </div>
          </div>
        </div>
        <p class="comment-text">{{ review.content }}</p>
      </div>
    </div>
    <div class="comment-empty" v-else>
      Chưa có bình luận nào. Hãy là người đầu tiên khai mở đàm đạo!
    </div>
  </div>
</template>

<style scoped>
.comment-section {
  padding: 24px;
  border-radius: var(--border-radius-lg);
  margin-top: 30px;
}

.success-banner {
  background: rgba(16, 185, 129, 0.15);
  color: #10b981;
  border: 1px solid rgba(16, 185, 129, 0.3);
  padding: 12px;
  border-radius: var(--border-radius-sm);
  margin-bottom: 20px;
  text-align: center;
  font-weight: 700;
  font-size: 13.5px;
}

.comment-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
  margin-bottom: 30px;
  background: var(--bg-tertiary);
  padding: 20px;
  border-radius: var(--border-radius-md);
  border: 1px solid var(--border-color);
}

.rating-picker {
  display: flex;
  align-items: center;
  gap: 12px;
}

.rating-picker .label {
  font-size: 13.5px;
  font-weight: 700;
  color: var(--text-secondary);
}

.stars {
  display: flex;
  gap: 4px;
}

.star-btn {
  font-size: 24px;
  color: var(--text-muted);
  transition: var(--transition-smooth);
}

.star-btn.active, .star-btn:hover {
  color: var(--star-color);
  transform: scale(1.1);
}

.rating-desc {
  font-size: 13px;
  font-weight: 700;
  color: var(--text-muted);
}

.input-wrapper {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.comment-form textarea {
  width: 100%;
  padding: 14px;
  background: var(--bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius-sm);
  font-size: 14px;
  resize: vertical;
  color: var(--text-primary);
  outline: none;
  transition: var(--transition-smooth);
}

.comment-form textarea:focus {
  border-color: var(--accent-cyan);
  box-shadow: 0 0 10px rgba(0, 229, 255, 0.1);
}

.btn-submit {
  align-self: flex-end;
  background: var(--accent-color);
  color: white;
  font-weight: 800;
  font-size: 13.5px;
  padding: 10px 24px;
  border-radius: 30px;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: var(--transition-smooth);
}

.btn-submit:hover {
  background: var(--accent-hover);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 70, 229, 0.3);
}

.comment-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.comment-card {
  padding: 16px;
  background: var(--bg-tertiary);
  border-radius: var(--border-radius-md);
  border: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.comment-user {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-avatar {
  width: 42px;
  height: 42px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid var(--border-color);
}

.user-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.user-row {
  display: flex;
  align-items: center;
  gap: 8px;
}

.username {
  font-size: 14px;
  font-weight: 800;
  color: var(--text-primary);
}

.level-badge {
  font-size: 10px;
  font-weight: 700;
  background: rgba(0, 229, 255, 0.12);
  color: var(--accent-cyan);
  padding: 2px 8px;
  border-radius: 10px;
  border: 1px solid rgba(0, 229, 255, 0.2);
}

.rating-row {
  display: flex;
  align-items: center;
  gap: 10px;
}

.star {
  color: var(--text-muted);
  font-size: 13px;
}

.star.filled {
  color: var(--star-color);
}

.date {
  font-size: 11.5px;
  color: var(--text-muted);
}

.comment-text {
  font-size: 14px;
  color: var(--text-secondary);
  line-height: 1.6;
}

.comment-empty {
  text-align: center;
  padding: 40px 20px;
  color: var(--text-muted);
  font-size: 13.5px;
}
</style>
