<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { mockNovels } from '../data/mockNovels';

const route = useRoute();
const router = useRouter();

// Retrieve params
const slug = computed(() => route.params.id as string);
const chapterId = computed(() => Number(route.params.chapterId));

const novel = computed(() => mockNovels.find(n => n.slug === slug.value));
const chapter = computed(() => {
  if (!novel.value) return null;
  return novel.value.chapters.find(c => c.id === chapterId.value);
});

// Navigation indices
const hasPrev = computed(() => chapterId.value > 1);
const hasNext = computed(() => {
  if (!novel.value) return false;
  return chapterId.value < novel.value.chapters.length;
});

// Reading custom options (Persisted state)
const options = ref({
  theme: 'beige', // 'beige' | 'dark' | 'white' | 'green'
  fontSize: 20, // in px
  fontFamily: 'serif', // 'serif' | 'sans-serif' | 'playfair'
  containerWidth: 'medium', // 'narrow' | 'medium' | 'wide'
  lineHeight: 'normal', // 'single' | 'normal' | 'double'
});

// Auto scroll settings
const isAutoScrolling = ref(false);
const scrollSpeed = ref(2); // 1: slow, 2: medium, 3: fast
let scrollInterval: any = null;

const showSettingsDrawer = ref(false);

// Load options from localStorage
const loadReaderSettings = () => {
  const saved = localStorage.getItem('cnnovels_reader_settings');
  if (saved) {
    try {
      options.value = { ...options.value, ...JSON.parse(saved) };
    } catch (e) {}
  }
};

// Save options
const saveReaderSettings = () => {
  localStorage.setItem('cnnovels_reader_settings', JSON.stringify(options.value));
};

// Sync reading history
const saveReadingHistory = () => {
  if (!novel.value) return;
  
  try {
    const raw = localStorage.getItem('cnnovels_history');
    let historyList: any[] = [];
    if (raw) {
      historyList = JSON.parse(raw);
    }
    
    // Filter existing for this novel
    historyList = historyList.filter(h => h.novelId !== novel.value!.id);
    
    // Unshift new record
    historyList.unshift({
      novelId: novel.value.id,
      chapterId: chapterId.value,
      timestamp: Date.now()
    });
    
    localStorage.setItem('cnnovels_history', JSON.stringify(historyList));
    
    // Dispatch storage event to alert navbar
    window.dispatchEvent(new Event('storage'));
  } catch (e) {
    console.error(e);
  }
};

// Go to another chapter
const navigateToChapter = (id: number) => {
  stopAutoScroll();
  router.push(`/truyen/${slug.value}/chuong-${id}`);
};

// Autoscroll mechanics
const toggleAutoScroll = () => {
  if (isAutoScrolling.value) {
    stopAutoScroll();
  } else {
    startAutoScroll();
  }
};

const startAutoScroll = () => {
  isAutoScrolling.value = true;
  stopAutoScroll(); // Clear existing

  const speedMs = scrollSpeed.value === 1 ? 50 : scrollSpeed.value === 2 ? 30 : 15;
  scrollInterval = setInterval(() => {
    window.scrollBy({ top: 1, behavior: 'auto' });
    
    // Check if bottom reached
    if ((window.innerHeight + window.scrollY) >= document.documentElement.scrollHeight - 2) {
      stopAutoScroll();
    }
  }, speedMs);
};

const stopAutoScroll = () => {
  isAutoScrolling.value = false;
  if (scrollInterval) {
    clearInterval(scrollInterval);
    scrollInterval = null;
  }
};

// Trigger scroll to top
const scrollToTop = () => {
  window.scrollTo({ top: 0, behavior: 'smooth' });
};

// Theme helper mapping
const getThemeClass = computed(() => {
  return `reader-theme-${options.value.theme}`;
});

const getFontFamilyStyle = computed(() => {
  if (options.value.fontFamily === 'serif') return "'Lora', 'Georgia', serif";
  if (options.value.fontFamily === 'playfair') return "'Playfair Display', serif";
  return "'Plus Jakarta Sans', sans-serif";
});

const getContainerWidthStyle = computed(() => {
  if (options.value.containerWidth === 'narrow') return '620px';
  if (options.value.containerWidth === 'wide') return '1000px';
  return '800px';
});

const getLineHeightStyle = computed(() => {
  if (options.value.lineHeight === 'single') return '1.5';
  if (options.value.lineHeight === 'double') return '2.2';
  return '1.85';
});

// Watch settings to save
watch(options, () => {
  saveReaderSettings();
}, { deep: true });

// Listen to route updates (navigating chapter to chapter)
watch(chapterId, () => {
  window.scrollTo({ top: 0 });
  saveReadingHistory();
});

onMounted(() => {
  loadReaderSettings();
  saveReadingHistory();
  window.scrollTo({ top: 0 });

  // Stop autoscroll on scroll wheel
  window.addEventListener('wheel', () => {
    if (isAutoScrolling.value) {
      stopAutoScroll();
    }
  });

  // Spacebar to play/pause autoscroll
  window.addEventListener('keydown', (e: KeyboardEvent) => {
    if (e.code === 'Space' && e.target === document.body) {
      e.preventDefault();
      toggleAutoScroll();
    }
  });
});
</script>

<template>
  <div class="reader-container" :class="getThemeClass" v-if="novel && chapter">
    <!-- Top Floating Header Bar -->
    <header class="reader-header glass-panel">
      <div class="header-inner container">
        <RouterLink to="/" class="btn-nav">🏠 Trang Chủ</RouterLink>
        <span class="sep">|</span>
        <RouterLink :to="`/truyen/${novel.slug}`" class="novel-title-link">{{ novel.title }}</RouterLink>
        
        <div class="chapter-selector-wrapper">
          <select :value="chapterId" @change="e => navigateToChapter(Number((e.target as HTMLSelectElement).value))" class="chapter-select">
            <option v-for="c in novel.chapters" :key="c.id" :value="c.id">
              {{ c.title }}
            </option>
          </select>
        </div>

        <button class="btn-nav settings-btn" @click="showSettingsDrawer = !showSettingsDrawer">
          ⚙️ Cài Đặt
        </button>
      </div>
    </header>

    <!-- Settings Controls Sidebar Drawer -->
    <Transition name="slide-panel">
      <div class="settings-drawer glass-panel" v-if="showSettingsDrawer">
        <div class="drawer-header">
          <h4>Bản Màu & Phông Chữ</h4>
          <button class="close-btn" @click="showSettingsDrawer = false">✕</button>
        </div>

        <div class="drawer-body">
          <!-- Reading background theme -->
          <div class="setting-item">
            <span class="setting-label">Màu Nền Đọc:</span>
            <div class="themes-grid">
              <button 
                class="theme-pick theme-beige" 
                :class="{ active: options.theme === 'beige' }"
                @click="options.theme = 'beige'"
                title="Vàng Kem"
              >Beige</button>
              <button 
                class="theme-pick theme-dark" 
                :class="{ active: options.theme === 'dark' }"
                @click="options.theme = 'dark'"
                title="Huyền Bí"
              >Dark</button>
              <button 
                class="theme-pick theme-white" 
                :class="{ active: options.theme === 'white' }"
                @click="options.theme = 'white'"
                title="Trắng Tinh"
              >White</button>
              <button 
                class="theme-pick theme-green" 
                :class="{ active: options.theme === 'green' }"
                @click="options.theme = 'green'"
                title="Bảo Vệ Mắt"
              >Green</button>
            </div>
          </div>

          <!-- Font Size Picker -->
          <div class="setting-item">
            <span class="setting-label">Kích thước chữ: {{ options.fontSize }}px</span>
            <div class="flex-row">
              <button class="adj-btn" @click="options.fontSize = Math.max(14, options.fontSize - 2)">A-</button>
              <button class="adj-btn" @click="options.fontSize = Math.min(32, options.fontSize + 2)">A+</button>
            </div>
          </div>

          <!-- Font Family Picker -->
          <div class="setting-item">
            <span class="setting-label">Phông Chữ:</span>
            <select v-model="options.fontFamily" class="select-field">
              <option value="serif">Lora (Serif Hoài Cổ)</option>
              <option value="playfair">Playfair Display (Kiêu Sa)</option>
              <option value="sans-serif">Plus Jakarta (Hiện Đại)</option>
            </select>
          </div>

          <!-- Line Height Picker -->
          <div class="setting-item">
            <span class="setting-label">Giãn Dòng:</span>
            <div class="flex-row gap-6">
              <button class="chip-btn" :class="{ active: options.lineHeight === 'single' }" @click="options.lineHeight = 'single'">Hẹp</button>
              <button class="chip-btn" :class="{ active: options.lineHeight === 'normal' }" @click="options.lineHeight = 'normal'">Vừa</button>
              <button class="chip-btn" :class="{ active: options.lineHeight === 'double' }" @click="options.lineHeight = 'double'">Rộng</button>
            </div>
          </div>

          <!-- Screen Column Width Picker -->
          <div class="setting-item">
            <span class="setting-label">Chiều rộng trang:</span>
            <div class="flex-row gap-6">
              <button class="chip-btn" :class="{ active: options.containerWidth === 'narrow' }" @click="options.containerWidth = 'narrow'">Hẹp</button>
              <button class="chip-btn" :class="{ active: options.containerWidth === 'medium' }" @click="options.containerWidth = 'medium'">Vừa</button>
              <button class="chip-btn" :class="{ active: options.containerWidth === 'wide' }" @click="options.containerWidth = 'wide'">Rộng</button>
            </div>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Reading main text segment -->
    <main class="reader-main container" :style="{ maxWidth: getContainerWidthStyle }">
      <div class="chapter-intro">
        <h3 class="novel-link">
          <RouterLink :to="`/truyen/${novel.slug}`">{{ novel.title }}</RouterLink>
        </h3>
        <h1 class="chapter-headline">{{ chapter.title }}</h1>
        <p class="chapter-meta">Cập nhật lúc {{ novel.updatedAt }}</p>
      </div>

      <!-- VIP block warn warning -->
      <div v-if="chapter.isVip" class="vip-warning glass-panel">
        <div class="vip-icon">🔒</div>
        <h4>Đây là Chương VIP giả lập!</h4>
        <p>Đạo hữu đã mở khóa miễn phí chương này dưới danh nghĩa Khách Quý.</p>
      </div>

      <div 
        class="chapter-content-body" 
        :style="{ 
          fontSize: `${options.fontSize}px`, 
          fontFamily: getFontFamilyStyle, 
          lineHeight: getLineHeightStyle 
        }"
      >
        <p v-for="(paragraph, pIdx) in chapter.content.split('\n\n')" :key="pIdx">
          {{ paragraph }}
        </p>
      </div>

      <!-- Lower Chapter Navigation bar -->
      <div class="chapter-footer-nav">
        <button 
          class="nav-btn prev-btn" 
          :disabled="!hasPrev" 
          @click="navigateToChapter(chapterId - 1)"
        >
          🡄 Chương Trước
        </button>
        
        <RouterLink :to="`/truyen/${novel.slug}`" class="nav-btn menu-btn">
          📂 Danh Sách
        </RouterLink>
        
        <button 
          class="nav-btn next-btn" 
          :disabled="!hasNext" 
          @click="navigateToChapter(chapterId + 1)"
        >
          Chương Tiếp 🡆
        </button>
      </div>
    </main>

    <!-- Floating Actions Panel (Autoscroll, Scroll to Top) -->
    <div class="floating-controls">
      <button 
        class="float-btn auto-scroll-btn" 
        :class="{ active: isAutoScrolling }" 
        @click="toggleAutoScroll"
        :title="isAutoScrolling ? 'Dừng Cuộn Tự Động' : 'Tự Động Cuộn'"
      >
        <span v-if="isAutoScrolling">⏹️</span>
        <span v-else>▶️</span>
        <span class="lbl">Cuộn</span>
      </button>

      <div class="scroll-speed-controls" v-if="isAutoScrolling">
        <button class="spd-btn" :class="{ active: scrollSpeed === 1 }" @click="scrollSpeed = 1; startAutoScroll();">Chậm</button>
        <button class="spd-btn" :class="{ active: scrollSpeed === 2 }" @click="scrollSpeed = 2; startAutoScroll();">Vừa</button>
        <button class="spd-btn" :class="{ active: scrollSpeed === 3 }" @click="scrollSpeed = 3; startAutoScroll();">Nhanh</button>
      </div>

      <button class="float-btn top-btn" @click="scrollToTop" title="Lên Đầu Trang">
        ▲
      </button>
    </div>
  </div>
  <div v-else class="reader-empty reader-theme-dark container">
    Truyện hoặc Chương này hiện tại chưa được cập nhật...
  </div>
</template>

<style scoped>
/* Theme colors configurations */
.reader-container {
  min-height: 100vh;
  padding: 100px 0 60px 0;
  transition: background-color 0.4s ease, color 0.3s ease;
}

/* Color palettes */
.reader-theme-beige {
  background-color: #f4ecd8 !important;
  color: #2c1d11 !important;
}

.reader-theme-dark {
  background-color: #121824 !important;
  color: #c2c7d0 !important;
}

.reader-theme-white {
  background-color: #ffffff !important;
  color: #181818 !important;
}

.reader-theme-green {
  background-color: #e8f5e9 !important;
  color: #1b3d22 !important;
}

/* Header bar styling */
.reader-header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 60px;
  display: flex;
  align-items: center;
  z-index: 90;
  border-radius: 0;
  border-top: none;
  border-left: none;
  border-right: none;
}

/* In beige / green theme make sure header adapts visually */
.reader-theme-beige .reader-header {
  background: rgba(244, 236, 216, 0.95);
  border-color: rgba(44, 29, 17, 0.15);
}
.reader-theme-green .reader-header {
  background: rgba(232, 245, 233, 0.95);
  border-color: rgba(27, 61, 34, 0.15);
}

.header-inner {
  display: flex;
  align-items: center;
  gap: 16px;
}

.btn-nav {
  font-size: 13.5px;
  font-weight: 700;
  color: inherit;
  padding: 6px 12px;
  border-radius: 6px;
}

.btn-nav:hover {
  background: rgba(0, 0, 0, 0.05);
}

.reader-theme-dark .btn-nav:hover {
  background: rgba(255, 255, 255, 0.05);
}

.sep {
  opacity: 0.3;
}

.novel-title-link {
  font-family: var(--font-heading);
  font-weight: 900;
  font-size: 16px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 300px;
  color: inherit;
}

.chapter-selector-wrapper {
  flex-grow: 1;
  display: flex;
  justify-content: center;
}

.chapter-select {
  padding: 6px 14px;
  border-radius: 6px;
  border: 1px solid rgba(0, 0, 0, 0.15);
  background: var(--bg-secondary);
  font-size: 13.5px;
  font-weight: 700;
  max-width: 280px;
  cursor: pointer;
  color: var(--text-primary);
}

.reader-theme-beige .chapter-select {
  background: #fdfaf2;
  border-color: rgba(44, 29, 17, 0.2);
  color: #2c1d11;
}

.reader-theme-green .chapter-select {
  background: #f1f8e9;
  border-color: rgba(27, 61, 34, 0.2);
  color: #1b3d22;
}

/* Settings drawer */
.settings-drawer {
  position: fixed;
  top: 70px;
  right: 20px;
  width: 320px;
  padding: 20px;
  z-index: 100;
  border-radius: var(--border-radius-md);
}

.reader-theme-beige .settings-drawer {
  background: #fdfaf2;
  color: #2c1d11;
  border-color: rgba(44, 29, 17, 0.2);
}

.reader-theme-green .settings-drawer {
  background: #f1f8e9;
  color: #1b3d22;
  border-color: rgba(27, 61, 34, 0.2);
}

.drawer-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
  border-bottom: 1px solid rgba(0, 0, 0, 0.1);
  padding-bottom: 8px;
}

.reader-theme-dark .drawer-header {
  border-color: rgba(255, 255, 255, 0.1);
}

.drawer-header h4 {
  font-family: var(--font-heading);
  font-size: 15px;
  font-weight: 800;
}

.close-btn {
  font-size: 16px;
  opacity: 0.7;
}

.close-btn:hover {
  opacity: 1;
}

.drawer-body {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.setting-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.setting-label {
  font-size: 12.5px;
  font-weight: 700;
  opacity: 0.85;
}

/* Themes Grid picker */
.themes-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 6px;
}

.theme-pick {
  font-size: 11px;
  font-weight: 700;
  height: 34px;
  border-radius: 6px;
  border: 1px solid rgba(0, 0, 0, 0.15);
  display: flex;
  align-items: center;
  justify-content: center;
}

.theme-pick.active {
  border-color: var(--accent-cyan);
  border-width: 2px;
  box-shadow: var(--shadow-sm);
}

.theme-beige { background: #f4ecd8; color: #2c1d11; }
.theme-dark { background: #121824; color: #c2c7d0; }
.theme-white { background: #ffffff; color: #181818; }
.theme-green { background: #e8f5e9; color: #1b3d22; }

/* Control adjustments */
.flex-row {
  display: flex;
  align-items: center;
}

.gap-6 {
  gap: 6px;
}

.adj-btn {
  flex: 1;
  height: 36px;
  border-radius: 6px;
  background: rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(0, 0, 0, 0.1);
  font-weight: 700;
  font-size: 14px;
}

.reader-theme-dark .adj-btn {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.1);
}

.select-field {
  padding: 8px 12px;
  border-radius: 6px;
  background: rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(0, 0, 0, 0.1);
  font-size: 13px;
  font-weight: 700;
  color: inherit;
}

.reader-theme-dark .select-field {
  background: #111827;
  border-color: rgba(255, 255, 255, 0.1);
}

.chip-btn {
  flex: 1;
  height: 36px;
  border-radius: 6px;
  background: rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(0, 0, 0, 0.1);
  font-size: 12px;
  font-weight: 700;
  transition: var(--transition-smooth);
}

.chip-btn.active {
  background: var(--accent-cyan);
  color: #090d16;
  border-color: var(--accent-cyan);
}

.reader-theme-dark .chip-btn {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.1);
}

/* Main Text reading styles */
.reader-main {
  width: 100%;
  margin: 0 auto;
  padding: 0 20px;
}

.chapter-intro {
  text-align: center;
  margin-bottom: 40px;
}

.novel-link {
  font-family: var(--font-heading);
  font-size: 16px;
  font-weight: 700;
  opacity: 0.7;
  margin-bottom: 8px;
}

.chapter-headline {
  font-family: var(--font-heading);
  font-size: 28px;
  font-weight: 900;
  line-height: 1.3;
  margin-bottom: 12px;
}

.chapter-meta {
  font-size: 12px;
  opacity: 0.6;
}

.vip-warning {
  padding: 20px;
  border-radius: var(--border-radius-md);
  margin-bottom: 30px;
  text-align: center;
  border: 1px dashed rgba(0, 0, 0, 0.15);
}

.reader-theme-beige .vip-warning {
  border-color: rgba(44, 29, 17, 0.3);
}
.reader-theme-dark .vip-warning {
  border-color: rgba(255, 255, 255, 0.15);
}

.vip-icon {
  font-size: 24px;
  margin-bottom: 8px;
}

.vip-warning h4 {
  font-size: 15px;
  font-weight: 800;
  margin-bottom: 4px;
}

.vip-warning p {
  font-size: 13px;
  opacity: 0.8;
}

.chapter-content-body {
  text-align: justify;
  margin-bottom: 60px;
  transition: font-size 0.2s ease;
}

.chapter-content-body p {
  margin-bottom: 24px;
  text-indent: 24px;
}

/* Chapter Navigation footer */
.chapter-footer-nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-top: 1px solid rgba(0, 0, 0, 0.1);
  padding-top: 30px;
  margin-top: 40px;
}

.reader-theme-dark .chapter-footer-nav {
  border-color: rgba(255, 255, 255, 0.1);
}

.nav-btn {
  font-size: 13.5px;
  font-weight: 800;
  padding: 10px 24px;
  border-radius: 30px;
  border: 1px solid rgba(0, 0, 0, 0.15);
  background: rgba(0, 0, 0, 0.05);
  color: inherit;
  transition: var(--transition-smooth);
}

.nav-btn:hover:not(:disabled) {
  background: rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.nav-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.reader-theme-dark .nav-btn {
  border-color: rgba(255, 255, 255, 0.15);
  background: rgba(255, 255, 255, 0.05);
}

.reader-theme-dark .nav-btn:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.1);
}

.menu-btn {
  border-color: var(--accent-cyan);
}

/* Floating controls */
.floating-controls {
  position: fixed;
  bottom: 30px;
  right: 30px;
  display: flex;
  flex-direction: column;
  gap: 10px;
  z-index: 100;
}

.float-btn {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: var(--bg-glass);
  backdrop-filter: blur(8px);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-md);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  transition: var(--transition-smooth);
  color: var(--text-primary);
}

.float-btn:hover {
  transform: scale(1.08);
  border-color: var(--accent-cyan);
}

.float-btn .lbl {
  font-size: 9px;
  font-weight: 700;
  margin-top: 2px;
}

.auto-scroll-btn.active {
  background: var(--accent-cyan);
  color: #090d16;
  border-color: var(--accent-cyan);
  box-shadow: var(--glow-cyan);
}

.scroll-speed-controls {
  position: absolute;
  right: 60px;
  bottom: 60px;
  background: var(--bg-glass);
  backdrop-filter: blur(8px);
  border: 1px solid var(--border-color);
  padding: 6px;
  border-radius: 20px;
  display: flex;
  gap: 4px;
  box-shadow: var(--shadow-md);
}

.spd-btn {
  font-size: 10px;
  font-weight: 700;
  padding: 4px 10px;
  border-radius: 12px;
  color: var(--text-secondary);
}

.spd-btn.active {
  background: var(--accent-cyan);
  color: #090d16;
}

.reader-empty {
  text-align: center;
  padding: 120px 20px;
  font-size: 16px;
}

/* Animations sliding */
.slide-panel-enter-active, .slide-panel-leave-active {
  transition: opacity 0.3s, transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.slide-panel-enter-from, .slide-panel-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}
</style>
