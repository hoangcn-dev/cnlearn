import { get, post, put } from "./config/axios";
import { endpoints } from "./config/endpoint";
import type { Exam } from "@/models/exams";

// Lấy danh sách đề thi (tất cả)
export const getAllExams = async () => {
  return await get(endpoints.exams.getAll);
};

// Lấy danh sách đề thi phân trang
export const getExamsPaging = async (request: any) => {
  return await post(endpoints.exams.paging, request);
};

// Lấy chi tiết đề thi (DTO)
export const getExamDetails = async (examId: string) => {
  return await get(`${endpoints.exams.getAll}/${examId}`);
};

// Lấy danh sách câu hỏi thuộc đề thi
export const getExamQuestions = async (examId: string) => {
  return await get(`${endpoints.exams.questions}/${examId}/questions`);
};

// Lấy danh sách đáp án đúng của đề thi
export const getExamKeys = async (examId: string) => {
  return await get(`${endpoints.exams.questions}/${examId}/keys`);
};

export const saveExam = async (exam: Exam, isEdit: boolean = false) => {
// Lưu đề thi sử dụng CRUD API
  if (isEdit) {
    return await put(endpoints.exams.save, [exam]);
  }
  return await post(endpoints.exams.save, [exam]);
};

// Xóa đề thi
export const deleteExams = async (ids: string[]) => {
  return await post(endpoints.exams.delete, { ids });
};

// Lấy số lượng câu hỏi của tất cả đề thi
export const getExamQuestionCounts = async () => {
  return await get(endpoints.exams.questionCounts);
};

