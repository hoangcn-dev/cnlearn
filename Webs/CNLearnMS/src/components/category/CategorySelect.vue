<template>
  <a-select
    v-bind="$attrs"
    v-model:value="computedValue"
    :placeholder="placeholder"
    :allow-clear="allowClear"
    show-search
    option-filter-prop="label"
    option-label-prop="label"
    class="category-select"
  >
    <a-select-option v-if="showAllOption" value="" label="Tất cả danh mục">
      <span class="fw-bold text-dark">📂 Tất cả danh mục</span>
    </a-select-option>
    <a-select-option 
      v-for="opt in options" 
      :key="opt.value" 
      :value="opt.value"
      :label="opt.rawName"
    >
      <span :style="{ paddingLeft: `${opt.level * 12}px` }" class="option-label">
        <span v-if="opt.level > 0" class="indent-connector">└─ </span>
        <span :class="{ 'fw-bold': opt.level === 0, 'text-secondary': opt.level > 0 }">
          {{ getLocalName(opt.rawName) }}
        </span>
      </span>
    </a-select-option>
  </a-select>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { buildIndentedOptions, type QuestionCategory } from './categoryHelper'

const props = withDefaults(defineProps<{
  value: string
  categories: QuestionCategory[]
  placeholder?: string
  allowClear?: boolean
  showAllOption?: boolean
}>(), {
  placeholder: 'Chọn danh mục...',
  allowClear: true,
  showAllOption: false
})

const emit = defineEmits<{
  (e: 'update:value', val: string): void
}>()

const computedValue = computed({
  get: () => props.value,
  set: (val) => emit('update:value', val || '')
})

const options = computed(() => {
  return buildIndentedOptions(props.categories)
})

const getLocalName = (fullName: string) => {
  const parts = fullName.split(' - ')
  return parts[parts.length - 1] || fullName
}
</script>

<style scoped>
.option-label {
  display: inline-block;
  white-space: pre;
}

.indent-connector {
  color: #a1a1aa;
  margin-right: 4px;
}
</style>
