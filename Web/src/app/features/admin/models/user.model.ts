import { UUID } from "crypto";
import { PagingRequest } from "../../../core/models/paging.model";

export interface UserListItem {
    id: UUID;
    email: string;
    firstName: string;
    lastName: string;
    isOnline: boolean;
    phoneNumber: string|null;
    isActived: boolean;
    lastLogin: Date;
    role: {
        id: UUID;
        name: string;
    },
}

export interface UserDetail extends UserListItem {
    avatarUrl: string;
    note: string|null;
    createdAt: Date;
    updatedAt: Date;
}

export interface UserLogListItem {
    log: string;
    isSystemAction: boolean,
    timestamp: Date;
    userId: UUID
}

export interface UpdateUserRequest {
    isActived: boolean;
    note: string|null;
    lockReason: string|null;
}

export interface SearchUserRequest extends PagingRequest {
    email: string|null;
}

export interface SearchUserLogRequest extends PagingRequest {
    userId: UUID|null;
}