import { Injectable } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingMessageId: string | null = null;

  constructor(private message: NzMessageService) {}

  show(content: string = 'Đang xử lý...'): void {
    if (this.loadingMessageId) {
      this.hide();
    }
    this.loadingMessageId = this.message.loading(content, { nzDuration: 0 }).messageId;
  }

  hide(): void {
    if (this.loadingMessageId) {
      this.message.remove(this.loadingMessageId);
      this.loadingMessageId = null;
    }
  }

  update(content: string): void {
    if (this.loadingMessageId) {
      this.hide();
      this.show(content);
    }
  }
}
