import { get, post, put } from "./config/axios";
import { endpoints } from "./config/endpoint";

// Lấy toàn bộ danh sách kỳ thi/bài kiểm tra
export const getAllQuizzes = async () => {
  return await get(endpoints.quizzes.getAll);
};

// Thêm mới kỳ thi/bài kiểm tra
export const addQuizDetails = async (quizData: any) => {
  const isArray = Array.isArray(quizData);
  const dataList = isArray ? quizData : [quizData];
  return await post(endpoints.quizzes.save, dataList);
};

// Cập nhật kỳ thi/bài kiểm tra
export const updateQuizDetails = async (quizData: any) => {
  const isArray = Array.isArray(quizData);
  const dataList = isArray ? quizData : [quizData];
  return await put(endpoints.quizzes.save, dataList);
};

// Xóa kỳ thi/bài kiểm tra
export const deleteQuiz = async (id: string) => {
  return await post(endpoints.quizzes.delete, { ids: [id] });
};
