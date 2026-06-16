/**
 * DTO dành cho Đăng ký tài khoản (Tương ứng SignUpRequest.cs)
 */
export interface RegisterDTO {
  email: string;
  password: string;
  displayName: string;
  userName: string;
}

/**
 * DTO dành cho Đăng nhập (Tương ứng SignInRequest.cs)
 */
export interface LoginDTO {
  emailOrUserName: string;
  password: string;
}

/**
 * DTO chứa thông tin phiên đăng nhập người dùng (Tương ứng LoginSessionInfoDto.cs)
 */
export interface LoginSessionInfoDto {
  userId: string;
  userName: string;
  displayName?: string;
  email: string;
  roleName: string;
}
