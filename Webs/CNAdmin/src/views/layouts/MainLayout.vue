<script setup lang="ts">
import { ref } from 'vue';
import Header from './Header.vue';
import Sidebar from './Sidebar.vue';

const sidebarWidth = ref(240);

const handleOnSidebarSizeChanged = (size: number) => {
    sidebarWidth.value = size;
};
</script>

<template>
    <div class="app-layout">
        <!-- Sidebar -->
        <Sidebar class="sidebar-aside" :style="{ width: sidebarWidth + 'px', transition: 'width 0.2s ease' }" @sizechanged="handleOnSidebarSizeChanged"/>
        
        <!-- Main Wrapper -->
        <div class="main-wrapper" :style="{ marginLeft: sidebarWidth + 'px', transition: 'margin-left 0.2s ease' }">
            <!-- Header -->
            <Header class="main-header" />
            
            <!-- Page Content -->
            <main class="page-content p-3">
                <RouterView />
            </main>
        </div>
    </div>
</template>

<style scoped>
.app-layout {
    min-height: 100vh;
    background-color: #f5f7fa;
    position: relative;
}

.sidebar-aside {
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    z-index: 100;
    height: 100vh;
}

.main-wrapper {
    height: 100vh;
    display: flex;
    flex-direction: column;
    overflow: hidden;
}

.main-header {
    flex-shrink: 0;
    z-index: 99;
    background-color: #ffffff;
}

.page-content {
    flex-grow: 1;
    overflow-y: auto;
}
</style>