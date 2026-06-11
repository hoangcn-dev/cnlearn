/**
 * Danh sách tập trung tất cả các API Endpoints của hệ thống HoangCN
 */
export const endpoints = {
  categories: {
    getAll: "/api/categories",
    save: "/api/categories",
    delete: "/api/categories/delete",
  },
  questions: {
    pagingDetails: "/api/questions/paging-details",
    savedPaging: "/api/questions/saved/paging",
    donePaging: "/api/questions/done/paging",
    details: "/api/questions/details",
    saveDetails: "/api/questions/save-details",
    delete: "/api/questions/delete",
    bulkJson: "/api/questions/bulk/json",
    bulkJsonFile: "/api/questions/bulk/json-file",
  },
  bookmarks: {
    toggleQuestion: "/api/bookmarks/question/toggle",
    toggleExam: "/api/bookmarks/exam/toggle",
    savedQuestionIds: "/api/bookmarks/questions/ids",
    savedExamIds: "/api/bookmarks/exams/ids",
  }
};
