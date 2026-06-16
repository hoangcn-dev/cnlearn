import { get, post } from "./config/axios";
import { endpoints } from "./config/endpoint";

// Lấy danh sách đề thi (tất cả)
export const getAllExams = async () => {
  return await get(endpoints.exams.getAll);
};

// Lấy danh sách đề thi phân trang
export const getExamsPaging = async (request: any) => {
  return await post(endpoints.exams.paging, request);
};

// Lấy danh sách câu hỏi thuộc đề thi
export const getExamQuestions = async (examId: string) => {
  return await get(`${endpoints.exams.questions}/${examId}/questions`);
};

// Lưu đề thi và danh sách câu hỏi đi kèm
export const saveExamDetails = async (examData: any) => {
  return await post(endpoints.exams.saveDetails, examData);
};

// Xóa đề thi
export const deleteExams = async (ids: string[]) => {
  return await post(endpoints.exams.delete, { ids });
};

// Lấy số lượng câu hỏi của tất cả đề thi
export const getExamQuestionCounts = async () => {
  return await get(endpoints.exams.questionCounts);
};

