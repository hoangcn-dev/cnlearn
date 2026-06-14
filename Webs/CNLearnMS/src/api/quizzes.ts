import { get, post } from "./config/axios";
import { endpoints } from "./config/endpoint";

// Lấy toàn bộ danh sách kỳ thi/bài kiểm tra
export const getAllQuizzes = async () => {
  return await get(endpoints.quizzes.getAll);
};

// Lưu danh sách kỳ thi/bài kiểm tra (thêm hoặc sửa)
export const saveQuizDetails = async (quizData: any) => {
  // BaseController.Save nhận vào danh sách thực thể List<TEntity>
  return await post(endpoints.quizzes.save, Array.isArray(quizData) ? quizData : [quizData]);
};

// Xóa kỳ thi/bài kiểm tra
export const deleteQuiz = async (id: string) => {
  return await post(endpoints.quizzes.delete, { ids: [id] });
};
