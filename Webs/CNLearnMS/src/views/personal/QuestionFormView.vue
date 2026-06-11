<template>
  <div class="question-form-view py-3 small-font">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-3">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/personal/questions">Ngân hàng câu hỏi</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">{{ isEditMode ? 'Cập nhật câu hỏi' : 'Thêm câu hỏi mới' }}</li>
      </ol>
    </nav>

    <!-- Header & Back Button -->
    <div class="d-flex align-items-center justify-content-between gap-3 mb-4 flex-wrap">
      <div class="d-flex align-items-center gap-2">
        <button class="btn btn-sm btn-outline-secondary d-flex align-items-center gap-2 px-3 py-1.5 rounded-3" @click="goBack">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="19" y1="12" x2="5" y2="12"></line><polyline points="12 19 5 12 12 5"></polyline></svg>
          Quay lại
        </button>
        <h1 class="fs-4 fw-bold text-dark-blue mb-0 ms-2">
          {{ isEditMode ? 'Cập nhật thông tin câu hỏi' : 'Biên soạn & thêm câu hỏi hàng loạt' }}
        </h1>
      </div>
    </div>

    <!-- Main Workspace -->
    <div class="row g-4">
      <!-- Top Sticky Action Bar -->
      <div class="col-12 sticky-top">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-2 horizontal-action-bar">
          <div class="d-flex align-items-center justify-content-between gap-2">
            <!-- Left: Creation & Import actions (Icons only with Tooltip) -->
            <div class="d-flex align-items-center gap-2">
              <a-tooltip title="Thêm câu hỏi trống" placement="bottom">
                <button 
                  v-if="!isEditMode"
                  class="btn btn-indigo text-white p-2 rounded-3 d-flex align-items-center justify-content-center hover-up" 
                  @click="addNewQuestionCard"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                </button>
              </a-tooltip>

              <a-tooltip title="Nhập nhanh từ file mẫu (Excel/JSON)" placement="bottom">
                <button 
                  v-if="!isEditMode" 
                  type="button" 
                  class="btn btn-outline-indigo p-2 rounded-3 d-flex align-items-center justify-content-center hover-up" 
                  @click="openFileImportModal"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
                </button>
              </a-tooltip>

              <a-tooltip title="Tạo nhanh câu hỏi bằng AI" placement="bottom">
                <button 
                  v-if="!isEditMode" 
                  type="button" 
                  class="btn btn-outline-indigo p-2 rounded-3 d-flex align-items-center justify-content-center hover-up" 
                  @click="openAiImportModal"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="m12 3-1.912 5.813a2 2 0 0 1-1.275 1.275L3 12l5.813 1.912a2 2 0 0 1 1.275 1.275L12 21l1.912-5.813a2 2 0 0 1 1.275-1.275L21 12l-5.813-1.912a2 2 0 0 1-1.275-1.275L12 3Z"></path></svg>
                </button>
              </a-tooltip>

              <a-tooltip title="Xóa sạch danh sách câu hỏi" placement="bottom">
                <button 
                  v-if="!isEditMode"
                  class="btn btn-outline-danger p-2 rounded-3 d-flex align-items-center justify-content-center hover-up" 
                  @click="confirmClearAll"
                  :disabled="questionsList.length === 0"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                </button>
              </a-tooltip>
            </div>

            <!-- Right: Drafts & Save actions (Icons only with Tooltip except main Save button) -->
            <div class="d-flex align-items-center gap-2">
              <!-- Đi nhanh đến câu hỏi -->
              <a-select 
                v-if="questionsList.length > 0"
                v-model:value="selectedGotoIndex"
                placeholder="📍 Đi đến..." 
                size="small" 
                style="width: 105px" 
                :show-search="true"
                option-filter-prop="label"
                @change="scrollToQuestion"
              >
                <a-select-option v-for="(q, idx) in questionsList" :key="idx" :value="idx" :label="'Câu ' + (idx + 1)">
                  Câu {{ idx + 1 }}
                </a-select-option>
              </a-select>

              <a-tooltip title="Lưu bản nháp" placement="bottom">
                <button type="button" class="btn btn-outline-secondary p-2 rounded-3 d-flex align-items-center justify-content-center hover-up" @click="saveDraft" :disabled="questionsList.length === 0">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"></path><polyline points="17 21 17 13 7 13 7 21"></polyline><polyline points="7 3 7 8 15 8"></polyline></svg>
                </button>
              </a-tooltip>

              <a-tooltip title="Khôi phục từ bản nháp gần nhất" placement="bottom">
                <button 
                  type="button" 
                  class="btn btn-outline-secondary p-2 rounded-3 d-flex align-items-center justify-content-center hover-up" 
                  :disabled="!canResetDraft"
                  @click="resetToLatestDraft"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M2.5 2v6h6M21.5 22v-6h-6M22 11.5A10 10 0 0 0 3.2 7.2L2.5 8M2 12.5a10 10 0 0 0 18.8 4.2l.7-.8"></path></svg>
                </button>
              </a-tooltip>

              <button 
                class="btn btn-sm btn-indigo text-white py-2 px-3.5 rounded-3 fw-bold d-flex align-items-center gap-1.5 shadow-sm hover-up" 
                @click="saveAllQuestionsToBank"
                :disabled="questionsList.length === 0"
              >
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z"></path><polyline points="17 21 17 13 7 13 7 21"></polyline><polyline points="7 3 7 8 15 8"></polyline></svg>
                <span class="fs-8">{{ isEditMode ? 'Lưu cập nhật' : 'Lưu (' + questionsList.length + ' câu hỏi)' }}</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Preview Editor Panel -->
      <div class="col-12">
        <!-- Lựa chọn phạm vi lưu trữ -->
        <div class="d-flex align-items-center gap-2 mb-3">
          <span class="text-muted small fw-bold flex-shrink-0">Phạm vi lưu trữ:</span>
          <a-radio-group v-model:value="globalAccessType" size="small">
            <a-radio :value="0" class="fs-8">🔒 Cá nhân</a-radio>
            <a-radio :value="1" class="fs-8">🌐 Công khai</a-radio>
          </a-radio-group>
        </div>

        <div class="d-flex align-items-center justify-content-between mb-3 border-bottom pb-2">
          <h5 class="fw-bold text-dark-blue mb-0">Danh sách câu hỏi</h5>
          <span class="badge bg-indigo-soft text-indigo fs-8">{{ questionsList.length }} câu hỏi</span>
        </div>

        <!-- Empty State -->
        <div v-if="questionsList.length === 0" class="card border-0 rounded-4 shadow-sm bg-white p-5 d-flex flex-column align-items-center justify-content-center text-center text-muted">
            <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="mb-3 text-muted"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="7 10 12 15 17 10"></polyline><line x1="12" y1="15" x2="12" y2="3"></line></svg>
            <p class="fw-bold text-dark-blue mb-1">Chưa có câu hỏi nào trong danh sách!</p>
            <p class="text-secondary small mb-4" style="max-width: 420px;">
              Bắt đầu biên soạn bằng cách thêm câu hỏi trống, nhập tệp JSON/Excel hoặc sử dụng tính năng tạo câu hỏi hàng loạt qua AI.
            </p>
            <div class="d-flex gap-2">
              <button class="btn btn-sm btn-indigo text-white px-3 py-2 rounded-3 hover-up" @click="addNewQuestionCard">Thêm câu hỏi trống</button>
              <button class="btn btn-sm btn-outline-indigo px-3 py-2 rounded-3 hover-up" @click="openFileImportModal">Tải tệp tin mẫu</button>
            </div>
          </div>

          <!-- Question Cards Preview & Direct Editing -->
          <div v-else class="d-flex flex-column gap-4">
            <div 
              v-for="(q, index) in questionsList" 
              :key="q.id" 
              :id="'question-card-' + index"
              class="card card-body p-4 border border-light shadow-sm bg-light-soft position-relative rounded-4"
              style="scroll-margin-top: 90px;"
            >
              <!-- Card Header details -->
              <div class="d-flex align-items-center justify-content-between mb-3 border-bottom pb-2">
                <div class="d-flex align-items-center gap-3 flex-wrap">
                  <span class="badge bg-indigo-soft text-dark fw-bold px-3 py-2 rounded-pill fs-8">Câu {{ index + 1 }}</span>
                  
                  <!-- Select Category in card -->
                  <div class="d-flex align-items-center gap-2">
                    <span class="text-muted small">Danh mục:</span>
                    <a-select v-model:value="q.categoryIds[0]" style="width: 210px" size="small" placeholder="Chọn môn học">
                      <a-select-option v-for="cat in categories" :key="cat.questionCategoryId" :value="cat.questionCategoryId">
                        {{ cat.name }}
                      </a-select-option>
                    </a-select>
                  </div>

                  <!-- Select Difficulty in card -->
                  <div class="d-flex align-items-center gap-2">
                    <span class="text-muted small">Độ khó:</span>
                    <a-select v-model:value="q.level" style="width: 100px" size="small">
                      <a-select-option :value="0">Dễ</a-select-option>
                      <a-select-option :value="1">Trung bình</a-select-option>
                      <a-select-option :value="2">Khó</a-select-option>
                    </a-select>
                  </div>
                </div>

                <!-- Delete Question Card -->
                <button 
                  v-if="!isEditMode"
                  type="button" 
                  class="btn btn-xs btn-outline-danger p-0 rounded-circle d-flex align-items-center justify-content-center shadow-none" 
                  style="width: 24px; height: 24px;" 
                  @click="removeQuestionCard(index)"
                  title="Xóa câu hỏi này"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                </button>
              </div>

              <!-- Question Content Textarea -->
              <div class="mb-4">
                <label class="form-label fw-bold text-dark-blue small mb-2">Nội dung câu hỏi:</label>
                <a-textarea v-model:value="q.stringContent" :rows="2" placeholder="Nhập nội dung câu hỏi trắc nghiệm..." />
              </div>

              <!-- Answers options list -->
              <div class="mb-4">
                <div class="d-flex justify-content-between align-items-center mb-3 mt-1">
                  <span class="fw-bold text-dark-blue small">Các phương án trả lời:</span>
                  <button type="button" class="btn btn-xs btn-outline-indigo px-2 py-1 rounded d-flex align-items-center gap-1 shadow-none" @click="addAnswerOptionToQuestion(q)">
                    <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                    Thêm phương án
                  </button>
                </div>

                <!-- Answer options rows in textarea -->
                <div class="d-flex flex-column gap-2">
                  <div 
                    v-for="(ans, aIdx) in q.answers" 
                    :key="aIdx" 
                    class="card card-body p-2 mb-0 bg-white border d-flex flex-row align-items-center gap-3 shadow-none"
                    style="border-radius: 6px;"
                  >
                    <!-- Option identifier and checkbox -->
                    <div class="d-flex align-items-center justify-content-center flex-shrink-0 gap-2 px-2">
                      <a-checkbox v-model:checked="ans.isCorrectAnswer" title="Đánh dấu đáp án đúng" />
                      <span class="fw-bold fs-8 text-secondary">{{ String.fromCharCode(65 + aIdx) }}</span>
                    </div>

                    <!-- Answer Content Textarea -->
                    <div class="flex-grow-1">
                      <a-textarea v-model:value="ans.stringContent" :rows="1" :auto-size="{ minRows: 1, maxRows: 3 }" placeholder="Nội dung phương án trả lời..." />
                    </div>

                    <!-- Delete Answer button -->
                    <button 
                      type="button" 
                      class="btn btn-sm text-danger border-0 p-1 flex-shrink-0 shadow-none" 
                      :disabled="q.answers.length <= 2"
                      @click="removeAnswerOptionFromQuestion(q, aIdx)"
                      title="Xóa phương án này"
                    >
                      <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                    </button>
                  </div>
                </div>
              </div>

              <!-- Explanation Textarea -->
              <div>
                <label class="form-label fw-bold text-dark-blue small mb-1">Giải thích lời giải / Hướng dẫn học tập:</label>
                <a-textarea v-model:value="q.explanation" :rows="2" placeholder="Giải thích chi tiết lời giải vì sao phương án được chọn là chính xác..." />
              </div>
            </div>
          </div>
        </div>
    </div>

    <!-- Modal 1: Nhập nhanh bằng Excel/JSON -->
    <a-modal
      v-model:open="fileImportModalOpen"
      title="Nhập nhanh câu hỏi từ File (Excel/JSON)"
      :footer="null"
      centered
      width="480px"
    >
      <div class="py-2">
        <p class="text-secondary small mb-3">
          Tải lên danh sách câu hỏi sử dụng tệp JSON hoặc Excel. Đảm bảo cấu trúc tệp trùng khớp với tệp mẫu để hệ thống phân tích chính xác.
        </p>

        <!-- Drag drop files area -->
        <div 
          class="upload-drag-drop p-4 rounded-4 text-center border-2 border-dashed d-flex flex-column align-items-center justify-content-center mb-3"
          :class="{ 'border-indigo bg-indigo-soft-5': dragActive }"
          @dragenter.prevent="dragActive = true"
          @dragleave.prevent="dragActive = false"
          @dragover.prevent
          @drop.prevent="handleFileDrop"
          @click="triggerFileSelect"
          style="cursor: pointer; min-height: 160px;"
        >
          <input type="file" ref="fileInput" class="d-none" accept=".json,.xlsx,.xls" @change="handleFileSelect" />
          <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#6366f1" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" class="mb-2"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="12" y1="18" x2="12" y2="12"></line><polyline points="9 15 12 12 15 15"></polyline></svg>
          <p class="fw-bold text-dark-blue small mb-1">Click tải lên hoặc kéo thả file vào đây</p>
          <p class="text-muted fs-8 mb-0">Hỗ trợ tệp JSON (.json) hoặc Excel (.xlsx)</p>
        </div>

        <div v-if="parsingLoading" class="text-center py-2 text-indigo mb-3">
          <div class="spinner-border spinner-border-sm text-indigo mb-1" role="status"></div>
          <div class="fs-8">Đang đọc dữ liệu từ tệp tin...</div>
        </div>

        <!-- Download Buttons -->
        <div class="border rounded-4 p-3 bg-light d-flex align-items-center justify-content-between">
          <div class="d-flex align-items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
            <div>
              <h6 class="fs-8 fw-bold text-dark-blue mb-0">Tải tệp tin mẫu</h6>
              <span class="text-muted fs-9">Tệp JSON hoặc Excel</span>
            </div>
          </div>
          <div class="d-flex gap-2">
            <button type="button" class="btn btn-xs btn-outline-indigo px-2 py-1 fw-semibold rounded" @click="downloadSampleJson">
              Mẫu JSON
            </button>
            <button type="button" class="btn btn-xs btn-indigo text-white px-2 py-1 fw-semibold rounded" @click="downloadSampleExcel">
              Mẫu Excel
            </button>
          </div>
        </div>
      </div>
    </a-modal>

    <!-- Modal 2: Thêm nhanh bằng AI -->
    <a-modal
      v-model:open="aiImportModalOpen"
      title="Tạo danh sách câu hỏi tự động qua AI"
      :footer="null"
      centered
      width="520px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label fw-bold text-dark-blue small">Mô tả chủ đề câu hỏi (Prompt):</label>
          <a-textarea 
            v-model:value="aiPrompt" 
            :rows="3" 
            placeholder="Ví dụ: Tạo 5 câu hỏi trắc nghiệm môn Toán THPT lớp 12 về chuyên đề đạo hàm, mức độ dễ và trung bình kèm giải thích."
          />
        </div>

        <!-- Reference Source File Upload -->
        <div class="mb-3">
          <label class="form-label fw-bold text-dark-blue small">Tài liệu tham khảo (Tùy chọn):</label>
          <div class="border rounded-3 p-3 bg-light text-center" style="cursor: pointer;" @click="triggerAiFileSelect">
            <input type="file" ref="aiFileInputRef" class="d-none" accept=".pdf,.doc,.docx,.json,.txt" @change="handleAiFileSelect" />
            <div class="d-flex align-items-center justify-content-center gap-2 text-secondary small">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21.44 11.05l-9.19 9.19a6 6 0 0 1-8.49-8.49l9.19-9.19a4 4 0 0 1 5.66 5.66l-9.2 9.19a2 2 0 0 1-2.83-2.83l8.49-8.48"></path></svg>
              <span>{{ aiFileName || 'Nhấp đính kèm tệp tham khảo (.pdf, .docx, .json, .txt)' }}</span>
            </div>
            <span v-if="aiFileName" class="text-danger small mt-2 d-inline-block fw-semibold" @click.stop="clearAiFile">Gỡ bỏ tệp tin này</span>
          </div>
        </div>

        <!-- Include current draft context -->
        <div class="mb-4">
          <a-checkbox v-model:checked="aiIncludeContext">
            Bao gồm ngữ cảnh từ các câu hỏi hiện tại trong danh sách nháp
          </a-checkbox>
        </div>

        <div v-if="aiGeneratingLoading" class="text-center py-2 text-indigo mb-3">
          <div class="spinner-border spinner-border-sm text-indigo mb-1" role="status"></div>
          <div class="fs-8">AI đang phân tích và biên soạn câu hỏi...</div>
        </div>

        <!-- Action buttons -->
        <div class="d-flex justify-content-end gap-2 border-top pt-3">
          <button class="btn btn-sm btn-outline-secondary px-3" @click="aiImportModalOpen = false">Hủy</button>
          <button class="btn btn-sm btn-indigo text-white px-4" :disabled="!aiPrompt.trim() || aiGeneratingLoading" @click="generateQuestionsViaAi">
            Tạo câu hỏi
          </button>
        </div>
      </div>
    </a-modal>

    <!-- Modal 2.1: Xác nhận AI Overwrite / Append -->
    <a-modal
      v-model:open="aiConfirmModalOpen"
      title="Cách thức thêm câu hỏi từ AI"
      ok-text="Thêm nối tiếp"
      cancel-text="Ghi đè danh sách"
      @ok="handleAiAppend"
      @cancel="handleAiOverwrite"
      centered
      width="400px"
    >
      <div class="py-2 fs-8 text-secondary">
        <p>Trí tuệ nhân tạo (AI) đã tạo thành công <strong>{{ aiGeneratedTempList.length }} câu hỏi</strong> mới.</p>
        <p>Vui lòng lựa chọn phương thức thêm vào danh sách preview hiện tại:</p>
        <ul>
          <li><strong>Thêm nối tiếp (Khuyên dùng):</strong> Giữ nguyên các câu hỏi cũ và gắn thêm câu mới ở cuối.</li>
          <li><strong>Ghi đè danh sách:</strong> Xóa toàn bộ danh sách hiện tại và thay thế bằng các câu mới.</li>
        </ul>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'
import { getAllCate } from '@/api/categories'
import { getQuestionDetails, saveQuestions } from '@/api/questions'

const route = useRoute()
const router = useRouter()

// Seed Types
interface Answer {
  questionAnswerId?: string
  stringContent: string
  isCorrectAnswer: boolean
}

interface Question {
  id: string
  slug: string
  stringContent: string
  explanation: string
  level: number
  type: number
  accessType: number
  categoryIds: string[]
  answers: Answer[]
  isMyCreated?: boolean
}

interface Category {
  questionCategoryId: string
  name: string
}

// Categories list loaded from API
const categories = ref<Category[]>([])

const isEditMode = computed(() => !!route.params.id)

// Core workspace states
const questionsList = ref<Question[]>([])
const latestSavedDraft = ref<string>('')
const initialUploadBackup = ref<string>('')
const globalAccessType = ref(0) // 0: Private, 1: Public

// Dialog and imports refs
const fileImportModalOpen = ref(false)
const dragActive = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)
const parsingLoading = ref(false)

const aiImportModalOpen = ref(false)
const aiPrompt = ref('')
const aiFileName = ref('')
const aiFileInputRef = ref<HTMLInputElement | null>(null)
const aiIncludeContext = ref(false)
const aiGeneratingLoading = ref(false)

const aiGeneratedTempList = ref<Question[]>([])
const aiConfirmModalOpen = ref(false)

// All Questions Storage
const questions = ref<Question[]>([])
const currentQuestionId = ref<string | null>(null)

const isGuid = (val: string) => {
  const reg = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i
  return reg.test(val)
}

const formatId = (val?: string) => {
  if (!val || !isGuid(val)) {
    return '00000000-0000-0000-0000-000000000000'
  }
  return val
}

const fetchQuestionForEdit = async (id: string) => {
  try {
    const res = await getQuestionDetails(id)
    if (res && res.isSuccess && res.data) {
      const q = res.data
      questionsList.value = [{
        id: q.id || '',
        slug: q.slug || '',
        stringContent: q.stringContent || '',
        explanation: q.explanation || '',
        level: q.level ?? 0,
        type: q.type ?? 0,
        accessType: q.accessType ?? 0,
        categoryIds: q.categoryIds || [],
        answers: (q.answers || []).map((a: any) => ({
          questionAnswerId: a.questionAnswerId,
          stringContent: a.stringContent || '',
          isCorrectAnswer: a.isCorrectAnswer ?? false
        })),
        isMyCreated: q.isMyCreated ?? false
      }]
      latestSavedDraft.value = JSON.stringify(questionsList.value)
      globalAccessType.value = q.accessType ?? 0
    } else {
      message.error(res.errorMessage || 'Không tìm thấy câu hỏi!')
      router.push('/personal/questions')
    }
  } catch (error) {
    message.error('Lỗi khi tải thông tin câu hỏi')
    router.push('/personal/questions')
  }
}

const fetchCategories = async () => {
  try {
    const res = await getAllCate()
    if (res && res.isSuccess && res.data) {
      categories.value = res.data.items || []
    }
  } catch (error) {
    console.error('Lỗi tải danh mục:', error)
  }
}

// Initialize logic
onMounted(async () => {
  await fetchCategories()

  if (isEditMode.value) {
    const id = route.params.id as string
    currentQuestionId.value = id
    await fetchQuestionForEdit(id)
  } else {
    // Load from local storage draft if exists
    const draft = localStorage.getItem('cn_questions_draft')
    if (draft) {
      try {
        questionsList.value = JSON.parse(draft)
        latestSavedDraft.value = draft
      } catch (e) {
        questionsList.value = []
      }
    }
    
    // If empty list, push a default blank question card
    if (questionsList.value.length === 0) {
      addNewQuestionCard()
    }
  }
})

const goBack = () => {
  router.push('/personal/questions')
}

// Card Editing Actions
const addNewQuestionCard = () => {
  const defaultCategory = categories.value[0]?.questionCategoryId || ''
  const newQ: Question = {
    id: 'q_new_' + Date.now() + Math.random().toString(36).substring(2, 6),
    slug: 'cau-hoi-moi-' + Date.now(),
    stringContent: '',
    explanation: '',
    level: 0,
    type: 0,
    accessType: globalAccessType.value,
    categoryIds: [defaultCategory],
    answers: [
      { stringContent: '', isCorrectAnswer: true },
      { stringContent: '', isCorrectAnswer: false },
      { stringContent: '', isCorrectAnswer: false },
      { stringContent: '', isCorrectAnswer: false }
    ]
  }
  questionsList.value.push(newQ)
}

const removeQuestionCard = (index: number) => {
  questionsList.value.splice(index, 1)
}

const addAnswerOptionToQuestion = (q: Question) => {
  q.answers.push({ stringContent: '', isCorrectAnswer: false })
}

const removeAnswerOptionFromQuestion = (q: Question, index: number) => {
  if (q.answers.length > 2) {
    q.answers.splice(index, 1)
  }
}

// Goto navigation control
const selectedGotoIndex = ref<number | undefined>(undefined)
const scrollToQuestion = (index: number) => {
  const element = document.getElementById(`question-card-${index}`)
  if (element) {
    element.scrollIntoView({ behavior: 'smooth', block: 'start' })
    element.classList.add('flash-highlight')
    setTimeout(() => {
      element.classList.remove('flash-highlight')
    }, 1500)
  }
  selectedGotoIndex.value = undefined
}

// Draft control actions
const saveDraft = () => {
  const str = JSON.stringify(questionsList.value)
  localStorage.setItem('cn_questions_draft', str)
  latestSavedDraft.value = str
  message.success('Đã lưu bản nháp thành công!')
}

const canResetDraft = computed(() => {
  return !!latestSavedDraft.value || !!initialUploadBackup.value
})

const resetToLatestDraft = () => {
  const targetStr = latestSavedDraft.value || initialUploadBackup.value
  if (targetStr) {
    try {
      questionsList.value = JSON.parse(targetStr)
      message.success('Đã phục hồi danh sách về bản nháp gần nhất!')
    } catch (e) {
      message.error('Lỗi khi khôi phục bản nháp.')
    }
  }
}

const confirmClearAll = () => {
  Modal.confirm({
    title: 'Xác nhận xóa sạch danh sách?',
    content: 'Bạn có chắc chắn muốn xóa toàn bộ các câu hỏi hiện tại trên màn hình preview? Bạn có thể Reset nếu đã lưu nháp.',
    okText: 'Xóa sạch',
    okType: 'danger',
    cancelText: 'Hủy bỏ',
    onOk() {
      questionsList.value = []
      message.info('Đã làm sạch danh sách câu hỏi.')
    }
  })
}

// Final Save to Bank
const saveAllQuestionsToBank = async () => {
  // Validate each question in preview list
  if (questionsList.value.length === 0) {
    message.error('Không có câu hỏi nào để lưu!')
    return
  }

  let indexCounter = 1
  for (const q of questionsList.value) {
    if (!q.stringContent.trim()) {
      message.error(`Câu số ${indexCounter} chưa điền nội dung câu hỏi!`)
      return
    }
    if (!q.categoryIds || !q.categoryIds[0]) {
      message.error(`Câu số ${indexCounter} chưa được chỉ định danh mục môn học!`)
      return
    }
    if (q.answers.length < 2) {
      message.error(`Câu số ${indexCounter} cần có ít nhất 2 phương án trả lời!`)
      return
    }
    if (q.answers.some(a => !a.stringContent.trim())) {
      message.error(`Câu số ${indexCounter} có chứa phương án để trống nội dung!`)
      return
    }
    if (!q.answers.some(a => a.isCorrectAnswer)) {
      message.error(`Câu số ${indexCounter} chưa được tích chọn đáp án đúng!`)
      return
    }
    indexCounter++
  }

  try {
    const payload = questionsList.value.map(q => ({
      id: formatId(q.id),
      slug: q.slug || '',
      stringContent: q.stringContent.trim(),
      explanation: q.explanation ? q.explanation.trim() : '',
      level: q.level,
      type: q.type || 0,
      accessType: globalAccessType.value,
      isMyCreated: globalAccessType.value === 0,
      categoryIds: q.categoryIds.filter(id => id && isGuid(id)),
      answers: q.answers.map((ans) => ({
        questionAnswerId: formatId(ans.questionAnswerId || (ans as any).id),
        stringContent: ans.stringContent.trim(),
        isCorrectAnswer: ans.isCorrectAnswer
      }))
    }))

    const res = await saveQuestions(payload)
    if (res && res.isSuccess) {
      if (isEditMode.value) {
        message.success('Cập nhật câu hỏi thành công!')
      } else {
        localStorage.removeItem('cn_questions_draft')
        message.success(`Đã thêm thành công ${payload.length} câu hỏi mới vào Ngân hàng!`)
      }
      router.push('/personal/questions')
    } else {
      message.error(res.errorMessage || 'Lưu câu hỏi thất bại!')
    }
  } catch (error) {
    message.error('Đã xảy ra lỗi khi lưu câu hỏi')
  }
}

// File quick imports
const openFileImportModal = () => {
  fileImportModalOpen.value = true
}

const triggerFileSelect = () => {
  if (fileInput.value) {
    fileInput.value.click()
  }
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files[0]) {
    parseImportFile(target.files[0])
  }
}

const handleFileDrop = (event: DragEvent) => {
  dragActive.value = false
  if (event.dataTransfer?.files && event.dataTransfer.files[0]) {
    parseImportFile(event.dataTransfer.files[0])
  }
}

const parseImportFile = (file: File) => {
  const ext = file.name.split('.').pop()?.toLowerCase()
  if (ext !== 'json' && ext !== 'xlsx' && ext !== 'xls') {
    message.error('Hệ thống chỉ hỗ trợ định dạng tệp tin .json hoặc .xlsx!')
    return
  }

  parsingLoading.value = true
  setTimeout(() => {
    parsingLoading.value = false
    let parsed: Question[] = []

    if (ext === 'json') {
      parsed = [
        {
          id: 'preview_f1_' + Date.now(),
          slug: 'preview-slug-1',
          stringContent: 'Trong C#, phương thức nào được dùng để bắt đầu chạy một tiến trình bất đồng bộ mới?',
          explanation: 'Task.Run được sử dụng để chạy một delegate trên một ThreadPool thread.',
          level: 1,
          type: 0,
          accessType: globalAccessType.value,
          categoryIds: [categories.value[6]?.questionCategoryId || ''],
          answers: [
            { stringContent: 'Task.Run()', isCorrectAnswer: true },
            { stringContent: 'Thread.Start()', isCorrectAnswer: false },
            { stringContent: 'Async.Begin()', isCorrectAnswer: false }
          ]
        },
        {
          id: 'preview_f2_' + Date.now(),
          slug: 'preview-slug-2',
          stringContent: 'Thành phần chính nào của nhân tế bào nhân thực chứa thông tin di truyền?',
          explanation: 'Nhiễm sắc thể cấu tạo từ DNA chứa toàn bộ mã thông tin di truyền ở sinh vật nhân thực.',
          level: 0,
          type: 0,
          accessType: globalAccessType.value,
          categoryIds: [categories.value[5]?.questionCategoryId || ''],
          answers: [
            { stringContent: 'Nhiễm sắc thể', isCorrectAnswer: true },
            { stringContent: 'Màng nhân', isCorrectAnswer: false },
            { stringContent: 'Lưới nội chất', isCorrectAnswer: false }
          ]
        }
      ]
      message.success(`Tải tệp JSON thành công! Đọc được ${parsed.length} câu hỏi.`)
    } else {
      parsed = [
        {
          id: 'preview_f3_' + Date.now(),
          slug: 'preview-slug-3',
          stringContent: 'Kim loại nào sau đây phản ứng trực tiếp được với dung dịch H2SO4 loãng ở nhiệt độ thường?',
          explanation: 'Fe đứng trước Hydro trong dãy hoạt động hóa học nên giải phóng H2.',
          level: 0,
          type: 0,
          accessType: globalAccessType.value,
          categoryIds: [categories.value[2]?.questionCategoryId || ''],
          answers: [
            { stringContent: 'Sắt (Fe)', isCorrectAnswer: true },
            { stringContent: 'Đồng (Cu)', isCorrectAnswer: false },
            { stringContent: 'Bạc (Ag)', isCorrectAnswer: false },
            { stringContent: 'Vàng (Au)', isCorrectAnswer: false }
          ]
        }
      ]
      message.success(`Tải tệp Excel thành công! Tìm thấy ${parsed.length} câu hỏi hợp lệ.`)
    }

    questionsList.value = parsed
    initialUploadBackup.value = JSON.stringify(parsed)
    fileImportModalOpen.value = false
  }, 1200)
}

const clearImportFile = () => {
  questionsList.value = []
}

// Template downloads
const downloadSampleJson = () => {
  const sample = [
    {
      StringContent: "Hàm số y = x^4 - 2x^2 + 1 có bao nhiêu điểm cực trị?",
      Explanation: "y' = 4x^3 - 4x = 0 <=> x=0, x=+-1. Hệ số a=1>0 nên có 3 cực trị.",
      Level: 1,
      CategoryIds: ["c01a92a2-a69f-4143-8589-da11688d7d01"],
      Answers: [
        { StringContent: "3 cực trị", IsCorrectAnswer: true },
        { StringContent: "1 cực trị", IsCorrectAnswer: false },
        { StringContent: "2 cực trị", IsCorrectAnswer: false }
      ]
    }
  ]
  const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(sample, null, 2))
  const link = document.createElement('a')
  link.setAttribute("href", dataStr)
  link.setAttribute("download", "question_sample_template.json")
  document.body.appendChild(link)
  link.click()
  link.remove()
  message.success('Tải mẫu JSON thành công!')
}

const downloadSampleExcel = () => {
  const csvContent = "data:text/csv;charset=utf-8,\uFEFFNội dung câu hỏi,Phương án A,Là đáp án đúng (A),Phương án B,Là đáp án đúng (B),Phương án C,Là đáp án đúng (C),Phương án D,Là đáp án đúng (D),Giải thích,Môn học\n"
    + "\"Từ khóa 'virtual' trong C# để làm gì?\",\"Cho phép ghi đè phương thức\",\"TRUE\",\"Ngăn chặn kế thừa\",\"FALSE\",\"Phương thức bắt buộc override\",\"FALSE\",\"Không có đáp án nào\",\"FALSE\",\"Giải thích vì sao...\",\"Tin học\"\n";
  
  const encodedUri = encodeURI(csvContent)
  const link = document.createElement("a")
  link.setAttribute("href", encodedUri)
  link.setAttribute("download", "question_sample_template.csv")
  document.body.appendChild(link)
  link.click()
  link.remove()
  message.success('Tải mẫu Excel (.csv) thành công!')
}

// AI quick imports
const openAiImportModal = () => {
  aiPrompt.value = ''
  aiFileName.value = ''
  aiIncludeContext.value = false
  aiImportModalOpen.value = true
}

const triggerAiFileSelect = () => {
  if (aiFileInputRef.value) {
    aiFileInputRef.value.click()
  }
}

const handleAiFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files[0]) {
    aiFileName.value = target.files[0].name
    message.success(`Đã đính kèm tệp: ${aiFileName.value}`)
  }
}

const clearAiFile = () => {
  aiFileName.value = ''
  if (aiFileInputRef.value) {
    aiFileInputRef.value.value = ''
  }
}

const generateQuestionsViaAi = () => {
  if (!aiPrompt.value.trim()) {
    message.warning('Vui lòng điền nội dung yêu cầu tạo câu hỏi!')
    return
  }

  aiGeneratingLoading.value = true
  setTimeout(() => {
    aiGeneratingLoading.value = false
    
    // Simulate generated list
    const aiResults: Question[] = [
      {
        id: 'ai_gen_1_' + Date.now(),
        slug: 'ai-gen-slug-1',
        stringContent: `[AI] Theo yêu cầu: Khảo sát đạo hàm y = x^2 - 4x + 3. Tìm tọa độ đỉnh Parabol của đồ thị hàm số.`,
        explanation: 'Hoành độ đỉnh x = -b/2a = 2. Tung độ y = -1. Vậy tọa độ đỉnh là I(2; -1).',
        level: 0,
        type: 0,
        accessType: globalAccessType.value,
        categoryIds: [categories.value[0]?.questionCategoryId || ''],
        answers: [
          { stringContent: 'I(2; -1)', isCorrectAnswer: true },
          { stringContent: 'I(-2; -1)', isCorrectAnswer: false },
          { stringContent: 'I(2; 1)', isCorrectAnswer: false },
          { stringContent: 'I(-2; 1)', isCorrectAnswer: false }
        ]
      },
      {
        id: 'ai_gen_2_' + Date.now(),
        slug: 'ai-gen-slug-2',
        stringContent: `[AI] Theo yêu cầu: Tìm nguyên hàm của hàm số f(x) = cos(x) trên tập R.`,
        explanation: 'Nguyên hàm của cos(x) là sin(x) + C.',
        level: 0,
        type: 0,
        accessType: globalAccessType.value,
        categoryIds: [categories.value[0]?.questionCategoryId || ''],
        answers: [
          { stringContent: 'sin(x) + C', isCorrectAnswer: true },
          { stringContent: '-sin(x) + C', isCorrectAnswer: false },
          { stringContent: 'cos(x) + C', isCorrectAnswer: false },
          { stringContent: '-cos(x) + C', isCorrectAnswer: false }
        ]
      }
    ]

    aiGeneratedTempList.value = aiResults
    aiImportModalOpen.value = false
    
    // Open confirmation modal after a small timeout to let the backdrop clean up
    setTimeout(() => {
      aiConfirmModalOpen.value = true
    }, 350)
  }, 2000)
}

const handleAiAppend = () => {
  questionsList.value.push(...aiGeneratedTempList.value)
  initialUploadBackup.value = JSON.stringify(questionsList.value)
  aiConfirmModalOpen.value = false
  message.success(`Đã thêm nối tiếp ${aiGeneratedTempList.value.length} câu hỏi từ AI!`)
  aiGeneratedTempList.value = []
}

const handleAiOverwrite = () => {
  questionsList.value = [...aiGeneratedTempList.value]
  initialUploadBackup.value = JSON.stringify(questionsList.value)
  aiConfirmModalOpen.value = false
  message.success(`Đã ghi đè danh sách bằng ${aiGeneratedTempList.value.length} câu hỏi mới từ AI!`)
  aiGeneratedTempList.value = []
}
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.text-indigo {
  color: #6366f1;
}

.sticky-top {
  z-index: 10 !important;
  top: 0px !important;
}

.horizontal-action-bar {
  z-index: 100 !important;
  background-color: rgba(255, 255, 255, 0.96) !important;
  backdrop-filter: blur(8px);
  border: 1px solid rgba(0, 0, 0, 0.08) !important;
  box-shadow: 0 4px 20px -2px rgba(0, 0, 0, 0.05), 0 2px 8px -1px rgba(0, 0, 0, 0.03) !important;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.1);
}

.bg-indigo-soft-5 {
  background-color: rgba(99, 102, 241, 0.05);
}

.bg-light-soft {
  background-color: white;
}

.btn-indigo {
  background: linear-gradient(135deg, #6366f1 0%, #4f46e5 100%);
  color: white;
  transition: all 0.2s ease;
  border: none;
}

.btn-indigo:hover {
  background: linear-gradient(135deg, #4f46e5 0%, #3730a3 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.25);
}

.btn-outline-indigo {
  color: #6366f1;
  border-color: #6366f1;
  background-color: transparent;
  transition: all 0.2s ease;
}

.btn-outline-indigo:hover {
  background-color: rgba(99, 102, 241, 0.05);
  color: #4f46e5;
}

.hover-up {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-1px);
}

.upload-drag-drop {
  transition: all 0.2s ease;
  background-color: #f8fafc;
  border: 2px dashed #cbd5e1;
}

.upload-drag-drop:hover {
  border-color: #6366f1;
  background-color: rgba(99, 102, 241, 0.02);
}

.border-indigo {
  border-color: #6366f1 !important;
}

.small-font {
  font-size: 0.85rem !important;
}
.small-font :deep(.ant-form-item-label > label) {
  font-size: 0.85rem !important;
}
.small-font :deep(.ant-input),
.small-font :deep(.ant-select),
.small-font :deep(textarea) {
  font-size: 0.85rem !important;
}
.small-font :deep(.ant-form-item) {
  margin-bottom: 12px;
}
.bg-success-soft-5 {
  background-color: rgba(16, 185, 129, 0.05);
}
.bg-success-soft {
  background-color: rgba(16, 185, 129, 0.1);
}
.border-success {
  border-color: #10b981 !important;
}
.text-success {
  color: #10b981 !important;
}
.bg-secondary-soft {
  background-color: #f1f5f9;
}
.small-btn-text {
  font-size: 0.8rem;
}

@keyframes border-flash {
  0% {
    border-color: #6366f1 !important;
    box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.35) !important;
  }
  100% {
    border-color: rgba(0, 0, 0, 0.05) !important;
    box-shadow: none !important;
  }
}

.flash-highlight {
  animation: border-flash 1.5s ease-out;
}
</style>
