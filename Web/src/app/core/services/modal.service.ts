import { inject, Injectable } from '@angular/core';
import { NzModalService } from 'ng-zorro-antd/modal';

@Injectable({
	providedIn: 'root',
})
export class ModalService {
	modal = inject(NzModalService);

	showConfirm(
		message: string, 
		onConfirm: () => void,
		title: string = "Xác nhận hành động?", 
		onCancel: () => void = () => {},
	): void {
		this.modal.confirm({
			nzTitle: `<i>${title}</i>`,
			nzContent: `<b>${message}</b>`,
			nzOnOk: onConfirm,
			nzOnCancel: onCancel,
			nzOkText: "Đồng ý",
			nzCancelText: "Hủy"
		});
	}
}
