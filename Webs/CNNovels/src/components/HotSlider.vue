<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import type { Novel } from '../data/mockNovels';

const props = defineProps<{
  novels: Novel[]
}>();

const currentIndex = ref(0);
let timer: any = null;

const nextSlide = () => {
  currentIndex.value = (currentIndex.value + 1) % props.novels.length;
};

const prevSlide = () => {
  currentIndex.value = (currentIndex.value - 1 + props.novels.length) % props.novels.length;
};

const setSlide = (idx: number) => {
  currentIndex.value = idx;
};

onMounted(() => {
  timer = setInterval(nextSlide, 6000);
});

onUnmounted(() => {
  if (timer) clearInterval(timer);
});
</script>

<template>
  <div class="hot-slider glass-panel" v-if="novels.length > 0">
    <!-- Main Slide -->
    <div 
      class="slider-wrapper" 
      :style="{ transform: `translateX(-${currentIndex * 100}%)` }"
    >
      <div 
        v-for="novel in novels" 
        :key="novel.id" 
        class="slide-item"
      >
        <!-- Background Blur Art -->
        <div class="slide-bg" :style="{ backgroundImage: `url(${novel.cover})` }"></div>
        
        <div class="slide-overlay">
          <div class="slide-content">
            <!-- Cover image -->
            <RouterLink :to="`/truyen/${novel.slug}`" class="slide-cover-link">
              <img :src="novel.cover" alt="" class="slide-cover"/>
            </RouterLink>

            <!-- Text Content -->
            <div class="slide-info animated-fade">
              <span class="slide-badge">⚡ TRUYỆN ĐỀ CỬ</span>
              <h2 class="slide-title">
                <RouterLink :to="`/truyen/${novel.slug}`">{{ novel.title }}</RouterLink>
              </h2>
              <div class="slide-author-genres">
                <span class="slide-author">✍️ {{ novel.author }}</span>
                <span class="divider">|</span>
                <div class="genres">
                  <span v-for="g in novel.genres" :key="g" class="genre-pill">{{ g }}</span>
                </div>
              </div>
              <p class="slide-desc">{{ novel.description }}</p>
              
              <div class="slide-actions">
                <RouterLink :to="`/truyen/${novel.slug}`" class="btn-read">Đọc Ngay</RouterLink>
                <RouterLink :to="`/truyen/${novel.slug}`" class="btn-details">Chi Tiết</RouterLink>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Left / Right Controls -->
    <button class="control-btn prev" @click="prevSlide" aria-label="Previous Slide">‹</button>
    <button class="control-btn next" @click="nextSlide" aria-label="Next Slide">›</button>

    <!-- Indicators -->
    <div class="slider-indicators">
      <button 
        v-for="(_, idx) in novels" 
        :key="idx" 
        class="indicator-dot" 
        :class="{ active: currentIndex === idx }"
        @click="setSlide(idx)"
        :aria-label="`Slide ${idx + 1}`"
      ></button>
    </div>
  </div>
</template>

<style scoped>
.hot-slider {
  position: relative;
  height: 380px;
  overflow: hidden;
  border-radius: var(--border-radius-lg);
  margin-bottom: 40px;
  border: 1px solid var(--border-color);
}

.slider-wrapper {
  display: flex;
  height: 100%;
  transition: transform 0.6s cubic-bezier(0.4, 0, 0.2, 1);
}

.slide-item {
  min-width: 100%;
  height: 100%;
  position: relative;
  overflow: hidden;
  display: flex;
  align-items: center;
}

.slide-bg {
  position: absolute;
  inset: 0;
  background-size: cover;
  background-position: center 30%;
  filter: blur(25px) brightness(0.3);
  transform: scale(1.1);
  z-index: 1;
}

.slide-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(to right, rgba(9, 13, 22, 0.9) 30%, rgba(9, 13, 22, 0.4) 100%);
  z-index: 2;
  display: flex;
  align-items: center;
  padding: 0 50px;
}

[data-theme='light'] .slide-overlay {
  background: linear-gradient(to right, rgba(255, 255, 255, 0.95) 30%, rgba(255, 255, 255, 0.5) 100%);
}

.slide-content {
  display: flex;
  align-items: center;
  gap: 30px;
  max-width: 900px;
  width: 100%;
}

.slide-cover-link {
  flex-shrink: 0;
  display: block;
}

.slide-cover {
  width: 160px;
  height: 220px;
  object-fit: cover;
  border-radius: var(--border-radius-md);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.5);
  border: 2px solid rgba(255, 255, 255, 0.1);
  transition: var(--transition-smooth);
}

.slide-cover:hover {
  transform: scale(1.05);
}

.slide-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 12px;
}

.slide-badge {
  background: rgba(0, 229, 255, 0.15);
  color: var(--accent-cyan);
  font-size: 11px;
  font-weight: 800;
  padding: 4px 10px;
  border-radius: 20px;
  border: 1px solid rgba(0, 229, 255, 0.3);
  text-shadow: var(--glow-cyan);
}

.slide-title {
  font-family: var(--font-heading);
  font-size: 28px;
  font-weight: 900;
  line-height: 1.2;
}

.slide-title a {
  color: var(--text-primary);
}

.slide-title a:hover {
  color: var(--accent-cyan);
}

.slide-author-genres {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 13.5px;
  color: var(--text-secondary);
}

.genres {
  display: flex;
  gap: 6px;
}

.genre-pill {
  font-size: 11px;
  background: var(--bg-tertiary);
  color: var(--text-secondary);
  padding: 2px 8px;
  border-radius: 4px;
}

.slide-desc {
  font-size: 13.5px;
  color: var(--text-secondary);
  line-height: 1.6;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  max-width: 600px;
}

.slide-actions {
  display: flex;
  gap: 12px;
  margin-top: 8px;
}

.btn-read {
  background: var(--accent-cyan);
  color: #090d16;
  font-weight: 800;
  font-size: 13.5px;
  padding: 10px 24px;
  border-radius: 30px;
  box-shadow: var(--glow-cyan);
  transition: var(--transition-smooth);
}

.btn-read:hover {
  transform: translateY(-2px);
  filter: brightness(1.15);
}

.btn-details {
  background: var(--bg-tertiary);
  color: var(--text-primary);
  font-weight: 700;
  font-size: 13.5px;
  padding: 10px 24px;
  border-radius: 30px;
  border: 1px solid var(--border-color);
  transition: var(--transition-smooth);
}

.btn-details:hover {
  background: var(--border-color);
  transform: translateY(-2px);
}

/* Controls */
.control-btn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: rgba(9, 13, 22, 0.6);
  color: white;
  font-size: 26px;
  font-weight: 300;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 5;
  transition: var(--transition-smooth);
}

.control-btn:hover {
  background: var(--accent-cyan);
  color: #090d16;
  box-shadow: var(--glow-cyan);
}

.control-btn.prev {
  left: 20px;
}

.control-btn.next {
  right: 20px;
}

/* Indicators */
.slider-indicators {
  position: absolute;
  bottom: 20px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  gap: 8px;
  z-index: 5;
}

.indicator-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.3);
  transition: var(--transition-smooth);
}

.indicator-dot.active {
  background: var(--accent-cyan);
  width: 24px;
  border-radius: 4px;
  box-shadow: var(--glow-cyan);
}

[data-theme='light'] .indicator-dot {
  background: rgba(0, 0, 0, 0.2);
}

[data-theme='light'] .indicator-dot.active {
  background: var(--accent-cyan);
}

@media (max-width: 768px) {
  .hot-slider {
    height: 280px;
  }
  .slide-cover, .slide-desc, .control-btn, .slide-badge {
    display: none;
  }
  .slide-overlay {
    padding: 0 20px;
  }
  .slide-title {
    font-size: 20px;
  }
}
</style>
