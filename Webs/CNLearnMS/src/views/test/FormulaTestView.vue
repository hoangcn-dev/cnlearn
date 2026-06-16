<template>
  <div class="container py-5">
    <div class="row mb-4">
      <div class="col-12">
        <div class="card border-0 rounded-4 shadow-sm bg-gradient-dark text-white p-4">
          <h1 class="fw-bold mb-1 text-white">🧪 Trình kiểm thử Công thức Khoa học</h1>
          <p class="text-white-50 mb-0">Hỗ trợ hiển thị công thức Toán học, Vật lý và Hóa học chuẩn LaTeX/KaTeX kết hợp Markdown</p>
        </div>
      </div>
    </div>

    <div class="row g-4">
      <!-- Cột bên trái: Trình soạn thảo & Tùy chọn -->
      <div class="col-lg-6">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4 h-100 d-flex flex-column">
          <h3 class="fw-bold text-dark-blue mb-3">✍ Bộ Soạn Thảo</h3>
          
          <!-- Tabs chọn chế độ gõ -->
          <div class="d-flex mb-3 bg-light p-1 rounded-3">
            <button 
              class="btn flex-fill py-2 rounded-2 fw-bold text-nowrap transition-all"
              :class="activeEditor === 'text' ? 'bg-white shadow-sm text-indigo' : 'text-secondary'"
              style="font-size: 0.85rem;"
              @click="activeEditor = 'text'"
            >
              📝 Nhập Văn Bản & LaTeX
            </button>
            <button 
              class="btn flex-fill py-2 rounded-2 fw-bold text-nowrap transition-all"
              :class="activeEditor === 'visual' ? 'bg-white shadow-sm text-indigo' : 'text-secondary'"
              style="font-size: 0.85rem;"
              @click="activeEditor = 'visual'"
            >
              📐 Bộ Gõ Trực Quan (MathLive)
            </button>
          </div>

          <!-- Presets -->
          <div class="mb-3">
            <label class="form-label text-secondary small fw-semibold">CHỌN MẪU TEST NHANH:</label>
            <div class="d-flex flex-wrap gap-2">
              <button 
                v-for="preset in presets" 
                :key="preset.name" 
                class="btn btn-light btn-sm rounded-pill px-3 border border-light-subtle"
                @click="loadPreset(preset.content)"
              >
                {{ preset.name }}
              </button>
            </div>
          </div>

          <!-- Editor Inputs -->
          <div class="flex-grow-1 d-flex flex-column">
            <!-- Mode 1: Textarea -->
            <div v-show="activeEditor === 'text'" class="flex-grow-1 d-flex flex-column">
              <div class="d-flex justify-content-between align-items-center mb-2">
                <label class="form-label text-secondary small fw-semibold mb-0">NỘI DUNG VĂN BẢN (Markdown + LaTeX):</label>
                <button 
                  class="btn btn-indigo btn-sm text-white px-3 rounded-2 fw-bold"
                  type="button"
                  @click="openModal = true"
                >
                  ➕ Soạn & Chèn bằng Popup
                </button>
              </div>
              <textarea 
                id="test-editor-textarea"
                v-model="editorContent" 
                class="form-control font-monospace border-2 rounded-3 p-3 flex-grow-1" 
                style="min-height: 250px; font-size: 0.95rem; resize: vertical;"
                placeholder="Nhập nội dung văn bản kèm ký tự $ để viết công thức toán..."
              ></textarea>
            </div>

            <!-- Mode 2: MathLive Editor -->
            <div v-show="activeEditor === 'visual'" class="flex-grow-1">
              <label class="form-label text-secondary small fw-semibold">BỘ SOẠN THẢO CÔNG THỨC TOÁN (Nhập bằng phím hoặc bấm nút):</label>
              <FormulaEditor 
                v-model="mathLiveContent" 
                placeholder="Gõ công thức tại đây..."
              />
              <div class="mt-3 p-3 bg-light rounded-3 border border-light-subtle">
                <span class="text-secondary small fw-semibold d-block mb-1">DỮ LIỆU LATEX ĐẦU RA (XUẤT SANG BACKEND):</span>
                <code class="text-indigo break-all font-monospace">{{ mathLiveContent || '(Trống)' }}</code>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Cột bên phải: Live Preview -->
      <div class="col-lg-6">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4 h-100">
          <h3 class="fw-bold text-dark-blue mb-3 d-flex align-items-center justify-content-between">
            <span>👁 Xem Trước Trực Tiếp</span>
            <span class="badge bg-success-soft text-success rounded-pill px-3 py-1 fs-9">KaTeX Live</span>
          </h3>

          <div class="preview-box p-4 border border-light rounded-3 bg-light-soft flex-grow-1 overflow-auto" style="min-height: 300px;">
            <FormulaRenderer :content="previewContent" />
          </div>
        </div>
      </div>
    </div>

    <!-- Hướng dẫn sử dụng nhanh -->
    <div class="row mt-4">
      <div class="col-12">
        <div class="card border-0 rounded-4 shadow-sm bg-light p-4">
          <h4 class="fw-bold text-dark-blue mb-2">💡 Hướng dẫn cú pháp & Trình quét đề:</h4>
          <ul class="text-secondary small mb-0 d-flex flex-column gap-2">
            <li><strong>Chế độ Văn Bản & LaTeX</strong>: Thích hợp cho toàn bộ đề thi. Các công thức được kẹp giữa cặp dấu <code>$ ... $</code> (cho inline) hoặc <code>$$ ... $$</code> (cho block).</li>
            <li><strong>Chế độ Gõ Trực Quan</strong>: Rất thích hợp để soạn thảo riêng từng công thức toán học mà không cần biết LaTeX. Bàn phím ảo tự động hiện lên khi bấm vào khung gõ.</li>
            <li><strong>Tích hợp Quét Đề (OCR)</strong>: Khi ảnh chụp đề thi được quét qua công cụ AI, dữ liệu chữ và công thức LaTeX sẽ được ghép thẳng vào dạng Văn Bản (như Tab 1) để lưu vào CSDL và tự động hiển thị chính xác.</li>
          </ul>
        </div>
      </div>
    </div>

    <!-- Popup soạn thảo công thức dùng chung -->
    <FormulaEditorModal 
      v-model:open="openModal" 
      title="🧮 Trình chèn công thức đề thi"
      @ok="handleInsertFormula"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { FormulaRenderer, FormulaEditor, FormulaEditorModal } from '@/components/formula-editor'

const activeEditor = ref<'text' | 'visual'>('text')
const openModal = ref(false)
const editorContent = ref('')
const mathLiveContent = ref('x = \\frac{-b \\pm \\sqrt{b^2 - 4ac}}{2a}')

const handleInsertFormula = (formulaCode: string) => {
  const textarea = document.getElementById('test-editor-textarea') as HTMLTextAreaElement | null
  if (textarea) {
    const start = textarea.selectionStart
    const end = textarea.selectionEnd
    const text = editorContent.value
    editorContent.value = text.substring(0, start) + formulaCode + text.substring(end)
    // Tự động focus lại và đặt con trỏ sau công thức vừa chèn
    setTimeout(() => {
      textarea.focus()
      textarea.setSelectionRange(start + formulaCode.length, start + formulaCode.length)
    }, 50)
  } else {
    editorContent.value += formulaCode
  }
}

const presets = [
  {
    name: '🧮 Giải Tích Toán',
    content: `# Toán học: Tích phân & Đạo harm\n\nCho hàm số liên tục trên đoạn $[a, b]$, tích phân xác định được tính theo công thức Newton-Leibniz:\n\n$$\\int_{a}^{b} f(x) \\,dx = F(b) - F(a)$$\n\nXét ví dụ tính tích phân phân thức hữu tỉ phức tạp:\n\n$$\\int_{1}^{2} \\frac{x^3 + 2x^2 + 5}{x^2 - 1} \\,dx$$`
  },
  {
    name: '📐 Hệ Phương Trình & Ma Trận',
    content: `# Hệ Phương Trình & Ma Trận\n\nGiải hệ phương trình tuyến tính 3 ẩn số:\n\n$$\\begin{cases} 2x + 3y - z = 4 \\\\ x - y + 2z = 5 \\\\ 3x + 2y + z = 9 \\end{cases}$$\n\nBiểu diễn ma trận xoay tọa độ trong không gian 2D:\n\n$$R(\\theta) = \\begin{pmatrix} \\cos\\theta & -\\sin\\theta \\\\ \\sin\\theta & \\cos\\theta \\end{pmatrix}$$`
  },
  {
    name: '⚡ Vật Lý Sóng & Điện Xoay Chiều',
    content: `# Vật Lý: Biểu thức Sóng và Trở Kháng\n\nPhương trình truyền sóng cơ học dọc theo trục $Ox$:\n\n$$u(x, t) = A \\cos\\left(2\\pi f t - \\frac{2\\pi x}{\\lambda} + \\varphi_0\\right)$$\n\nTổng trở của mạch điện xoay chiều RLC mắc nối tiếp:\n\n$$Z = \\sqrt{R^2 + (Z_L - Z_C)^2}$$\n\ntrong đó cảm kháng $Z_L = \\omega L$ và dung kháng $$Z_C = \\frac{1}{\\omega C}$$`
  },
  {
    name: '🧪 Hóa Học Oxi Hóa Khử',
    content: `# Hóa Học: Phương Trình Phản Ứng & Cân Bằng\n\nPhản ứng tạo sắt từ oxit khi cho sắt tác dụng với nước ở nhiệt độ cao:\n\n$$\\text{3Fe} + \\text{4H}_2\\text{O} \\xrightarrow{t^\\circ} \\text{Fe}_3\\text{O}_4 + \\text{4H}_2 \\uparrow$$\n\nHằng số cân bằng hóa học $K_c$ của phản ứng tổng quát:\n\n$$K_c = \\frac{[\\text{C}]^c [\\text{D}]^d}{[\\text{A}]^a [\\text{B}]^b}$$`
  },
  {
    name: '📝 Đề Thi Thực Tế (Hỗn Hợp)',
    content: `# Đề khảo sát thực tế môn Toán\n\n**Câu 1 (2.0 điểm):** Cho tam giác $ABC$ vuông tại $A$ có cạnh bên $AB = 3\\text{cm}$, cạnh huyền $BC = 5\\text{cm}$.\n\n1. Tính độ dài cạnh $AC$ dựa trên định lý Pythagoras: \n   $$BC^2 = AB^2 + AC^2 \\Rightarrow AC = \\sqrt{BC^2 - AB^2}$$\n2. Kẻ đường cao $AH$ vuông góc với $BC$ tại $H$. Tính độ dài $AH$ bằng hệ thức lượng:\n   $$\\frac{1}{AH^2} = \\frac{1}{AB^2} + \\frac{1}{AC^2}$$`
  }
]

editorContent.value = presets[0]?.content || ''

// Khi chọn tab gõ trực quan, cập nhật nội dung từ editorContent nếu nó chỉ chứa công thức đơn giản
// Hoặc ngược lại để có sự chuyển giao mượt mà
watch(activeEditor, (newVal) => {
  if (newVal === 'visual') {
    // Nếu nội dung dạng text có công thức block $$, cố gắng tách để đưa vào bộ gõ trực quan
    const match = editorContent.value.match(/\$\$([\s\S]*?)\$\$/)
    if (match && match[1]) {
      mathLiveContent.value = match[1].trim()
    }
  } else {
    // Cập nhật lại editorContent tương ứng dưới dạng block
    if (mathLiveContent.value) {
      editorContent.value = `# Đề bài tự động soạn thảo\n\nGiải biểu thức toán học sau:\n\n$$${mathLiveContent.value}$$\n\n*(Sử dụng trình gõ trực quan MathLive để chỉnh sửa lại)*`
    }
  }
})

const previewContent = computed(() => {
  if (activeEditor.value === 'text') {
    return editorContent.value
  } else {
    // Hiển thị công thức MathLive dưới dạng khối biệt lập
    return `### Biểu thức Toán học (MathLive Output):\n\n$$${mathLiveContent.value}$$`
  }
})

const loadPreset = (content: string) => {
  editorContent.value = content
  activeEditor.value = 'text'
}
</script>

<style scoped>
.bg-gradient-dark {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
}
.text-dark-blue {
  color: #1e293b;
}
.bg-light-soft {
  background-color: #f8fafc;
}
.bg-success-soft {
  background-color: #e2fbf0;
}
.fs-9 {
  font-size: 0.75rem;
}
.preview-box {
  border-style: dashed !important;
  border-width: 2px !important;
}
.break-all {
  word-break: break-all;
}
.transition-all {
  transition: all 0.25s ease-in-out;
}
</style>
