import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { LoginSessionInfoDto } from '@/model/auth';
import { getLoginInfo } from '@/api/user';

export const useAuthStore = defineStore('auth', () => {
  // Trạng thái lưu trữ thuần bộ nhớ (In-Memory), không dựa vào localStorage/Storage
  const isLoggedIn = ref<boolean>(false);
  const user = ref<LoginSessionInfoDto | null>(null);
  const isInitialized = ref<boolean>(false);

  /**
   * Thiết lập phiên đăng nhập mới (Lưu reactive state trong Pinia)
   */
  const setLoginSession = (info: LoginSessionInfoDto) => {
    if (!info) {
      throw new Error('Thông tin đăng nhập không được để trống.');
    }
    if (!info.userName) {
      throw new Error('Tên tài khoản không được để trống.');
    }
    if (!info.email) {
      throw new Error('Email không được để trống.');
    }
    if (!info.roleName) {
      throw new Error('Vai trò người dùng không được để trống.');
    }

    isLoggedIn.value = true;
    user.value = info;
    isInitialized.value = true;
  };

  /**
   * Xóa bỏ phiên đăng nhập hiện tại
   */
  const clearLoginSession = () => {
    isLoggedIn.value = false;
    user.value = null;
    isInitialized.value = true;
  };

  /**
   * Gọi API /me để lấy thông tin phiên đăng nhập thực tế từ Cookie
   * Được gọi tự động khi lần đầu tiên người dùng vào ứng dụng
   */
  const checkAuthSession = async (): Promise<boolean> => {
    try {
      const res = await getLoginInfo();
      if (res && res.isSuccess && res.data) {
        setLoginSession(res.data);
        return true;
      }
    } catch (error) {
      console.warn('Phiên đăng nhập chưa được khởi tạo hoặc đã hết hạn.');
      clearLoginSession();
    } finally {
      isInitialized.value = true;
    }
    return false;
  };

  return {
    isLoggedIn,
    user,
    isInitialized,
    setLoginSession,
    clearLoginSession,
    checkAuthSession
  };
});
