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

// Response interceptor để xử lý tập trung phản hồi và lỗi từ API
axiosInstance.interceptors.response.use(
  (response: AxiosResponse) => {
    return response;
  },
  (error: any) => {
    // Trích xuất thông tin lỗi chuẩn hóa từ CatchExceptionMiddleware
    const apiError = error.response?.data;
    const msg = apiError?.Data?.UserMsg || apiError?.Data?.DevMsg || "Đã xảy ra lỗi, vui lòng thử lại!";
    
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