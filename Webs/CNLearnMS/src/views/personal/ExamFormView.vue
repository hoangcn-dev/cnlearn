<template>
  <div class="exam-form-view py-3 small-font">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-3">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item"><router-link to="/personal/exams">Quản lý đề & kỳ thi</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">
          <template v-if="route.params.id">
            {{ isQuizMode ? 'Chỉnh sửa bài kiểm tra / kỳ thi' : 'Chỉnh sửa đề thi' }}
          </template>
          <template v-else>
            {{ isQuizMode ? 'Tạo bài kiểm tra / Kỳ thi mới' : 'Biên soạn đề thi mới' }}
          </template>
        </li>
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
          <span v-if="route.params.id">
            {{ isQuizMode ? 'Chỉnh sửa bài kiểm tra / kỳ thi' : 'Chỉnh sửa đề thi' }}
          </span>
          <span v-else>
            {{ isQuizMode ? 'Cấu hình & Thiết lập bài kiểm tra mới' : 'Biên soạn thông tin đề thi mới' }}
          </span>
        </h1>
      </div>
      <!-- Nút lưu phía trên cùng -->
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

    <!-- Toggle Mode Selector at Top -->
    <div class="card border-0 rounded-4 shadow-sm bg-white p-3 mb-4">
      <div class="d-flex align-items-center gap-3">
        <span class="fw-bold text-dark-blue small">Hình thức khởi tạo:</span>
        <a-radio-group v-model:value="creationType" button-style="solid">
          <a-radio-button value="exam">📝 Đề thi (Chỉ soạn câu hỏi)</a-radio-button>
          <a-radio-button value="quiz">⏰ Bài kiểm tra / Kỳ thi (Có lịch trình & chống gian lận)</a-radio-button>
        </a-radio-group>
      </div>
    </div>

    <div class="row g-4">
      <!-- Cấu hình thông tin cơ bản & bảo mật (Phần trên) -->
      <div class="col-12">
        <div class="card border-0 rounded-4 shadow-sm bg-white p-4">
          <h5 class="fw-bold text-dark-blue mb-3 pb-2 border-bottom">⚙️ Cấu hình thông tin</h5>
          
          <a-form layout="vertical">
            <div class="row g-3">
              <!-- Cột trái: Thông tin cơ bản -->
              <div :class="isQuizMode ? 'col-lg-6' : 'col-lg-7'">
                <!-- Tên đề/bài thi -->
                <a-form-item :label="isQuizMode ? 'Tên bài kiểm tra / Kỳ thi:' : 'Tên đề thi:'" required class="mb-2">
                  <a-input v-model:value="formData.name" placeholder="Ví dụ: Đề kiểm tra học kỳ 1 môn Tin học lớp 12" />
                </a-form-item>

                <div class="row g-2 mb-2">
                  <div class="col-md-6">
                    <!-- Danh mục môn học -->
                    <a-form-item label="Danh mục môn học:" required class="mb-0">
                      <a-select v-model:value="formData.categoryId" placeholder="Chọn danh mục môn học">
                        <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id">
                          {{ cat.name }}
                        </a-select-option>
                      </a-select>
                    </a-form-item>
                  </div>
                  <div class="col-md-6">
                    <!-- Thời gian làm bài -->
                    <a-form-item label="Thời gian làm bài (phút):" required class="mb-0">
                      <a-input-number v-model:value="formData.duration" :min="1" :max="360" style="width: 100%" />
                    </a-form-item>
                  </div>
                </div>

                <!-- Mô tả -->
                <a-form-item label="Mô tả chi tiết (nếu có):" class="mb-0">
                  <a-textarea v-model:value="formData.description" :rows="2" placeholder="Mô tả nội dung, quy định phòng thi..." />
                </a-form-item>
              </div>

              <!-- Cột phải: Thiết lập Bảo mật (Quiz) hoặc Tiện ích truy cập (Exam) -->
              <div :class="isQuizMode ? 'col-lg-6' : 'col-lg-5'">
                <!-- DÀNH RIÊNG CHO CHẾ ĐỘ BÀI KIỂM TRA (QUIZ) -->
                <div v-if="isQuizMode" class="bg-light-soft p-3 rounded-4 h-100 border border-light d-flex flex-column justify-content-between">
                  <h6 class="fw-bold text-dark-blue mb-2 pb-1 border-bottom" style="font-size: 0.85rem;">🛠️ Thiết lập Kỳ thi & Bảo mật</h6>
                  
                  <div class="row g-2 flex-grow-1">
                    <div class="col-md-6">
                      <!-- Đối tượng tham gia -->
                      <a-form-item label="Đối tượng tham gia (Tạm khóa):" class="mb-2">
                        <a-input v-model:value="formData.targetGroup" placeholder="Tính năng đang tạm khóa..." size="small" disabled />
                      </a-form-item>

                      <!-- Thời gian bắt đầu -->
                      <a-form-item label="Thời gian bắt đầu:" class="mb-0">
                        <a-radio-group v-model:value="formData.startType" class="w-100 mb-1" size="small">
                          <a-radio value="now">Ngay lập tức</a-radio>
                          <a-radio value="custom">Chọn lịch</a-radio>
                        </a-radio-group>
                        <div v-if="formData.startType === 'custom'">
                          <input type="datetime-local" class="form-control form-control-xs py-1" v-model="formData.startDate" style="font-size: 0.8rem;" />
                        </div>
                      </a-form-item>
                    </div>

                    <div class="col-md-6 border-start ps-3">
                      <!-- Cơ chế chống gian lận -->
                      <div class="mb-2">
                        <label class="form-label small fw-bold text-dark-blue mb-1" style="font-size: 0.75rem;">Chống gian lận:</label>
                        <div class="d-flex flex-column gap-1 ms-1">
                          <a-checkbox v-model:checked="formData.antiCheat.lockBrowser"><span style="font-size: 0.75rem;">Khóa màn hình</span></a-checkbox>
                          <a-checkbox v-model:checked="formData.antiCheat.shuffleQuestions"><span style="font-size: 0.75rem;">Đảo đề thi</span></a-checkbox>
                          <a-checkbox v-model:checked="formData.antiCheat.disableCopyPaste"><span style="font-size: 0.75rem;">Cấm copy/paste</span></a-checkbox>
                          <a-checkbox v-model:checked="formData.antiCheat.fullscreen"><span style="font-size: 0.75rem;">Yêu cầu toàn màn hình</span></a-checkbox>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Tiện ích phụ -->
                  <div class="d-flex flex-wrap gap-x-3 gap-y-1 mt-2 pt-2 border-top align-items-center" style="font-size: 0.75rem;">
                    <a-checkbox v-model:checked="formData.contributeToBank"><span style="font-size: 0.75rem;">Đóng góp ngân hàng</span></a-checkbox>
                    <a-checkbox v-model:checked="formData.allowLateJoin"><span style="font-size: 0.75rem;">Bù giờ vào muộn</span></a-checkbox>
                    <a-checkbox v-model:checked="formData.allowLateSubmit"><span style="font-size: 0.75rem;">Nhận nộp muộn</span></a-checkbox>
                    <a-checkbox v-model:checked="formData.publicLeaderboard"><span style="font-size: 0.75rem;">BXH công khai</span></a-checkbox>
                  </div>
                </div>

                <!-- DÀNH CHO CHẾ ĐỘ ĐỀ THI (EXAM) -->
                <div v-else class="bg-light-soft p-3 rounded-4 h-100 border border-light d-flex flex-column justify-content-center">
                  <h6 class="fw-bold text-dark-blue mb-3 pb-1 border-bottom" style="font-size: 0.85rem;">⚙️ Thiết lập truy cập & Lưu trữ</h6>
                  
                  <a-form-item label="Phạm vi truy cập đề thi:" class="mb-3">
                    <a-radio-group v-model:value="formData.accessType">
                      <a-radio :value="1">Công khai (Mọi người đều có thể tìm thấy)</a-radio>
                      <a-radio :value="0">Riêng tư (Chỉ của riêng bạn)</a-radio>
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

      <!-- Danh sách câu hỏi (Phần dưới) -->
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

          <!-- 4 Nút thêm câu hỏi -->
          <div class="row g-2 mb-4">
            <div class="col-sm-3 col-6">
              <button class="btn btn-outline-indigo w-100 py-2 rounded-3 d-flex flex-column align-items-center justify-content-center gap-2 btn-sm hover-up text-indigo" @click="autoAddModalOpen = true">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"></polygon></svg>
                <span class="fw-bold text-nowrap" style="font-size: 0.73rem;">Thêm tự động</span>
              </button>
            </div>
            <div class="col-sm-3 col-6">
              <button class="btn btn-outline-indigo w-100 py-2 rounded-3 d-flex flex-column align-items-center justify-content-center gap-2 btn-sm hover-up text-indigo" @click="openBankModal">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path></svg>
                <span class="fw-bold text-nowrap" style="font-size: 0.73rem;">Chọn từ ngân hàng</span>
              </button>
            </div>
            <div class="col-sm-3 col-6">
              <button class="btn btn-outline-indigo w-100 py-2 rounded-3 d-flex flex-column align-items-center justify-content-center gap-2 btn-sm hover-up text-indigo" @click="pdfModalOpen = true">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
                <span class="fw-bold text-nowrap" style="font-size: 0.73rem;">Phân tích từ PDF</span>
              </button>
            </div>
            <div class="col-sm-3 col-6">
              <button class="btn btn-outline-indigo w-100 py-2 rounded-3 d-flex flex-column align-items-center justify-content-center gap-2 btn-sm hover-up text-indigo" @click="openImportExamModal">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><line x1="9" y1="3" x2="9" y2="21"></line></svg>
                <span class="fw-bold text-nowrap" style="font-size: 0.73rem;">Thêm từ đề</span>
              </button>
            </div>
          </div>

          <!-- Đi nhanh đến câu hỏi / Xem trước các câu hỏi đã chọn -->
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
                Xóa tất cả
              </button>
            </div>

            <!-- Empty questions list state -->
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
                :key="q.id" 
                :id="'selected-question-card-' + index"
                class="card card-body p-3 border border-light bg-light-soft position-relative rounded-3 shadow-none mb-0"
              >
                <!-- Row controls -->
                <div class="d-flex align-items-center justify-content-between mb-2 pb-2 border-bottom flex-wrap gap-2">
                  <div class="d-flex align-items-center gap-2">
                    <span class="badge bg-indigo-soft text-dark-blue fw-bold px-3 py-1 rounded-pill fs-8">Câu {{ index + 1 }}</span>
                    <span :class="getLevelBadgeClass(q.level)">{{ getLevelText(q.level) }}</span>
                    <span class="text-muted fs-8">{{ getCategoryName(q.categoryIds[0]) }}</span>
                  </div>

                  <!-- Reorder buttons, Edit & Delete icon -->
                  <div class="d-flex align-items-center gap-2">
                    <button class="btn btn-xs btn-outline-indigo d-flex align-items-center justify-content-center rounded-circle" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" @click="openEditQuestionModal(q, index)" title="Chỉnh sửa câu hỏi">
                      <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
                    </button>
                    <button class="btn btn-xs btn-outline-secondary d-flex align-items-center justify-content-center rounded-circle" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" :disabled="index === 0" @click="moveQuestionUp(index)" title="Di chuyển lên">
                      <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="18 15 12 9 6 15"></polyline></svg>
                    </button>
                    <button class="btn btn-xs btn-outline-secondary d-flex align-items-center justify-content-center rounded-circle" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" :disabled="index === questionsList.length - 1" @click="moveQuestionDown(index)" title="Di chuyển xuống">
                      <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="6 9 12 15 18 9"></polyline></svg>
                    </button>
                    <button class="btn btn-xs btn-outline-danger d-flex align-items-center justify-content-center rounded-circle ms-1" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" @click="removeSelectedQuestion(index)" title="Xóa câu hỏi khỏi đề">
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
        <!-- Số lượng câu hỏi -->
        <div class="mb-3">
          <label class="form-label small fw-bold">Số lượng câu muốn thêm:</label>
          <a-input-number v-model:value="autoAddForm.totalCount" :min="1" :max="100" style="width: 100%" />
        </div>
        <!-- Chọn môn học/danh mục -->
        <div class="mb-3">
          <label class="form-label small fw-bold">Chủ đề câu hỏi:</label>
          <a-select v-model:value="autoAddForm.categoryId" placeholder="Chọn chủ đề" style="width: 100%">
            <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id">
              {{ cat.name }}
            </a-select-option>
          </a-select>
        </div>
        <!-- Phân bổ độ khó -->
        <div class="mb-3 bg-light-soft p-3 rounded-3">
          <label class="form-label small fw-bold text-dark-blue mb-2">Phân bổ độ khó (Tổng số câu):</label>
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
        <!-- Đảo đáp án -->
        <div class="mb-1">
          <a-checkbox v-model:checked="autoAddForm.shuffleOptions">🔀 Tự động đảo thứ tự các đáp án (A, B, C, D)</a-checkbox>
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
        <!-- Search & Filter Bar -->
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
              <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </a-select-option>
            </a-select>
          </div>
        </div>

        <!-- Table list of matching questions -->
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
              <tr v-for="q in paginatedBankQuestions" :key="q.id" class="hover-pointer" @click.stop="toggleSelectBankQuestion(q.id)">
                <td class="text-center" @click.stop>
                  <a-checkbox :checked="selectedBankQuestionIds.includes(q.id)" @change="toggleSelectBankQuestion(q.id)" />
                </td>
                <td class="text-truncate-2" style="max-width: 380px;">{{ q.stringContent }}</td>
                <td class="text-muted text-truncate" style="max-width: 120px;">{{ getCategoryName(q.categoryIds[0]) }}</td>
                <td class="text-center">
                  <span :class="getLevelBadgeClass(q.level)">{{ getLevelText(q.level) }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
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

    <!-- MODAL 3: PHÂN TÍCH TỪ PDF/WORD (SPLIT SCREEN) -->
    <a-modal
      v-model:open="pdfModalOpen"
      title="Phân tích & Tách câu hỏi tự động từ file tài liệu PDF / Word"
      :footer="null"
      width="96%"
      :z-index="1100"
      style="top: 20px; margin-bottom: 0;"
      :body-style="{ height: 'calc(100vh - 120px)', display: 'flex', flexDirection: 'column' }"
    >
      <input type="file" ref="fileInput" class="d-none" @change="handleFileUpload" />
      <div class="row g-3 flex-grow-1 overflow-hidden h-100 py-2">

        <!-- Split Left: File Upload & PDF Document Preview -->
        <div class="col-md-5 d-flex flex-column h-100 border-end pe-3 overflow-hidden">
          <!-- Big drag drop uploader, shown only when no file is uploaded yet -->
          <div v-if="!uploadedFileName" class="card card-body p-4 border border-dashed rounded-4 bg-light-soft text-center justify-content-center align-items-center flex-shrink-0 mb-3">
            <svg xmlns="http://www.w3.org/2000/svg" width="36" height="36" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-indigo mb-2"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="17 8 12 3 7 8"></polyline><line x1="12" y1="3" x2="12" y2="15"></line></svg>
            <h6 class="fw-bold text-dark-blue mb-1" style="font-size: 0.85rem;">Kéo thả file đề thi PDF hoặc Word vào đây</h6>
            <p class="text-secondary small mb-3">Hỗ trợ các định dạng .pdf, .docx, .doc, .txt tối đa 25MB</p>
            <button class="btn btn-indigo text-white btn-sm px-4 py-2 rounded-3" @click="fileInput?.click()">
              Chọn file từ máy tính
            </button>
          </div>

          <!-- Small collapsed upload banner when file is active, freeing space for document preview -->
          <div v-else class="card card-body p-2 px-3 border rounded-3 bg-light-soft d-flex flex-row align-items-center justify-content-between flex-shrink-0 mb-3">
            <div class="d-flex align-items-center gap-2 text-truncate">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="text-success flex-shrink-0"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
              <span class="small fw-bold text-dark-blue text-truncate" style="max-width: 180px;">📄 {{ uploadedFileName }}</span>
            </div>
            <button class="btn btn-link btn-xs text-indigo p-0 text-decoration-none fw-bold small flex-shrink-0" @click="fileInput?.click()">
              Đổi file khác
            </button>
          </div>

          <!-- PDF Previewer Mockup -->
          <div class="flex-grow-1 overflow-y-auto bg-secondary bg-opacity-10 rounded-3 p-3 border d-flex flex-column align-items-center gap-3">
            <div v-if="!uploadedFileName" class="m-auto text-muted text-center py-5">
              <p class="mb-0 small">Chưa có file nào được tải lên.</p>
              <p class="small text-secondary fst-italic">Hệ thống sẽ giả lập nội dung đề thi gốc sau khi tải file.</p>
            </div>
            <template v-else>
              <div class="w-100 d-flex justify-content-between align-items-center bg-white border p-2 rounded shadow-sm mb-2 flex-shrink-0">
                <span class="fw-bold text-truncate text-dark-blue small">📄 {{ uploadedFileName }}</span>
                <span class="badge bg-success text-white">Trạng thái: Đã tải</span>
              </div>
              <!-- Simulated Document Pages -->
              <div class="bg-white p-3 border w-100 shadow-sm rounded-3 position-relative mockup-document-page">
                <span class="position-absolute top-0 end-0 m-2 badge bg-secondary">Trang 1 / 1</span>
                <h6 class="fw-bold text-center border-bottom pb-2">ĐỀ THI KHẢO SÁT CHẤT LƯỢNG MÔN TIN HỌC</h6>
                <p class="small mb-2 fw-semibold">Câu 1: Ngôn ngữ lập trình nào dưới đây được sử dụng để xây dựng dự án CNLearnMS frontend?</p>
                <p class="small text-muted ms-3">A. Java<br/>B. Python<br/>C. VueJS & TypeScript<br/>D. C++</p>
                
                <p class="small mb-2 fw-semibold">Câu 2: Độ phức tạp thời gian trung bình của thuật toán Quick Sort là gì?</p>
                <p class="small text-muted ms-3">A. O(N)<br/>B. O(N log N)<br/>C. O(N^2)<br/>D. O(log N)</p>

                <p class="small mb-2 fw-semibold">Câu 3: Giao thức HTTP hoạt động mặc định ở cổng nào?</p>
                <p class="small text-muted ms-3">A. Cổng 80<br/>B. Cổng 443<br/>C. Cổng 8080<br/>D. Cổng 21</p>
              </div>
            </template>
          </div>
        </div>

        <!-- Split Right: Extracted & Editable Questions List -->
        <div class="col-md-7 d-flex flex-column h-100 overflow-hidden">
          <div class="d-flex gap-2 align-items-center mb-2 flex-shrink-0">
            <h6 class="fw-bold text-dark-blue flex-fill mb-0">Preview ({{ pdfQuestions.length }} câu)</h6>
            <button class="btn btn-indigo btn-sm" :disabled="!uploadedFileName" @click="addNewPdfQuestion">
              + Thêm câu trống
            </button>
            <button class="btn btn-indigo btn-sm" @click="pdfModalOpen = false">Đóng</button>
            <button class="btn btn-indigo text-white btn-sm fw-bold" :disabled="pdfQuestions.length === 0" @click="savePdfQuestions">
              Nạp câu hỏi vào đề
            </button>
          </div>

          <!-- Editable List scrollable -->
          <div class="flex-grow-1 overflow-y-auto px-1 d-flex flex-column gap-3 mb-3 border rounded-3 p-3 bg-light-soft">
            <div v-if="pdfQuestions.length === 0" class="m-auto text-muted text-center py-5">
              <p class="small mb-0">Tải file tài liệu lên để tự động bóc tách thành các câu hỏi trắc nghiệm có thể sửa đổi.</p>
            </div>
            
            <div 
              v-for="(q, idx) in pdfQuestions" 
              :key="idx" 
              class="card card-body p-3 border border-light bg-white rounded-3 mb-0 shadow-sm position-relative"
            >
              <button class="btn btn-xs btn-outline-danger d-flex align-items-center justify-content-center position-absolute top-0 end-0 m-3 rounded-circle" style="width: 24px; height: 24px; padding: 0; min-width: 24px; min-height: 24px;" @click="pdfQuestions.splice(idx, 1)">
                <svg xmlns="http://www.w3.org/2000/svg" width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>
              </button>

              <div class="d-flex align-items-center gap-2 mb-2">
                <span class="badge bg-indigo-soft text-dark-blue fw-bold px-3 py-1 rounded">Câu {{ idx + 1 }}</span>
                <!-- Level Select -->
                <a-select v-model:value="q.level" style="width: 110px" size="small">
                  <a-select-option :value="0">Dễ</a-select-option>
                  <a-select-option :value="1">Trung bình</a-select-option>
                  <a-select-option :value="2">Khó</a-select-option>
                </a-select>
              </div>

              <!-- Question Input -->
              <div class="mb-3">
                <label class="form-label small text-muted mb-1">Nội dung câu hỏi:</label>
                <a-textarea v-model:value="q.stringContent" :rows="1" :auto-size="{ minRows: 1, maxRows: 3 }" placeholder="Nhập câu hỏi..." />
              </div>

              <!-- Options -->
              <div class="d-flex flex-column gap-2">
                <div v-for="(ans, aIdx) in q.answers" :key="aIdx" class="d-flex align-items-center gap-2">
                  <a-checkbox v-model:checked="ans.isCorrectAnswer" />
                  <span class="small fw-bold text-secondary">{{ String.fromCharCode(65 + aIdx) }}:</span>
                  <a-input v-model:value="ans.stringContent" size="small" placeholder="Nội dung đáp án..." />
                </div>
              </div>
            </div>
          </div>

          
        </div>
      </div>
    </a-modal>

    <!-- MODAL 4: THÊM TỪ ĐỀ -->
    <a-modal
      v-model:open="importExamModalOpen"
      title="Nhập câu hỏi từ đề thi đã có sẵn"
      ok-text="Thêm toàn bộ câu hỏi"
      cancel-text="Hủy"
      @ok="handleImportExam"
      width="550px"
    >
      <div class="py-2">
        <!-- Search bar -->
        <div class="mb-3">
          <label class="form-label small fw-bold">Tìm kiếm đề thi gốc:</label>
          <a-input v-model:value="importExamFilter.search" placeholder="Nhập từ khóa tìm tên đề thi..." allow-clear>
            <template #prefix>
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
            </template>
          </a-input>
        </div>

        <!-- Scrollable list of exams -->
        <div class="border rounded bg-white p-2 mb-3" style="max-height: 250px; overflow-y: auto;">
          <div v-if="filteredExamsList.length === 0" class="text-center text-muted py-4 small">
            Không tìm thấy đề thi phù hợp.
          </div>
          <div 
            v-for="ex in filteredExamsList" 
            :key="ex.id" 
            class="d-flex justify-content-between align-items-center p-2 rounded mb-1 hover-pointer"
            :class="selectedImportExamId === ex.id ? 'bg-indigo-soft border-indigo' : 'border border-light'"
            @click="selectedImportExamId = ex.id"
          >
            <div>
              <p class="fw-bold text-dark-blue mb-0 small">{{ ex.name }}</p>
              <span class="text-muted fs-8">{{ getCategoryName(ex.categoryId) }} | {{ ex.questions?.length || 0 }} câu hỏi</span>
            </div>
            <a-radio :checked="selectedImportExamId === ex.id" @change="selectedImportExamId = ex.id" />
          </div>
        </div>

        <!-- Options -->
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
        <!-- Nội dung câu hỏi -->
        <div class="mb-3">
          <label class="form-label small fw-bold">Nội dung câu hỏi:</label>
          <a-textarea v-model:value="singleQuestionForm.stringContent" :rows="3" placeholder="Nhập câu hỏi..." />
        </div>

        <!-- Chuyên đề -->
        <div class="row g-2 mb-3">
          <div class="col-md-6">
            <label class="form-label small fw-bold">Chuyên đề:</label>
            <a-select v-model:value="singleQuestionForm.categoryIds[0]" style="width: 100%">
              <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id">
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

        <!-- Các đáp án -->
        <div class="mb-2">
          <label class="form-label small fw-bold mb-1">Các phương án trả lời (Tick vào ô tròn để chọn đáp án đúng):</label>
          <div class="d-flex flex-column gap-2">
            <div v-for="(ans, idx) in singleQuestionForm.answers" :key="idx" class="d-flex align-items-center gap-2">
              <a-radio 
                :checked="ans.isCorrectAnswer" 
                @change="() => {
                  singleQuestionForm.answers.forEach((a, aI) => a.isCorrectAnswer = aI === idx)
                }" 
              />
              <span class="small fw-bold text-secondary">{{ String.fromCharCode(65 + idx) }}:</span>
              <a-input v-model:value="ans.stringContent" placeholder="Nhập nội dung phương án..." />
            </div>
          </div>
        </div>

        <!-- Giải thích -->
        <div class="mt-3">
          <label class="form-label small fw-bold">Giải thích đáp án (nếu có):</label>
          <a-textarea v-model:value="singleQuestionForm.explanation" :rows="2" placeholder="Giải thích vì sao đáp án này đúng..." />
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { message, Modal } from 'ant-design-vue'

const route = useRoute()
const router = useRouter()

// Structures interfaces
interface Answer {
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
}

interface Exam {
  id: string
  name: string
  description: string
  categoryId: string
  duration: number
  accessType: number // 0: Private, 1: Public
  questions: Question[]
  isMyCreated?: boolean
  creationMode?: string
  isDraft?: boolean
}

interface Quiz {
  id: string
  title: string
  targetGroup: string
  sourceType: 'exam' | 'direct'
  examId?: string
  startDate: string
  endDate: string
  directRule?: {
    categoryId: string
    totalQuestions: number
    duration: number
  }
  isMyCreated?: boolean
  isDraft?: boolean
}

interface Category {
  id: string
  name: string
}

// Reference Data
const categories = ref<Category[]>([
  { id: "c01a92a2-a69f-4143-8589-da11688d7d01", name: "Toán Học - Luyện Thi THPT Quốc Gia" },
  { id: "c02a92a2-a69f-4143-8589-da11688d7d02", name: "Vật Lý 12 - Chuyên Đề Dòng Điện Xoay Chiều" },
  { id: "c03a92a2-a69f-4143-8589-da11688d7d03", name: "Hóa Học - Chuyên Đề Hóa Hữu Cơ" },
  { id: "c04a92a2-a69f-4143-8589-da11688d7d04", name: "Tiếng Anh - IELTS Reading Academic" },
  { id: "c05a92a2-a69f-4143-8589-da11688d7d05", name: "Lịch Sử - Lịch Sử Việt Nam Cận & Hiện Đại" },
  { id: "c06a92a2-a69f-4143-8589-da11688d7d06", name: "Sinh Học - Di Truyền Học & Biến Dị" },
  { id: "c07a92a2-a69f-4143-8589-da11688d7d07", name: "Tin Học - Lập Trình C++ Cơ Bản & Nâng Cao" },
  { id: "c08a92a2-a69f-4143-8589-da11688d7d08", name: "Địa Lý - Địa Lý Kinh Tế Xã Hội Việt Nam" },
  { id: "c09a92a2-a69f-4143-8589-da11688d7d09", name: "Giáo Dục Công Dân - Đạo Đức & Pháp Luật" }
])

const bankQuestions = ref<Question[]>([])
const examsList = ref<Exam[]>([])
const quizzesList = ref<Quiz[]>([])

// Creation config type
const creationType = ref<'exam' | 'quiz'>('exam')
const isQuizMode = computed(() => creationType.value === 'quiz')

// Form values
const formData = reactive({
  name: '',
  categoryId: undefined as string | undefined,
  duration: 45,
  accessType: 1,
  description: '',
  contributeToBank: true,

  // Quiz details
  targetGroup: 'Lớp 12A1',
  startType: 'now' as 'now' | 'custom',
  startDate: '',
  antiCheat: {
    lockBrowser: true,
    shuffleQuestions: true,
    disableCopyPaste: true,
    webcam: false,
    ipLimit: false,
    fullscreen: true
  },
  allowLateJoin: true,
  allowLateSubmit: false,
  publicLeaderboard: true,
  sendEmailReport: true
})

// Current Exam Questions list
const questionsList = ref<Question[]>([])
const selectedGotoIndex = ref<number | undefined>(undefined)

// Auto Add state
const autoAddModalOpen = ref(false)
const autoAddForm = reactive({
  totalCount: 10,
  categoryId: undefined as string | undefined,
  easyCount: 4,
  mediumCount: 4,
  hardCount: 2,
  shuffleOptions: true
})

// Choose from Bank state
const bankModalOpen = ref(false)
const selectedBankQuestionIds = ref<string[]>([])
const bankFilter = reactive({
  search: '',
  categoryId: undefined as string | undefined,
  page: 1,
  pageSize: 8
})

// PDF Split View panel state
const pdfModalOpen = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)
const uploadedFileName = ref('')
const pdfQuestions = ref<Array<{
  stringContent: string
  level: number
  answers: Answer[]
}>>([])

// Import from Exam state
const importExamModalOpen = ref(false)
const selectedImportExamId = ref<string | undefined>(undefined)
const importExamFilter = reactive({
  search: '',
  shuffleOptions: true
})

// Single Question manual editor state
const singleQuestionModalOpen = ref(false)
const singleQuestionModalMode = ref<'create' | 'edit'>('create')
const singleQuestionModalIndex = ref<number | null>(null)

const singleQuestionForm = reactive<Question>({
  id: '',
  slug: '',
  stringContent: '',
  explanation: '',
  level: 0,
  type: 0,
  accessType: 1,
  categoryIds: [],
  answers: []
})

// Lifecycle
onMounted(() => {
  // Query mode override
  const typeParam = route.query.type
  if (typeParam === 'quiz' || typeParam === 'exam') {
    creationType.value = typeParam
  }

  // Load questions
  const storedQuestions = localStorage.getItem('cn_questions')
  if (storedQuestions) {
    bankQuestions.value = JSON.parse(storedQuestions)
  }

  // Load exams
  const storedEx = localStorage.getItem('cn_exams')
  if (storedEx) {
    examsList.value = JSON.parse(storedEx)
  }

  // Load quizzes
  const storedQu = localStorage.getItem('cn_quizzes')
  if (storedQu) {
    quizzesList.value = JSON.parse(storedQu)
  }

  // Pre-select category if none
  if (categories.value.length > 0 && categories.value[0]) {
    formData.categoryId = categories.value[0].id
    autoAddForm.categoryId = categories.value[0].id
  }

  // Load for editing/updating mode
  const editId = route.params.id as string | undefined
  if (editId) {
    // 1. Try to find in examsList
    const existingExam = examsList.value.find(e => e.id === editId)
    if (existingExam) {
      creationType.value = 'exam'
      formData.name = existingExam.name
      formData.description = existingExam.description
      formData.categoryId = existingExam.categoryId
      formData.duration = existingExam.duration
      formData.accessType = existingExam.accessType
      questionsList.value = JSON.parse(JSON.stringify(existingExam.questions || []))
    } else {
      // 2. Try to find in quizzesList
      const existingQuiz = quizzesList.value.find(q => q.id === editId)
      if (existingQuiz) {
        creationType.value = 'quiz'
        formData.name = existingQuiz.title.replace(' [Bản nháp]', '').replace(' (Kỳ thi/Lớp học)', '')
        formData.targetGroup = existingQuiz.targetGroup
        formData.startDate = existingQuiz.startDate
        formData.startType = existingQuiz.startDate ? 'custom' : 'now'
        
        // Load associated exam data
        if (existingQuiz.examId) {
          const associatedExam = examsList.value.find(e => e.id === existingQuiz.examId)
          if (associatedExam) {
            formData.description = associatedExam.description
            formData.categoryId = associatedExam.categoryId
            formData.duration = associatedExam.duration
            formData.accessType = associatedExam.accessType
            questionsList.value = JSON.parse(JSON.stringify(associatedExam.questions || []))
          }
        }
      } else {
        message.error('Không tìm thấy dữ liệu đề thi/kỳ thi cần chỉnh sửa!')
      }
    }
  }
})

// Helpers
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

// Question positioning
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

// Utility: Shuffle choices of questions helper
const shuffleChoices = (q: Question) => {
  const choices = [...q.answers]
  for (let i = choices.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1))
    const temp = choices[i]
    if (temp && choices[j]) {
      choices[i] = choices[j] as Answer
      choices[j] = temp
    }
  }
  q.answers = choices
}

// Action: Handle Auto Add questions
const handleAutoAdd = () => {
  const matching = bankQuestions.value.filter(q => {
    if (autoAddForm.categoryId && !q.categoryIds.includes(autoAddForm.categoryId)) return false
    return true
  })

  if (matching.length === 0) {
    message.error('Không tìm thấy câu hỏi nào thuộc chủ đề này trong ngân hàng!')
    return
  }

  // Group by level
  const easy = matching.filter(q => q.level === 0)
  const medium = matching.filter(q => q.level === 1)
  const hard = matching.filter(q => q.level === 2)

  // Shuffle arrays helper
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

  // If we drew less due to lacking pool size, just top it up from matching
  if (selected.length < autoAddForm.totalCount) {
    const remainingCount = autoAddForm.totalCount - selected.length
    const unused = matching.filter(q => !selected.some(s => s.id === q.id))
    selected.push(...draw(unused, remainingCount))
  }

  if (selected.length === 0) {
    message.warning('Không thể rút tự động vì chủ đề rỗng!')
    return
  }

  // Shuffle answers if checked
  if (autoAddForm.shuffleOptions) {
    selected.forEach(q => shuffleChoices(q))
  }

  // Append to current list, avoid duplicates
  let addedCount = 0
  selected.forEach(q => {
    if (!questionsList.value.some(el => el.id === q.id)) {
      questionsList.value.push(JSON.parse(JSON.stringify(q))) // deep copy
      addedCount++
    }
  })

  message.success(`Đã chọn ngẫu nhiên và thêm ${addedCount} câu hỏi mới vào đề!`)
  autoAddModalOpen.value = false
}

// Action: Choose from Bank
const openBankModal = () => {
  selectedBankQuestionIds.value = questionsList.value.map(q => q.id)
  bankFilter.categoryId = formData.categoryId
  bankFilter.page = 1
  bankModalOpen.value = true
}

const filteredBankQuestions = computed(() => {
  return bankQuestions.value.filter(q => {
    if (bankFilter.search && !q.stringContent.toLowerCase().includes(bankFilter.search.toLowerCase())) return false
    if (bankFilter.categoryId && !q.categoryIds.includes(bankFilter.categoryId)) return false
    return true
  })
})

const paginatedBankQuestions = computed(() => {
  const start = (bankFilter.page - 1) * bankFilter.pageSize
  return filteredBankQuestions.value.slice(start, start + bankFilter.pageSize)
})

const isAllBankSelected = computed(() => {
  const pageIds = paginatedBankQuestions.value.map(q => q.id)
  if (pageIds.length === 0) return false
  return pageIds.every(id => selectedBankQuestionIds.value.includes(id))
})

const toggleSelectAllBank = () => {
  const pageIds = paginatedBankQuestions.value.map(q => q.id)
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
  // Clear and rebuild based on selection
  const added: Question[] = []
  selectedBankQuestionIds.value.forEach(id => {
    const orig = bankQuestions.value.find(q => q.id === id)
    if (orig) {
      added.push(JSON.parse(JSON.stringify(orig)))
    }
  })

  // Append new ones that aren't duplicate
  let addedCount = 0
  added.forEach(q => {
    if (!questionsList.value.some(el => el.id === q.id)) {
      questionsList.value.push(q)
      addedCount++
    }
  })

  message.success(`Đã thêm ${addedCount} câu hỏi từ ngân hàng!`)
  bankModalOpen.value = false
}

// Action: PDF Analysis
const handleFileUpload = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files.length > 0) {
    const file = target.files[0]
    if (file) {
      uploadedFileName.value = file.name
      message.loading('Đang phân tích cấu trúc tài liệu PDF/Word...', 1.5)

      // Simulate extracting 3 mock questions after a delay
      setTimeout(() => {
        pdfQuestions.value = [
          {
            stringContent: 'Ngôn ngữ lập trình nào dưới đây được sử dụng để xây dựng dự án CNLearnMS frontend?',
            level: 0,
            answers: [
              { stringContent: 'Java', isCorrectAnswer: false },
              { stringContent: 'Python', isCorrectAnswer: false },
              { stringContent: 'VueJS & TypeScript', isCorrectAnswer: true },
              { stringContent: 'C++', isCorrectAnswer: false }
            ]
          },
          {
            stringContent: 'Độ phức tạp thời gian trung bình của thuật toán sắp xếp nhanh (Quick Sort) là gì?',
            level: 1,
            answers: [
              { stringContent: 'O(N)', isCorrectAnswer: false },
              { stringContent: 'O(N log N)', isCorrectAnswer: true },
              { stringContent: 'O(N^2)', isCorrectAnswer: false },
              { stringContent: 'O(log N)', isCorrectAnswer: false }
            ]
          },
          {
            stringContent: 'Giao thức HTTP hoạt động mặc định ở cổng truyền thông nào?',
            level: 0,
            answers: [
              { stringContent: 'Cổng 80', isCorrectAnswer: true },
              { stringContent: 'Cổng 443', isCorrectAnswer: false },
              { stringContent: 'Cổng 8080', isCorrectAnswer: false },
              { stringContent: 'Cổng 21', isCorrectAnswer: false }
            ]
          }
        ]
        message.success('Đã bóc tách thành công 3 câu hỏi từ file!')
      }, 1500)
    }
  }
}

const addNewPdfQuestion = () => {
  pdfQuestions.value.push({
    stringContent: 'Nội dung câu hỏi mới...',
    level: 0,
    answers: [
      { stringContent: 'Phương án A', isCorrectAnswer: false },
      { stringContent: 'Phương án B', isCorrectAnswer: false },
      { stringContent: 'Phương án C', isCorrectAnswer: false },
      { stringContent: 'Phương án D', isCorrectAnswer: false }
    ]
  })
}

const savePdfQuestions = () => {
  let count = 0
  pdfQuestions.value.forEach(q => {
    const newQ: Question = {
      id: 'pdf_q_' + Date.now() + Math.random().toString(36).substr(2, 5),
      slug: 'pdf-imported',
      stringContent: q.stringContent,
      explanation: 'Câu hỏi được bóc tách từ file tài liệu của tôi.',
      level: q.level,
      type: 0,
      accessType: formData.accessType,
      categoryIds: [formData.categoryId || 'c07a92a2-a69f-4143-8589-da11688d7d07'],
      answers: q.answers
    }
    questionsList.value.push(newQ)
    count++
  })

  message.success(`Đã nạp ${count} câu hỏi bóc tách vào danh sách đề thi!`)
  pdfModalOpen.value = false
  // Clean
  pdfQuestions.value = []
  uploadedFileName.value = ''
}

// Action: Import from Exam
const openImportExamModal = () => {
  selectedImportExamId.value = undefined
  importExamModalOpen.value = true
}

const filteredExamsList = computed(() => {
  if (!importExamFilter.search) return examsList.value
  return examsList.value.filter(e => e.name.toLowerCase().includes(importExamFilter.search.toLowerCase()))
})

const handleImportExam = () => {
  if (!selectedImportExamId.value) {
    message.error('Vui lòng chọn 1 đề thi gốc để chèn!')
    return
  }

  const selectedEx = examsList.value.find(e => e.id === selectedImportExamId.value)
  if (!selectedEx || !selectedEx.questions || selectedEx.questions.length === 0) {
    message.error('Đề thi này không có câu hỏi nào!')
    return
  }

  let count = 0
  selectedEx.questions.forEach(q => {
    const copyQ = JSON.parse(JSON.stringify(q)) // deep copy
    // Change id to make unique
    copyQ.id = 'imported_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5)
    
    if (importExamFilter.shuffleOptions) {
      shuffleChoices(copyQ)
    }

    questionsList.value.push(copyQ)
    count++
  })

  message.success(`Đã sao chép và chèn ${count} câu hỏi vào đề hiện tại!`)
  importExamModalOpen.value = false
}

// Single Question manual editor helper functions
const resetSingleQuestionForm = () => {
  singleQuestionForm.id = ''
  singleQuestionForm.slug = ''
  singleQuestionForm.stringContent = ''
  singleQuestionForm.explanation = ''
  singleQuestionForm.level = 0
  singleQuestionForm.type = 0
  singleQuestionForm.accessType = 1
  singleQuestionForm.categoryIds = [formData.categoryId || categories.value[0]?.id || '']
  singleQuestionForm.answers = [
    { stringContent: '', isCorrectAnswer: false },
    { stringContent: '', isCorrectAnswer: false },
    { stringContent: '', isCorrectAnswer: false },
    { stringContent: '', isCorrectAnswer: false }
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
  
  // Clone values
  singleQuestionForm.id = q.id
  singleQuestionForm.slug = q.slug
  singleQuestionForm.stringContent = q.stringContent
  singleQuestionForm.explanation = q.explanation || ''
  singleQuestionForm.level = q.level
  singleQuestionForm.type = q.type
  singleQuestionForm.accessType = q.accessType
  singleQuestionForm.categoryIds = [...q.categoryIds]
  singleQuestionForm.answers = JSON.parse(JSON.stringify(q.answers))
  
  // Ensure we have 4 options
  while (singleQuestionForm.answers.length < 4) {
    singleQuestionForm.answers.push({ stringContent: '', isCorrectAnswer: false })
  }
  
  singleQuestionModalOpen.value = true
}

const saveSingleQuestion = () => {
  if (!singleQuestionForm.stringContent.trim()) {
    message.error('Vui lòng nhập nội dung câu hỏi!')
    return
  }

  // Validate answers
  const filledAnswers = singleQuestionForm.answers.filter(a => a.stringContent.trim())
  if (filledAnswers.length < 2) {
    message.error('Câu hỏi phải có ít nhất 2 đáp án!')
    return
  }

  const hasCorrect = singleQuestionForm.answers.some(a => a.isCorrectAnswer)
  if (!hasCorrect) {
    message.error('Vui lòng chọn ít nhất 1 đáp án đúng!')
    return
  }

  // Build the Question object
  const cleanAnswers = singleQuestionForm.answers.map(a => ({
    stringContent: a.stringContent.trim(),
    isCorrectAnswer: a.isCorrectAnswer
  }))

  if (singleQuestionModalMode.value === 'create') {
    const newQ: Question = {
      id: 'q_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5),
      slug: 'manual-draft',
      stringContent: singleQuestionForm.stringContent.trim(),
      explanation: singleQuestionForm.explanation.trim() || 'Giải thích chi tiết của câu hỏi.',
      level: singleQuestionForm.level,
      type: singleQuestionForm.type,
      accessType: singleQuestionForm.accessType,
      categoryIds: [...singleQuestionForm.categoryIds],
      answers: cleanAnswers
    }
    questionsList.value.push(newQ)
    message.success('Đã thêm câu hỏi mới vào đề!')
  } else {
    // Edit mode
    const idx = singleQuestionModalIndex.value
    if (idx !== null && questionsList.value[idx]) {
      const existingQ = questionsList.value[idx] as Question
      
      // Update values inline
      existingQ.stringContent = singleQuestionForm.stringContent.trim()
      existingQ.explanation = singleQuestionForm.explanation.trim()
      existingQ.level = singleQuestionForm.level
      existingQ.categoryIds = [...singleQuestionForm.categoryIds]
      existingQ.answers = cleanAnswers
      // Note: we KEEP the existing ID! We do NOT generate a new ID!
      message.success('Đã cập nhật câu hỏi!')
    }
  }

  singleQuestionModalOpen.value = false
}

// Global Save Form Action
const saveForm = (isDraft = false) => {
  if (!formData.name.trim()) {
    message.error('Vui lòng nhập tên đề/kỳ thi!')
    return
  }

  if (!isDraft && questionsList.value.length === 0) {
    message.error('Đề thi phải chứa ít nhất 1 câu hỏi! Vui lòng chọn hoặc nạp thêm câu hỏi.')
    return
  }

  const editId = route.params.id as string | undefined

  if (editId) {
    // Editing mode: find and update
    let targetExamId = editId

    // If it was a quiz, find the quiz and the associated examId
    if (isQuizMode.value) {
      const quizIndex = quizzesList.value.findIndex(q => q.id === editId)
      if (quizIndex !== -1) {
        const existingQuiz = quizzesList.value[quizIndex]
        if (existingQuiz) {
          targetExamId = existingQuiz.examId || ''

          // Update quiz properties
          existingQuiz.title = formData.name.trim() + (isDraft ? ' [Bản nháp]' : ' (Kỳ thi/Lớp học)')
          existingQuiz.targetGroup = formData.targetGroup.trim()
          existingQuiz.isDraft = isDraft
          if (formData.startType === 'custom') {
            existingQuiz.startDate = formData.startDate
          }
          
          quizzesList.value[quizIndex] = existingQuiz
          localStorage.setItem('cn_quizzes', JSON.stringify(quizzesList.value))
        }
      }
    }

    // Now update the exam itself
    const examIndex = examsList.value.findIndex(e => e.id === targetExamId)
    if (examIndex !== -1) {
      const existingExam = examsList.value[examIndex]
      if (existingExam) {
        existingExam.name = formData.name.trim()
        existingExam.description = formData.description.trim()
        existingExam.categoryId = formData.categoryId || categories.value[0]?.id || ''
        existingExam.duration = formData.duration
        existingExam.accessType = formData.accessType
        existingExam.questions = questionsList.value
        existingExam.isDraft = isDraft

        examsList.value[examIndex] = existingExam
        localStorage.setItem('cn_exams', JSON.stringify(examsList.value))
      }
    }

    message.success(isDraft ? 'Đã cập nhật bản nháp thành công!' : 'Đã cập nhật dữ liệu thành công!')
  } else {
    // Creation mode:
    // 1. Create Exam Object
    const examId = 'exam_' + Date.now()
    const newExam: Exam = {
      id: examId,
      name: formData.name.trim(),
      description: formData.description.trim() || 'Đề thi tự soạn cá nhân.',
      categoryId: formData.categoryId || categories.value[0]?.id || '',
      duration: formData.duration,
      accessType: formData.accessType,
      questions: questionsList.value,
      isMyCreated: true,
      creationMode: 'manual',
      isDraft: isDraft
    }

    // Push to local exams list
    examsList.value.unshift(newExam)
    localStorage.setItem('cn_exams', JSON.stringify(examsList.value))

    // 2. If contributeToBank checked & not draft: Add any new questions to bank questions
    if (formData.contributeToBank && !isDraft) {
      let contributedCount = 0
      questionsList.value.forEach(q => {
        // Check if it is a new question (has a temporary ID or content doesn't exist in bank)
        const existsInBank = bankQuestions.value.some(bq => bq.id === q.id || bq.stringContent === q.stringContent)
        if (!existsInBank) {
          // Deep copy
          const contributedQ: Question = JSON.parse(JSON.stringify(q))
          // Set unique bank id if it doesn't have a valid bank ID
          if (!contributedQ.id.startsWith('bank_')) {
            contributedQ.id = 'bank_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5)
          }
          contributedQ.accessType = formData.accessType
          contributedQ.categoryIds = [formData.categoryId || 'c07a92a2-a69f-4143-8589-da11688d7d07']
          bankQuestions.value.unshift(contributedQ)
          contributedCount++
        }
      })
      if (contributedCount > 0) {
        localStorage.setItem('cn_questions', JSON.stringify(bankQuestions.value))
      }
    }

    // 3. If QuizMode: Create Quiz Object
    if (isQuizMode.value) {
      const quizId = 'quiz_' + Date.now()
      const now = new Date()
      const startDateVal = formData.startType === 'now' 
        ? now.toISOString().slice(0, 16) 
        : (formData.startDate || now.toISOString().slice(0, 16))
      
      // Default end date is 7 days from start
      const end = new Date(new Date(startDateVal).getTime() + 7 * 24 * 60 * 60000)
      const endDateVal = end.toISOString().slice(0, 16)

      const newQuiz: Quiz = {
        id: quizId,
        title: formData.name.trim() + (isDraft ? ' [Bản nháp]' : ' (Kỳ thi/Lớp học)'),
        targetGroup: formData.targetGroup.trim(),
        sourceType: 'exam',
        examId: examId,
        startDate: startDateVal,
        endDate: endDateVal,
        isMyCreated: true,
        isDraft: isDraft
      }

      quizzesList.value.unshift(newQuiz)
      localStorage.setItem('cn_quizzes', JSON.stringify(quizzesList.value))
      
      if (isDraft) {
        message.success(`Đã lưu bản nháp kỳ thi "${newQuiz.title}" thành công!`)
      } else {
        message.success(`Đã lưu và lên lịch tổ chức kỳ thi "${newQuiz.title}" thành công!`)
      }
    } else {
      if (isDraft) {
        message.success(`Đã lưu bản nháp đề thi "${newExam.name}" thành công!`)
      } else {
        message.success(`Đã tạo đề thi "${newExam.name}" thành công!`)
      }
    }
  }

  // Return
  router.push('/personal/exams')
}
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

.mockup-document-page {
  font-family: 'Times New Roman', Times, serif;
  min-height: 297mm;
  padding: 20mm;
}

@media(max-width: 576px) {
  .mockup-document-page {
    padding: 10mm;
  }
}
</style>
