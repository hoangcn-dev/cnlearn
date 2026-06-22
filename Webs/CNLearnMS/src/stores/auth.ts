import { defineStore } from 'pinia';
import { ref } from 'vue';
import { getLoginInfo } from '@/api/user';

export const useAuthStore = defineStore('auth', () => {
  const isLoggedIn = ref<boolean>(false);
  const user = ref<any | null>(null);
  const isInitialized = ref<boolean>(false);

  const setLoginSession = (info: any) => {
    isLoggedIn.value = true;
    user.value = info;
    isInitialized.value = true;
  };

  const clearLoginSession = () => {
    isLoggedIn.value = false;
    user.value = null;
    isInitialized.value = true;
  };

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
