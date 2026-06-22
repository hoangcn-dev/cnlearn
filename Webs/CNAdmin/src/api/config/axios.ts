import axios, { type AxiosResponse } from "axios";
import { message } from "ant-design-vue";

// URL của Backend API HoangCN
const baseURL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5038";

const axiosInstance = axios.create({
  baseURL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  }
});

// Cấu hình để chỉ hiện tối đa 1 thông báo toast cùng lúc
message.config({
  maxCount: 1,
});

let lastErrorMessage = "";
let lastErrorTime = 0;
let lastTabOpenTime = 0;

/**
 * Trích xuất thông tin thông báo lỗi chuẩn hóa từ API response hoặc Axios error
 */
export const getErrorMessage = (error: any, defaultMsg: string = "Đã xảy ra lỗi, vui lòng thử lại!"): string => {
  if (!error) return defaultMsg;

  // 1. Lỗi từ Axios (catch block)
  const apiError = error.response?.data;
  if (apiError?.Data) {
    return apiError.Data.UserMsg || defaultMsg;
  }

  // 2. Response trả về trực tiếp (res)
  if (error?.Data) {
    return error.Data.UserMsg || defaultMsg;
  }

  // 3. Fallback cuối cùng
  return error.message || defaultMsg;
};

// Response interceptor để xử lý tập trung phản hồi và lỗi từ API
axiosInstance.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  (error: any) => {
    // Xử lý tự động mở tab login khi gặp lỗi 401
    if (error.response?.status === 401) {
      const currentPath = window.location.pathname;
      if (!currentPath.includes('/auth') && !currentPath.includes('/login-success')) {
        const now = Date.now();
        if (now - lastTabOpenTime > 5000) {
          lastTabOpenTime = now;
          const loginUrl = `${window.location.origin}/auth?return_url=${encodeURIComponent('/login-success')}`;
          window.open(loginUrl, '_blank');
        }
      }
    }

    const msg = getErrorMessage(error);
    
    // Chỉ hiển thị toast lỗi nếu không phải là endpoint kiểm tra auth
    if (error.config && !error.config.url.endsWith('/users/me')) {
      const now = Date.now();
      // Không hiển thị trùng lặp cùng một thông báo lỗi trong vòng 1.5 giây
      if (msg !== lastErrorMessage || now - lastErrorTime > 1500) {
        lastErrorMessage = msg;
        lastErrorTime = now;
        message.error(msg);
      }
    }
    
    return Promise.reject(error);
  }
);

export const get = async <T = any>(url: string, params?: any): Promise<T> => {
  const response = await axiosInstance.get(url, { params });
  return response.data;
};

export const post = async <T = any>(url: string, data?: any): Promise<T> => {
  const response = await axiosInstance.post(url, data);
  return response.data;
};

export const put = async <T = any>(url: string, data?: any): Promise<T> => {
  const response = await axiosInstance.put(url, data);
  return response.data;
};

export const del = async <T = any>(url: string): Promise<T> => {
  const response = await axiosInstance.delete(url);
  return response.data;
};

export default axiosInstance;