import { Injectable } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzNotificationService } from 'ng-zorro-antd/notification';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

/**
 * Toast Service - Reusable notification service
 * Sử dụng NG-ZORRO message và notification components
 */
@Injectable({
  providedIn: 'root'
})
export class ToastService {
  constructor(
    private message: NzMessageService,
    private notification: NzNotificationService
  ) {}

  /**
   * Hiển thị message toast ngắn (tự động đóng sau 3s)
   */
  showMessage(type: ToastType, content: string, duration: number = 3000): void {
    this.message.create(type, content, { nzDuration: duration });
  }

  /**
   * Hiển thị success message
   */
  success(content: string, duration?: number): void {
    this.showMessage('success', content, duration);
  }

  /**
   * Hiển thị error message
   */
  error(content: string, duration?: number): void {
    this.showMessage('error', content, duration);
  }

  /**
   * Hiển thị warning message
   */
  warning(content: string, duration?: number): void {
    this.showMessage('warning', content, duration);
  }

  /**
   * Hiển thị info message
   */
  info(content: string, duration?: number): void {
    this.showMessage('info', content, duration);
  }

  /**
   * Hiển thị notification với title và content (cho thông báo chi tiết hơn)
   */
  showNotification(
    type: ToastType,
    title: string,
    content: string,
    duration: number = 4500
  ): void {
    this.notification.create(type, title, content, {
      nzDuration: duration,
      nzPlacement: 'topRight'
    });
  }

  /**
   * Hiển thị success notification
   */
  successNotification(title: string, content: string, duration?: number): void {
    this.showNotification('success', title, content, duration);
  }

  /**
   * Hiển thị error notification
   */
  errorNotification(title: string, content: string, duration?: number): void {
    this.showNotification('error', title, content, duration);
  }

  /**
   * Hiển thị warning notification
   */
  warningNotification(title: string, content: string, duration?: number): void {
    this.showNotification('warning', title, content, duration);
  }

  /**
   * Hiển thị info notification
   */
  infoNotification(title: string, content: string, duration?: number): void {
    this.showNotification('info', title, content, duration);
  }

  /**
   * Đóng tất cả messages
   */
  removeAllMessages(): void {
    this.message.remove();
  }

  /**
   * Đóng tất cả notifications
   */
  removeAllNotifications(): void {
    this.notification.remove();
  }
}
