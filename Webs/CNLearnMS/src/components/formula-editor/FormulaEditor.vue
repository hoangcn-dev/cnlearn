<template>
  <div class="formula-editor-container border border-2 rounded-3 p-3 bg-white">
    <!-- Phím tắt chèn nhanh công thức -->
    <div class="shortcut-toolbar d-flex flex-wrap gap-1 mb-3 pb-2 border-bottom border-light">
      <button 
        v-for="btn in shortcutButtons" 
        :key="btn.label" 
        class="btn btn-light btn-sm border border-light-subtle d-flex align-items-center justify-content-center"
        style="min-width: 32px; height: 32px; font-size: 0.85rem;"
        type="button"
        :title="btn.title"
        @click="insertTemplate(btn.template)"
      >
        <span v-html="btn.htmlLabel"></span>
      </button>
    </div>

    <!-- Vùng chứa trình gõ Mathfield (MathfieldElement) -->
    <div ref="mathfieldContainer" class="mathfield-wrapper"></div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { MathfieldElement } from 'mathlive'

interface Props {
  modelValue: string
  placeholder?: string
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Nhập công thức...'
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}>()

const mathfieldContainer = ref<HTMLDivElement | null>(null)
let mfe: MathfieldElement | null = null

// Định nghĩa các nút phím tắt chèn nhanh
const shortcutButtons = [
  { label: 'Phân số', htmlLabel: '½', template: '\\frac{#0}{#1}', title: 'Phân số (Fraction)' },
  { label: 'Căn bậc 2', htmlLabel: '√', template: '\\sqrt{#0}', title: 'Căn bậc 2 (Square Root)' },
  { label: 'Số mũ', htmlLabel: 'x²', template: '{#0}^{#1}', title: 'Mũ (Power)' },
  { label: 'Chỉ số dưới', htmlLabel: 'x₂', template: '{#0}_{#1}', title: 'Chỉ số dưới (Subscript)' },
  { label: 'Tích phân', htmlLabel: '∫', template: '\\int_{#0}^{#1} #2 \\,dx', title: 'Tích phân (Integral)' },
  { label: 'Tổng sigma', htmlLabel: '∑', template: '\\sum_{#0}^{#1} #2', title: 'Tổng sigma (Summation)' },
  { label: 'Giới hạn', htmlLabel: 'lim', template: '\\lim_{#0 \\to #1} #2', title: 'Giới hạn (Limit)' },
  { label: 'Mũi tên hóa học', htmlLabel: '→', template: '\\xrightarrow[#1]{#0}', title: 'Phản ứng hóa học (Chemical reaction)' },
  { label: 'Khí bay lên', htmlLabel: '↑', template: '\\uparrow', title: 'Khí thoát ra (Gas)' },
  { label: 'Kết tủa', htmlLabel: '↓', template: '\\downarrow', title: 'Kết tủa (Precipitate)' },
  { label: 'Độ C', htmlLabel: '°C', template: 't^\\circ', title: 'Nhiệt độ (Degree Celsius)' }
]

onMounted(() => {
  if (!mathfieldContainer.value) return

  // Khởi tạo MathfieldElement
  mfe = new MathfieldElement()
  mfe.value = props.modelValue
  mfe.placeholder = props.placeholder
  mfe.style.width = '100%'
  mfe.style.fontSize = '1.2rem'
  mfe.style.padding = '8px'
  mfe.style.border = '1px solid #cbd5e1'
  mfe.style.borderRadius = '6px'
  mfe.style.outline = 'none'

  // Lắng nghe sự kiện thay đổi dữ liệu
  mfe.addEventListener('input', () => {
    if (mfe) {
      const val = mfe.value
      emit('update:modelValue', val)
      emit('change', val)
    }
  })

  mathfieldContainer.value.appendChild(mfe)
})

// Đồng bộ hóa khi modelValue thay đổi từ bên ngoài (ví dụ load preset)
watch(() => props.modelValue, (newVal) => {
  if (mfe && mfe.value !== newVal) {
    mfe.value = newVal
  }
})

// Chèn mẫu công thức tại vị trí con trỏ hiện tại
const insertTemplate = (template: string) => {
  if (mfe) {
    mfe.insert(template)
    mfe.focus()
  }
}
</script>

<style scoped>
.shortcut-toolbar button {
  background-color: #f8fafc;
  color: #334155;
  transition: all 0.2s ease;
}
.shortcut-toolbar button:hover {
  background-color: #e2e8f0;
  border-color: #cbd5e1 !important;
}
.mathfield-wrapper :deep(math-field:focus-within) {
  border-color: #6366f1 !important;
  box-shadow: 0 0 0 2px rgba(99, 102, 241, 0.2);
}
</style>
