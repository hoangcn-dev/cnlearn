<template>
  <div class="exam-management-view py-3">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb" class="mb-3">
      <ol class="breadcrumb mb-0">
        <li class="breadcrumb-item"><router-link to="/">Trang chủ</router-link></li>
        <li class="breadcrumb-item active" aria-current="page">Quản lý đề & kỳ thi</li>
      </ol>
    </nav>

    <!-- Header & Action Buttons -->
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-4">
      <div>
        <h1 class="fs-4 fw-bold text-dark-blue mb-1">Quản Lý Đề Thi & Kỳ Thi</h1>
        <p class="text-secondary small mb-0">Tạo đề thi tự động từ ngân hàng câu hỏi, thiết lập kỳ thi thử hoặc bài kiểm tra định kỳ có giới hạn thời gian.</p>
      </div>
      <div class="d-flex gap-2">
        <button class="btn btn-indigo text-white fw-semibold hover-up d-flex align-items-center gap-2 btn-sm" @click="router.push('/personal/exams/create?type=exam')">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="me-1"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
          Tạo đề thi mới
        </button>
        <button class="btn btn-accent text-white fw-semibold hover-up d-flex align-items-center gap-2 btn-sm" @click="router.push('/personal/exams/create?type=quiz')">
          <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="me-1"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
          Tạo bài kiểm tra
        </button>
      </div>
    </div>

    <!-- Tabs Container -->
    <div class="card border-0 rounded-4 shadow-sm bg-white overflow-hidden mb-5">
      <a-tabs v-model:activeKey="activeTab" class="custom-tabs px-4 pt-3">
        
        <!-- Tab 1: Danh sách Đề thi -->
        <a-tab-pane key="exams" tab="Danh sách đề thi">
          <!-- Sub-tabs -->
          <a-tabs v-model:activeKey="examSubTab" class="custom-list-tabs mb-2 px-1">
            <a-tab-pane key="mine" tab="Của tôi" />
            <a-tab-pane key="saved" tab="Đã lưu" />
          </a-tabs>

          <!-- Filter Bar -->
          <div class="row g-3 py-2 border-bottom mb-3 align-items-center mx-0">
            <div class="col-md-9 d-flex gap-2 align-items-center px-1">
              <div style="max-width: 300px; width: 100%;">
                <a-input v-model:value="examFilters.search" placeholder="Tìm kiếm đề thi..." allow-clear>
                  <template #prefix>
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="text-muted"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
                  </template>
                </a-input>
              </div>

              <button type="button" class="btn btn-outline-indigo btn-sm d-flex align-items-center gap-1.5 px-3 py-1.5 rounded" @click="examFilterModalOpen = true">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon></svg>
                <span>Bộ lọc</span>
                <span v-if="hasActiveExamFilters" class="badge bg-indigo text-white rounded-pill ms-1" style="font-size: 0.7rem;">{{ activeExamFiltersCount }}</span>
              </button>

              <span class="badge bg-indigo-soft text-indigo px-3 py-2 rounded-pill fw-semibold small ms-auto">
                Lọc được: {{ filteredExams.length }} đề
              </span>
            </div>
          </div>

          <!-- Exams Table -->
          <div class="table-responsive py-2 px-1">
            <a-table 
              :columns="examColumns" 
              :data-source="filteredExams" 
              row-key="id"
              :pagination="{ pageSize: 5 }"
              class="custom-table"
            >
              <template #bodyCell="{ column, record }">
                <template v-if="column.key === 'name'">
                  <div class="fw-bold text-dark-blue d-flex align-items-center gap-2">
                    <span>{{ record.name }}</span>
                    <span v-if="record.isDraft" class="badge bg-secondary text-white fw-bold px-2 py-0.5 rounded fs-8" style="font-size: 0.7rem;">Nháp</span>
                  </div>
                  <div class="text-secondary small mt-0.5">Mô tả: {{ record.description || 'Không có mô tả' }}</div>
                </template>

                <template v-else-if="column.key === 'category'">
                  <span class="badge bg-light text-dark border">{{ getCategoryName(record.categoryId) }}</span>
                </template>

                <template v-else-if="column.key === 'questionsCount'">
                  <span class="fw-semibold text-indigo">{{ record.questions?.length || 0 }} câu hỏi</span>
                </template>

                <template v-else-if="column.key === 'duration'">
                  <span class="text-secondary">{{ record.duration }} phút</span>
                </template>

                <template v-else-if="column.key === 'actions'">
                  <div class="d-flex justify-content-end pe-2">
                    <a-dropdown trigger="click" placement="bottomRight">
                      <button class="btn btn-xs btn-outline-secondary p-1 rounded-circle d-flex align-items-center justify-content-center" style="width: 26px; height: 26px; border-color: #cbd5e1;">
                        <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>
                      </button>
                      <template #overlay>
                        <a-menu>
                          <!-- Tab ngân hàng: Chỉ xem câu hỏi (ko đáp án) -->
                          <template v-if="examSubTab === 'bank'">
                            <a-menu-item key="detail-bank" @click="viewExamDetails(record, false)">
                              <span class="d-flex align-items-center gap-2 text-indigo small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                                Xem câu hỏi
                              </span>
                            </a-menu-item>
                          </template>

                          <!-- Tab của tôi: Xem câu hỏi (đầy đủ đáp án), Xóa đề thi -->
                          <template v-else-if="examSubTab === 'mine'">
                            <a-menu-item key="detail-mine" @click="viewExamDetails(record, true)">
                              <span class="d-flex align-items-center gap-2 text-indigo small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                                Xem câu hỏi (Có đáp án)
                              </span>
                            </a-menu-item>
                            <a-menu-item key="edit-exam" @click="router.push('/personal/exams/edit/' + record.id)">
                              <span class="d-flex align-items-center gap-2 text-primary small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
                                Chỉnh sửa đề thi
                              </span>
                            </a-menu-item>
                            <a-menu-item key="delete" @click="confirmDeleteExam(record.id)">
                              <span class="d-flex align-items-center gap-2 text-danger small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
                                Xóa đề thi
                              </span>
                            </a-menu-item>
                          </template>

                          <!-- Tab đã lưu: Xem câu hỏi (đầy đủ đáp án), Bỏ lưu khỏi danh sách -->
                          <template v-else-if="examSubTab === 'saved'">
                            <a-menu-item key="detail-saved" @click="viewExamDetails(record, true)">
                              <span class="d-flex align-items-center gap-2 text-indigo small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
                                Xem câu hỏi
                              </span>
                            </a-menu-item>
                            <a-menu-item key="unsave" @click="unsaveExam(record.id)">
                              <span class="d-flex align-items-center gap-2 text-danger small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
                                Xóa khỏi danh sách lưu
                              </span>
                            </a-menu-item>
                          </template>

                          <!-- Tab đã làm: Chỉ xem chi tiết lịch sử làm -->
                          <template v-else-if="examSubTab === 'done'">
                            <a-menu-item key="history" @click="viewExamHistory(record)">
                              <span class="d-flex align-items-center gap-2 text-indigo small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
                                Xem chi tiết lịch sử làm
                              </span>
                            </a-menu-item>
                          </template>
                        </a-menu>
                      </template>
                    </a-dropdown>
                  </div>
                </template>
              </template>
            </a-table>
          </div>
        </a-tab-pane>

        <!-- Tab 2: Quản lý Bài kiểm tra / Kỳ thi -->
        <a-tab-pane key="quizzes" tab="Bài kiểm tra / Kỳ thi đang tổ chức">
          <!-- Sub-tabs -->
          <a-tabs v-model:activeKey="quizSubTab" class="custom-list-tabs mb-2 px-1">
            <a-tab-pane key="mine" tab="Của tôi" />
            <a-tab-pane key="saved" tab="Đã lưu" />
          </a-tabs>

          <!-- Quiz Filters -->
          <div class="row g-3 py-2 border-bottom mb-3 align-items-center mx-0">
            <div class="col-md-9 d-flex gap-2 align-items-center px-1">
              <div style="max-width: 300px; width: 100%;">
                <a-input v-model:value="quizFilters.search" placeholder="Tìm kiếm bài kiểm tra..." allow-clear>
                  <template #prefix>
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" class="text-muted"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
                  </template>
                </a-input>
              </div>

              <button type="button" class="btn btn-outline-indigo btn-sm d-flex align-items-center gap-1.5 px-3 py-1.5 rounded" @click="quizFilterModalOpen = true">
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon></svg>
                <span>Bộ lọc</span>
                <span v-if="hasActiveQuizFilters" class="badge bg-indigo text-white rounded-pill ms-1" style="font-size: 0.7rem;">{{ activeQuizFiltersCount }}</span>
              </button>

              <span class="badge bg-accent-soft text-accent px-3 py-2 rounded-pill fw-semibold small ms-auto">
                Lọc được: {{ filteredQuizzes.length }} bài
              </span>
            </div>
          </div>

          <!-- Quizzes Table -->
          <div class="table-responsive py-2 px-1">
            <a-table 
              :columns="quizColumns" 
              :data-source="filteredQuizzes" 
              row-key="id"
              :pagination="{ pageSize: 5 }"
              class="custom-table"
            >
              <template #bodyCell="{ column, record }">
                <template v-if="column.key === 'title'">
                  <div class="fw-bold text-dark-blue d-flex align-items-center gap-2">
                    <span>{{ record.title }}</span>
                    <span v-if="record.isDraft" class="badge bg-secondary text-white fw-bold px-2 py-0.5 rounded fs-8" style="font-size: 0.7rem;">Nháp</span>
                  </div>
                  <div class="text-secondary small mt-0.5">Đối tượng: <span class="fw-semibold">{{ record.targetGroup }}</span></div>
                </template>

                <template v-else-if="column.key === 'examSource'">
                  <span class="text-secondary small">
                    {{ record.sourceType === 'exam' ? 'Đề thi: ' + getExamName(record.examId) : 'Trộn từ Ngân hàng câu hỏi' }}
                  </span>
                </template>

                <template v-else-if="column.key === 'timeWindow'">
                  <div class="small">
                    <div>Bắt đầu: <span class="text-muted">{{ formatDate(record.startDate) }}</span></div>
                    <div>Kết thúc: <span class="text-muted">{{ formatDate(record.endDate) }}</span></div>
                  </div>
                </template>

                <template v-else-if="column.key === 'status'">
                  <span :class="isQuizActive(record) ? 'badge bg-success-soft text-success' : 'badge bg-danger-soft text-danger'">
                    {{ isQuizActive(record) ? 'Đang mở' : 'Đã đóng/hết hạn' }}
                  </span>
                </template>

                <template v-else-if="column.key === 'actions'">
                  <div class="d-flex justify-content-end pe-2">
                    <a-dropdown trigger="click" placement="bottomRight">
                      <button class="btn btn-xs btn-outline-secondary p-1 rounded-circle d-flex align-items-center justify-content-center" style="width: 26px; height: 26px; border-color: #cbd5e1;">
                        <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-secondary"><circle cx="12" cy="12" r="3"></circle><path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 1 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 1 1-2.83-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 1 1 2.83-2.83l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 1 1 2.83 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z"></path></svg>
                      </button>
                      <template #overlay>
                        <a-menu>
                          <!-- Tab ngân hàng: Chỉ làm thử -->
                          <template v-if="quizSubTab === 'bank'">
                            <a-menu-item key="practice" @click="simulateTakeQuiz(record)" :disabled="!isQuizActive(record)">
                              <span class="d-flex align-items-center gap-2 text-accent small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
                                Làm thử
                              </span>
                            </a-menu-item>
                          </template>

                          <!-- Tab của tôi: Làm thử, Xóa kỳ thi -->
                          <template v-else-if="quizSubTab === 'mine'">
                            <a-menu-item key="practice" @click="simulateTakeQuiz(record)" :disabled="!isQuizActive(record)">
                              <span class="d-flex align-items-center gap-2 text-accent small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
                                Làm thử
                              </span>
                            </a-menu-item>
                            <a-menu-item key="edit-quiz" @click="router.push('/personal/exams/edit/' + record.id)">
                              <span class="d-flex align-items-center gap-2 text-primary small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 1 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
                                Chỉnh sửa kỳ thi
                              </span>
                            </a-menu-item>
                            <a-menu-item key="candidates" @click="router.push('/personal/exams/candidates/' + record.id)">
                              <span class="d-flex align-items-center gap-2 text-indigo small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>
                                Quản lý phòng thi
                              </span>
                            </a-menu-item>
                            <a-menu-item key="delete" @click="confirmDeleteQuiz(record.id)">
                              <span class="d-flex align-items-center gap-2 text-danger small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path><line x1="10" y1="11" x2="10" y2="17"></line><line x1="14" y1="11" x2="14" y2="17"></line></svg>
                                Xóa kỳ thi
                              </span>
                            </a-menu-item>
                          </template>

                          <!-- Tab đã lưu: Làm thử, Xóa khỏi danh sách lưu -->
                          <template v-else-if="quizSubTab === 'saved'">
                            <a-menu-item key="practice" @click="simulateTakeQuiz(record)" :disabled="!isQuizActive(record)">
                              <span class="d-flex align-items-center gap-2 text-accent small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path><polyline points="14 2 14 8 20 8"></polyline><line x1="16" y1="13" x2="8" y2="13"></line><line x1="16" y1="17" x2="8" y2="17"></line></svg>
                                Làm thử
                              </span>
                            </a-menu-item>
                            <a-menu-item key="unsave" @click="unsaveQuiz(record.id)">
                              <span class="d-flex align-items-center gap-2 text-danger small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
                                Xóa khỏi danh sách lưu
                              </span>
                            </a-menu-item>
                          </template>

                          <!-- Tab đã làm: Chỉ xem chi tiết lịch sử làm -->
                          <template v-else-if="quizSubTab === 'done'">
                            <a-menu-item key="history" @click="viewQuizHistory(record)">
                              <span class="d-flex align-items-center gap-2 text-indigo small">
                                <svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><polyline points="12 6 12 12 16 14"></polyline></svg>
                                Xem chi tiết lịch sử làm
                              </span>
                            </a-menu-item>
                          </template>
                        </a-menu>
                      </template>
                    </a-dropdown>
                  </div>
                </template>
              </template>
            </a-table>
          </div>
        </a-tab-pane>
      </a-tabs>
    </div>

    <!-- Modal 1: Tạo Đề thi mới -->
    <a-modal 
      v-model:open="examModalOpen" 
      title="Tạo đề thi mới từ Ngân hàng câu hỏi" 
      @ok="saveExam"
      ok-text="Tạo đề thi"
      cancel-text="Hủy bỏ"
      width="780px"
    >
      <div class="py-3">
        <a-form layout="vertical">
          <div class="row">
            <div class="col-md-6">
              <a-form-item label="Tên đề thi (ví dụ: Đề cương chương 1):" required>
                <a-input v-model:value="examForm.name" placeholder="Nhập tên đề thi..." />
              </a-form-item>
            </div>
            <div class="col-md-6">
              <a-form-item label="Danh mục môn học tương ứng:" required>
                <a-select v-model:value="examForm.categoryId" placeholder="Chọn môn học">
                  <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id">
                    {{ cat.name }}
                  </a-select-option>
                </a-select>
              </a-form-item>
            </div>
          </div>

          <div class="row">
            <div class="col-md-4">
              <a-form-item label="Thời gian làm bài (phút):" required>
                <a-input-number v-model:value="examForm.duration" :min="5" :max="180" style="width: 100%" />
              </a-form-item>
            </div>
            <div class="col-md-4">
              <a-form-item label="Phạm vi chia sẻ đề:" required>
                <a-select v-model:value="examForm.accessType" style="width: 100%">
                  <a-select-option :value="0">Chỉ mình tôi (Cá nhân)</a-select-option>
                  <a-select-option :value="1">Công khai cho mọi học viên</a-select-option>
                </a-select>
              </a-form-item>
            </div>
            <div class="col-md-4">
              <a-form-item label="Cách thức tạo đề:" required>
                <a-radio-group v-model:value="examForm.mode" button-style="solid" class="w-100 d-flex">
                  <a-radio-button value="auto" class="flex-grow-1 text-center">Trộn Tự Động</a-radio-button>
                  <a-radio-button value="manual" class="flex-grow-1 text-center">Chọn Thủ Công</a-radio-button>
                </a-radio-group>
              </a-form-item>
            </div>
          </div>

          <a-form-item label="Mô tả đề thi:">
            <a-textarea v-model:value="examForm.description" placeholder="Nhập mô tả ngắn gọn..." :rows="2" />
          </a-form-item>

          <!-- MODE: AUTO GENERATE -->
          <div v-if="examForm.mode === 'auto'" class="border rounded-4 p-4 bg-light-soft mb-3">
            <h5 class="fs-6 fw-bold text-dark-blue mb-3">Thiết lập quy tắc chọn câu hỏi tự động</h5>
            <div class="row g-3">
              <div class="col-md-4">
                <label class="form-label small fw-bold mb-1 text-success">Số câu Dễ:</label>
                <a-input-number v-model:value="examForm.rules.easyCount" :min="0" :max="50" style="width: 100%" />
              </div>
              <div class="col-md-4">
                <label class="form-label small fw-bold mb-1 text-warning">Số câu Trung Bình:</label>
                <a-input-number v-model:value="examForm.rules.mediumCount" :min="0" :max="50" style="width: 100%" />
              </div>
              <div class="col-md-4">
                <label class="form-label small fw-bold mb-1 text-danger">Số câu Khó:</label>
                <a-input-number v-model:value="examForm.rules.hardCount" :min="0" :max="50" style="width: 100%" />
              </div>
            </div>
            <div class="mt-3 text-secondary small d-flex justify-content-between align-items-center">
              <span>Đề thi sẽ tự động lấy ngẫu nhiên các câu hỏi từ Ngân hàng tương ứng với bộ lọc môn học trên.</span>
              <span class="fw-bold text-indigo fs-6">Tổng cộng: {{ totalAutoQuestionsSelected }} câu hỏi</span>
            </div>
          </div>

          <!-- MODE: MANUAL SELECTION -->
          <div v-else class="border rounded-4 p-3 bg-light-soft mb-3">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h5 class="fs-6 fw-bold text-dark-blue mb-0">Chọn câu hỏi từ Ngân hàng câu hỏi môn đã chọn</h5>
              <span class="badge bg-indigo text-white fs-7">{{ selectedManualQuestions.length }} câu hỏi đã chọn</span>
            </div>

            <!-- Manual question selection table -->
            <div class="table-responsive" style="max-height: 250px; overflow-y: auto;">
              <table class="table table-sm table-hover table-bordered bg-white small">
                <thead class="table-light sticky-top">
                  <tr>
                    <th style="width: 40px;" class="text-center">Chọn</th>
                    <th>Nội dung câu hỏi</th>
                    <th style="width: 100px;">Độ khó</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="q in getQuestionsForSelectedCategory" :key="q.id">
                    <td class="text-center">
                      <input 
                        type="checkbox" 
                        :checked="selectedManualQuestions.includes(q.id)"
                        @change="toggleManualQuestion(q.id)"
                      />
                    </td>
                    <td>{{ q.stringContent }}</td>
                    <td>
                      <span :class="getLevelBadgeClass(q.level)">{{ getLevelText(q.level) }}</span>
                    </td>
                  </tr>
                  <tr v-if="getQuestionsForSelectedCategory.length === 0">
                    <td colspan="3" class="text-center text-muted py-4">Không có câu hỏi nào trong ngân hàng phù hợp với môn học này. Hãy thêm câu hỏi trước!</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </a-form>
      </div>
    </a-modal>

    <!-- Modal 2: Tạo Bài kiểm tra / Kỳ thi mới -->
    <a-modal 
      v-model:open="quizModalOpen" 
      title="Tổ chức Bài kiểm tra mới" 
      @ok="saveQuiz"
      ok-text="Bắt đầu tổ chức"
      cancel-text="Hủy bỏ"
      width="650px"
    >
      <div class="py-3">
        <a-form layout="vertical">
          <a-form-item label="Tên bài kiểm tra / Kỳ thi (ví dụ: Thi thử giữa kỳ Toán):" required>
            <a-input v-model:value="quizForm.title" placeholder="Nhập tiêu đề kỳ thi..." />
          </a-form-item>

          <div class="row">
            <div class="col-md-6">
              <a-form-item label="Đối tượng tham gia (Lớp/Nhóm):" required>
                <a-input v-model:value="quizForm.targetGroup" placeholder="Ví dụ: Lớp 12A1, Mọi học viên..." />
              </a-form-item>
            </div>
            <div class="col-md-6">
              <a-form-item label="Nguồn câu hỏi cho bài thi:" required>
                <a-select v-model:value="quizForm.sourceType" style="width: 100%">
                  <a-select-option value="exam">Lấy từ Đề thi sẵn có</a-select-option>
                  <a-select-option value="direct">Tạo ngẫu nhiên từ Ngân hàng câu hỏi</a-select-option>
                </a-select>
              </a-form-item>
            </div>
          </div>

          <!-- If using existing exam -->
          <div v-if="quizForm.sourceType === 'exam'">
            <a-form-item label="Chọn đề thi có sẵn:" required>
              <a-select v-model:value="quizForm.examId" placeholder="Chọn đề thi từ danh sách">
                <a-select-option v-for="ex in exams" :key="ex.id" :value="ex.id">
                  {{ ex.name }} ({{ ex.questions?.length || 0 }} câu - {{ ex.duration }} phút)
                </a-select-option>
              </a-select>
            </a-form-item>
          </div>

          <!-- If generating directly from bank -->
          <div v-else class="border rounded-4 p-3 bg-light-soft mb-3">
            <h5 class="fs-7 fw-bold text-dark-blue mb-2">Quy tắc trộn đề thi trực tiếp:</h5>
            <div class="row g-2">
              <div class="col-md-6">
                <label class="form-label small mb-1">Chọn chuyên đề/môn học:</label>
                <a-select v-model:value="quizForm.directRule.categoryId" style="width: 100%" placeholder="Môn học">
                  <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id">
                    {{ cat.name }}
                  </a-select-option>
                </a-select>
              </div>
              <div class="col-md-3">
                <label class="form-label small mb-1">Số câu hỏi:</label>
                <a-input-number v-model:value="quizForm.directRule.totalQuestions" :min="5" :max="100" style="width: 100%" />
              </div>
              <div class="col-md-3">
                <label class="form-label small mb-1">Thời gian (phút):</label>
                <a-input-number v-model:value="quizForm.directRule.duration" :min="5" :max="180" style="width: 100%" />
              </div>
            </div>
          </div>

          <!-- Open/Close Schedule -->
          <div class="row">
            <div class="col-md-6">
              <a-form-item label="Thời gian mở đề:" required>
                <a-input type="datetime-local" v-model:value="quizForm.startDate" style="width: 100%" />
              </a-form-item>
            </div>
            <div class="col-md-6">
              <a-form-item label="Thời gian đóng đề:" required>
                <a-input type="datetime-local" v-model:value="quizForm.endDate" style="width: 100%" />
              </a-form-item>
            </div>
          </div>
        </a-form>
      </div>
    </a-modal>

    <!-- Modal 3: Chi tiết câu hỏi trong Đề thi -->
    <a-modal 
      v-model:open="detailModalOpen" 
      :title="selectedExamForDetail ? 'Chi tiết đề thi: ' + selectedExamForDetail.name : 'Chi tiết đề'"
      :footer="null"
      width="750px"
    >
      <div class="py-3" v-if="selectedExamForDetail">
        <div class="d-flex justify-content-between align-items-center pb-3 border-bottom mb-3">
          <div>
            <span class="badge bg-indigo-soft text-indigo me-2">{{ getCategoryName(selectedExamForDetail.categoryId) }}</span>
            <span class="text-secondary small">{{ selectedExamForDetail.duration }} phút làm bài</span>
          </div>
          <span class="fw-bold text-dark-blue fs-6">Tổng số câu hỏi: {{ selectedExamForDetail.questions?.length || 0 }} câu</span>
        </div>

        <div class="exam-questions-list" style="max-height: 400px; overflow-y: auto;">
          <div v-for="(q, index) in selectedExamForDetail.questions" :key="q.id" class="card card-body p-3 mb-3 border-light shadow-sm bg-light-soft">
            <p class="fw-bold text-dark-blue mb-2">Câu {{ index + 1 }}: <span class="fw-normal">{{ q.stringContent }}</span></p>
            <div class="row g-2 mb-2">
              <div v-for="(ans, aIdx) in q.answers" :key="aIdx" class="col-md-6">
                <div class="p-2 border rounded-3 bg-white small d-flex align-items-center gap-2">
                  <span class="badge" :class="(viewDetailShowAnswers && ans.isCorrectAnswer) ? 'bg-success text-white' : 'bg-light text-dark border'">
                    {{ String.fromCharCode(65 + aIdx) }}
                  </span>
                  <span>{{ ans.stringContent }}</span>
                </div>
              </div>
            </div>
            <div v-if="viewDetailShowAnswers" class="text-muted small mt-1">
              Hướng dẫn giải: <span class="fst-italic">{{ q.explanation || 'Không có hướng dẫn giải.' }}</span>
            </div>
          </div>
        </div>
      </div>
    </a-modal>

    <!-- Modal 5: Xem chi tiết lịch sử làm bài thi / bài kiểm tra -->
    <a-modal 
      v-model:open="historyModalOpen" 
      :title="historyModalTitle"
      :footer="null"
      width="780px"
    >
      <div class="py-2" v-if="historyModalQuestions.length > 0">
        <!-- Summary bar -->
        <div class="row g-3 p-3 bg-light rounded-3 mb-4 text-center">
          <div class="col-4">
            <div class="text-secondary small">Điểm số</div>
            <div class="fs-5 fw-bold text-indigo">{{ historyModalScore }}</div>
          </div>
          <div class="col-4 border-start border-end">
            <div class="text-secondary small">Thời gian làm</div>
            <div class="fs-5 fw-bold text-dark-blue">{{ historyModalDuration }}</div>
          </div>
          <div class="col-4">
            <div class="text-secondary small">Thời điểm nộp</div>
            <div class="fs-6 fw-bold text-dark-blue mt-1">{{ historyModalTime }}</div>
          </div>
        </div>

        <h4 class="fs-6 fw-bold text-dark-blue mb-3">Danh sách câu hỏi và đáp án chi tiết:</h4>
        <div style="max-height: 450px; overflow-y: auto; padding-right: 4px;">
          <div 
            v-for="(q, index) in historyModalQuestions" 
            :key="q.id" 
            class="card p-3 mb-3 border-light shadow-sm"
            :class="isModalQuestionCorrect(q) ? 'bg-success-soft border-success-subtle' : 'bg-danger-soft border-danger-subtle'"
          >
            <div class="d-flex justify-content-between align-items-center mb-2">
              <span class="fw-bold text-indigo">Câu {{ index + 1 }}</span>
              <span :class="isModalQuestionCorrect(q) ? 'badge bg-success text-white px-2 py-1' : 'badge bg-danger text-white px-2 py-1'">
                {{ isModalQuestionCorrect(q) ? 'Đúng' : 'Sai' }}
              </span>
            </div>

            <p class="fw-bold text-dark-blue mb-3 small">{{ q.stringContent }}</p>

            <div class="row g-2 mb-3">
              <div v-for="(ans, aIdx) in q.answers" :key="aIdx" class="col-md-6">
                <div 
                  class="p-2 border rounded-3 small d-flex align-items-center gap-2 h-100"
                  :class="getModalAnswerRowClass(q, aIdx, ans)"
                >
                  <span 
                    class="badge" 
                    :class="ans.isCorrectAnswer ? 'bg-success text-white' : (isModalUserChosen(q, aIdx) ? 'bg-danger text-white' : 'bg-light text-dark border')"
                  >
                    {{ String.fromCharCode(65 + Number(aIdx)) }}
                  </span>
                  <span class="flex-grow-1">{{ ans.stringContent }}</span>
                  <span v-if="ans.isCorrectAnswer" class="badge bg-success-soft text-success fs-9 ms-auto">Đúng</span>
                  <span v-if="isModalUserChosen(q, aIdx)" class="badge bg-indigo-soft text-indigo fs-9 ms-auto">Bạn chọn</span>
                </div>
              </div>
            </div>
            <div class="text-muted small mt-1 p-2 bg-white rounded border-start border-indigo border-3">
              <span class="fw-bold text-indigo">Giải thích:</span> <span class="fst-italic">{{ q.explanation || q.explaination || 'Không có giải thích chi tiết.' }}</span>
            </div>
          </div>
        </div>
      </div>
      <div v-else class="text-center py-5">
        <div class="fs-2 mb-2">📄</div>
        <div class="text-secondary small">Không tìm thấy danh sách câu hỏi của bài thi này.</div>
      </div>
    </a-modal>

    <!-- Centered Exam Filter Modal -->
    <a-modal
      v-model:open="examFilterModalOpen"
      title="Bộ lọc đề thi"
      :footer="null"
      centered
      width="380px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label small fw-semibold">Danh mục môn học:</label>
          <a-select v-model:value="examFilters.categoryId" style="width: 100%" placeholder="Tất cả môn học" allow-clear>
            <a-select-option v-for="cat in categories" :key="cat.id" :value="cat.id">
              {{ cat.name }}
            </a-select-option>
          </a-select>
        </div>
        <div class="mb-3">
          <label class="form-label small fw-semibold">Phạm vi chia sẻ:</label>
          <a-select v-model:value="examFilters.accessType" style="width: 100%" placeholder="Tất cả phạm vi" allow-clear>
            <a-select-option :value="0">🔒 Cá nhân</a-select-option>
            <a-select-option :value="1">🌐 Công khai</a-select-option>
          </a-select>
        </div>
        <div class="border-top pt-2 mt-4 d-flex justify-content-between align-items-center">
          <button class="btn btn-link btn-xs text-secondary p-0 text-decoration-none small" @click="resetExamFilters">Đặt lại bộ lọc</button>
          <button class="btn btn-indigo text-white btn-sm px-4" @click="examFilterModalOpen = false">Áp dụng</button>
        </div>
      </div>
    </a-modal>

    <!-- Centered Quiz Filter Modal -->
    <a-modal
      v-model:open="quizFilterModalOpen"
      title="Bộ lọc bài kiểm tra"
      :footer="null"
      centered
      width="380px"
    >
      <div class="py-2">
        <div class="mb-3">
          <label class="form-label small fw-semibold">Trạng thái mở đề:</label>
          <a-select v-model:value="quizFilters.status" style="width: 100%" placeholder="Tất cả trạng thái" allow-clear>
            <a-select-option value="active">Đang mở làm bài</a-select-option>
            <a-select-option value="expired">Đã hết hạn</a-select-option>
          </a-select>
        </div>
        <div class="border-top pt-2 mt-4 d-flex justify-content-between align-items-center">
          <button class="btn btn-link btn-xs text-secondary p-0 text-decoration-none small" @click="resetQuizFilters">Đặt lại bộ lọc</button>
          <button class="btn btn-indigo text-white btn-sm px-4" @click="quizFilterModalOpen = false">Áp dụng</button>
        </div>
      </div>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { useRouter } from 'vue-router'
import { getAllCate } from '@/api/categories'
import { getAllExams, getExamQuestions, deleteExams, getExamQuestionCounts } from '@/api/exams'
import { getAllQuizzes, saveQuizDetails, deleteQuiz } from '@/api/quizzes'
import { getSavedExamIds, toggleExamBookmark } from '@/api/bookmarks'

const router = useRouter()

// Seed data
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
}

interface Category {
  id: string
  name: string
}

const categories = ref<Category[]>([])

// Helper to check and return a valid Guid
const ensureGuid = (id: string | null | undefined): string | null => {
  if (!id) return null
  const isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(id)
  return isGuid ? id : null
}

// Mock DB values to link together
const questionsList = ref<Question[]>([])
const exams = ref<Exam[]>([])
const quizzes = ref<Quiz[]>([])

// State
const activeTab = ref('exams')
const examModalOpen = ref(false)
const quizModalOpen = ref(false)
const detailModalOpen = ref(false)
const selectedExamForDetail = ref<Exam | null>(null)

const historyModalOpen = ref(false)
const historyModalTitle = ref('')
const historyModalScore = ref('8.5 / 10.0')
const historyModalDuration = ref('32 phút')
const historyModalTime = ref('13:42:05 - 06/07/2026')
const historyModalQuestions = ref<any[]>([])
const historyModalAnswers = ref<Record<string, number[]>>({})

const populateMockHistoryAnswers = (questions: any[]) => {
  const answerMap: Record<string, number[]> = {}
  questions.forEach(q => {
    const correctIdxs = q.answers
      .map((a: any, i: number) => a.isCorrectAnswer ? i : -1)
      .filter((i: number) => i !== -1)
    
    if (Math.random() < 0.85) {
      answerMap[q.id] = correctIdxs
    } else {
      const incorrectIdx = q.answers.findIndex((a: any) => !a.isCorrectAnswer)
      answerMap[q.id] = incorrectIdx !== -1 ? [incorrectIdx] : [0]
    }
  })
  historyModalAnswers.value = answerMap
}

const getFallbackQuestions = (): Question[] => {
  const chars = ['A', 'B', 'C', 'D']
  return [
    {
      id: 'q001',
      slug: 'q001',
      level: 1,
      type: 1,
      accessType: 1,
      categoryIds: [],
      stringContent: 'Điểm khác biệt lớn nhất giữa IEnumerable và IQueryable là gì?',
      explanation: 'IEnumerable thực hiện lọc dữ liệu trên Client (In-Memory), còn IQueryable thực hiện lọc phía Server (Database).',
      answers: [
        { stringContent: 'IEnumerable lọc ở Client, IQueryable lọc ở Server Database', isCorrectAnswer: true },
        { stringContent: 'IEnumerable lọc ở Server, IQueryable lọc ở Client', isCorrectAnswer: false },
        { stringContent: 'IEnumerable thuộc .NET Framework cũ, IQueryable thuộc .NET Core', isCorrectAnswer: false }
      ].map((ans, idx) => ({ ...ans, indexChar: chars[idx] }))
    },
    {
      id: 'q002',
      slug: 'q002',
      level: 1,
      type: 1,
      accessType: 1,
      categoryIds: [],
      stringContent: 'Cú pháp khai báo biến hằng trong C# là gì?',
      explanation: 'Biến hằng trong C# được khai báo bằng từ khóa const đứng trước kiểu dữ liệu.',
      answers: [
        { stringContent: 'const <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;', isCorrectAnswer: true },
        { stringContent: 'readonly <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;', isCorrectAnswer: false },
        { stringContent: 'static <kiểu_dữ_liệu> <tên_biến> = <giá_trị>;', isCorrectAnswer: false }
      ].map((ans, idx) => ({ ...ans, indexChar: chars[idx] }))
    },
    {
      id: 'q003',
      slug: 'q003',
      level: 1,
      type: 1,
      accessType: 1,
      categoryIds: [],
      stringContent: 'Một sóng âm truyền từ không khí vào nước thì:',
      explanation: 'Khi truyền qua các môi trường khác nhau, tần số của sóng âm không đổi, nhưng vận tốc truyền sóng đổi dẫn đến bước sóng đổi.',
      answers: [
        { stringContent: 'tần số không đổi, bước sóng tăng.', isCorrectAnswer: true },
        { stringContent: 'chu kỳ tăng, vận tốc giảm.', isCorrectAnswer: false },
        { stringContent: 'bước sóng không đổi, chu kỳ giảm.', isCorrectAnswer: false },
        { stringContent: 'tần số tăng, bước sóng giảm.', isCorrectAnswer: false }
      ].map((ans, idx) => ({ ...ans, indexChar: chars[idx] }))
    }
  ]
}

const viewExamHistory = (exam: Exam) => {
  historyModalTitle.value = 'Lịch sử làm bài: ' + exam.name
  historyModalScore.value = '8.5 / 10.0'
  historyModalDuration.value = '32 phút 15 giây'
  historyModalTime.value = '13:42:05 - 06/07/2026'
  
  let qs = exam.questions || []
  if (qs.length === 0) {
    qs = getFallbackQuestions()
  }
  historyModalQuestions.value = qs
  populateMockHistoryAnswers(qs)
  historyModalOpen.value = true
}

const viewQuizHistory = (quiz: Quiz) => {
  historyModalTitle.value = 'Lịch sử làm bài: ' + quiz.title
  historyModalScore.value = '9.0 / 10.0'
  historyModalDuration.value = '25 phút'
  historyModalTime.value = '15:30:10 - 05/07/2026'
  
  let qs: any[] = []
  const foundExam = exams.value.find(e => e.id === quiz.examId)
  if (foundExam && foundExam.questions && foundExam.questions.length > 0) {
    qs = foundExam.questions
  } else {
    qs = getFallbackQuestions()
  }
  historyModalQuestions.value = qs
  populateMockHistoryAnswers(qs)
  historyModalOpen.value = true
}

const isModalUserChosen = (q: any, aIdx: number | string) => {
  const chosen = historyModalAnswers.value[q.id]
  const idx = typeof aIdx === 'string' ? parseInt(aIdx) : aIdx
  return chosen ? chosen.includes(idx) : false
}

const isModalQuestionCorrect = (q: any) => {
  const chosen = historyModalAnswers.value[q.id]
  if (!chosen) return false
  const correctIdxs = q.answers
    .map((a: any, i: number) => a.isCorrectAnswer ? i : -1)
    .filter((i: number) => i !== -1)
  return correctIdxs.length === chosen.length &&
         correctIdxs.every((i: number) => chosen.includes(i))
}

const getModalAnswerRowClass = (q: any, aIdx: number | string, ans: any) => {
  const chosen = isModalUserChosen(q, aIdx)
  if (ans.isCorrectAnswer) {
    return 'border-success bg-success-soft text-success'
  }
  if (chosen) {
    return 'border-danger bg-danger-soft text-danger'
  }
  return 'bg-white border-light'
}

const examSubTab = ref('mine')
const quizSubTab = ref('mine')
const examFilterModalOpen = ref(false)
const quizFilterModalOpen = ref(false)
const savedExamIds = ref<string[]>([])
const doneExamIds = ref<string[]>([])
const savedQuizIds = ref<string[]>([])
const doneQuizIds = ref<string[]>([])

// Filters
const examFilters = reactive({
  search: '',
  categoryId: undefined as string | undefined,
  accessType: undefined as number | undefined
})

const activeExamFiltersCount = computed(() => {
  let count = 0
  if (examFilters.categoryId !== undefined) count++
  if (examFilters.accessType !== undefined) count++
  return count
})
const hasActiveExamFilters = computed(() => activeExamFiltersCount.value > 0)

const resetExamFilters = () => {
  examFilters.categoryId = undefined
  examFilters.accessType = undefined
}

const quizFilters = reactive({
  search: '',
  status: undefined as string | undefined
})

const activeQuizFiltersCount = computed(() => {
  let count = 0
  if (quizFilters.status !== undefined) count++
  return count
})
const hasActiveQuizFilters = computed(() => activeQuizFiltersCount.value > 0)

const resetQuizFilters = () => {
  quizFilters.status = undefined
}

// Tables Column configs
const examColumns = [
  { title: 'Tên đề thi', dataIndex: 'name', key: 'name' },
  { title: 'Danh mục', dataIndex: 'category', key: 'category', width: '260px' },
  { title: 'Số câu', dataIndex: 'questionsCount', key: 'questionsCount', width: '120px' },
  { title: 'Thời gian', dataIndex: 'duration', key: 'duration', width: '120px' },
  { title: 'Thao tác', key: 'actions', width: '90px' }
]

const quizColumns = [
  { title: 'Kỳ thi / Bài kiểm tra', dataIndex: 'title', key: 'title' },
  { title: 'Nguồn bài thi', dataIndex: 'examSource', key: 'examSource', width: '220px' },
  { title: 'Khung thời gian', dataIndex: 'timeWindow', key: 'timeWindow', width: '220px' },
  { title: 'Trạng thái', dataIndex: 'status', key: 'status', width: '130px' },
  { title: 'Thao tác', key: 'actions', width: '90px' }
]

// Exam creation Form State
const examForm = reactive({
  name: '',
  description: '',
  categoryId: undefined as string | undefined,
  duration: 45,
  accessType: 1, // Public
  mode: 'auto' as 'auto' | 'manual',
  rules: {
    easyCount: 10,
    mediumCount: 10,
    hardCount: 5
  }
})
const selectedManualQuestions = ref<string[]>([])

// Quiz creation Form State
const quizForm = reactive({
  title: '',
  targetGroup: 'Lớp 12A1',
  sourceType: 'exam' as 'exam' | 'direct',
  examId: undefined as string | undefined,
  startDate: '',
  endDate: '',
  directRule: {
    categoryId: undefined as string | undefined,
    totalQuestions: 20,
    duration: 45
  }
})

// Computed values for exam creation
const totalAutoQuestionsSelected = computed(() => {
  return examForm.rules.easyCount + examForm.rules.mediumCount + examForm.rules.hardCount
})

const getQuestionsForSelectedCategory = computed(() => {
  if (!examForm.categoryId) return []
  return questionsList.value.filter(q => q.categoryIds.includes(examForm.categoryId!))
})

// Filtered data
const filteredExams = computed(() => {
  return exams.value.filter(e => {
    // Tab filtering
    if (examSubTab.value === 'mine' && !e.isMyCreated) {
      return false
    }
    if (examSubTab.value === 'saved' && !savedExamIds.value.includes(e.id)) {
      return false
    }
    if (examSubTab.value === 'done' && !doneExamIds.value.includes(e.id)) {
      return false
    }

    if (examFilters.search && !e.name.toLowerCase().includes(examFilters.search.toLowerCase())) return false
    if (examFilters.categoryId && e.categoryId !== examFilters.categoryId) return false
    if (examFilters.accessType !== undefined && e.accessType !== examFilters.accessType) return false
    return true
  })
})

const filteredQuizzes = computed(() => {
  return quizzes.value.filter(q => {
    // Tab filtering
    if (quizSubTab.value === 'mine' && !q.isMyCreated) {
      return false
    }
    if (quizSubTab.value === 'saved' && !savedQuizIds.value.includes(q.id)) {
      return false
    }
    if (quizSubTab.value === 'done' && !doneQuizIds.value.includes(q.id)) {
      return false
    }

    if (quizFilters.search && !q.title.toLowerCase().includes(quizFilters.search.toLowerCase())) return false
    if (quizFilters.status) {
      const active = isQuizActive(q)
      if (quizFilters.status === 'active' && !active) return false
      if (quizFilters.status === 'expired' && active) return false
    }
    return true
  })
})

// Helpers
const getCategoryName = (id: string) => {
  const cat = categories.value.find(c => c.id === id)
  return cat ? cat.name : 'Chuyên đề chung'
}

const getExamName = (id?: string) => {
  if (!id) return 'Đề thi không xác định'
  const ex = exams.value.find(e => e.id === id)
  return ex ? ex.name : 'Đề thi đã bị xóa'
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

const formatDate = (dateStr: string) => {
  if (!dateStr) return '--:--'
  const date = new Date(dateStr)
  return date.toLocaleString('vi-VN', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric' })
}

const isQuizActive = (quiz: Quiz): boolean => {
  const now = new Date()
  const start = new Date(quiz.startDate)
  const end = new Date(quiz.endDate)
  return now >= start && now <= end
}

// Manual question selection helper
const toggleManualQuestion = (id: string) => {
  const index = selectedManualQuestions.value.indexOf(id)
  if (index === -1) {
    selectedManualQuestions.value.push(id)
  } else {
    selectedManualQuestions.value.splice(index, 1)
  }
}

// Modal opening
const openCreateExamModal = () => {
  examForm.name = ''
  examForm.description = ''
  examForm.categoryId = categories.value[0]?.id
  examForm.duration = 45
  examForm.accessType = 1
  examForm.mode = 'auto'
  examForm.rules.easyCount = 3
  examForm.rules.mediumCount = 2
  examForm.rules.hardCount = 1
  selectedManualQuestions.value = []
  examModalOpen.value = true
}

const openCreateQuizModal = () => {
  if (exams.value.length === 0) {
    message.warning('Vui lòng tạo ít nhất một Đề thi mẫu trước khi tổ chức Kỳ thi!')
    return
  }
  
  // Set default dates
  const now = new Date()
  const localStart = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16)
  const localEnd = new Date(now.getTime() + 7 * 24 * 60 * 60000 - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16)

  quizForm.title = ''
  quizForm.targetGroup = 'Lớp 12A1'
  quizForm.sourceType = 'exam'
  quizForm.examId = exams.value[0]?.id
  quizForm.startDate = localStart
  quizForm.endDate = localEnd
  if (quizForm.directRule) {
    quizForm.directRule.categoryId = categories.value[0]?.id || ''
    quizForm.directRule.totalQuestions = 15
    quizForm.directRule.duration = 45
  }
  quizModalOpen.value = true
}

const viewDetailShowAnswers = ref(true)
const viewExamDetails = async (exam: Exam, showAnswers: boolean = true) => {
  const hasRealQuestions = exam.questions && exam.questions.length > 0 && exam.questions[0] && Object.keys(exam.questions[0]).length > 0
  if (!hasRealQuestions && exam.id) {
    try {
      const res = await getExamQuestions(exam.id)
      if (res && res.isSuccess && res.data) {
        exam.questions = res.data.map((q: any) => ({
          id: q.id,
          slug: q.slug || '',
          stringContent: q.stringContent,
          explanation: q.explanation || q.explaination || '',
          level: q.level,
          type: q.type,
          accessType: q.accessType,
          categoryIds: q.categoryIds || [],
          answers: (q.answers || []).map((a: any) => ({
            questionAnswerId: a.questionAnswerId,
            stringContent: a.stringContent,
            isCorrectAnswer: a.isCorrectAnswer
          }))
        }))
      }
    } catch (err) {
      console.error('Lỗi khi tải câu hỏi của đề thi:', err)
    }
  }

  selectedExamForDetail.value = exam
  viewDetailShowAnswers.value = showAnswers
  detailModalOpen.value = true
}

// Actions: Delete & Unsave & History
const confirmDeleteExam = (id: string) => {
  Modal.confirm({
    title: 'Xác nhận xóa đề thi',
    content: 'Bạn có chắc chắn muốn xóa đề thi này? Các kỳ thi liên quan đang trỏ tới đề thi này có thể bị ảnh hưởng.',
    okText: 'Xóa đề',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        const res = await deleteExams([id])
        if (res && res.isSuccess) {
          message.success('Đã xóa đề thi thành công!')
          await fetchExamsList()
        } else {
          message.error(res.errorMessage || 'Lỗi khi xóa đề thi.')
        }
      } catch (error) {
        console.error('Lỗi khi xóa đề thi:', error)
        message.error('Đã xảy ra lỗi khi kết nối máy chủ để xóa đề thi.')
      }
    }
  })
}

const unsaveExam = (id: string) => {
  Modal.confirm({
    title: 'Xác nhận bỏ lưu đề thi',
    content: 'Bạn có chắc chắn muốn bỏ lưu đề thi này khỏi danh sách đã lưu của bạn?',
    okText: 'Bỏ lưu',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        const res = await toggleExamBookmark(id)
        if (res && res.isSuccess) {
          savedExamIds.value = savedExamIds.value.filter(item => item !== id)
          message.success('Đã xóa đề thi khỏi danh sách đã lưu!')
        } else {
          message.error((res as any)?.userMsg || 'Lỗi khi bỏ lưu đề thi')
        }
      } catch (error) {
        message.error('Lỗi khi bỏ lưu đề thi')
      }
    }
  })
}

const confirmDeleteQuiz = (id: string) => {
  Modal.confirm({
    title: 'Xác nhận hủy bài kiểm tra',
    content: 'Bạn có chắc chắn muốn xóa bài kiểm tra/kỳ thi này khỏi danh sách tổ chức?',
    okText: 'Xóa kỳ thi',
    okType: 'danger',
    cancelText: 'Hủy',
    async onOk() {
      try {
        const res = await deleteQuiz(id)
        if (res && res.isSuccess) {
          message.success('Đã xóa bài kiểm tra thành công!')
          await fetchQuizzesList()
        } else {
          message.error(res.errorMessage || 'Lỗi khi xóa bài kiểm tra.')
        }
      } catch (error) {
        console.error('Lỗi khi xóa bài kiểm tra:', error)
        message.error('Đã xảy ra lỗi khi kết nối máy chủ để xóa bài kiểm tra.')
      }
    }
  })
}

const unsaveQuiz = (id: string) => {
  Modal.confirm({
    title: 'Xác nhận bỏ lưu kỳ thi',
    content: 'Bạn có chắc chắn muốn bỏ lưu kỳ thi này khỏi danh sách đã lưu của bạn?',
    okText: 'Bỏ lưu',
    okType: 'danger',
    cancelText: 'Hủy',
    onOk() {
      savedQuizIds.value = savedQuizIds.value.filter(item => item !== id)
      localStorage.setItem('cn_saved_quizzes', JSON.stringify(savedQuizIds.value))
      message.success('Đã xóa kỳ thi khỏi danh sách đã lưu!')
    }
  })
}

// Action: Save Exam
const saveExam = () => {
  if (!examForm.name.trim()) {
    message.error('Vui lòng nhập tên đề thi!')
    return
  }
  if (!examForm.categoryId) {
    message.error('Vui lòng chọn danh mục môn học!')
    return
  }

  let finalQuestions: Question[] = []

  if (examForm.mode === 'auto') {
    // Pick questions automatically from the Category pool
    const pool = questionsList.value.filter(q => q.categoryIds.includes(examForm.categoryId!))
    
    const easyPool = pool.filter(q => q.level === 0)
    const mediumPool = pool.filter(q => q.level === 1)
    const hardPool = pool.filter(q => q.level === 2)

    if (easyPool.length < examForm.rules.easyCount || 
        mediumPool.length < examForm.rules.mediumCount || 
        hardPool.length < examForm.rules.hardCount) {
      Modal.error({
        title: 'Không đủ câu hỏi trong Ngân hàng',
        content: `Môn này chỉ có ${easyPool.length} câu Dễ, ${mediumPool.length} câu TB, ${hardPool.length} câu Khó. Hãy giảm số lượng yêu cầu hoặc tạo thêm câu hỏi.`
      })
      return
    }

    // Helper to randomly shuffle and pick N items
    const pickRandom = (arr: Question[], n: number) => {
      const shuffled = [...arr].sort(() => 0.5 - Math.random())
      return shuffled.slice(0, n)
    }

    finalQuestions.push(...pickRandom(easyPool, examForm.rules.easyCount))
    finalQuestions.push(...pickRandom(mediumPool, examForm.rules.mediumCount))
    finalQuestions.push(...pickRandom(hardPool, examForm.rules.hardCount))

  } else {
    // Pick manually selected questions
    if (selectedManualQuestions.value.length === 0) {
      message.error('Vui lòng chọn ít nhất một câu hỏi từ danh sách!')
      return
    }
    finalQuestions = questionsList.value.filter(q => selectedManualQuestions.value.includes(q.id))
  }

  // Compile new exam object
  const newExam: Exam = {
    id: 'exam_' + Date.now(),
    name: examForm.name.trim(),
    description: examForm.description.trim(),
    categoryId: examForm.categoryId,
    duration: examForm.duration,
    accessType: examForm.accessType,
    questions: finalQuestions,
    isMyCreated: true
  }

  exams.value.unshift(newExam)
  saveToStorage()

  message.success(`Tạo đề thi "${newExam.name}" thành công với ${finalQuestions.length} câu hỏi!`)
  examModalOpen.value = false
}

// Action: Save Quiz
const saveQuiz = async () => {
  if (!quizForm.title.trim()) {
    message.error('Vui lòng nhập tên bài kiểm tra!')
    return
  }
  if (!quizForm.startDate || !quizForm.endDate) {
    message.error('Vui lòng điền đủ khung thời gian bắt đầu và kết thúc!')
    return
  }
  if (new Date(quizForm.startDate) >= new Date(quizForm.endDate)) {
    message.error('Thời gian bắt đầu phải trước thời gian kết thúc!')
    return
  }

  const quizPayload = {
    quizId: ensureGuid(''),
    title: quizForm.title.trim(),
    targetGroup: quizForm.targetGroup.trim(),
    sourceType: quizForm.sourceType,
    examId: quizForm.sourceType === 'exam' ? ensureGuid(quizForm.examId) : null,
    startDate: quizForm.startDate ? new Date(quizForm.startDate).toISOString() : null,
    endDate: quizForm.endDate ? new Date(quizForm.endDate).toISOString() : null,
    isDraft: false,
    lockBrowser: false,
    shuffleQuestions: false,
    disableCopyPaste: false,
    fullscreen: false,
    webcam: false,
    ipLimit: false,
    allowLateJoin: true,
    allowLateSubmit: false,
    publicLeaderboard: true,
    sendEmailReport: true,
    directCategoryId: quizForm.sourceType === 'direct' && quizForm.directRule ? ensureGuid(quizForm.directRule.categoryId) : null,
    directTotalQuestions: quizForm.sourceType === 'direct' && quizForm.directRule ? quizForm.directRule.totalQuestions : null,
    directEasyCount: quizForm.sourceType === 'direct' && quizForm.directRule ? Math.floor(quizForm.directRule.totalQuestions * 0.5) : 0,
    directMediumCount: quizForm.sourceType === 'direct' && quizForm.directRule ? Math.floor(quizForm.directRule.totalQuestions * 0.3) : 0,
    directHardCount: quizForm.sourceType === 'direct' && quizForm.directRule ? Math.floor(quizForm.directRule.totalQuestions * 0.2) : 0
  }

  try {
    const res = await saveQuizDetails(quizPayload)
    if (res && res.isSuccess) {
      message.success(`Lên lịch tổ chức bài kiểm tra "${quizForm.title.trim()}" thành công!`)
      await fetchQuizzesList()
      quizModalOpen.value = false
    } else {
      message.error(res.errorMessage || 'Lỗi khi lưu bài kiểm tra.')
    }
  } catch (error) {
    console.error('Lỗi khi lưu bài kiểm tra:', error)
    message.error('Đã xảy ra lỗi khi kết nối máy chủ để lưu bài kiểm tra.')
  }
}

// Simulate Taking Quiz
const simulateTakeQuiz = (quiz: Quiz) => {
  message.info(`Chuẩn bị vào phòng thi thử bài kiểm tra "${quiz.title}"...`)
  
  // Direct to mock quiz practice view
  setTimeout(() => {
    if (quiz.sourceType === 'exam') {
      router.push({ name: 'quiz-practice', params: { id: quiz.examId } })
    } else {
      router.push('/categories')
    }
  }, 1000)
}

// Storage helpers
const saveToStorage = () => {
  localStorage.setItem('cn_exams', JSON.stringify(exams.value))
  localStorage.setItem('cn_quizzes', JSON.stringify(quizzes.value))
}

const loadFromStorage = () => {
  // Load questions to pull from
  const storedQuestions = localStorage.getItem('cn_questions')
  if (storedQuestions) {
    questionsList.value = JSON.parse(storedQuestions)
  }

  // Load exams
  const storedExams = localStorage.getItem('cn_exams')
  if (storedExams) {
    exams.value = JSON.parse(storedExams).map((e: any) => ({ ...e, isMyCreated: e.isMyCreated !== undefined ? e.isMyCreated : true }))
  } else {
    // Seed default exams using our initial questions
    exams.value = [
      {
        id: "exam_default_1",
        name: "Đề ôn tập Tin học và Lập trình nâng cao",
        description: "Đề thi khảo sát kỹ năng lập trình .NET Core và C# căn bản dành cho kỹ sư.",
        categoryId: "c07a92a2-a69f-4143-8589-da11688d7d07",
        duration: 45,
        accessType: 1,
        questions: questionsList.value.filter(q => q.categoryIds.includes("c07a92a2-a69f-4143-8589-da11688d7d07")),
        isMyCreated: true
      },
      {
        id: "exam_default_2",
        name: "Khảo sát Vật Lý Chuyên Đề Dòng Điện Xoay Chiều",
        description: "Đề kiểm tra nhanh 15 phút chuyên đề lý thuyết sóng và dao động.",
        categoryId: "c02a92a2-a69f-4143-8589-da11688d7d02",
        duration: 15,
        accessType: 0,
        questions: questionsList.value.filter(q => q.categoryIds.includes("c02a92a2-a69f-4143-8589-da11688d7d02")),
        isMyCreated: true
      }
    ]
    saveToStorage()
  }

  // Load quizzes
  const storedQuizzes = localStorage.getItem('cn_quizzes')
  if (storedQuizzes) {
    quizzes.value = JSON.parse(storedQuizzes).map((q: any) => ({ ...q, isMyCreated: q.isMyCreated !== undefined ? q.isMyCreated : true }))
  } else {
    // Seed default quizzes
    const now = new Date()
    const startDateVal = new Date(now.getTime() - 24 * 60 * 60000).toISOString().slice(0, 16) // yesterday
    const endDateVal = new Date(now.getTime() + 6 * 24 * 60 * 60000).toISOString().slice(0, 16) // next week

    quizzes.value = [
      {
        id: "quiz_default_1",
        title: "Thi thử học kỳ môn Tin Học lớp 12",
        targetGroup: "Lớp 12A1 & 12A2",
        sourceType: 'exam',
        examId: 'exam_default_1',
        startDate: startDateVal,
        endDate: endDateVal,
        isMyCreated: true
      }
    ]
    saveToStorage()
  }


  const doneEx = localStorage.getItem('cn_done_exams')
  if (doneEx) {
    doneExamIds.value = JSON.parse(doneEx)
  } else {
    doneExamIds.value = ['exam_default_1']
    localStorage.setItem('cn_done_exams', JSON.stringify(doneExamIds.value))
  }

  const savedQz = localStorage.getItem('cn_saved_quizzes')
  if (savedQz) {
    savedQuizIds.value = JSON.parse(savedQz)
  } else {
    savedQuizIds.value = []
    localStorage.setItem('cn_saved_quizzes', JSON.stringify(savedQuizIds.value))
  }

  const doneQz = localStorage.getItem('cn_done_quizzes')
  if (doneQz) {
    doneQuizIds.value = JSON.parse(doneQz)
  } else {
    doneQuizIds.value = ['quiz_default_1']
    localStorage.setItem('cn_done_quizzes', JSON.stringify(doneQuizIds.value))
  }
}

const fetchCategories = async () => {
  try {
    const res = await getAllCate()
    if (res && res.isSuccess && res.data) {
      categories.value = (res.data.items || []).map((cat: any) => ({
        id: cat.questionCategoryId,
        name: cat.name
      }))
    }
  } catch (error) {
    console.error('Lỗi tải danh mục:', error)
  }
}

const fetchExamsList = async () => {
  try {
    const [examsRes, countsRes] = await Promise.all([
      getAllExams(),
      getExamQuestionCounts()
    ])

    if (examsRes && examsRes.isSuccess && examsRes.data) {
      const counts = countsRes?.isSuccess ? (countsRes.data || {}) : {}
      
      exams.value = (examsRes.data.items || []).map((exam: any) => {
        const questionCount = counts[exam.examId] || 0
        return {
          id: exam.examId,
          name: exam.name,
          description: exam.description || '',
          categoryId: exam.categoryId,
          duration: exam.duration,
          accessType: exam.accessType,
          isDraft: exam.isDraft,
          questions: Array(questionCount).fill({}),
          isMyCreated: exam.isMyCreated
        }
      })
    }
  } catch (error) {
    console.error('Lỗi tải danh sách đề thi:', error)
  }
}

const fetchSavedExamsList = async () => {
  try {
    const res = await getSavedExamIds()
    if (res && res.isSuccess && res.data) {
      savedExamIds.value = res.data
    }
  } catch (error) {
    console.error('Lỗi tải danh sách đề thi đã lưu:', error)
  }
}

const fetchQuizzesList = async () => {
  try {
    const res = await getAllQuizzes()
    
    if (res && res.isSuccess && res.data) {
      quizzes.value = (res.data.items || []).map((q: any) => ({
        id: q.quizId,
        title: q.title,
        targetGroup: q.targetGroup || '',
        sourceType: q.sourceType,
        examId: q.examId,
        startDate: q.startDate || '',
        endDate: q.endDate || '',
        directRule: q.sourceType === 'direct' ? {
          categoryId: q.directCategoryId || '',
          totalQuestions: q.directTotalQuestions || 0,
          duration: 45
        } : undefined,
        isMyCreated: q.isMyCreated
      }))
    }
  } catch (error) {
    console.error('Lỗi tải danh sách bài kiểm tra:', error)
  }
}

onMounted(async () => {
  await fetchCategories()
  loadFromStorage()
  await Promise.all([
    fetchExamsList(),
    fetchSavedExamsList(),
    fetchQuizzesList()
  ])
})
</script>

<style scoped>
.text-dark-blue {
  color: #1e1b4b;
}

.text-indigo {
  color: #6366f1;
}

.text-accent {
  color: #06b6d4;
}

.bg-indigo-soft {
  background-color: rgba(99, 102, 241, 0.1);
}

.bg-accent-soft {
  background-color: rgba(6, 182, 212, 0.1);
}

.bg-success-soft {
  background-color: rgba(16, 185, 129, 0.1);
}

.bg-warning-soft {
  background-color: rgba(245, 158, 11, 0.1);
}

.bg-danger-soft {
  background-color: rgba(239, 68, 68, 0.1);
}

.bg-light-soft {
  background-color: #f8fafc;
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

.btn-accent {
  background: linear-gradient(135deg, #06b6d4 0%, #0891b2 100%);
  color: white;
  transition: all 0.2s ease;
  border: none;
}

.btn-accent:hover {
  background: linear-gradient(135deg, #0891b2 0%, #0f766e 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(6, 182, 212, 0.25);
}

.hover-up {
  transition: all 0.2s ease;
}

.hover-up:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(99, 102, 241, 0.25);
}

.custom-tabs :deep(.ant-tabs-nav) {
  margin-bottom: 8px;
}

.custom-tabs :deep(.ant-tabs-tab-active .ant-tabs-tab-btn) {
  color: #6366f1;
  font-weight: 600;
}

.custom-tabs :deep(.ant-tabs-ink-bar) {
  background-color: #6366f1;
}

.badge-public {
  background-color: #dcfce7;
  color: #15803d;
  font-size: 0.8rem;
  padding: 3px 8px;
  border-radius: 6px;
  font-weight: 500;
}

.badge-private {
  background-color: #f1f5f9;
  color: #475569;
  font-size: 0.8rem;
  padding: 3px 8px;
  border-radius: 6px;
  font-weight: 500;
}

.fs-8 {
  font-size: 0.75rem;
}

.custom-table :deep(.ant-table-thead > tr > th) {
  background-color: #f8fafc;
  color: #1e1b4b;
  font-weight: 600;
}

.custom-table :deep(.ant-table-row:hover) {
  background-color: rgba(99, 102, 241, 0.02);
}

.custom-nav-tabs :deep(.ant-tabs-tab),
.custom-list-tabs :deep(.ant-tabs-tab) {
  user-select: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
}
</style>
