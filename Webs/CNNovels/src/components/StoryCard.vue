<script setup lang="ts">
import { computed } from 'vue';
import type { Novel } from '../data/mockNovels';

const props = defineProps<{
  novel: Novel
}>();

// Helper to format views (e.g., 1250000 -> 1.2M)
const formatViews = computed(() => {
  const v = props.novel.views;
  if (v >= 1000000) {
    return (v / 1000000).toFixed(1) + 'M';
  }
  if (v >= 1000) {
    return (v / 1000).toFixed(1) + 'K';
  }
  return v.toString();
});
</script>

<template>
  <RouterLink :to="`/truyen/${novel.slug}`" class="story-card glass-panel animated-fade">
    <div class="cover-wrapper">
      <img :src="novel.cover" :alt="novel.title" class="story-cover" loading="lazy" />
      <span 
        class="badge" 
        :class="{
          'badge-complete': novel.status === 'Hoàn thành',
          'badge-ongoing': novel.status === 'Đang ra'
        }"
      >
        {{ novel.status }}
      </span>
      <span class="badge-hot" v-if="novel.isHot">HOT</span>
    </div>
    
    <div class="story-meta">
      <h3 class="story-title" :title="novel.title">{{ novel.title }}</h3>
      <span class="story-author">{{ novel.author }}</span>
      
      <div class="story-stats">
        <span class="rating">⭐ {{ novel.rating.toFixed(1) }}</span>
        <span class="views">👁️ {{ formatViews }}</span>
      </div>

      <div class="story-genres">
        <span v-for="g in novel.genres.slice(0, 2)" :key="g" class="genre-tag">{{ g }}</span>
      </div>
    </div>
  </RouterLink>
</template>

<style scoped>
.story-card {
  display: flex;
  flex-direction: column;
  overflow: hidden;
  border-radius: var(--border-radius-md);
  transition: var(--transition-smooth);
  height: 100%;
}

.story-card:hover {
  transform: translateY(-6px);
  border-color: var(--accent-cyan);
  box-shadow: 0 10px 20px rgba(0, 229, 255, 0.1);
}

.cover-wrapper {
  position: relative;
  width: 100%;
  padding-bottom: 135%; /* Aspect ratio 3:4 */
  overflow: hidden;
  background: var(--bg-tertiary);
}

.story-cover {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.story-card:hover .story-cover {
  transform: scale(1.1);
}

.cover-wrapper .badge {
  position: absolute;
  bottom: 8px;
  left: 8px;
  z-index: 2;
  font-size: 10px;
}

.badge-hot {
  position: absolute;
  top: 8px;
  right: 8px;
  z-index: 2;
  background: #ff3b30;
  color: white;
  font-weight: 800;
  font-size: 9px;
  padding: 3px 6px;
  border-radius: 4px;
  box-shadow: 0 2px 6px rgba(255, 59, 48, 0.4);
}

.story-meta {
  padding: 14px;
  display: flex;
  flex-direction: column;
  flex-grow: 1;
  gap: 6px;
}

.story-title {
  font-family: var(--font-heading);
  font-size: 14.5px;
  font-weight: 800;
  color: var(--text-primary);
  line-height: 1.3;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  height: 38px;
}

.story-author {
  font-size: 12px;
  color: var(--text-secondary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.story-stats {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 12px;
  font-weight: 600;
}

.rating {
  color: var(--star-color);
}

.views {
  color: var(--text-muted);
}

.story-genres {
  display: flex;
  gap: 6px;
  margin-top: auto;
  padding-top: 4px;
}

.genre-tag {
  font-size: 10.5px;
  font-weight: 600;
  background: var(--bg-tertiary);
  color: var(--text-secondary);
  padding: 2px 8px;
  border-radius: 4px;
  border: 1px solid var(--border-color);
}
</style>
