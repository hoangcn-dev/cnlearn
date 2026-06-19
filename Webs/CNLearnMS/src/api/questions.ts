import { get, post } from "./config/axios";
import { endpoints } from "./config/endpoint";
import axiosInstance from "./config/axios";

// Lấy danh sách câu hỏi phân trang chi tiết
export const getQuestionsPaging = async (request: any, isMine: boolean = false) => {
  return await post(`${endpoints.questions.pagingDetails}?isMine=${isMine}`, request);
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
  return await get(`${endpoints.questions.details}/${id}`);
};

// Lưu danh sách câu hỏi chi tiết (Thêm mới/Cập nhật)
export const saveQuestions = async (questions: any[]) => {
  return await post(endpoints.questions.saveDetails, questions);
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
export const toggleSaveQuestion = async (id: string) => {
  return await post(`${endpoints.bookmarks.toggleQuestion}/${id}`);
};

// Lấy danh sách ID câu hỏi đã lưu
export const getSavedQuestionIds = async () => {
  return await get(endpoints.bookmarks.savedQuestionIds);
};

// Toggle trạng thái lưu đề thi
export const toggleSaveExam = async (id: string) => {
  return await post(`${endpoints.bookmarks.toggleExam}/${id}`);
};

// Lấy danh sách ID đề thi đã lưu
export const getSavedExamIds = async () => {
  return await get(endpoints.bookmarks.savedExamIds);
};
