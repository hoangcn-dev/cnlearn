import { get, post, put } from "./config/axios";
import { endpoints } from "./config/endpoint";
import axiosInstance from "./config/axios";

// Lấy danh sách câu hỏi phân trang chi tiết
export const getQuestionsPaging = async (request: any, isMine: boolean = false) => {
  return await post(`${endpoints.questions.search}?isMine=${isMine}`, request);
};

// Lấy danh sách câu hỏi đã lưu phân trang
export const getSavedQuestionsPaging = async (request: any) => {
  return await post(endpoints.questions.savedPaging, request);
};

// Lấy danh sách câu hỏi đã làm phân trang
export const getDoneQuestionsPaging = async (request: any) => {
  return await post(endpoints.questions.donePaging, request);
};

// Lấy chi tiết câu hỏi theo ID
export const getQuestionDetails = async (id: string) => {
  return await get(`${endpoints.questions.detail}/${id}`);
};

// Lưu danh sách câu hỏi chi tiết (Thêm mới/Cập nhật)
export const saveQuestions = async (questions: any[], isEdit: boolean = false) => {
  if (isEdit) {
    return await put(endpoints.questions.save, questions);
  }
  return await post(endpoints.questions.save, questions);
};

// Xóa câu hỏi theo danh sách ID
export const deleteQuestions = async (ids: string[]) => {
  return await post(endpoints.questions.delete, { ids });
};

// Import hàng loạt từ JSON string/object
export const importBulkJson = async (jsonBody: any) => {
  return await post(endpoints.questions.bulkJson, jsonBody);
};

// Chấm điểm đáp án
export const checkAnswer = async (payload: { questionId: string, selectedAnswerIds: string[] }) => {
  return await post(endpoints.questions.checkAnswer, payload);
};

// Import hàng loạt từ JSON file
export const importBulkJsonFile = async (formData: FormData) => {
  try {
    const response = await axiosInstance.post(endpoints.questions.bulkJsonFile, formData, {
      headers: {
        "Content-Type": "multipart/form-data"
      }
    });
    return response.data;
  } catch (error: any) {
    return {
      isSuccess: false,
      errorMessage: error.message || "Tải lên tệp JSON thất bại."
    };
  }
};

// Toggle trạng thái lưu câu hỏi
export const toggleSaveQuestion = async (payload: { TargetId: string, IsSaved: boolean }) => {
  return await post(endpoints.questions.saved, payload);
};

// Lấy danh sách câu hỏi đã lưu
export const getSavedQuestions = async () => {
  return await get(endpoints.questions.saved);
};

// Lấy danh sách ID câu hỏi đã lưu
export const getSavedQuestionIds = async () => {
  const res = await get(endpoints.questions.saved);
  if (res && res.isSuccess && res.data) {
    return {
      isSuccess: true,
      data: (res.data as any[]).map(x => x.targetId)
    };
  }
  return {
    isSuccess: false,
    errorMessage: res?.errorMessage || "Không thể lấy danh sách ID đã lưu"
  };
};

// Toggle trạng thái lưu đề thi
export const toggleSaveExam = async (id: string) => {
  return await post(`${endpoints.bookmarks.toggleExam}/${id}`);
};

// Lấy danh sách ID đề thi đã lưu
export const getSavedExamIds = async () => {
  return await get(endpoints.bookmarks.savedExamIds);
};

// Lấy danh sách câu trả lời của danh sách câu hỏi
export const getQuestionAnswers = async (questionIds: string[]) => {
  return await post(endpoints.questions.answers, questionIds);
};

// Lấy mapping key (đáp án đúng) cho danh sách câu hỏi
export const getQuestionKeys = async (questionIds: string[]) => {
  return await post(endpoints.questions.key, questionIds);
};
