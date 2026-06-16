import { get, post } from "./config/axios";
import { endpoints } from "./config/endpoint";

export const toggleQuestionBookmark = async (id: string) => {
  return await post(`${endpoints.bookmarks.toggleQuestion}/${id}`, {});
};

export const toggleExamBookmark = async (id: string) => {
  return await post(`${endpoints.bookmarks.toggleExam}/${id}`, {});
};

export const getSavedQuestionIds = async () => {
  return await get(endpoints.bookmarks.savedQuestionIds);
};

export const getSavedExamIds = async () => {
  return await get(endpoints.bookmarks.savedExamIds);
};
