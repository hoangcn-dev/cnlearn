export interface ApiResponse<T> {
  data?: T;
  message?: string;
  errorMessage?: string;
}
