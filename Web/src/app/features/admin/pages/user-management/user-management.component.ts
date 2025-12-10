import { Component, inject } from '@angular/core';
import { NzButtonComponent } from "ng-zorro-antd/button";
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzIconModule } from "ng-zorro-antd/icon";
import { FormsModule } from '@angular/forms';
import { NzInputModule } from 'ng-zorro-antd/input';
import { SearchUserRequest, UserListItem } from '../../models/user.model';
import { Paginated } from '../../../../core/models/paging.model';
import { UUID } from 'crypto';
import { UserService } from '../../services/user.service';
import { map, tap } from 'rxjs';
import { ToastService } from '../../../../core/services/toast.service';
import { error } from 'console';
import { LoadingService } from '../../../../core/services/loading.service';


@Component({
	selector: 'app-user-management',
	standalone: true,
	imports: [NzButtonComponent, NzTableModule, NzIconModule, FormsModule, NzInputModule],
	templateUrl: './user-management.component.html',
	styleUrl: './user-management.component.css',
})
export class UserManagementComponent {
	api = inject(UserService);
	loading = inject(LoadingService);
	toast = inject(ToastService);

	searchData: SearchUserRequest = {
		email: null,
		isAsc: true,
		orderBy: null,
		pageIndex: 1,
		pageSize: 20
	};
	defaultData : Paginated<UserListItem> = {
		items: [],
		pageIndex: 1,
		pageSize: 20,
		totalItems: 0,
		totalPages: 1
	};
	data: Paginated<UserListItem> = this.defaultData;
	checked = false;
	indeterminate = false;
	setOfCheckedId = new Set<UUID>();

	ngOnInit(): void {
		this.api.getAllUsers(this.searchData).subscribe({
			next: response => {
				this.data = response.data!;
			}
		});
	}

	updateCheckedSet(id: UUID, checked: boolean): void {
		if (checked) {
			this.setOfCheckedId.add(id);
		} else {
			this.setOfCheckedId.delete(id);
		}
	}

	onItemChecked(id: UUID, checked: boolean): void {
		this.updateCheckedSet(id, checked);
		this.refreshCheckedStatus();
	}

	onAllChecked(value: boolean): void {
		this.data.items.forEach(item => this.updateCheckedSet(item.id, value));
		this.refreshCheckedStatus();
	}

	onCurrentPageDataChange($event: readonly UserListItem[]): void {
		// this.listOfCurrentPageData = $event;
		// this.refreshCheckedStatus();
	}

	refreshCheckedStatus(): void {
		this.checked = this.data.items.every(item => this.setOfCheckedId.has(item.id));
		this.indeterminate = this.data.items.some(item => this.setOfCheckedId.has(item.id)) && !this.checked;
	}
}
