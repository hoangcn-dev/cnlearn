/**
 * Toast Service Usage Examples
 * 
 * File này chứa các ví dụ về cách sử dụng ToastService trong các component khác
 */

import { Component } from '@angular/core';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-example',
  template: ''
})
export class ExampleUsageComponent {
  
  constructor(private toast: ToastService) {}

  // ============================================
  // 1. BASIC MESSAGES (Tự động đóng sau 3s)
  // ============================================
  
  showSuccessMessage() {
    this.toast.success('Thao tác thành công!');
  }

  showErrorMessage() {
    this.toast.error('Có lỗi xảy ra!');
  }

  showWarningMessage() {
    this.toast.warning('Cảnh báo: Dữ liệu chưa được lưu!');
  }

  showInfoMessage() {
    this.toast.info('Thông tin: Hệ thống sẽ bảo trì vào 2h sáng');
  }

  // ============================================
  // 2. MESSAGES VỚI CUSTOM DURATION
  // ============================================
  
  showLongDurationMessage() {
    // Hiển thị trong 5 giây
    this.toast.success('Message này sẽ hiển thị lâu hơn', 5000);
  }

  showShortDurationMessage() {
    // Hiển thị trong 1 giây
    this.toast.error('Message này biến mất nhanh', 1000);
  }

  // ============================================
  // 3. NOTIFICATIONS (Chi tiết hơn, có title)
  // ============================================
  
  showSuccessNotification() {
    this.toast.successNotification(
      'Đăng nhập thành công',
      'Chào mừng bạn quay trở lại hệ thống!'
    );
  }

  showErrorNotification() {
    this.toast.errorNotification(
      'Xóa thất bại',
      'Không thể xóa người dùng này vì đang có dữ liệu liên quan'
    );
  }

  showWarningNotification() {
    this.toast.warningNotification(
      'Cảnh báo bảo mật',
      'Phiên đăng nhập của bạn sẽ hết hạn trong 5 phút'
    );
  }

  showInfoNotification() {
    this.toast.infoNotification(
      'Cập nhật mới',
      'Phiên bản 2.0 đã có sẵn. Click để cập nhật ngay!'
    );
  }

  // ============================================
  // 4. PRACTICAL EXAMPLES (Ví dụ thực tế)
  // ============================================
  
  // Ví dụ: Sau khi submit form
  submitForm() {
    // ... logic submit form
    
    if (true) { // success
      this.toast.success('Form đã được gửi thành công!');
    } else {
      this.toast.error('Không thể gửi form. Vui lòng thử lại!');
    }
  }

  // Ví dụ: Xóa item
  deleteItem() {
    // ... logic delete
    
    this.toast.successNotification(
      'Xóa thành công',
      'Item đã được xóa khỏi hệ thống'
    );
  }

  // Ví dụ: Upload file
  uploadFile() {
    this.toast.info('Đang upload file...');
    
    // ... logic upload
    
    setTimeout(() => {
      this.toast.success('Upload thành công!');
    }, 2000);
  }

  // Ví dụ: API error
  handleApiError(error: any) {
    if (error.status === 401) {
      this.toast.errorNotification(
        'Phiên đăng nhập hết hạn',
        'Vui lòng đăng nhập lại để tiếp tục'
      );
    } else if (error.status === 403) {
      this.toast.warningNotification(
        'Không có quyền truy cập',
        'Bạn không có quyền thực hiện thao tác này'
      );
    } else if (error.status === 500) {
      this.toast.error('Lỗi server. Vui lòng thử lại sau!');
    } else {
      this.toast.error('Có lỗi xảy ra. Vui lòng thử lại!');
    }
  }

  // Ví dụ: Validation errors
  showValidationErrors(errors: string[]) {
    if (errors.length === 1) {
      this.toast.warning(errors[0]);
    } else {
      this.toast.warningNotification(
        'Lỗi validation',
        errors.join('\n')
      );
    }
  }

  // Ví dụ: Copy to clipboard
  copyToClipboard(text: string) {
    navigator.clipboard.writeText(text);
    this.toast.success('Đã copy vào clipboard!');
  }

  // Ví dụ: Save changes
  saveChanges() {
    this.toast.info('Đang lưu thay đổi...');
    
    // ... API call
    
    setTimeout(() => {
      this.toast.successNotification(
        'Lưu thành công',
        'Thay đổi của bạn đã được cập nhật'
      );
    }, 1500);
  }

  // ============================================
  // 5. ADVANCED USAGE (Sử dụng nâng cao)
  // ============================================
  
  // Xóa tất cả messages hiện tại
  clearAllMessages() {
    this.toast.removeAllMessages();
  }

  // Xóa tất cả notifications hiện tại
  clearAllNotifications() {
    this.toast.removeAllNotifications();
  }

  // Chain nhiều messages
  showProgressMessages() {
    this.toast.info('Bước 1: Đang xử lý dữ liệu...');
    
    setTimeout(() => {
      this.toast.info('Bước 2: Đang validate...');
    }, 1000);
    
    setTimeout(() => {
      this.toast.info('Bước 3: Đang lưu vào database...');
    }, 2000);
    
    setTimeout(() => {
      this.toast.success('Hoàn tất!');
    }, 3000);
  }

  // Message với action callback
  showUndoMessage() {
    this.toast.successNotification(
      'Đã xóa',
      'Item đã được xóa. Click để hoàn tác.'
    );
    // Note: NG-ZORRO notification không hỗ trợ action buttons mặc định
    // Cần custom nếu muốn có nút action
  }
}

/**
 * BEST PRACTICES:
 * 
 * 1. Success: Dùng cho các thao tác thành công (save, delete, update)
 * 2. Error: Dùng cho lỗi nghiêm trọng, cần user biết ngay
 * 3. Warning: Dùng cho cảnh báo, user có thể bỏ qua nhưng nên chú ý
 * 4. Info: Dùng cho thông tin chung, không quan trọng lắm
 * 
 * 5. Message vs Notification:
 *    - Message: Ngắn gọn, một dòng, ở giữa màn hình phía trên
 *    - Notification: Chi tiết hơn, có title và content, ở góc phải màn hình
 * 
 * 6. Duration:
 *    - Success: 3s (mặc định)
 *    - Error: 4-5s (để user có thời gian đọc)
 *    - Warning: 4s
 *    - Info: 3s
 * 
 * 7. Tránh spam:
 *    - Không show quá nhiều toast cùng lúc
 *    - Clear old messages nếu cần show message mới quan trọng
 * 
 * 8. User Experience:
 *    - Success: Ngắn gọn, khích lệ
 *    - Error: Rõ ràng, hướng dẫn cách khắc phục
 *    - Warning: Lịch sự, không đe dọa
 *    - Info: Thân thiện, hữu ích
 */
