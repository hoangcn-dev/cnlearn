<template>
  <a-modal
    :open="open"
    :title="title"
    width="800px"
    ok-text="Chèn Công Thức"
    cancel-text="Hủy Bỏ"
    destroy-on-close
    @ok="handleConfirm"
    @cancel="handleCancel"
  >
    <div class="py-2">
      <!-- Lựa chọn định dạng chèn -->
      <div class="mb-3 d-flex align-items-center gap-4">
        <span class="fw-semibold text-secondary small">ĐỊNH DẠNG ĐẦU RA:</span>
        <a-radio-group v-model:value="outputFormat" button-style="solid">
          <a-radio-button value="inline">Công thức dòng ($...$)</a-radio-button>
          <a-radio-button value="block">Công thức khối ($$...$$)</a-radio-button>
          <a-radio-button value="raw">Chỉ lấy mã LaTeX thô</a-radio-button>
        </a-radio-group>
      </div>

      <div class="row g-3">
        <!-- Trình soạn thảo MathLive -->
        <div class="col-md-6">
          <label class="form-label text-secondary small fw-semibold">BỘ SOẠN THẢO TRỰC QUAN:</label>
          <FormulaEditor 
            v-model="latexValue" 
            placeholder="Nhập công thức tại đây..."
          />
        </div>

        <!-- Trình xem trước KaTeX -->
        <div class="col-md-6">
          <label class="form-label text-secondary small fw-semibold">XEM TRƯỚC HIỂN THỊ:</label>
          <div class="preview-container p-3 border border-light rounded-3 bg-light-soft h-100 overflow-auto" style="min-height: 180px;">
            <FormulaRenderer :content="wrappedContent" />
          </div>
        </div>
      </div>

      <!-- Preview code -->
      <div class="mt-3 p-3 bg-light rounded-3 border border-light-subtle">
        <span class="text-secondary small fw-semibold d-block mb-1">MÃ SẼ ĐƯỢC CHÈN VÀO VĂN BẢN:</span>
        <code class="text-indigo break-all font-monospace fw-bold">{{ finalInsertedCode }}</code>
      </div>
    </div>
  </a-modal>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import FormulaEditor from './FormulaEditor.vue'
import FormulaRenderer from './FormulaRenderer.vue'

interface Props {
  open: boolean
  title?: string
  initialValue?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: '🧮 Trình Soạn Thảo Công Thức',
  initialValue: ''
})

const emit = defineEmits<{
  (e: 'update:open', val: boolean): void
  (e: 'ok', value: string): void
  (e: 'cancel'): void
}>()

const latexValue = ref('')
const outputFormat = ref<'inline' | 'block' | 'raw'>('inline')

// Đồng bộ hóa giá trị ban đầu khi mở Modal
watch(() => props.open, (isOpen) => {
  if (isOpen) {
    // Trích xuất mã LaTeX thô từ initialValue (nếu có dấu đô-la)
    let val = props.initialValue || ''
    if (val.startsWith('$$') && val.endsWith('$$')) {
      latexValue.value = val.slice(2, -2).trim()
      outputFormat.value = 'block'
    } else if (val.startsWith('$') && val.endsWith('$')) {
      latexValue.value = val.slice(1, -1).trim()
      outputFormat.value = 'inline'
    } else {
      latexValue.value = val
      outputFormat.value = 'raw'
    }
  }
})

// Bọc công thức để render xem trước đúng định dạng
const wrappedContent = computed(() => {
  if (!latexValue.value) return '*(Trống)*'
  if (outputFormat.value === 'inline') {
    return `$${latexValue.value}$`
  } else if (outputFormat.value === 'block') {
    return `$$${latexValue.value}$$`
  }
  return latexValue.value
})

// Chuỗi code đầu ra cuối cùng
const finalInsertedCode = computed(() => {
  if (outputFormat.value === 'inline') {
    return `$${latexValue.value}$`
  } else if (outputFormat.value === 'block') {
    return `$$${latexValue.value}$$`
  }
  return latexValue.value
})

const handleConfirm = () => {
  emit('ok', finalInsertedCode.value)
  emit('update:open', false)
}

const handleCancel = () => {
  emit('cancel')
  emit('update:open', false)
}
</script>

<style scoped>
.bg-light-soft {
  background-color: #f8fafc;
}
.preview-container {
  border-style: dashed !important;
  border-width: 2px !important;
}
.break-all {
  word-break: break-all;
}
</style>
