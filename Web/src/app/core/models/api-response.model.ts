export interface ApiResponse<T=void> {
  success: boolean;
  data?: T;
  message?: string;
  errorMessage?: string;
}

export interface UpdatedResponse {
  updatedId: any
}

export interface DeletedResponse {
  deletedId: any
}
