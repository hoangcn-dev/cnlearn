import axios, { type AxiosResponse } from "axios";
import { message } from "ant-design-vue";

// URL của Backend API HoangCN
const baseURL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5006";

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
    const msg = getErrorMessage(error);
    
    // Kiểm tra lỗi 401 Unauthorized
    if (error.response && error.response.status === 401) {
      const pathname = window.location.pathname;
      if (pathname.includes('/practice') || pathname.includes('/room')) {
        const url = import.meta.env.VITE_MAIN_URL || 'http://localhost:5173';
        message.warning('Phiên đăng nhập hết hạn! Hệ thống đã mở Tab mới để đăng nhập lại. Hãy QUAY LẠI ĐÂY nộp bài sau khi xong.', 8);
        window.open(`${url}/auth/login`, '_blank');
      } else {
        window.dispatchEvent(new CustomEvent('auth-required'));
      }
    } else {
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
    }
    
    return Promise.reject(error);
  }
);

export interface ApiResponseDto<T> {
  isSuccess: boolean,
  data?: T,
  errorMessage?: string
}

export const get = async <T = any>(url: string, params?: any): Promise<ApiResponseDto<T>> => {
  try {
		const response = await axiosInstance.get<ApiResponseDto<T>>(url, { params });
    return response.data;
	} catch (error: any) {
		return {
			isSuccess: false,
      errorMessage: getErrorMessage(error)
		};
	}
};

export const post = async <T = any>(url: string, data?: any): Promise<ApiResponseDto<T>> => {
  try {
		const response = await axiosInstance.post<ApiResponseDto<T>>(url, data);
    return response.data;
	} catch (error: any) {
		return {
			isSuccess: false,
      errorMessage: getErrorMessage(error)
		};
	}
};

export const put = async <T = any>(url: string, data?: any): Promise<ApiResponseDto<T>> => {
  try {
		const response = await axiosInstance.put<ApiResponseDto<T>>(url, data);
    return response.data;
	} catch (error: any) {
		return {
			isSuccess: false,
      errorMessage: getErrorMessage(error)
		};
	}
};

export const del = async <T = any>(url: string): Promise<ApiResponseDto<T>> => {
  try {
		const response = await axiosInstance.delete<ApiResponseDto<T>>(url);
    return response.data;
	} catch (error: any) {
		return {
			isSuccess: false,
      errorMessage: getErrorMessage(error)
		};
	}
};

export default axiosInstance;