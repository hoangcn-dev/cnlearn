import axios, { type AxiosResponse } from "axios";
import { message } from "ant-design-vue";
import { useLoadingStore } from "@/stores/loading";

// Định nghĩa kiểu dữ liệu mở rộng cho Axios config
declare module "axios" {
  export interface AxiosRequestConfig {
    showLoading?: boolean;
    loadingText?: string;
    _hasLoading?: boolean;
  }
}

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

// Request interceptor để quản lý loading
axiosInstance.interceptors.request.use(
  (config) => {
    const isBackgroundApi = 
      config.url?.includes('/users/profile') || 
      config.url?.includes('/users/me') ||
      config.url?.includes('/heartbeat') ||
      config.url?.includes('/cheat-log');
    const showLoading = !isBackgroundApi && (config.showLoading ?? true);

    if (showLoading) {
      const loadingStore = useLoadingStore();
      loadingStore.showLoading(config.loadingText || "Đang xử lý...");
      config._hasLoading = true;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor để xử lý tập trung phản hồi và lỗi từ API
axiosInstance.interceptors.response.use(
  (response: AxiosResponse) => {
    const config = response.config;
    if (config?._hasLoading) {
      const loadingStore = useLoadingStore();
      loadingStore.hideLoading();
    }
    return response;
  },
  (error: any) => {
    const config = error.config;
    if (config?._hasLoading) {
      const loadingStore = useLoadingStore();
      loadingStore.hideLoading();
    }

    const msg = getErrorMessage(error);
    
    // Kiểm tra lỗi 401 Unauthorized
    if (error.response && error.response.status === 401) {
      // Luôn phát event auth-required để xử lý đăng nhập tại tab mới, bảo vệ dữ liệu đang thao tác
      window.dispatchEvent(new CustomEvent('auth-required'));
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

export interface ApiOptions {
  showLoading?: boolean;   // Bật/tắt hiển thị loading toast (mặc định: true)
  loadingText?: string;    // Nội dung chữ hiển thị (mặc định tương ứng với từng method)
}

export const get = async <T = any>(url: string, params?: any, options?: ApiOptions): Promise<ApiResponseDto<T>> => {
  try {
    const response = await axiosInstance.get<ApiResponseDto<T>>(url, { 
      params,
      ...options
    });
    return response.data;
  } catch (error: any) {
    return {
      isSuccess: false,
      errorMessage: getErrorMessage(error)
    };
  }
};

export const post = async <T = any>(url: string, data?: any, options?: ApiOptions): Promise<ApiResponseDto<T>> => {
  try {
    const response = await axiosInstance.post<ApiResponseDto<T>>(url, data, options);
    return response.data;
  } catch (error: any) {
    return {
      isSuccess: false,
      errorMessage: getErrorMessage(error)
    };
  }
};

export const put = async <T = any>(url: string, data?: any, options?: ApiOptions): Promise<ApiResponseDto<T>> => {
  try {
    const response = await axiosInstance.put<ApiResponseDto<T>>(url, data, options);
    return response.data;
  } catch (error: any) {
    return {
      isSuccess: false,
      errorMessage: getErrorMessage(error)
    };
  }
};

export const del = async <T = any>(url: string, options?: ApiOptions): Promise<ApiResponseDto<T>> => {
  try {
    const response = await axiosInstance.delete<ApiResponseDto<T>>(url, options);
    return response.data;
  } catch (error: any) {
    return {
      isSuccess: false,
      errorMessage: getErrorMessage(error)
    };
  }
};

export default axiosInstance;