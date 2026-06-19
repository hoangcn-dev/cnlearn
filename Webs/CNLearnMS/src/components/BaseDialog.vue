<template>
  <a-modal
    v-model:open="internalVisible"
    :title="title"
    :width="width"
    :centered="true"
    :closable="closable"
    :maskClosable="maskClosable"
    class="premium-modal base-dialog"
    @cancel="handleCancel"
  >
    <div class="py-3">
      <slot></slot>
    </div>
    
    <template #footer>
      <slot name="footer">
        <div class="d-flex justify-content-end gap-2 pt-3 border-top border-light">
          <button v-if="showCancel" class="btn btn-outline-secondary px-4 py-1.5 rounded-3 fw-semibold small" @click="handleCancel">
            {{ cancelText }}
          </button>
          <button v-if="showOk" class="btn text-white px-4 py-1.5 rounded-3 fw-semibold small d-flex align-items-center gap-2" :class="buttonType === 'danger' ? 'btn-danger' : 'btn-indigo'" :disabled="okLoading" @click="handleOk">
            <span v-if="okLoading" class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            {{ okText }}
          </button>
        </div>
      </slot>
    </template>
  </a-modal>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: 'Thông báo'
  },
  width: {
    type: [String, Number],
    default: 500
  },
  closable: {
    type: Boolean,
    default: true
  },
  maskClosable: {
    type: Boolean,
    default: true
  },
  showCancel: {
    type: Boolean,
    default: true
  },
  showOk: {
    type: Boolean,
    default: true
  },
  cancelText: {
    type: String,
    default: 'Hủy'
  },
  okText: {
    type: String,
    default: 'Xác nhận'
  },
  okLoading: {
    type: Boolean,
    default: false
  },
  buttonType: {
    type: String,
    default: 'indigo'
  }
})

const emit = defineEmits(['update:visible', 'update:open', 'cancel', 'ok'])

const internalVisible = computed({
  get: () => props.visible,
  set: (val) => {
    emit('update:visible', val)
    emit('update:open', val)
  }
})

const handleCancel = () => {
  emit('update:visible', false)
  emit('cancel')
}

const handleOk = () => {
  emit('ok')
}
</script>

<style scoped>
.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  border: none;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
}

.base-dialog .ant-modal-content {
  border-radius: 12px;
  overflow: hidden;
}

.base-dialog .ant-modal-header {
  border-bottom: 1px solid #f3f4f6;
  padding: 16px 24px;
}

.base-dialog .ant-modal-title {
  font-weight: 700;
  color: #1e1b4b;
}
</style>
