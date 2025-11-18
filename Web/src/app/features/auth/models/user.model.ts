export interface LoginInfo {
  fullName: string;
  email: string;
  avatarUrl: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

