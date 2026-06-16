import { get, post, put } from "./config/axios";
import { endpoints } from "./config/endpoint";

// Lấy toàn bộ danh sách kỳ thi/bài kiểm tra
export const getAllQuizzes = async () => {
  return await get(endpoints.quizzes.getAll);
};

// Lưu danh sách kỳ thi/bài kiểm tra (thêm hoặc sửa)
export const saveQuizDetails = async (quizData: any) => {
  const isArray = Array.isArray(quizData);
  const dataList = isArray ? quizData : [quizData];
  
  // Xác định xem đây là tạo mới (Insert) hay cập nhật (Update) dựa trên quizId
  const isUpdate = dataList.some(item => {
    const id = item.quizId || item.id;
    return id && id !== '00000000-0000-0000-0000-000000000000';
  });

  if (isUpdate) {
    return await put(endpoints.quizzes.save, dataList);
  } else {
    return await post(endpoints.quizzes.save, dataList);
  }
};

// Xóa kỳ thi/bài kiểm tra
export const deleteQuiz = async (id: string) => {
  return await post(endpoints.quizzes.delete, { ids: [id] });
};
