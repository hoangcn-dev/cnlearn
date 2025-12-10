import { inject, Injectable } from '@angular/core';
import { ApiService, endpoints } from '../../../core/services/api.service';
import { SearchUserLogRequest, SearchUserRequest, UpdateUserRequest, UserDetail, UserListItem, UserLogListItem } from '../models/user.model';
import { Paginated } from '../../../core/models/paging.model';
import { UtilService } from '../../../core/services/util.service';
import { UUID } from 'crypto';
import { DeletedResponse, UpdatedResponse } from '../../../core/models/api-response.model';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    private api = inject(ApiService);
    private util = inject(UtilService);

    getAllUsers(request: SearchUserRequest) {
        const queryParams = this.util.toQueryString(request);
        return this.api.get<Paginated<UserListItem>>(endpoints.manageUser.getAll(queryParams));
    }

    getLogs(request: SearchUserLogRequest) {
        const queryParams = this.util.toQueryString(request);
        return this.api.get<Paginated<UserLogListItem>>(endpoints.manageUser.logs(queryParams));
    }

    getDetail(userId: UUID) {
        return this.api.get<UserDetail>(endpoints.manageUser.getDetail(userId));
    }

    update(userId: UUID, request: UpdateUserRequest) {
        return this.api.put<UpdatedResponse>(endpoints.manageUser.update(userId), request);
    }

    delete(userId: UUID) {
        return this.api.delete<DeletedResponse>(endpoints.manageUser.delete(userId));
    }
}
