<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { mockNovels, GENRES } from '../data/mockNovels';

const router = useRouter();

const searchReady = ref(false);
const searchQuery = ref('');
const showSearchSuggest = ref(false);
const theme = ref('dark'); // Default to dark mode for premium look
const showHistory = ref(false);
const historyList = ref<any[]>([]);
const favoritesList = ref<any[]>([]);
const showFavorites = ref(false);

// Cultivation Rank Mock system
const currentRank = ref('Luyện Khí Kỳ');
const isLoggedIn = ref(false);
const showAuthModal = ref(false);
const usernameInput = ref('');
const passwordInput = ref('');

// Computed filtered novels for suggest box
const searchResults = computed(() => {
  if (!searchQuery.value.trim()) return [];
  const q = searchQuery.value.toLowerCase().trim();
  return mockNovels.filter(
    n => n.title.toLowerCase().includes(q) || n.author.toLowerCase().includes(q)
  ).slice(0, 5);
});

// Toggle Theme
const toggleTheme = () => {
  theme.value = theme.value === 'dark' ? 'light' : 'dark';
  document.documentElement.setAttribute('data-theme', theme.value);
  localStorage.setItem('cnnovels_theme', theme.value);
};

// Open Novel Details from Search
const selectNovel = (slug: string) => {
  searchQuery.value = '';
  showSearchSuggest.value = false;
  router.push(`/truyen/${slug}`);
};

// Load storage utilities
const loadHistory = () => {
  try {
    const raw = localStorage.getItem('cnnovels_history');
    if (raw) {
      const items = JSON.parse(raw);
      // Map with actual novel data
      historyList.value = items.map((h: any) => {
        const novel = mockNovels.find(n => n.id === h.novelId);
        return {
          ...h,
          novelTitle: novel ? novel.title : h.novelId,
          novelCover: novel ? novel.cover : '',
          novelSlug: novel ? novel.slug : ''
        };
      }).slice(0, 6);
    } else {
      historyList.value = [];
    }
  } catch (e) {
    console.error(e);
  }
};

const loadFavorites = () => {
  try {
    const raw = localStorage.getItem('cnnovels_bookmarks');
    if (raw) {
      const bookmarkedSlugs = JSON.parse(raw);
      favoritesList.value = mockNovels.filter(n => bookmarkedSlugs.includes(n.id));
    } else {
      favoritesList.value = [];
    }
  } catch (e) {
    console.error(e);
  }
};

// Fake Login
const handleLogin = () => {
  if (usernameInput.value.trim()) {
    isLoggedIn.value = true;
    showAuthModal.value = false;
    // Random cultivation level
    const ranks = ['Hóa Thần Kỳ', 'Trúc Cơ Kỳ', 'Nguyên Anh Kỳ', 'Kim Đan Kỳ', 'Đại Thừa Kỳ'];
    currentRank.value = ranks[Math.floor(Math.random() * ranks.length)] || 'Hóa Thần Kỳ';
    localStorage.setItem('cnnovels_user', JSON.stringify({
      username: usernameInput.value,
      rank: currentRank.value
    }));
  }
};

const handleLogout = () => {
  isLoggedIn.value = false;
  localStorage.removeItem('cnnovels_user');
};

onMounted(() => {
  // Set theme from storage
  const savedTheme = localStorage.getItem('cnnovels_theme');
  if (savedTheme) {
    theme.value = savedTheme;
  }
  document.documentElement.setAttribute('data-theme', theme.value);

  // Load history & favorites
  loadHistory();
  loadFavorites();

  // Load logged in user
  const savedUser = localStorage.getItem('cnnovels_user');
  if (savedUser) {
    try {
      const u = JSON.parse(savedUser);
      isLoggedIn.value = true;
      usernameInput.value = u.username;
      currentRank.value = u.rank;
    } catch (e) {}
  }

  // Set up storage listeners
  window.addEventListener('storage', () => {
    loadHistory();
    loadFavorites();
  });

  // Global click listener to close suggestions and menus
  document.addEventListener('click', (e: MouseEvent) => {
    const target = e.target as HTMLElement;
    if (!target.closest('.search-container')) {
      showSearchSuggest.value = false;
    }
    if (!target.closest('.history-dropdown')) {
      showHistory.value = false;
    }
    if (!target.closest('.fav-dropdown')) {
      showFavorites.value = false;
    }
  });
});

// Expose refresh functionality for history and bookmarks on route change
watch(() => router.currentRoute.value.path, () => {
  loadHistory();
  loadFavorites();
});
</script>

<template>
  <nav class="navbar glass-panel">
    <div class="navbar-container container">
      <!-- Logo -->
      <RouterLink to="/" class="logo-wrapper">
        <span class="logo-icon">📖</span>
        <span class="logo-text">CN<span class="accent">Novels</span></span>
      </RouterLink>

      <!-- Menu links -->
      <div class="nav-menu">
        <div class="nav-item dropdown">
          <button class="nav-btn">
            Thể loại 
            <svg class="chevron" viewBox="0 0 24 24" width="14" height="14">
              <path d="M7 10l5 5 5-5H7z" fill="currentColor"/>
            </svg>
          </button>
          <div class="dropdown-menu genres-grid glass-panel">
            <RouterLink 
              v-for="genre in GENRES" 
              :key="genre" 
              :to="{ path: '/', query: { genre } }" 
              class="genre-item"
            >
              {{ genre }}
            </RouterLink>
          </div>
        </div>

        <RouterLink to="/" class="nav-link">Bảng Xếp Hạng</RouterLink>
      </div>

      <!-- Search Bar -->
      <div class="search-container">
        <div class="search-box">
          <input 
            type="text" 
            placeholder="Tìm tên truyện, tác giả..." 
            v-model="searchQuery"
            @focus="showSearchSuggest = true"
            @keyup.enter="searchResults.length > 0 && searchResults[0] && selectNovel(searchResults[0].slug)"
          />
          <button class="search-icon-btn">
            <svg viewBox="0 0 24 24" width="18" height="18">
              <path fill="currentColor" d="M15.5 14h-.79l-.28-.27A6.471 6.471 0 0 0 16 9.5 6.5 6.5 0 1 0 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/>
            </svg>
          </button>
        </div>

        <!-- Suggest Dropdown -->
        <Transition name="fade-slide">
          <div class="search-suggest glass-panel" v-if="showSearchSuggest && searchQuery.trim() !== ''">
            <div v-if="searchResults.length > 0" class="suggest-list">
              <div 
                v-for="novel in searchResults" 
                :key="novel.id" 
                class="suggest-item"
                @click="selectNovel(novel.slug)"
              >
                <img :src="novel.cover" alt="" class="suggest-cover"/>
                <div class="suggest-info">
                  <span class="suggest-title">{{ novel.title }}</span>
                  <span class="suggest-author">{{ novel.author }}</span>
                </div>
              </div>
            </div>
            <div v-else class="suggest-empty">
              Không tìm thấy truyện nào...
            </div>
          </div>
        </Transition>
      </div>

      <!-- Actions (History, Bookmark, Theme, Cultivation Login) -->
      <div class="nav-actions">
        <!-- Bookmark Favorites -->
        <div class="action-item fav-dropdown">
          <button class="action-btn" @click.stop="showFavorites = !showFavorites; showHistory = false;">
            🔖 <span class="action-label">Tủ Truyện</span>
            <span class="badge-count" v-if="favoritesList.length > 0">{{ favoritesList.length }}</span>
          </button>

          <Transition name="fade-slide">
            <div class="action-dropdown glass-panel" v-if="showFavorites">
              <div class="dropdown-header">Truyện Yêu Thích</div>
              <div class="dropdown-list" v-if="favoritesList.length > 0">
                <RouterLink 
                  v-for="fav in favoritesList" 
                  :key="fav.id" 
                  :to="`/truyen/${fav.slug}`"
                  class="history-item"
                  @click="showFavorites = false"
                >
                  <img :src="fav.cover" alt="" class="history-cover"/>
                  <div class="history-info">
                    <span class="history-title">{{ fav.title }}</span>
                    <span class="history-chapter">{{ fav.status }}</span>
                  </div>
                </RouterLink>
              </div>
              <div class="dropdown-empty" v-else>
                Chưa lưu truyện nào.
              </div>
            </div>
          </Transition>
        </div>

        <!-- Reading History -->
        <div class="action-item history-dropdown">
          <button class="action-btn" @click.stop="showHistory = !showHistory; showFavorites = false;">
            ⏳ <span class="action-label">Lịch sử</span>
            <span class="badge-count count-blue" v-if="historyList.length > 0">{{ historyList.length }}</span>
          </button>

          <Transition name="fade-slide">
            <div class="action-dropdown glass-panel" v-if="showHistory">
              <div class="dropdown-header">Lịch Sử Đọc</div>
              <div class="dropdown-list" v-if="historyList.length > 0">
                <RouterLink 
                  v-for="hist in historyList" 
                  :key="hist.novelId" 
                  :to="`/truyen/${hist.novelSlug}/chuong-${hist.chapterId}`"
                  class="history-item"
                  @click="showHistory = false"
                >
                  <img :src="hist.novelCover" alt="" class="history-cover"/>
                  <div class="history-info">
                    <span class="history-title">{{ hist.novelTitle }}</span>
                    <span class="history-chapter">Đã đọc: Chương {{ hist.chapterId }}</span>
                  </div>
                </RouterLink>
              </div>
              <div class="dropdown-empty" v-else>
                Lịch sử trống.
              </div>
            </div>
          </Transition>
        </div>

        <!-- Theme Toggle -->
        <button class="theme-toggle action-btn" @click="toggleTheme" aria-label="Toggle Theme">
          <span v-if="theme === 'dark'">☀️</span>
          <span v-else>🌙</span>
        </button>

        <!-- User Authentication -->
        <div class="user-profile">
          <button v-if="isLoggedIn" class="user-avatar-btn" @click="handleLogout" title="Đăng xuất">
            <div class="avatar-ring">
              <span class="avatar-emoji">👤</span>
            </div>
            <div class="user-meta">
              <span class="user-name">{{ usernameInput }}</span>
              <span class="user-rank">{{ currentRank }}</span>
            </div>
          </button>
          <button v-else class="login-btn" @click="showAuthModal = true">
            Đăng Nhập
          </button>
        </div>
      </div>
    </div>
  </nav>

  <!-- Login Modal Mock -->
  <Transition name="fade">
    <div class="modal-overlay" v-if="showAuthModal" @click.self="showAuthModal = false">
      <div class="modal-content glass-panel animated-fade">
        <button class="modal-close-btn" @click="showAuthModal = false">✕</button>
        <h2>Đăng Nhập Đại Đạo</h2>
        <p class="modal-sub">Trở thành Đạo Hữu để đánh dấu truyện, tích lũy tu vi tăng cảnh giới!</p>

        <form @submit.prevent="handleLogin" class="auth-form">
          <div class="form-group">
            <label>Đạo danh (Tên tài khoản)</label>
            <input type="text" v-model="usernameInput" placeholder="Ví dụ: Hàn Lập Đạo Nhân" required />
          </div>
          <div class="form-group">
            <label>Mật khẩu mật pháp</label>
            <input type="password" v-model="passwordInput" placeholder="••••••••" required />
          </div>

          <button type="submit" class="submit-auth-btn">Khởi Động Thần Khí</button>
        </form>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.navbar {
  position: sticky;
  top: 0;
  z-index: 100;
  height: 70px;
  border-radius: 0 0 var(--border-radius-md) var(--border-radius-md);
  border-top: none;
  border-left: none;
  border-right: none;
  display: flex;
  align-items: center;
}

.navbar-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
}

.logo-wrapper {
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: var(--font-heading);
  font-size: 22px;
  font-weight: 900;
  color: var(--text-primary);
}

.logo-icon {
  font-size: 26px;
}

.logo-text .accent {
  color: var(--accent-cyan);
  text-shadow: var(--glow-cyan);
}

.nav-menu {
  display: flex;
  align-items: center;
  gap: 16px;
}

.nav-link, .nav-btn {
  font-weight: 600;
  font-size: 14px;
  color: var(--text-secondary);
  padding: 8px 12px;
  border-radius: var(--border-radius-sm);
  display: flex;
  align-items: center;
  gap: 4px;
}

.nav-link:hover, .nav-btn:hover {
  color: var(--text-primary);
  background: var(--bg-tertiary);
}

.chevron {
  transition: var(--transition-smooth);
}

.dropdown {
  position: relative;
}

.dropdown:hover .chevron {
  transform: rotate(180deg);
}

.dropdown:hover .dropdown-menu {
  opacity: 1;
  visibility: visible;
  transform: translateY(0);
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  margin-top: 8px;
  opacity: 0;
  visibility: hidden;
  transform: translateY(10px);
  transition: var(--transition-smooth);
  z-index: 10;
  padding: 16px;
  min-width: 320px;
}

.genres-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
}

.genre-item {
  font-size: 13px;
  color: var(--text-secondary);
  padding: 6px 10px;
  border-radius: var(--border-radius-sm);
  text-align: center;
  border: 1px solid transparent;
}

.genre-item:hover {
  background: var(--bg-primary);
  color: var(--accent-cyan);
  border-color: var(--border-color);
}

/* Search Box styling */
.search-container {
  flex: 1;
  max-width: 380px;
  position: relative;
}

.search-box {
  display: flex;
  align-items: center;
  background: var(--bg-tertiary);
  border: 1px solid var(--border-color);
  border-radius: 30px;
  padding: 4px 6px 4px 16px;
  transition: var(--transition-smooth);
}

.search-box:focus-within {
  border-color: var(--accent-cyan);
  background: var(--bg-secondary);
  box-shadow: 0 0 10px rgba(0, 229, 255, 0.15);
}

.search-box input {
  width: 100%;
  padding: 6px 0;
  font-size: 13px;
  border: none;
  background: transparent;
}

.search-icon-btn {
  padding: 8px;
  border-radius: 50%;
  color: var(--text-muted);
  display: flex;
  align-items: center;
  justify-content: center;
}

.search-box:focus-within .search-icon-btn {
  color: var(--accent-cyan);
}

.search-suggest {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  margin-top: 10px;
  max-height: 380px;
  overflow-y: auto;
  z-index: 10;
  padding: 8px;
}

.suggest-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px;
  border-radius: var(--border-radius-sm);
  cursor: pointer;
  transition: var(--transition-smooth);
}

.suggest-item:hover {
  background: var(--bg-tertiary);
}

.suggest-cover {
  width: 36px;
  height: 48px;
  object-fit: cover;
  border-radius: 4px;
}

.suggest-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.suggest-title {
  font-size: 13.5px;
  font-weight: 700;
  color: var(--text-primary);
}

.suggest-author {
  font-size: 11px;
  color: var(--text-secondary);
}

.suggest-empty {
  padding: 20px;
  text-align: center;
  color: var(--text-muted);
  font-size: 13px;
}

/* Actions */
.nav-actions {
  display: flex;
  align-items: center;
  gap: 12px;
}

.action-item {
  position: relative;
}

.action-btn {
  font-size: 14px;
  font-weight: 600;
  color: var(--text-secondary);
  padding: 8px 12px;
  border-radius: var(--border-radius-sm);
  display: flex;
  align-items: center;
  gap: 6px;
  position: relative;
}

.action-btn:hover {
  background: var(--bg-tertiary);
  color: var(--text-primary);
}

.badge-count {
  position: absolute;
  top: -2px;
  right: -2px;
  background: #ef4444;
  color: white;
  font-size: 9px;
  font-weight: 800;
  padding: 2px 5px;
  border-radius: 10px;
  min-width: 16px;
  text-align: center;
}

.count-blue {
  background: var(--accent-color);
}

.action-dropdown {
  position: absolute;
  top: 100%;
  right: 0;
  margin-top: 10px;
  width: 280px;
  padding: 8px;
  z-index: 10;
}

.dropdown-header {
  font-weight: 800;
  font-size: 13px;
  padding: 8px 10px;
  color: var(--text-primary);
  border-bottom: 1px solid var(--border-color);
  margin-bottom: 6px;
}

.dropdown-list {
  max-height: 280px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.history-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px;
  border-radius: var(--border-radius-sm);
}

.history-item:hover {
  background: var(--bg-tertiary);
}

.history-cover {
  width: 32px;
  height: 42px;
  object-fit: cover;
  border-radius: 4px;
}

.history-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
  overflow: hidden;
}

.history-title {
  font-size: 12.5px;
  font-weight: 700;
  color: var(--text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.history-chapter {
  font-size: 11px;
  color: var(--text-muted);
}

.dropdown-empty {
  padding: 20px;
  text-align: center;
  color: var(--text-muted);
  font-size: 12px;
}

/* User profile */
.user-profile {
  display: flex;
  align-items: center;
}

.login-btn {
  background: var(--accent-color);
  color: white;
  font-weight: 700;
  font-size: 13px;
  padding: 8px 16px;
  border-radius: 20px;
  transition: var(--transition-smooth);
}

.login-btn:hover {
  background: var(--accent-hover);
  box-shadow: 0 4px 12px rgba(79, 70, 229, 0.3);
}

.user-avatar-btn {
  display: flex;
  align-items: center;
  gap: 10px;
  text-align: left;
}

.avatar-ring {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  background: var(--bg-tertiary);
  border: 2px solid var(--accent-cyan);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: var(--glow-cyan);
}

.avatar-emoji {
  font-size: 18px;
}

.user-meta {
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 13px;
  font-weight: 700;
  color: var(--text-primary);
}

.user-rank {
  font-size: 10px;
  color: var(--accent-cyan);
  font-weight: 600;
}

/* Modal Styling */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(8px);
  z-index: 200;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-content {
  width: 100%;
  max-width: 400px;
  padding: 30px;
  position: relative;
}

.modal-close-btn {
  position: absolute;
  top: 16px;
  right: 16px;
  font-size: 18px;
  color: var(--text-muted);
}

.modal-close-btn:hover {
  color: var(--text-primary);
}

.modal-content h2 {
  font-family: var(--font-heading);
  font-size: 22px;
  margin-bottom: 6px;
  text-align: center;
}

.modal-sub {
  font-size: 13px;
  color: var(--text-secondary);
  margin-bottom: 24px;
  text-align: center;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group label {
  font-size: 12px;
  font-weight: 700;
  color: var(--text-secondary);
}

.form-group input {
  background: var(--bg-tertiary);
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius-sm);
  padding: 10px 14px;
  font-size: 14px;
  transition: var(--transition-smooth);
}

.form-group input:focus {
  border-color: var(--accent-cyan);
  background: var(--bg-secondary);
}

.submit-auth-btn {
  background: var(--accent-color);
  color: white;
  font-weight: 700;
  padding: 12px;
  border-radius: var(--border-radius-sm);
  margin-top: 10px;
  transition: var(--transition-smooth);
}

.submit-auth-btn:hover {
  background: var(--accent-hover);
  box-shadow: 0 4px 15px rgba(79, 70, 229, 0.4);
}

/* Animations transition */
.fade-slide-enter-active, .fade-slide-leave-active {
  transition: opacity 0.25s, transform 0.25s;
}

.fade-slide-enter-from, .fade-slide-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

.fade-enter-active, .fade-leave-active {
  transition: opacity 0.25s;
}

.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

/* Responsive adjustment */
@media (max-width: 768px) {
  .logo-text, .action-label, .user-meta {
    display: none;
  }
  .search-container {
    max-width: 140px;
  }
  .action-btn {
    padding: 8px;
  }
}
</style>
