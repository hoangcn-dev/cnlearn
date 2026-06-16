<template>
  <div 
    class="question-card mb-4 border-0 rounded-4 shadow-sm bg-white overflow-hidden"
    :class="cardBorderClass"
  >
    <!-- Question Header -->
    <div class="px-4 py-3 bg-light border-bottom d-flex flex-wrap justify-content-between align-items-center gap-3">
      <div class="d-flex align-items-center gap-2">
        <a-tag color="blue" class="fw-bold px-3 py-1 fs-7 rounded-3 m-0">
          Câu {{ index }}
        </a-tag>
        <a-tag v-if="question.level !== undefined" :color="getLevelColor(question.level)" class="m-0 fw-semibold">
          {{ getLevelText(question.level) }}
        </a-tag>
        <a-tag v-if="isMultipleChoice" color="purple" class="m-0">
          Chọn nhiều đáp án
        </a-tag>
      </div>
      
      <!-- Result Badge (Result Mode) -->
      <div v-if="mode === 'result'" class="ms-auto">
        <a-tag :color="isCorrectResult ? 'success' : 'error'" class="m-0 px-3 py-1 fw-bold fs-7 rounded-pill">
          {{ isCorrectResult ? '✓ Trả lời Đúng' : '✗ Trả lời Sai' }}
        </a-tag>
      </div>
    </div>

    <!-- Question Content -->
    <div class="p-4">
      <div class="question-text mb-4 text-dark-blue fs-6 fw-semibold leading-relaxed">
        <FormulaRenderer :content="question.stringContent" />
      </div>

      <!-- Options Container -->
      <div :class="mode === 'result' ? 'row g-3 mb-4' : 'd-flex flex-column gap-3 mb-4'">
        <div 
          v-for="(ans, aIdx) in question.answers" 
          :key="ans.id || ans.questionAnswerId || aIdx" 
          :class="[
            mode === 'result' ? 'col-md-6' : '',
            'option-item-wrapper'
          ]"
        >
          <div 
            class="option-item-box p-3 rounded-3 border d-flex align-items-center gap-3 transition hover-pointer h-100"
            :class="getOptionBoxClass(ans, aIdx)"
            @click="handleSelect(ans, aIdx)"
          >
            <!-- Input Control (only interactive in practice mode and when not revealed) -->
            <div class="d-flex align-items-center flex-shrink-0" @click.stop>
              <a-checkbox 
                v-if="isMultipleChoice"
                :checked="isAnsChosen(ans, aIdx)"
                :disabled="isInputDisabled"
                @change="handleSelect(ans, aIdx)"
              />
              <a-radio 
                v-else
                :checked="isAnsChosen(ans, aIdx)"
                :disabled="isInputDisabled"
                @change="handleSelect(ans, aIdx)"
              />
            </div>

            <!-- Option Text -->
            <div class="option-content-text text-dark flex-grow-1 ps-1 mb-0 small">
              <span class="fw-bold text-dark-blue me-1">{{ getIndexChar(ans, aIdx) }}.</span>
              <FormulaRenderer :content="ans.stringContent" :inline="true" />
            </div>

            <!-- Badges (Result & Revealed Instant mode) -->
            <div class="d-flex flex-column align-items-end gap-1 flex-shrink-0">
              <!-- Result / Revealed Correct Answer -->
              <a-tag 
                v-if="shouldShowCorrectAnswers && ans.isCorrectAnswer" 
                color="success" 
                class="m-0 fs-9"
              >
                Đáp án đúng
              </a-tag>
              
              <!-- Result / Revealed Incorrect User Selection -->
              <a-tag 
                v-if="shouldShowIncorrectSelections && isAnsChosen(ans, aIdx) && !ans.isCorrectAnswer" 
                color="error" 
                class="m-0 fs-9"
              >
                {{ mode === 'result' ? 'Bạn chọn' : 'Sai' }}
              </a-tag>

              <!-- Result / Correct User Selection (just mark "Bạn chọn" since correct answer is already shown) -->
              <a-tag 
                v-if="mode === 'result' && isAnsChosen(ans, aIdx) && ans.isCorrectAnswer" 
                color="blue" 
                class="m-0 fs-9"
              >
                Bạn chọn
              </a-tag>
            </div>
          </div>
        </div>
      </div>

      <!-- Explanation Box -->
      <div 
        v-if="shouldShowExplanation && explanationContent"
        class="explanation-box p-4 rounded-4 border-start border-4 bg-light"
        style="border-left-color: var(--ant-primary-color, #4f46e5) !important;"
      >
        <div class="fw-bold text-indigo mb-2 d-flex align-items-center gap-2 fs-6">
          <span>💡</span> Hướng dẫn giải chi tiết:
        </div>
        <div class="text-secondary leading-relaxed fs-7">
          <FormulaRenderer :content="explanationContent" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import FormulaRenderer from './formula-editor/FormulaRenderer.vue'

interface Answer {
  id?: string
  questionAnswerId?: string
  stringContent: string
  isCorrectAnswer?: boolean
  indexChar?: string
}

interface Question {
  id?: string
  questionId?: string
  stringContent: string
  explaination?: string
  explanation?: string
  level?: number
  type?: number
  answers: Answer[]
  chosenAnswerIds?: string[]
  isConfirmed?: boolean
}

interface Props {
  question: Question
  index: number
  mode?: 'practice' | 'result' | 'detail'
  showResultInstant?: boolean
  // Specifically for result mode which maps chosen answers by indexes:
  chosenAnswerIndexes?: number[]
}

const props = withDefaults(defineProps<Props>(), {
  mode: 'practice',
  showResultInstant: false,
  chosenAnswerIndexes: () => []
})

const emit = defineEmits<{
  (e: 'chooseAnswer', ans: Answer): void
}>()

// Check if multiple choice
const isMultipleChoice = computed(() => {
  return props.question.type === 1
})

// Check if input selection should be disabled
const isInputDisabled = computed(() => {
  if (props.mode === 'result' || props.mode === 'detail') {
    return true
  }
  // If instant result mode and question is confirmed, disable further input
  if (props.showResultInstant && props.question.isConfirmed) {
    return true
  }
  return false
})

// Check if answer is chosen
const isAnsChosen = (ans: Answer, aIdx: number): boolean => {
  if (props.mode === 'result') {
    return props.chosenAnswerIndexes.includes(aIdx)
  }
  const chosenIds = props.question.chosenAnswerIds || []
  const ansId = ans.id || ans.questionAnswerId
  return !!(ansId && chosenIds.includes(ansId))
}

// Check if correct/incorrect showing condition is met
const isConfirmedOrResult = computed(() => {
  if (props.mode === 'result') return true
  if (props.mode === 'detail') return true
  return !!(props.showResultInstant && props.question.isConfirmed)
})

// Show correct answers
const shouldShowCorrectAnswers = computed(() => {
  return isConfirmedOrResult.value
})

// Show incorrect selections
const shouldShowIncorrectSelections = computed(() => {
  return isConfirmedOrResult.value
})

// Check if the whole question was answered correctly (specifically for result mode highlight)
const isCorrectResult = computed(() => {
  if (props.mode !== 'result') return false
  
  const correctIndexes = props.question.answers
    .map((ans, idx) => (ans.isCorrectAnswer ? idx : -1))
    .filter(idx => idx !== -1)
  
  const chosen = props.chosenAnswerIndexes || []
  
  return (
    correctIndexes.length === chosen.length &&
    correctIndexes.every(idx => chosen.includes(idx))
  )
})

// Show explanation box
const shouldShowExplanation = computed(() => {
  return isConfirmedOrResult.value
})

// Get explanation content
const explanationContent = computed(() => {
  return props.question.explanation || props.question.explaination || ''
})

// Card outer border class based on result
const cardBorderClass = computed(() => {
  if (props.mode === 'result') {
    return isCorrectResult.value 
      ? 'border border-success-subtle bg-success-soft' 
      : 'border border-danger-subtle bg-danger-soft'
  }
  return ''
})

// Helper to character A, B, C, D
const getIndexChar = (ans: Answer, aIdx: number): string => {
  if (ans.indexChar) return ans.indexChar
  return String.fromCharCode(65 + aIdx)
}

// Get styles/classes for option boxes
const getOptionBoxClass = (ans: Answer, aIdx: number) => {
  const chosen = isAnsChosen(ans, aIdx)
  
  if (isConfirmedOrResult.value) {
    if (ans.isCorrectAnswer) {
      return 'border-success bg-success-soft text-success fw-semibold shadow-sm'
    }
    if (chosen && !ans.isCorrectAnswer) {
      return 'border-danger bg-danger-soft text-danger fw-semibold shadow-sm'
    }
    return 'border-light bg-light text-secondary opacity-75'
  }

  // Normal / Practice mode selection styles
  if (chosen) {
    return 'border-indigo bg-indigo-light-opacity text-indigo fw-bold'
  }
  
  return 'border-light hover-shadow bg-white text-dark'
}

// Level display helpers
const getLevelText = (level: number): string => {
  switch (level) {
    case 0: return 'Dễ'
    case 1: return 'Trung bình'
    case 2: return 'Khó'
    case 3: return 'Cực khó'
    default: return 'Chưa xác định'
  }
}

const getLevelColor = (level: number): string => {
  switch (level) {
    case 0: return 'green'
    case 1: return 'orange'
    case 2: return 'red'
    case 3: return 'purple'
    default: return 'default'
  }
}

// Select handler
const handleSelect = (ans: Answer, aIdx: number) => {
  if (isInputDisabled.value) return
  emit('chooseAnswer', ans)
}
</script>

<style scoped>
.question-card {
  transition: all 0.3s ease;
}

.option-item-box {
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.02);
}

.option-item-box:hover:not(.border-light) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.border-indigo {
  border-color: #4f46e5 !important;
}

.bg-indigo-light-opacity {
  background-color: rgba(79, 70, 229, 0.08) !important;
}

.text-indigo {
  color: #4f46e5 !important;
}

.hover-pointer {
  cursor: pointer;
}

.leading-relaxed {
  line-height: 1.625;
}

.leading-normal {
  line-height: 1.5;
}

.fs-9 {
  font-size: 0.75rem;
}
</style>
