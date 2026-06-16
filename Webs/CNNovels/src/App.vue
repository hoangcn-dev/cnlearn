<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from './components/Navbar.vue'
import Footer from './components/Footer.vue'
</script>

<template>
  <div class="app-layout">
    <!-- Premium Navbar -->
    <Navbar />

    <!-- View Content Wrapper -->
    <div class="main-content">
      <RouterView />
    </div>

    <!-- Responsive Footer -->
    <Footer />
  </div>
</template>

<style>
/* App Layout Container */
.app-layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  width: 100%;
}
</style>
