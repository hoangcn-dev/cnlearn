<template>
  <div class="category-child-grid">
    <div class="row g-4">
      <div v-for="cat in children" :key="cat.questionCategoryId" class="col-xl-4 col-md-6 col-12">
        <div 
          class="card border-0 rounded-4 shadow-sm h-100 hover-up bg-white p-4 d-flex flex-column justify-content-between cursor-pointer"
          @click="selectCategory(cat)"
        >
          <div>
            <div class="icon-box mb-3 d-flex align-items-center justify-content-center">
              <span>📁</span>
            </div>
            <h3 class="h5 fw-bold text-dark-blue mb-2 card-title">{{ getLocalName(cat.name) }}</h3>
            <p class="text-secondary small mb-0 desc-text">
              Khám phá các đề thi thử và tài liệu thuộc phần {{ getLocalName(cat.name) }}.
            </p>
          </div>
          <div class="d-flex align-items-center justify-content-between border-top pt-3 mt-4 text-indigo">
            <span class="small fw-semibold">Khám phá ngay</span>
            <span class="arrow-indicator">➔</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import type { QuestionCategory } from './categoryHelper'

const router = useRouter()

const props = defineProps<{
  children: QuestionCategory[]
}>()

const getLocalName = (fullName: string) => {
  const parts = fullName.split(' - ')
  return parts[parts.length - 1] || fullName
}

const selectCategory = (cat: QuestionCategory) => {
  router.push({
    name: 'quiz-list',
    params: { id: cat.questionCategoryId },
    query: { name: cat.name }
  })
}
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.text-indigo {
  color: #4f46e5;
}

.icon-box {
  width: 52px;
  height: 52px;
  background-color: rgba(99, 102, 241, 0.08);
  border-radius: 14px;
  font-size: 1.6rem;
  transition: all 0.3s ease;
}

.card:hover .icon-box {
  background-color: #6366f1;
  color: white;
  transform: scale(1.05);
}

.card {
  transition: all 0.3s ease;
  border: 1px solid rgba(0, 0, 0, 0.04) !important;
}

.hover-up:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 25px rgba(99, 102, 241, 0.08) !important;
  border-color: #c7d2fe !important;
}

.card-title {
  transition: color 0.2s ease;
}

.card:hover .card-title {
  color: #4f46e5;
}

.desc-text {
  line-height: 1.5;
}

.arrow-indicator {
  transition: transform 0.2s ease;
}

.card:hover .arrow-indicator {
  transform: translateX(4px);
}
</style>
