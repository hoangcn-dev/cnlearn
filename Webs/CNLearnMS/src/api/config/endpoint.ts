/**
 * Danh sách tập trung tất cả các API Endpoints của hệ thống HoangCN
 */
export const endpoints = {
  categories: {
    getAll: "/api/categories",
    paging: "/api/categories/paging",
    save: "/api/categories",
    delete: "/api/categories/delete",
  },
  questions: {
    search: "/api/questions/search",
    detail: "/api/questions",
    save: "/api/questions",
    delete: "/api/questions/delete",
    answers: "/api/questions/answers",
    key: "/api/questions/key",
    saved: "/api/questions/saved",
    savedPaging: "/api/questions/saved/paging",
    donePaging: "/api/questions/done/paging",
    bulkJson: "/api/questions/bulk/json",
    bulkJsonFile: "/api/questions/bulk/json-file",
    checkAnswer: "/api/questions/check-answer",
  },
  bookmarks: {
    toggleQuestion: "/api/bookmarks/question/toggle",
    toggleExam: "/api/bookmarks/exam/toggle",
    savedQuestionIds: "/api/bookmarks/questions/ids",
    savedExamIds: "/api/bookmarks/exams/ids",
  },
  exams: {
    getAll: "/api/exams",
    paging: "/api/exams/paging",
    questions: "/api/exams",
    save: "/api/exams",
    delete: "/api/exams/delete",
    questionCounts: "/api/exams/question-counts"
  },
  quizzes: {
    getAll: "/api/quizzes",
    save: "/api/quizzes",
    delete: "/api/quizzes/delete"
  }
};
