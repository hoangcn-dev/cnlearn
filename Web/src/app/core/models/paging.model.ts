export interface PagingRequest {
    pageIndex: number;
    pageSize: number;
    orderBy: string|null;
    isAsc: boolean;
}

export interface Paginated<T> {
    totalPages: number;
    totalItems: number;
    pageIndex: number;
    pageSize: number;
    items: T[];
}