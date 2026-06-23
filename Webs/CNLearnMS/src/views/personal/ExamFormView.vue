<template>
  <div class="exam-form-view py-3 small-font">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-3">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/personal/exams">Quản lý đề thi</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">
          {{ isEditMode ? 'Chỉnh sửa đề thi' : 'Biên soạn đề thi mới' }}
        </li>
      </ol>
    </nav>

    <!-- Header & Action Buttons -->
    <div class="d-flex align-items-center justify-content-between gap-3 mb-4 flex-wrap">
      <div class="d-flex align-items-center gap-2">
        <button class="btn btn-sm btn-outline-secondary d-flex align-items-center gap-2 px-3 py-1.5 rounded-3" @click="goBack">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="19" y1="12" x2="5" y2="12"></line><polyline points="12 19 5 12 12 5"></polyline></svg>
          Quay lại
        </button>
        <h1 class="fs-4 fw-bold text-dark-blue mb-0 ms-2">
          {{ isEditMode ? 'Chỉnh sửa đề thi' : 'Biên soạn thông tin đề thi mới' }}
        </h1>
      </div>
      <div class="d-flex gap-2">
        <button class="btn btn-outline-secondary btn-sm px-4 py-2 rounded-3" @click="goBack">
          Hủy bỏ
        </button>
        <button class="btn btn-outline-indigo btn-sm px-4 py-2 rounded-3 fw-bold" @click="saveForm(true)">
          Lưu bản nháp
        </button>
        <button class="btn btn-indigo text-white btn-sm px-4 py-2 rounded-3 fw-bold shadow-sm" @click="saveForm(false)">
          Lưu và hoàn tất
        </button>
      </div>
    </div>

    <div class="row g-4">
      <!-- Exam Configuration Card -->
      <div class="col-12">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4">
          <h5 class="fw-bold text-dark-blue mb-3 pb-2 border-bottom">⚙️ Cấu hình thông tin</h5>
          
          <a-form layout="vertical">
            <div class="row g-3">
              <!-- Left Column: Basic Info -->
              <div class="col-lg-7">
                <a-form-item label="Tên đề thi:" required class="mb-2">
                  <a-input v-model:value="formData.name" placeholder="Ví dụ: Đề kiểm tra học kỳ 1 môn Tin học lớp 12" />
                </a-form-item>

                <div class="row g-2 mb-2">
                  <div class="col-md-6">
                    <a-form-item label="Danh mục môn học:" required class="mb-0">
                      <a-select v-model:value="formData.categoryId" placeholder="Chọn danh mục môn học">
                        <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id" :disabled="cat.hasChildren">
                          {{ cat.name }}
                        </a-select-option>
                      </a-select>
                    </a-form-item>
                  </div>
                  <div class="col-md-6">
                    <a-form-item label="Thời gian làm bài (phút):" required class="mb-0">
                      <a-input-number v-model:value="formData.duration" :min="1" :max="360" style="width: 100%" />
                    </a-form-item>
                  </div>
                </div>

                <a-form-item label="Mô tả chi tiết (nếu có):" class="mb-0">
                  <a-textarea v-model:value="formData.description" :rows="2" placeholder="Mô tả nội dung đề thi..." />
                </a-form-item>
              </div>

              <!-- Right Column: Access Rules -->
              <div class="col-lg-5">
                <div class="bg-light-soft p-3 rounded-4 h-100 border border-light d-flex flex-column justify-content-center">
                  <h6 class="fw-bold text-dark-blue mb-3 pb-1 border-bottom" style="font-size: 0.85rem;">⚙️ Thiết lập truy cập & Lưu trữ</h6>
                  
                  <a-form-item label="Phạm vi truy cập đề thi:" class="mb-3">
                    <a-radio-group v-model:value="formData.accessType">
                      <a-radio :value="0">🌐 Công khai (Mọi người đều có thể tìm thấy)</a-radio>
                      <a-radio :value="1">🔒 Riêng tư (Chỉ của riêng bạn)</a-radio>
                    </a-radio-group>
                  </a-form-item>

                  <a-form-item class="mb-0 border-top pt-2">
                    <a-checkbox v-model:checked="formData.contributeToBank">
                      Đóng góp các câu hỏi trong đề này vào Ngân hàng câu hỏi dùng chung
                    </a-checkbox>
                  </a-form-item>
                </div>
              </div>
            </div>
          </a-form>
        </div>
      </div>

      <!-- Questions List Card -->
      <div class="col-12">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4">
          <div class="d-flex justify-content-between align-items-center mb-3 pb-2 border-bottom flex-wrap gap-2">
            <div class="d-flex align-items-center gap-2">
              <h5 class="fw-bold text-dark-blue mb-0">📝 Danh sách câu hỏi trong đề</h5>
              <span class="badge bg-indigo-soft text-indigo px-3 py-1.5 rounded-pill fw-bold fs-8">
                {{ questionsList.length }} câu hỏi
              </span>
            </div>
            <button class="btn btn-indigo text-white btn-sm px-3 py-1.5 rounded-3 fw-bold shadow-sm" @click="openCreateQuestionModal">
              ➕ Soạn câu hỏi mới
            </button>
          </div>

          <!-- Add question buttons -->
          <div class="row g-2 mb-4">
            <div class="col-sm-4 col-6">
              <button class="btn btn-outline-indigo w-100 py-2 rounded-3 d-flex flex-column align-items-center justify-content-center gap-2 btn-sm hover-up text-indigo" @click="autoAddModalOpen = true">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"></polygon></svg>
                <span class="fw-bold text-nowrap" style="font-size: 0.73rem;">Thêm tự động</span>
              </button>
            </div>
            <div class="col-sm-4 col-6">
              <button class="btn btn-outline-indigo w-100 py-2 rounded-3 d-flex flex-column align-items-center justify-content-center gap-2 btn-sm hover-up text-indigo" @click="openBankModal">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path></svg>
                <span class="fw-bold text-nowrap" style="font-size: 0.73rem;">Chọn từ ngân hàng</span>
              </button>
            </div>
            <div class="col-sm-4 col-12">
              <button class="btn btn-outline-indigo w-100 py-2 rounded-3 d-flex flex-column align-items-center justify-content-center gap-2 btn-sm hover-up text-indigo" @click="openImportExamModal">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><line x1="9" y1="3" x2="9" y2="21"></line></svg>
                <span class="fw-bold text-nowrap" style="font-size: 0.73rem;">Thêm từ đề thi khác</span>
              </button>
            </div>
          </div>

          <!-- Fast navigation / Clear actions -->
          <div class="flex-grow-1 d-flex flex-column overflow-hidden">
            <div class="d-flex align-items-center justify-content-between mb-3 bg-light-soft p-2 rounded-3" v-if="questionsList.length > 0">
              <div class="d-flex align-items-center gap-2">
                <span class="text-muted small fw-bold">Tìm nhanh:</span>
                <a-select 
                  v-model:value="selectedGotoIndex" 
                  placeholder="Chọn số câu..." 
                  size="small" 
                  style="width: 140px"
                  @change="scrollToQuestion"
                >
                  <a-select-option v-for="(q, idx) in questionsList" :key="idx" :value="idx">
                    Câu {{ idx + 1 }}
                  </a-select-option>
                </a-select>
              </div>
              <button class="btn btn-link btn-xs text-danger text-decoration-none fw-bold" @click="questionsList = []">
                Xóa tất cả câu hỏi
              </button>
            </div>

            <!-- Empty State -->
            <div v-if="questionsList.length === 0" class="flex-grow-1 d-flex flex-column align-items-center justify-content-center text-center p-5 text-muted border border-dashed rounded-4 bg-light-soft">
              <svg xmlns="http://www.w3.org/2000/svg" width="42" height="42" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="mb-3 text-secondary"><path d="M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5L14.5 2z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line><line x1="10" y1="9" x2="8" y2="9"></line></svg>
              <p class="fw-bold text-dark-blue mb-1">Chưa có câu hỏi nào trong đề thi!</p>
              <p class="text-secondary small mb-0" style="max-width: 320px;">
                Vui lòng nhấp vào các nút hành động phía trên để chọn hoặc nhập nhanh câu hỏi vào đề thi này.
              </p>
            </div>

            <!-- List of selected questions -->
            <div v-else class="d-flex flex-column gap-3">
              <div 
                v-for="(q, index) in questionsList" 
                :key="q.questionId" 
                :id="'selected-question-card-' + index"
                class="card card-body p-3 border border-light bg-light-soft position-relative rounded-3 shadow-none mb-0"
              >
                <!-- Row Controls -->
                <div class="d-flex align-items-center justify-content-between mb-2 pb-2 border-bottom flex-wrap gap-2">
                  <div class="d-flex align-items-center gap-2">
                    <span class="badge bg-indigo-soft text-dark-blue fw-bold px-3 py-1 rounded-pill fs-8">Câu {{ index + 1 }}</span>
                    <span :class="getLevelBadgeClass(q.level)">{{ getLevelText(q.level) }}</span>
                    <span class="text-muted fs-8">{{ getCategoryName(q.questionCategoryId) }}</span>
                  </div>

                  <div class="d-flex align-items-center gap-2">
                    <button class="btn btn-xs btn-outline-indigo d-flex align-items-center justify-content-center rounded-circle" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" @click="openEditQuestionModal(q, index)" title="Chỉnh sửa câu hỏi">
                      <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4z"></path></svg>
                    </button>
                    <button class="btn btn-xs btn-outline-secondary d-flex align-items-center justify-content-center rounded-circle" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" :disabled="index === 0" @click="moveQuestionUp(index)" title="Di chuyển lên">
                      <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="18 15 12 9 6 15"></polyline></svg>
                    </button>
                    <button class="btn btn-xs btn-outline-secondary d-flex align-items-center justify-content-center rounded-circle" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" :disabled="index === questionsList.length - 1" @click="moveQuestionDown(index)" title="Di chuyển xuống">
                      <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="6 9 12 15 18 9"></polyline></svg>
                    </button>
                    <button class="btn btn-xs btn-outline-danger d-flex align-items-center justify-content-center rounded-circle ms-1" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" @click="removeSelectedQuestion(index)" title="Xóa khỏi đề">
                      <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
                    </button>
                  </div>
                </div>

                <!-- Content text -->
                <p class="text-dark-blue small mb-2 text-truncate-2">{{ q.stringContent }}</p>

                <!-- Options -->
                <div class="row g-2">
                  <div v-for="(ans, aIdx) in q.answers" :key="aIdx" class="col-md-6">
                    <div class="p-1 px-2 rounded-2 bg-white border border-light small d-flex gap-2 align-items-center text-truncate">
                      <span class="fw-bold fs-8" :class="ans.isCorrectAnswer ? 'text-success' : 'text-secondary'">
                        {{ String.fromCharCode(65 + aIdx) }}.
                      </span>
                      <span class="text-secondary text-truncate" style="font-size: 0.8rem;">{{ ans.stringContent }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Bottom Actions -->
          <div class="mt-4 pt-3 border-top d-flex justify-content-end gap-2">
            <button class="btn btn-outline-secondary btn-sm px-4 py-2 rounded-3" @click="goBack">
              Hủy bỏ
            </button>
            <button class="btn btn-outline-indigo btn-sm px-4 py-2 rounded-3 fw-bold" @click="saveForm(true)">
              Lưu bản nháp
            </button>
            <button class="btn btn-indigo text-white btn-sm px-4 py-2 rounded-3 fw-bold shadow-sm" @click="saveForm(false)">
              Lưu và hoàn tất
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- MODAL 1: THÊM TỰ ĐỘNG -->
    <a-modal
      v-model:open="autoAddModalOpen"
      title="Thêm câu hỏi ngẫu nhiên tự động"
      ok-text="Thêm vào đề"
      cancel-text="Hủy"
      @ok="handleAutoAdd"
      width="420px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label small fw-bold">Số lượng câu muốn thêm:</label>
          <a-input-number v-model:value="autoAddForm.totalCount" :min="1" :max="100" style="width: 100%" />
        </div>
        <div class="mb-3">
          <label class="form-label small fw-bold">Chủ đề câu hỏi:</label>
          <a-select v-model:value="autoAddForm.categoryId" placeholder="Chọn chủ đề" style="width: 100%">
            <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id" :disabled="cat.hasChildren">
              {{ cat.name }}
            </a-select-option>
          </a-select>
        </div>
        <div class="mb-3 bg-light-soft p-3 rounded-3">
          <label class="form-label small fw-bold text-dark-blue mb-2">Phân bổ độ khó:</label>
          <div class="row g-2">
            <div class="col-4">
              <label class="small text-muted mb-1">Dễ:</label>
              <a-input-number v-model:value="autoAddForm.easyCount" :min="0" style="width: 100%" size="small" />
            </div>
            <div class="col-4">
              <label class="small text-muted mb-1">T.Bình:</label>
              <a-input-number v-model:value="autoAddForm.mediumCount" :min="0" style="width: 100%" size="small" />
            </div>
            <div class="col-4">
              <label class="small text-muted mb-1">Khó:</label>
              <a-input-number v-model:value="autoAddForm.hardCount" :min="0" style="width: 100%" size="small" />
            </div>
          </div>
        </div>
        <div class="mb-1">
          <a-checkbox v-model:checked="autoAddForm.shuffleOptions">🔀 Tự động đảo thứ tự các đáp án</a-checkbox>
        </div>
      </div>
    </a-modal>

    <!-- MODAL 2: CHỌN TỪ NGÂN HÀNG -->
    <a-modal
      v-model:open="bankModalOpen"
      title="Chọn câu hỏi từ Ngân hàng câu hỏi"
      ok-text="Thêm câu hỏi đã chọn"
      cancel-text="Hủy"
      @ok="handleBankAdd"
      width="800px"
    >
      <div class="py-2">
        <div class="row g-2 mb-3">
          <div class="col-md-7">
            <a-input v-model:value="bankFilter.search" placeholder="Tìm kiếm từ khóa câu hỏi..." allow-clear>
              <template #prefix>
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
              </template>
            </a-input>
          </div>
          <div class="col-md-5">
            <a-select v-model:value="bankFilter.categoryId" placeholder="Tất cả chủ đề" allow-clear style="width: 100%">
              <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id" :disabled="cat.hasChildren">
                {{ cat.name }}
              </a-select-option>
            </a-select>
          </div>
        </div>

        <div class="table-responsive rounded-3 border bg-white mb-3" style="max-height: 380px; overflow-y: auto;">
          <table class="table table-hover align-middle mb-0 small-font">
            <thead class="table-light">
              <tr>
                <th style="width: 50px;" class="text-center">
                  <a-checkbox :checked="isAllBankSelected" @change="toggleSelectAllBank" />
                </th>
                <th>Nội dung câu hỏi</th>
                <th style="width: 130px;">Danh mục</th>
                <th style="width: 90px;" class="text-center">Độ khó</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="filteredBankQuestions.length === 0">
                <td colspan="4" class="text-center text-muted py-4">Không tìm thấy câu hỏi phù hợp trong ngân hàng.</td>
              </tr>
              <tr v-for="q in paginatedBankQuestions" :key="q.questionId" class="hover-pointer" @click="toggleSelectBankQuestion(q.questionId)">
                <td class="text-center" @click.stop>
                  <a-checkbox :checked="selectedBankQuestionIds.includes(q.questionId)" @change="toggleSelectBankQuestion(q.questionId)" />
                </td>
                <td class="text-truncate-2" style="max-width: 380px;">{{ q.stringContent }}</td>
                <td class="text-muted text-truncate" style="max-width: 120px;">{{ getCategoryName(q.questionCategoryId) }}</td>
                <td class="text-center">
                  <span :class="getLevelBadgeClass(q.level)">{{ getLevelText(q.level) }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="d-flex justify-content-between align-items-center flex-wrap gap-2" v-if="filteredBankQuestions.length > 0">
          <span class="text-muted small">Đã chọn: {{ selectedBankQuestionIds.length }} / {{ filteredBankQuestions.length }} câu hỏi</span>
          <a-pagination 
            v-model:current="bankFilter.page" 
            :total="filteredBankQuestions.length" 
            :page-size="bankFilter.pageSize" 
            size="small" 
          />
        </div>
      </div>
    </a-modal>

    <!-- MODAL 4: THÊM TỪ ĐỀ THI KHÁC -->
    <a-modal
      v-model:open="importExamModalOpen"
      title="Nhập câu hỏi từ đề thi đã có sẵn"
      ok-text="Thêm toàn bộ câu hỏi"
      cancel-text="Hủy"
      @ok="handleImportExam"
      width="550px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label small fw-bold">Tìm kiếm đề thi gốc:</label>
          <a-input v-model:value="importExamFilter.search" placeholder="Nhập từ khóa tìm tên đề thi..." allow-clear>
            <template #prefix>
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
            </template>
          </a-input>
        </div>

        <div class="border rounded bg-white p-2 mb-2" style="max-height: 250px; overflow-y: auto;">
          <div v-if="importExamLoading" class="text-center py-4 text-indigo">
            <div class="spinner-border spinner-border-sm text-indigo mb-1" role="status"></div>
            <div class="fs-8">Đang tải danh sách đề thi...</div>
          </div>
          <template v-else>
            <div v-if="importExamsList.length === 0" class="text-center text-muted py-4 small">
              Không tìm thấy đề thi phù hợp.
            </div>
            <div 
              v-for="ex in importExamsList" 
              :key="ex.examId" 
              class="d-flex justify-content-between align-items-center p-2 rounded mb-1 hover-pointer"
              :class="selectedImportExamId === ex.examId ? 'bg-indigo-soft border-indigo' : 'border border-light'"
              @click="selectedImportExamId = ex.examId"
            >
              <div>
                <p class="fw-bold text-dark-blue mb-0 small">{{ ex.name }}</p>
                <span class="text-muted fs-8">{{ ex.questionCategoryName || 'Chuyên đề chung' }} | {{ ex.durationMin }} phút</span>
              </div>
              <a-radio :checked="selectedImportExamId === ex.examId" @change="selectedImportExamId = ex.examId" />
            </div>
          </template>
        </div>

        <div class="d-flex justify-content-between align-items-center flex-wrap gap-2 mb-3" v-if="importExamsList.length > 0">
          <span class="text-muted small">Tổng số đề thi: {{ importExamTotalItems }}</span>
          <a-pagination 
            v-model:current="importExamRequest.page" 
            :total="importExamTotalItems" 
            :page-size="importExamRequest.size" 
            size="small" 
            @change="fetchImportExams"
          />
        </div>

        <div class="mb-1">
          <a-checkbox v-model:checked="importExamFilter.shuffleOptions">🔀 Đảo thứ tự các đáp án khi import câu hỏi</a-checkbox>
        </div>
      </div>
    </a-modal>

    <!-- MODAL 5: SOẠN/CHỈNH SỬA CÂU HỎI TRỰC TIẾP -->
    <a-modal
      v-model:open="singleQuestionModalOpen"
      :title="singleQuestionModalMode === 'create' ? '➕ Soạn câu hỏi mới' : '📝 Chỉnh sửa câu hỏi'"
      ok-text="Lưu câu hỏi"
      cancel-text="Hủy"
      @ok="saveSingleQuestion"
      width="600px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label small fw-bold">Nội dung câu hỏi:</label>
          <a-textarea v-model:value="singleQuestionForm.stringContent" :rows="3" placeholder="Nhập câu hỏi..." />
        </div>

        <div class="row g-2 mb-3">
          <div class="col-md-6">
            <label class="form-label small fw-bold">Chuyên đề:</label>
            <a-select v-model:value="singleQuestionForm.questionCategoryId" style="width: 100%">
              <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id" :disabled="cat.hasChildren">
                {{ cat.name }}
              </a-select-option>
            </a-select>
          </div>
          <div class="col-md-6">
            <label class="form-label small fw-bold">Độ khó:</label>
            <a-select v-model:value="singleQuestionForm.level" style="width: 100%">
              <a-select-option :value="0">Dễ</a-select-option>
              <a-select-option :value="1">Trung bình</a-select-option>
              <a-select-option :value="2">Khó</a-select-option>
            </a-select>
          </div>
        </div>

        <div class="mb-2">
          <label class="form-label small fw-bold mb-1">Các phương án trả lời (Tích vào ô chọn để đánh dấu đáp án đúng):</label>
          <div class="d-flex flex-column gap-2">
            <div v-for="(ans, idx) in singleQuestionForm.answers" :key="idx" class="d-flex align-items-center gap-2">
              <a-checkbox v-model:checked="ans.isCorrectAnswer" />
              <span class="small fw-bold text-secondary">{{ String.fromCharCode(65 + idx) }}:</span>
              <a-input v-model:value="ans.stringContent" placeholder="Nhập nội dung phương án..." />
            </div>
          </div>
        </div>

        <div class="mt-3">
          <label class="form-label small fw-bold">Giải thích đáp án (nếu có):</label>
          <a-textarea v-model:value="singleQuestionForm.explaination" :rows="2" placeholder="Giải thích vì sao đáp án này đúng..." />
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message } from 'ant-design-vue'
import { getAllCate } from '@/api/categories'
import { getQuestionsPaging, getQuestionKeys } from '@/api/questions'
import { getExamDetails, getExamsPaging, getExamQuestions, getExamKeys, saveExam } from '@/api/exams'

import type { Exam, ExamQuestion, ExamDto } from '@/models/exams'
import type { Question, QuestionAnswer } from '@/models/questions'

const route = useRoute()
const router = useRouter()

const isEditMode = computed(() => !!route.params.id)

interface Category {
  id: string
  name: string
  hasChildren?: boolean
}

// Data lists
const categories = ref<Category[]>([])
const bankQuestions = ref<Question[]>([])

// Import from exam dialog state
const importExamsList = ref<ExamDto[]>([])
const importExamTotalItems = ref(0)
const importExamLoading = ref(false)
const importExamRequest = ref({
  page: 1,
  size: 5,
  isPaging: true,
  key: "",
  filters: [] as any[],
  filterGroupType: 1 // FilterGroupType.And
})

// Form state
const formData = reactive({
  name: '',
  categoryId: undefined as string | undefined,
  duration: 45,
  accessType: 1, // Default: Private (Riêng tư = 1)
  description: '',
  contributeToBank: true
})

const questionsList = ref<(Question & { examQuestionId?: string })[]>([])
const selectedGotoIndex = ref<number | undefined>(undefined)

// Auto-add questions state
const autoAddModalOpen = ref(false)
const autoAddForm = reactive({
  totalCount: 10,
  categoryId: undefined as string | undefined,
  easyCount: 4,
  mediumCount: 4,
  hardCount: 2,
  shuffleOptions: true
})

// Bank select state
const bankModalOpen = ref(false)
const selectedBankQuestionIds = ref<string[]>([])
const bankFilter = reactive({
  search: '',
  categoryId: undefined as string | undefined,
  page: 1,
  pageSize: 8
})

// Import from exam state
const importExamModalOpen = ref(false)
const selectedImportExamId = ref<string | undefined>(undefined)
const importExamFilter = reactive({
  search: '',
  shuffleOptions: true
})

// Single question editor state
const singleQuestionModalOpen = ref(false)
const singleQuestionModalMode = ref<'create' | 'edit'>('create')
const singleQuestionModalIndex = ref<number | null>(null)

const singleQuestionForm = reactive<Question>({
  questionId: '',
  questionSlug: '',
  stringContent: '',
  explaination: '',
  attemptCount: 0,
  level: 0,
  type: 0,
  accessType: 1,
  isInBank: false,
  questionCategoryId: '',
  answers: []
})

const fetchCategories = async () => {
  try {
    const res = await getAllCate()
    if (res && res.isSuccess && res.data) {
      const items = res.data.items || []
      categories.value = items.map((cat: any) => ({
        id: cat.questionCategoryId,
        name: cat.questionCategoryName,
        hasChildren: items.some((c: any) => c.parentId === cat.questionCategoryId)
      }))
    }
  } catch (error) {
    console.error('Lỗi tải danh mục:', error)
  }
}

// Fetch correct answers keys and set correctness for specified question IDs
const fetchAndFillAnswers = async (questionIds: string[]) => {
  if (!questionIds || questionIds.length === 0) return
  try {
    const resKeys = await getQuestionKeys(questionIds)
    const correctKeysMap = (resKeys && resKeys.isSuccess && resKeys.data?.correctMap) ? resKeys.data.correctMap : {}
    
    questionsList.value.forEach(q => {
      if (questionIds.includes(q.questionId)) {
        const correctIds = correctKeysMap[q.questionId] || []
        q.answers.forEach((ans: any) => {
          ans.isCorrectAnswer = correctIds.includes(ans.questionAnswerId)
        })
      }
    })
  } catch (error) {
    console.error('Lỗi khi tải đáp án đúng cho câu hỏi:', error)
  }
}

const fetchBankQuestions = async () => {
  try {
    const res = await getQuestionsPaging({
      page: 1,
      size: 1000,
      isPaging: true
    })
    if (res && res.isSuccess && res.data) {
      bankQuestions.value = (res.data.items || []).map((item: any) => ({
        questionId: item.questionId || item.id,
        questionSlug: item.slug || '',
        stringContent: item.stringContent || '',
        explaination: item.explanation || item.explaination || '',
        level: item.level || 0,
        type: item.type || 0,
        accessType: item.accessType || 1,
        questionCategoryId: item.questionCategoryId,
        answers: (item.answers || []).map((a: any, aIdx: number) => ({
          questionAnswerId: a.questionAnswerId || '',
          stringContent: a.stringContent || '',
          isCorrectAnswer: false,
          questionId: item.questionId || item.id,
          orderInList: a.orderInList || (aIdx + 1)
        }))
      }))
    }
  } catch (error) {
    console.error('Lỗi tải ngân hàng câu hỏi:', error)
  }
}

const fetchImportExams = async () => {
  importExamLoading.value = true
  try {
    const res = await getExamsPaging(importExamRequest.value)
    if (res && res.isSuccess && res.data) {
      importExamsList.value = res.data.items || []
      importExamTotalItems.value = res.data.total || 0
    }
  } catch (error) {
    console.error('Lỗi tải danh sách đề thi để import:', error)
    message.error('Không thể tải danh sách đề thi.')
  } finally {
    importExamLoading.value = false
  }
}

let importExamSearchTimeout: any = null
watch(() => importExamFilter.search, (newVal) => {
  if (importExamSearchTimeout) clearTimeout(importExamSearchTimeout)
  importExamSearchTimeout = setTimeout(() => {
    importExamRequest.value.key = newVal || ""
    importExamRequest.value.page = 1
    fetchImportExams()
  }, 400)
})

const goBack = () => {
  router.push('/personal/exams')
}

const getCategoryName = (id?: string) => {
  if (!id) return '--'
  const cat = categories.value.find(c => c.id === id)
  return cat ? cat.name : 'Chuyên đề chung'
}

const getLevelText = (lvl: number) => {
  if (lvl === 2) return 'Khó'
  if (lvl === 1) return 'Trung bình'
  return 'Dễ'
}

const getLevelBadgeClass = (lvl: number) => {
  if (lvl === 2) return 'badge bg-danger text-white fs-8'
  if (lvl === 1) return 'badge bg-warning text-dark fs-8'
  return 'badge bg-success text-white fs-8'
}

const scrollToQuestion = (idx: number) => {
  if (idx === undefined) return
  const el = document.getElementById('selected-question-card-' + idx)
  if (el) {
    el.scrollIntoView({ behavior: 'smooth', block: 'center' })
  }
}

const moveQuestionUp = (idx: number) => {
  if (idx <= 0 || idx >= questionsList.value.length) return
  const temp = questionsList.value[idx]
  if (temp) {
    questionsList.value[idx] = questionsList.value[idx - 1] as Question
    questionsList.value[idx - 1] = temp
  }
}

const moveQuestionDown = (idx: number) => {
  if (idx < 0 || idx >= questionsList.value.length - 1) return
  const temp = questionsList.value[idx]
  if (temp) {
    questionsList.value[idx] = questionsList.value[idx + 1] as Question
    questionsList.value[idx + 1] = temp
  }
}

const removeSelectedQuestion = (idx: number) => {
  questionsList.value.splice(idx, 1)
  message.success('Đã gỡ câu hỏi khỏi đề!')
}

const shuffleChoices = (q: Question) => {
  const choices = [...q.answers]
  for (let i = choices.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1))
    const temp = choices[i]
    if (temp && choices[j]) {
      choices[i] = choices[j] as QuestionAnswer
      choices[j] = temp
    }
  }
  q.answers = choices
}

const handleAutoAdd = () => {
  const matching = bankQuestions.value.filter(q => {
    if (autoAddForm.categoryId && q.questionCategoryId !== autoAddForm.categoryId) return false
    return true
  })

  if (matching.length === 0) {
    message.error('Không tìm thấy câu hỏi nào thuộc chủ đề này trong ngân hàng!')
    return
  }

  const easy = matching.filter(q => q.level === 0)
  const medium = matching.filter(q => q.level === 1)
  const hard = matching.filter(q => q.level === 2)

  const draw = (arr: Question[], count: number) => {
    const list = [...arr]
    const result: Question[] = []
    for (let i = 0; i < count && list.length > 0; i++) {
      const idx = Math.floor(Math.random() * list.length)
      const item = list.splice(idx, 1)[0]
      if (item) {
        result.push(item)
      }
    }
    return result
  }

  const selected = [
    ...draw(easy, autoAddForm.easyCount),
    ...draw(medium, autoAddForm.mediumCount),
    ...draw(hard, autoAddForm.hardCount)
  ]

  if (selected.length < autoAddForm.totalCount) {
    const remainingCount = autoAddForm.totalCount - selected.length
    const unused = matching.filter(q => !selected.some(s => s.questionId === q.questionId))
    selected.push(...draw(unused, remainingCount))
  }

  if (selected.length === 0) {
    message.warning('Không thể rút tự động vì chủ đề rỗng!')
    return
  }

  if (autoAddForm.shuffleOptions) {
    selected.forEach(q => shuffleChoices(q))
  }

  let addedCount = 0
  const newlyAddedIds: string[] = []
  selected.forEach(q => {
    if (!questionsList.value.some(el => el.questionId === q.questionId)) {
      questionsList.value.push(JSON.parse(JSON.stringify(q)))
      newlyAddedIds.push(q.questionId)
      addedCount++
    }
  })

  if (newlyAddedIds.length > 0) {
    fetchAndFillAnswers(newlyAddedIds)
  }

  message.success(`Đã chọn ngẫu nhiên và thêm ${addedCount} câu hỏi mới vào đề!`)
  autoAddModalOpen.value = false
}

const openBankModal = () => {
  selectedBankQuestionIds.value = questionsList.value.map(q => q.questionId)
  bankFilter.categoryId = undefined
  bankFilter.page = 1
  bankModalOpen.value = true
}

const filteredBankQuestions = computed(() => {
  return bankQuestions.value.filter(q => {
    if (bankFilter.search && !(q.stringContent || '').toLowerCase().includes(bankFilter.search.toLowerCase())) return false
    if (bankFilter.categoryId && q.questionCategoryId !== bankFilter.categoryId) return false
    return true
  })
})

const paginatedBankQuestions = computed(() => {
  const start = (bankFilter.page - 1) * bankFilter.pageSize
  return filteredBankQuestions.value.slice(start, start + bankFilter.pageSize)
})

const isAllBankSelected = computed(() => {
  const pageIds = paginatedBankQuestions.value.map(q => q.questionId)
  if (pageIds.length === 0) return false
  return pageIds.every(id => selectedBankQuestionIds.value.includes(id))
})

const toggleSelectAllBank = () => {
  const pageIds = paginatedBankQuestions.value.map(q => q.questionId)
  if (isAllBankSelected.value) {
    selectedBankQuestionIds.value = selectedBankQuestionIds.value.filter(id => !pageIds.includes(id))
  } else {
    pageIds.forEach(id => {
      if (!selectedBankQuestionIds.value.includes(id)) {
        selectedBankQuestionIds.value.push(id)
      }
    })
  }
}

const toggleSelectBankQuestion = (id: string) => {
  const idx = selectedBankQuestionIds.value.indexOf(id)
  if (idx > -1) {
    selectedBankQuestionIds.value.splice(idx, 1)
  } else {
    selectedBankQuestionIds.value.push(id)
  }
}

const handleBankAdd = () => {
  const added: Question[] = []
  selectedBankQuestionIds.value.forEach(id => {
    const orig = bankQuestions.value.find(q => q.questionId === id)
    if (orig) {
      added.push(JSON.parse(JSON.stringify(orig)))
    }
  })

  let addedCount = 0
  const newlyAddedIds: string[] = []
  added.forEach(q => {
    if (!questionsList.value.some(el => el.questionId === q.questionId)) {
      questionsList.value.push(q)
      newlyAddedIds.push(q.questionId)
      addedCount++
    }
  })

  if (newlyAddedIds.length > 0) {
    fetchAndFillAnswers(newlyAddedIds)
  }

  message.success(`Đã thêm ${addedCount} câu hỏi từ ngân hàng!`)
  bankModalOpen.value = false
}

const openImportExamModal = () => {
  selectedImportExamId.value = undefined
  importExamFilter.search = ''
  importExamRequest.value.key = ''
  importExamRequest.value.page = 1
  importExamModalOpen.value = true
  fetchImportExams()
}

const handleImportExam = async () => {
  if (!selectedImportExamId.value) {
    message.error('Vui lòng chọn 1 đề thi gốc để chèn!')
    return
  }

  try {
    const res = await getExamQuestions(selectedImportExamId.value)
    if (res && res.isSuccess && res.data) {
      const examQuestions = res.data || []
      if (examQuestions.length === 0) {
        message.error('Đề thi này không có câu hỏi nào!')
        return
      }

      const [resKeys] = await Promise.all([
        getExamKeys(selectedImportExamId.value)
      ])

      const correctKeysMap = (resKeys && resKeys.isSuccess && resKeys.data?.correctMap) ? resKeys.data.correctMap : {}

      let count = 0
      examQuestions.forEach((q: any) => {
        const targetQuestionId = 'imported_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5)
        const rawAnswers = q.answers || []
        const correctIds = correctKeysMap[q.examQuestionId] || []

        const normalizedQ: Question = {
          questionId: targetQuestionId,
          questionSlug: q.questionSlug || '',
          stringContent: q.stringContent || '',
          explaination: q.explaination || q.explanation || '',
          attemptCount: q.attemptCount || 0,
          level: q.level || 0,
          type: q.type || 0,
          accessType: q.accessType || 1,
          isInBank: q.isInBank || false,
          questionCategoryId: q.questionCategoryId || '',
          answers: rawAnswers.map((ans: any, aIdx: number) => ({
            questionAnswerId: '',
            stringContent: ans.stringContent || '',
            isCorrectAnswer: correctIds.includes(ans.questionAnswerId),
            questionId: targetQuestionId,
            orderInList: aIdx + 1
          }))
        }
        
        if (importExamFilter.shuffleOptions) {
          shuffleChoices(normalizedQ)
        }

        questionsList.value.push(normalizedQ)
        count++
      })

      message.success(`Đã sao chép và chèn ${count} câu hỏi vào đề hiện tại!`)
      importExamModalOpen.value = false
    } else {
      message.error('Không thể tải danh sách câu hỏi của đề thi này.')
    }
  } catch (error) {
    console.error('Lỗi khi chèn câu hỏi từ đề thi:', error)
    message.error('Đã xảy ra lỗi khi chèn câu hỏi.')
  }
}

const resetSingleQuestionForm = () => {
  singleQuestionForm.questionId = ''
  singleQuestionForm.questionSlug = ''
  singleQuestionForm.stringContent = ''
  singleQuestionForm.explaination = ''
  singleQuestionForm.attemptCount = 0
  singleQuestionForm.level = 0
  singleQuestionForm.type = 0
  singleQuestionForm.accessType = 1
  singleQuestionForm.isInBank = false
  singleQuestionForm.questionCategoryId = formData.categoryId || categories.value[0]?.id || ''
  singleQuestionForm.answers = [
    { questionAnswerId: '', stringContent: '', isCorrectAnswer: false, orderInList: 1 },
    { questionAnswerId: '', stringContent: '', isCorrectAnswer: false, orderInList: 2 },
    { questionAnswerId: '', stringContent: '', isCorrectAnswer: false, orderInList: 3 },
    { questionAnswerId: '', stringContent: '', isCorrectAnswer: false, orderInList: 4 }
  ]
}

const openCreateQuestionModal = () => {
  singleQuestionModalMode.value = 'create'
  singleQuestionModalIndex.value = null
  resetSingleQuestionForm()
  singleQuestionModalOpen.value = true
}

const openEditQuestionModal = (q: Question, idx: number) => {
  singleQuestionModalMode.value = 'edit'
  singleQuestionModalIndex.value = idx
  
  singleQuestionForm.questionId = q.questionId
  singleQuestionForm.questionSlug = q.questionSlug || ''
  singleQuestionForm.stringContent = q.stringContent || ''
  singleQuestionForm.explaination = q.explaination || ''
  singleQuestionForm.level = q.level
  singleQuestionForm.type = q.type
  singleQuestionForm.accessType = q.accessType
  singleQuestionForm.questionCategoryId = q.questionCategoryId
  singleQuestionForm.answers = JSON.parse(JSON.stringify(q.answers || []))
  
  while (singleQuestionForm.answers.length < 4) {
    singleQuestionForm.answers.push({
      questionAnswerId: '',
      stringContent: '',
      isCorrectAnswer: false,
      orderInList: singleQuestionForm.answers.length + 1
    })
  }
  
  singleQuestionModalOpen.value = true
}

const saveSingleQuestion = () => {
  if (!singleQuestionForm.stringContent || !singleQuestionForm.stringContent.trim()) {
    message.error('Vui lòng nhập nội dung câu hỏi!')
    return
  }

  const filledAnswers = singleQuestionForm.answers.filter(a => a.stringContent && a.stringContent.trim())
  if (filledAnswers.length < 2) {
    message.error('Câu hỏi phải có ít nhất 2 đáp án!')
    return
  }

  const hasCorrect = singleQuestionForm.answers.some(a => a.isCorrectAnswer)
  if (!hasCorrect) {
    message.error('Vui lòng chọn ít nhất 1 đáp án đúng!')
    return
  }

  const cleanAnswers = singleQuestionForm.answers
    .filter(a => a.stringContent && a.stringContent.trim())
    .map((a, aIdx) => ({
      questionAnswerId: a.questionAnswerId || '00000000-0000-0000-0000-000000000000',
      stringContent: a.stringContent!.trim(),
      isCorrectAnswer: a.isCorrectAnswer,
      questionId: singleQuestionForm.questionId || '',
      orderInList: aIdx + 1
    }))

  if (singleQuestionModalMode.value === 'create') {
    const newQ: Question = {
      questionId: 'q_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5),
      questionSlug: 'manual-draft',
      stringContent: singleQuestionForm.stringContent.trim(),
      explaination: singleQuestionForm.explaination ? singleQuestionForm.explaination.trim() : 'Giải thích chi tiết của câu hỏi.',
      attemptCount: 0,
      level: singleQuestionForm.level,
      type: singleQuestionForm.type,
      accessType: singleQuestionForm.accessType,
      isInBank: false,
      questionCategoryId: singleQuestionForm.questionCategoryId,
      answers: cleanAnswers
    }
    questionsList.value.push(newQ)
    message.success('Đã thêm câu hỏi mới vào đề!')
  } else {
    const idx = singleQuestionModalIndex.value
    if (idx !== null && questionsList.value[idx]) {
      const existingQ = questionsList.value[idx] as Question
      existingQ.stringContent = singleQuestionForm.stringContent.trim()
      existingQ.explaination = singleQuestionForm.explaination ? singleQuestionForm.explaination.trim() : ''
      existingQ.level = singleQuestionForm.level
      existingQ.questionCategoryId = singleQuestionForm.questionCategoryId
      existingQ.answers = cleanAnswers
      existingQ.questionId = 'manual_edited_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5)
      message.success('Đã cập nhật câu hỏi!')
    }
  }

  singleQuestionModalOpen.value = false
}

const generateUUID = (): string => {
  try {
    return self.crypto.randomUUID()
  } catch (e) {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}

const ensureGuid = (id: string): string => {
  const isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(id)
  return isGuid ? id : '00000000-0000-0000-0000-000000000000'
}

const saveForm = async (isDraft = false) => {
  if (!formData.name.trim()) {
    message.error('Vui lòng nhập tên đề thi!')
    return
  }

  if (questionsList.value.length > 100) {
    message.error('Mỗi đề thi chỉ được phép chứa tối đa 100 câu hỏi. Vui lòng giảm số lượng câu hỏi!')
    return
  }

  if (!isDraft && questionsList.value.length === 0) {
    message.error('Đề thi phải chứa ít nhất 1 câu hỏi! Vui lòng chọn hoặc nạp thêm câu hỏi.')
    return
  }

  const editId = route.params.id as string | undefined
  let parsedExamId: string | undefined = undefined
  if (editId) {
    const isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(editId)
    if (isGuid) {
      parsedExamId = editId
    }
  }

  const payload: Exam = {
  
    examId: parsedExamId || '00000000-0000-0000-0000-000000000000',
    name: formData.name.trim(),
    description: formData.description.trim(),
    questionCategoryId: formData.categoryId || '',
    durationMin: formData.duration,
    accessType: formData.accessType,
    isDraft: isDraft,
    contributeToBank: formData.contributeToBank,
    learnMsUserId: '00000000-0000-0000-0000-000000000000',
    questions: questionsList.value.map((q, idx) => {
      const targetExamQuestionId = (parsedExamId && q.examQuestionId) ? q.examQuestionId : '00000000-0000-0000-0000-000000000000'
      
      return {
        examQuestionId: targetExamQuestionId,
        stringContent: q.stringContent || '',
        orderInExam: idx + 1,
        explaination: q.explaination || 'Giải thích chi tiết của câu hỏi.',
        level: q.level,
        type: q.type,
        answers: (q.answers || []).map((ans, aIdx) => ({
          questionAnswerId: ensureGuid(ans.questionAnswerId || ''),
          stringContent: ans.stringContent || '',
          isCorrectAnswer: ans.isCorrectAnswer,
          orderInList: aIdx + 1
        }))
      }
    })
  }

  let hideLoading: any = null
  hideLoading = message.loading('Đang lưu đề thi...', 0)

  try {
    const res = await saveExam(payload, !!parsedExamId)
    if (hideLoading) {
      hideLoading()
    }
    if (res && res.isSuccess) {
      if (editId) {
        message.success(isDraft ? 'Đã cập nhật bản nháp thành công!' : 'Đã cập nhật dữ liệu thành công!')
      } else {
        message.success(isDraft ? `Đã lưu bản nháp đề thi "${formData.name.trim()}" thành công!` : `Đã tạo đề thi "${formData.name.trim()}" thành công!`)
      }
      router.push('/personal/exams')
    } else {
      message.error(res.errorMessage || 'Lỗi khi lưu đề thi.')
    }
  } catch (error) {
    if (hideLoading) {
      hideLoading()
    }
    console.error('Lỗi lưu đề thi:', error)
    message.error('Đã xảy ra lỗi khi kết nối máy chủ để lưu đề thi.')
  }
}

onMounted(async () => {
  await fetchCategories()
  await fetchBankQuestions()

  if (categories.value.length > 0 && categories.value[0]) {
    formData.categoryId = categories.value[0].id
    autoAddForm.categoryId = categories.value[0].id
  }

  const editId = route.params.id as string | undefined
  if (editId) {
    try {
      const res = await getExamDetails(editId)
      if (res && res.isSuccess && res.data) {
        const existingExam = res.data
        formData.name = existingExam.name
        formData.description = existingExam.description || ''
        formData.categoryId = existingExam.questionCategoryId
        formData.duration = existingExam.durationMin
        formData.accessType = existingExam.accessType
        
        try {
          const [resQuestions, resKeys] = await Promise.all([
            getExamQuestions(editId),
            getExamKeys(editId)
          ])
          if (resQuestions && resQuestions.isSuccess && resQuestions.data) {
            const correctKeysMap = (resKeys && resKeys.isSuccess && resKeys.data?.correctMap) ? resKeys.data.correctMap : {}
            
            questionsList.value = resQuestions.data.map((q: any) => {
              const qId = q.examQuestionId
              const rawAnswers = q.answers || []
              const correctIds = correctKeysMap[qId] || []
              
              const mappedAnswers = rawAnswers.map((a: any, aIdx: number) => ({
                questionAnswerId: a.questionAnswerId || '',
                stringContent: a.stringContent || '',
                isCorrectAnswer: correctIds.includes(a.questionAnswerId),
                questionId: qId,
                orderInList: a.orderInList || (aIdx + 1)
              }))

              return {
                questionId: qId,
                examQuestionId: qId,
                questionSlug: q.questionSlug || '',
                stringContent: q.stringContent || '',
                explaination: q.explaination || '',
                level: q.level || 0,
                type: q.type || 0,
                accessType: q.accessType || 1,
                questionCategoryId: q.questionCategoryId || '',
                answers: mappedAnswers
              }
            })
          }
        } catch (error) {
          console.error('Lỗi tải câu hỏi của đề thi cần chỉnh sửa:', error)
        }
      } else {
        message.error('Không tìm thấy dữ liệu đề thi cần chỉnh sửa!')
      }
    } catch (error) {
      console.error('Lỗi tải thông tin đề thi cần chỉnh sửa:', error)
      message.error('Đã xảy ra lỗi khi tải thông tin đề thi.')
    }
  }
})
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.text-indigo {
  color: #6366f1;
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
  border: 1px solid #6366f1;
  color: #6366f1;
  background-color: transparent;
  transition: all 0.2s ease;
}

.btn-outline-indigo:hover {
  background-color: rgba(99, 102, 241, 0.05);
  color: #4f46e5;
  border-color: #4f46e5;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.08);
}

.bg-light-soft {
  background-color: #f8fafc;
}

.hover-pointer:hover {
  cursor: pointer;
  background-color: rgba(99, 102, 241, 0.02) !important;
}

.hover-up {
  transition: all 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-2px);
}

.text-truncate-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
