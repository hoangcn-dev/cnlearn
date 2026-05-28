<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { mockNovels } from '../data/mockNovels';
import CommentSection from '../components/CommentSection.vue';

const route = useRoute();
const router = useRouter();

const slug = computed(() => route.params.id as string);
const novel = computed(() => mockNovels.find(n => n.slug === slug.value));

const isBookmarked = ref(false);
const isDescExpanded = ref(false);
const sortAscending = ref(true);
const continueChapterId = ref<number | null>(null);

// Format statistics helpers
const formatNumber = (num: number) => {
  if (num >= 1000000) {
    return (num / 1000000).toFixed(1) + 'M';
  }
  if (num >= 1000) {
    return (num / 1000).toFixed(1) + 'K';
  }
  return num.toString();
};

// Check bookmark state
const checkBookmark = () => {
  if (!novel.value) return;
  const raw = localStorage.getItem('cnnovels_bookmarks');
  if (raw) {
    try {
      const bookmarked = JSON.parse(raw);
      isBookmarked.value = bookmarked.includes(novel.value.id);
    } catch (e) {}
  }
};

// Toggle bookmark state
const toggleBookmark = () => {
  if (!novel.value) return;
  const raw = localStorage.getItem('cnnovels_bookmarks');
  let bookmarked: string[] = [];
  if (raw) {
    try {
      bookmarked = JSON.parse(raw);
    } catch (e) {}
  }

  if (isBookmarked.value) {
    bookmarked = bookmarked.filter(id => id !== novel.value!.id);
  } else {
    bookmarked.push(novel.value.id);
  }

  localStorage.setItem('cnnovels_bookmarks', JSON.stringify(bookmarked));
  isBookmarked.value = !isBookmarked.value;

  // Custom storage event trigger for Navbar
  window.dispatchEvent(new Event('storage'));
};

// Check history for "Continue Reading" button
const checkHistory = () => {
  if (!novel.value) return;
  const raw = localStorage.getItem('cnnovels_history');
  if (raw) {
    try {
      const history = JSON.parse(raw);
      const record = history.find((h: any) => h.novelId === novel.value!.id);
      if (record) {
        continueChapterId.value = record.chapterId;
      }
    } catch (e) {}
  }
};

// Sort chapters list
const sortedChapters = computed(() => {
  if (!novel.value) return [];
  const list = [...novel.value.chapters];
  return sortAscending.value ? list : list.reverse();
});

onMounted(() => {
  window.scrollTo({ top: 0, behavior: 'smooth' });
  checkBookmark();
  checkHistory();
});
</script>

<template>
  <div class="detail-view container animated-fade" v-if="novel">
    <!-- Breadcrumb -->
    <div class="breadcrumb">
      <RouterLink to="/">Trang chủ</RouterLink>
      <span class="sep">›</span>
      <span class="curr">{{ novel.title }}</span>
    </div>

    <!-- Novel Header Panel -->
    <div class="novel-header-panel glass-panel">
      <div class="header-art-bg" :style="{ backgroundImage: `url(${novel.cover})` }"></div>
      
      <div class="header-content">
        <img :src="novel.cover" :alt="novel.title" class="novel-cover" />
        
        <div class="novel-info">
          <div class="badges-row">
            <span 
              class="badge" 
              :class="{
                'badge-complete': novel.status === 'Hoàn thành',
                'badge-ongoing': novel.status === 'Đang ra'
              }"
            >
              {{ novel.status }}
            </span>
            <span class="badge badge-hot" v-if="novel.isHot">ĐỀ CỬ</span>
          </div>

          <h1 class="novel-title">{{ novel.title }}</h1>
          
          <div class="author-row">
            <span>Tác giả: <b class="author-name">{{ novel.author }}</b></span>
          </div>

          <!-- Stats Grid -->
          <div class="stats-grid">
            <div class="stat-item">
              <span class="stat-num">⭐ {{ novel.rating.toFixed(2) }}</span>
              <span class="stat-label">Đánh giá</span>
            </div>
            <div class="stat-item">
              <span class="stat-num">{{ formatNumber(novel.views) }}</span>
              <span class="stat-label">Lượt xem</span>
            </div>
            <div class="stat-item">
              <span class="stat-num">{{ formatNumber(novel.recommendCount) }}</span>
              <span class="stat-label">Đề cử</span>
            </div>
            <div class="stat-item">
              <span class="stat-num">{{ novel.chaptersCount }}</span>
              <span class="stat-label">Số chương</span>
            </div>
          </div>

          <!-- Genres list -->
          <div class="genres-row">
            <RouterLink 
              v-for="g in novel.genres" 
              :key="g" 
              :to="{ path: '/', query: { genre: g } }" 
              class="genre-link"
            >
              {{ g }}
            </RouterLink>
          </div>

          <!-- Actions -->
          <div class="actions-row">
            <!-- Read first chapter -->
            <RouterLink 
              v-if="novel.chapters.length > 0" 
              :to="`/truyen/${novel.slug}/chuong-1`" 
              class="btn-action primary-btn"
            >
              📖 Đọc Từ Đầu
            </RouterLink>
            
            <!-- Read next / continue -->
            <RouterLink 
              v-if="continueChapterId && novel.chapters.length > 0" 
              :to="`/truyen/${novel.slug}/chuong-${continueChapterId}`" 
              class="btn-action continue-btn"
            >
              ⚡ Đọc Tiếp (C.{{ continueChapterId }})
            </RouterLink>

            <!-- Bookmark -->
            <button class="btn-action bookmark-btn" :class="{ bookmarked: isBookmarked }" @click="toggleBookmark">
              <span v-if="isBookmarked">❤️ Đã Lưu</span>
              <span v-else>🤍 Thêm Vào Tủ</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Description & Chapter list Grid -->
    <div class="detail-grid">
      <!-- Main Content Area -->
      <div class="main-details">
        <!-- Synopsis Panel -->
        <div class="synopsis-panel glass-panel">
          <h3 class="section-title">📝 Giới Thiệu</h3>
          <p class="synopsis-text" :class="{ collapsed: !isDescExpanded }">
            {{ novel.description }}
          </p>
          <button class="expand-btn" @click="isDescExpanded = !isDescExpanded">
            {{ isDescExpanded ? 'Thu Gọn ▲' : 'Đọc Thêm ▼' }}
          </button>
        </div>

        <!-- Chapters List Panel -->
        <div class="chapters-panel glass-panel">
          <div class="chapters-header">
            <h3 class="section-title">📂 Danh Sách Chương</h3>
            <button class="sort-btn" @click="sortAscending = !sortAscending">
              Sắp xếp: {{ sortAscending ? 'Cũ nhất ➔ Mới nhất' : 'Mới nhất ➔ Cũ nhất' }}
            </button>
          </div>

          <div class="chapters-list-wrapper" v-if="sortedChapters.length > 0">
            <div class="chapters-grid">
              <RouterLink 
                v-for="chap in sortedChapters" 
                :key="chap.id" 
                :to="`/truyen/${novel.slug}/chuong-${chap.id}`"
                class="chapter-link-item"
              >
                <span class="chapter-title">{{ chap.title }}</span>
                <span class="badge-vip" v-if="chap.isVip">VIP</span>
                <span class="chapter-date">2026</span>
              </RouterLink>
            </div>
          </div>
          <div v-else class="chapters-empty">
            Danh sách chương đang được cập nhật...
          </div>
        </div>

        <!-- Comment section component -->
        <CommentSection :novelId="novel.id" :initialReviews="novel.reviews" />
      </div>

      <!-- Detail Sidebar -->
      <aside class="detail-sidebar">
        <!-- Meta Details card -->
        <div class="meta-card glass-panel">
          <h4 class="card-title">ℹ️ Thông Tin Chi Tiết</h4>
          <div class="meta-list">
            <div class="meta-row">
              <span class="label">Người đăng:</span>
              <span class="val">Hệ Thống</span>
            </div>
            <div class="meta-row">
              <span class="label">Khởi tạo:</span>
              <span class="val">{{ novel.createdAt }}</span>
            </div>
            <div class="meta-row">
              <span class="label">Cập nhật:</span>
              <span class="val">{{ novel.updatedAt }}</span>
            </div>
          </div>
        </div>
      </aside>
    </div>
  </div>
  <div v-else class="container empty-detail">
    Truyện không tồn tại trên hệ thống hoặc đã bị gỡ bỏ.
  </div>
</template>

<style scoped>
.detail-view {
  padding-top: 10px;
}

.breadcrumb {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: var(--text-secondary);
  margin-bottom: 20px;
}

.breadcrumb .sep {
  color: var(--text-muted);
}

.breadcrumb .curr {
  color: var(--text-primary);
  font-weight: 700;
}

/* Header Panel styling */
.novel-header-panel {
  position: relative;
  overflow: hidden;
  border-radius: var(--border-radius-lg);
  margin-bottom: 30px;
}

.header-art-bg {
  position: absolute;
  inset: 0;
  background-size: cover;
  background-position: center 30%;
  filter: blur(40px) brightness(0.25);
  transform: scale(1.1);
  z-index: 1;
}

[data-theme='light'] .header-art-bg {
  filter: blur(40px) brightness(0.95);
  opacity: 0.15;
}

.header-content {
  position: relative;
  z-index: 2;
  display: flex;
  gap: 30px;
  padding: 40px;
}

.novel-cover {
  width: 190px;
  height: 260px;
  object-fit: cover;
  border-radius: var(--border-radius-md);
  box-shadow: var(--shadow-lg);
  border: 3px solid rgba(255, 255, 255, 0.15);
  flex-shrink: 0;
}

.novel-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 12px;
  color: var(--text-primary);
}

.badges-row {
  display: flex;
  gap: 8px;
}

.badges-row .badge {
  font-size: 10.5px;
}

.novel-title {
  font-family: var(--font-heading);
  font-size: 32px;
  font-weight: 900;
  line-height: 1.2;
}

.author-row {
  font-size: 14.5px;
  color: var(--text-secondary);
}

.author-name {
  color: var(--accent-cyan);
}

/* Stats grid */
.stats-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 24px;
  background: rgba(255, 255, 255, 0.05);
  padding: 12px 24px;
  border-radius: var(--border-radius-md);
  border: 1px solid rgba(255, 255, 255, 0.1);
  margin: 6px 0;
}

[data-theme='light'] .stats-grid {
  background: var(--bg-tertiary);
  border-color: var(--border-color);
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 60px;
}

.stat-num {
  font-family: var(--font-heading);
  font-size: 18px;
  font-weight: 800;
  color: var(--text-primary);
}

.stat-label {
  font-size: 11px;
  color: var(--text-secondary);
  font-weight: 600;
  text-transform: uppercase;
}

.genres-row {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.genre-link {
  font-size: 12px;
  font-weight: 700;
  background: var(--bg-tertiary);
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
  padding: 4px 14px;
  border-radius: 20px;
  transition: var(--transition-smooth);
}

.genre-link:hover {
  background: var(--accent-cyan);
  color: #090d16;
  border-color: var(--accent-cyan);
}

/* Action Buttons */
.actions-row {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-top: 10px;
}

.btn-action {
  font-size: 14.5px;
  font-weight: 800;
  padding: 12px 28px;
  border-radius: 30px;
  transition: var(--transition-smooth);
}

.primary-btn {
  background: var(--accent-cyan);
  color: #090d16;
  box-shadow: var(--glow-cyan);
}

.primary-btn:hover {
  transform: translateY(-2px);
  filter: brightness(1.15);
}

.continue-btn {
  background: var(--accent-color);
  color: white;
}

.continue-btn:hover {
  background: var(--accent-hover);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(79, 70, 229, 0.3);
}

.bookmark-btn {
  background: var(--bg-tertiary);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.bookmark-btn:hover {
  background: var(--border-color);
  transform: translateY(-2px);
}

.bookmark-btn.bookmarked {
  background: rgba(239, 68, 68, 0.15);
  color: #ef4444;
  border-color: rgba(239, 68, 68, 0.3);
}

/* Detail Grid Layout */
.detail-grid {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 30px;
}

.main-details {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.synopsis-panel {
  padding: 24px;
  border-radius: var(--border-radius-lg);
}

.synopsis-text {
  font-size: 14.5px;
  color: var(--text-secondary);
  line-height: 1.8;
  white-space: pre-line;
  transition: var(--transition-smooth);
}

.synopsis-text.collapsed {
  display: -webkit-box;
  -webkit-line-clamp: 4;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.expand-btn {
  margin-top: 14px;
  font-size: 13px;
  font-weight: 700;
  color: var(--accent-cyan);
}

/* Chapters panel */
.chapters-panel {
  padding: 24px;
  border-radius: var(--border-radius-lg);
}

.chapters-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 24px;
  border-bottom: 1px solid var(--border-color);
  padding-bottom: 12px;
}

.chapters-header .section-title {
  margin: 0;
}

.chapters-header .section-title::after {
  display: none;
}

.sort-btn {
  font-size: 13px;
  font-weight: 700;
  color: var(--text-secondary);
  padding: 6px 12px;
  border-radius: 6px;
  border: 1px solid var(--border-color);
  background: var(--bg-tertiary);
  transition: var(--transition-smooth);
}

.sort-btn:hover {
  color: var(--text-primary);
  background: var(--border-color);
}

.chapters-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 12px;
}

.chapter-link-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  background: var(--bg-tertiary);
  border-radius: var(--border-radius-sm);
  border: 1px solid var(--border-color);
  transition: var(--transition-smooth);
}

.chapter-link-item:hover {
  border-color: var(--accent-cyan);
  background: var(--bg-secondary);
  transform: translateX(4px);
}

.chapter-title {
  font-size: 13.5px;
  font-weight: 700;
  color: var(--text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  flex-grow: 1;
}

.badge-vip {
  font-size: 9px;
  font-weight: 800;
  background: #f59e0b;
  color: #090d16;
  padding: 2px 4px;
  border-radius: 4px;
  margin-right: 8px;
}

.chapter-date {
  font-size: 11px;
  color: var(--text-muted);
}

.chapters-empty {
  padding: 40px 20px;
  text-align: center;
  color: var(--text-muted);
  font-size: 13.5px;
}

/* Sidebar styling */
.detail-sidebar {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.meta-card {
  padding: 20px;
  border-radius: var(--border-radius-md);
}

.card-title {
  font-family: var(--font-heading);
  font-size: 15px;
  font-weight: 800;
  margin-bottom: 16px;
  color: var(--text-primary);
  border-left: 3px solid var(--accent-cyan);
  padding-left: 8px;
}

.meta-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.meta-row {
  display: flex;
  justify-content: space-between;
  font-size: 13px;
}

.meta-row .label {
  color: var(--text-secondary);
  font-weight: 600;
}

.meta-row .val {
  color: var(--text-primary);
  font-weight: 700;
}

.empty-detail {
  padding: 100px 20px;
  text-align: center;
  color: var(--text-muted);
  font-size: 16px;
}

@media (max-width: 1024px) {
  .detail-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .header-content {
    flex-direction: column;
    align-items: center;
    padding: 20px;
  }
  .novel-cover {
    width: 140px;
    height: 190px;
  }
  .novel-info {
    align-items: center;
    text-align: center;
  }
  .novel-title {
    font-size: 24px;
  }
  .chapters-grid {
    grid-template-columns: 1fr;
  }
}
</style>
