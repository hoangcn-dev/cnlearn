import { defineStore } from 'pinia';
import { ref } from 'vue';
import { message } from 'ant-design-vue';

export const useLoadingStore = defineStore('loading', () => {
  const activeRequestCount = ref(0);
  const loadingText = ref("Đang xử lý...");
  let hideLoadingToast: (() => void) | null = null;
  let closeTimeoutId: any = null;

  const showLoading = (text: string) => {
    // Nếu có một tiến trình chuẩn bị đóng toast trước đó, hủy bỏ nó ngay vì có request mới
    if (closeTimeoutId) {
      clearTimeout(closeTimeoutId);
      closeTimeoutId = null;
    }

    activeRequestCount.value++;
    
    // Nếu chưa có toast nào hiển thị, tạo mới toast
    if (!hideLoadingToast) {
      loadingText.value = text || "Đang xử lý...";
      hideLoadingToast = message.loading(loadingText.value, 0);
    }
  };

  const hideLoading = () => {
    activeRequestCount.value--;
    if (activeRequestCount.value <= 0) {
      activeRequestCount.value = 0;

      // Trì hoãn việc đóng toast (debounce) 300ms để xem có request nào gọi liên tiếp ngay sau đó không
      if (!closeTimeoutId) {
        closeTimeoutId = setTimeout(() => {
          if (hideLoadingToast) {
            hideLoadingToast();
            hideLoadingToast = null;
          }
          closeTimeoutId = null;
        }, 300);
      }
    }
  };

  return {
    activeRequestCount,
    loadingText,
    showLoading,
    hideLoading
  };
});
