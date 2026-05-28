<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { mockNovels } from '../data/mockNovels';
import StoryCard from '../components/StoryCard.vue';
import HotSlider from '../components/HotSlider.vue';

const route = useRoute();
const activeRankTab = ref('views'); // 'views' | 'rating' | 'recommend'

// 1. Extract hot novels for Slider (based on isHot flag)
const sliderHotNovels = computed(() => mockNovels.filter(n => n.isHot));

// 2. 16 Hotest novels (sorted by rating descending, max 16)
const topHotNovels = computed(() => {
  return [...mockNovels]
    .sort((a, b) => b.rating - a.rating)
    .slice(0, 16);
});

// 3. Newest novels (sorted by creation date descending)
const latestNovels = computed(() => {
  return [...mockNovels]
    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
});

// 4. Completed novels (status === 'Hoàn thành', sorted by rating descending)
const completedNovels = computed(() => {
  return [...mockNovels]
    .filter(n => n.status === 'Hoàn thành')
    .sort((a, b) => b.rating - a.rating);
});

// Compute sidebar rankings
const rankedNovels = computed(() => {
  const list = [...mockNovels];
  if (activeRankTab.value === 'views') {
    return list.sort((a, b) => b.views - a.views).slice(0, 5);
  } else if (activeRankTab.value === 'rating') {
    return list.sort((a, b) => b.rating - a.rating).slice(0, 5);
  } else {
    return list.sort((a, b) => b.recommendCount - a.recommendCount).slice(0, 5);
  }
});

// Helper to format rankings
const formatRankMetric = (novel: any) => {
  if (activeRankTab.value === 'views') {
    const v = novel.views;
    return v >= 1000000 ? (v / 1000000).toFixed(1) + 'M' : v.toLocaleString();
  } else if (activeRankTab.value === 'rating') {
    return novel.rating.toFixed(2) + ' ★';
  } else {
    return novel.recommendCount.toLocaleString() + ' vote';
  }
};
</script>

<template>
  <div class="home-view container animated-fade">
    <!-- Hero Slider for featured highlights -->
    <HotSlider :novels="sliderHotNovels" />

    <!-- Main Content Layout -->
    <div class="main-layout">
      <!-- Novels Catalog divided by the 3 criteria -->
      <div class="catalog-section">
        
        <!-- 1. Truyện Hot Nhất (Sorted by rating decreasing, Max 16) -->
        <section class="section-group">
          <div class="section-header">
            <h2 class="section-title">🔥 Truyện Hot Nhất</h2>
            <span class="section-subtitle">Đỉnh cấp linh lực, vạn chúng tôn sùng (Tối đa 16 bộ)</span>
          </div>
          <div class="novels-grid" v-if="topHotNovels.length > 0">
            <StoryCard 
              v-for="novel in topHotNovels" 
              :key="novel.id" 
              :novel="novel"
            />
          </div>
          <div class="novels-empty" v-else>
            Chưa có thông tin truyện hot.
          </div>
        </section>

        <!-- 2. Truyện Mới Nhất (Sorted by creation date descending) -->
        <section class="section-group">
          <div class="section-header">
            <h2 class="section-title">✨ Truyện Mới Nhất</h2>
            <span class="section-subtitle">Mới khai thiên lập địa, pháp trận mở ra</span>
          </div>
          <div class="novels-grid" v-if="latestNovels.length > 0">
            <StoryCard 
              v-for="novel in latestNovels" 
              :key="novel.id" 
              :novel="novel"
            />
          </div>
          <div class="novels-empty" v-else>
            Chưa có thông tin truyện mới.
          </div>
        </section>

        <!-- 3. Truyện Hoàn Thành (Completed novels, Sorted by rating decreasing) -->
        <section class="section-group">
          <div class="section-header">
            <h2 class="section-title">🏆 Truyện Hoàn Thành</h2>
            <span class="section-subtitle">Đắc đạo thành tiên, công đức viên mãn</span>
          </div>
          <div class="novels-grid" v-if="completedNovels.length > 0">
            <StoryCard 
              v-for="novel in completedNovels" 
              :key="novel.id" 
              :novel="novel"
            />
          </div>
          <div class="novels-empty" v-else>
            Chưa có thông tin truyện đã hoàn thành.
          </div>
        </section>

      </div>

      <!-- Sidebar leaderboard and cultivation updates -->
      <aside class="sidebar-section">
        <!-- Rankings tab panel -->
        <div class="rankings-panel glass-panel">
          <h3 class="panel-title">🏆 Bảng Xếp Hạng</h3>
          <div class="rank-tabs">
            <button 
              class="tab-btn" 
              :class="{ active: activeRankTab === 'views' }"
              @click="activeRankTab = 'views'"
            >
              Xem Nhiều
            </button>
            <button 
              class="tab-btn" 
              :class="{ active: activeRankTab === 'rating' }"
              @click="activeRankTab = 'rating'"
            >
              Đánh Giá
            </button>
            <button 
              class="tab-btn" 
              :class="{ active: activeRankTab === 'recommend' }"
              @click="activeRankTab = 'recommend'"
            >
              Đề Cử
            </button>
          </div>

          <div class="rank-list">
            <div 
              v-for="(novel, idx) in rankedNovels" 
              :key="novel.id" 
              class="rank-item"
            >
              <span class="rank-number" :class="`rank-${idx + 1}`">{{ idx + 1 }}</span>
              <img :src="novel.cover" alt="" class="rank-cover" />
              <div class="rank-info">
                <RouterLink :to="`/truyen/${novel.slug}`" class="rank-title">{{ novel.title }}</RouterLink>
                <span class="rank-author">{{ novel.author }}</span>
              </div>
              <span class="rank-metric">{{ formatRankMetric(novel) }}</span>
            </div>
          </div>
        </div>

        <!-- Quick Info Panel -->
        <div class="info-panel glass-panel">
          <h3 class="panel-title">💡 Mẹo Tu Luyện</h3>
          <p class="info-text">
            Đạo hữu hãy đăng nhập và tích cực đàm luận ở các chương truyện để nhận thêm linh khí giúp tăng tốc tu vi, thăng cấp từ <b>Phàm Nhân</b> lên các cảnh giới cao hơn!
          </p>
        </div>
      </aside>
    </div>
  </div>
</template>

<style scoped>
.home-view {
  padding-top: 20px;
}

.main-layout {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 30px;
  margin-top: 24px;
}

.catalog-section {
  display: flex;
  flex-direction: column;
  gap: 40px;
}

/* Custom gorgeous section groupings */
.section-group {
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.section-header {
  border-left: 4px solid var(--accent-cyan);
  padding-left: 14px;
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-bottom: 2px;
}

.section-title {
  font-family: var(--font-heading);
  font-size: 19px;
  font-weight: 900;
  color: var(--text-primary);
  line-height: 1.25;
}

.section-subtitle {
  font-size: 12.5px;
  color: var(--text-muted);
  font-weight: 500;
}

.novels-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(170px, 1fr));
  gap: 20px;
}

.novels-empty {
  padding: 40px 20px;
  text-align: center;
  color: var(--text-muted);
  font-size: 13.5px;
  background: var(--bg-secondary);
  border-radius: var(--border-radius-md);
  border: 1px dashed var(--border-color);
}

/* Sidebar styling */
.sidebar-section {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.panel-title {
  font-family: var(--font-heading);
  font-size: 16px;
  font-weight: 800;
  margin-bottom: 16px;
  color: var(--text-primary);
  border-left: 4px solid var(--accent-cyan);
  padding-left: 10px;
}

.rankings-panel {
  padding: 20px;
  border-radius: var(--border-radius-md);
}

.rank-tabs {
  display: flex;
  background: var(--bg-tertiary);
  padding: 4px;
  border-radius: 8px;
  margin-bottom: 16px;
}

.tab-btn {
  flex: 1;
  font-size: 12px;
  font-weight: 700;
  padding: 8px 0;
  text-align: center;
  border-radius: 6px;
  color: var(--text-secondary);
  transition: var(--transition-smooth);
}

.tab-btn.active {
  background: var(--bg-secondary);
  color: var(--accent-cyan);
  box-shadow: var(--shadow-sm);
}

.rank-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.rank-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px;
  border-radius: var(--border-radius-sm);
  transition: var(--transition-smooth);
}

.rank-item:hover {
  background: var(--bg-tertiary);
}

.rank-number {
  font-family: var(--font-heading);
  font-size: 14px;
  font-weight: 900;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--bg-tertiary);
  color: var(--text-muted);
}

.rank-number.rank-1 {
  background: #f59e0b;
  color: #090d16;
}

.rank-number.rank-2 {
  background: #cbd5e1;
  color: #090d16;
}

.rank-number.rank-3 {
  background: #b45309;
  color: white;
}

.rank-cover {
  width: 32px;
  height: 44px;
  object-fit: cover;
  border-radius: 4px;
}

.rank-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  flex-grow: 1;
  overflow: hidden;
}

.rank-title {
  font-size: 13px;
  font-weight: 700;
  color: var(--text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.rank-title:hover {
  color: var(--accent-cyan);
}

.rank-author {
  font-size: 11px;
  color: var(--text-muted);
}

.rank-metric {
  font-size: 11.5px;
  font-weight: 700;
  color: var(--accent-cyan);
}

.info-panel {
  padding: 20px;
  border-radius: var(--border-radius-md);
}

.info-text {
  font-size: 13px;
  color: var(--text-secondary);
  line-height: 1.6;
}

@media (max-width: 1024px) {
  .main-layout {
    grid-template-columns: 1fr;
  }
}
</style>
