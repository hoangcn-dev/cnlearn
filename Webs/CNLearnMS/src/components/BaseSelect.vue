<template>
  <a-select
    v-bind="$attrs"
    v-model:value="computedValue"
    :placeholder="placeholder"
    :allow-clear="allowClear"
    :disabled="disabled"
    :loading="loading"
    class="base-select"
  >
    <a-select-option
      v-for="opt in options"
      :key="opt.value"
      :value="opt.value"
    >
      <span class="d-flex align-items-center gap-2">
        <span v-if="opt.icon" class="option-icon">{{ opt.icon }}</span>
        <span>{{ opt.label }}</span>
      </span>
    </a-select-option>
  </a-select>
</template>

<script setup lang="ts">
import { computed } from 'vue'

export interface SelectOption {
  value: string | number
  label: string
  icon?: string
}

const props = withDefaults(defineProps<{
  value: any
  options: SelectOption[]
  placeholder?: string
  allowClear?: boolean
  disabled?: boolean
  loading?: boolean
}>(), {
  placeholder: 'Chọn...',
  allowClear: false,
  disabled: false,
  loading: false
})

const emit = defineEmits<{
  (e: 'update:value', val: any): void
}>()

const computedValue = computed({
  get: () => props.value,
  set: (val) => emit('update:value', val)
})
</script>

<style scoped>
.base-select {
  width: 100%;
}
.option-icon {
  font-size: 0.9rem;
}
</style>
