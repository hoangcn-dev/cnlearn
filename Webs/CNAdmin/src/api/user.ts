import { post, get } from '@/api/config/axios';
import { endpoints } from '@/api/config/endpoint';
import type { User } from '@/model/user/User';
import type { RegisterDTO, LoginDTO } from '@/model/auth';

/**
 * =========================================================================
 * 1. CÁC API XÁC THỰC THỰC TẾ (UsersController)
 * =========================================================================
 */

/**
 * Đăng ký tài khoản người dùng mới (Signup)
 */
export const register = async (data: RegisterDTO) => {
  if (!data) {
    throw new Error("Dữ liệu đăng ký không được phép để trống.");
  }
  return await post(endpoints.users.signup, data);
};

/**
 * Đăng nhập vào hệ thống (Signin)
 */
export const login = async (data: LoginDTO) => {
  if (!data) {
    throw new Error("Dữ liệu đăng nhập không được phép để trống.");
  }
  return await post(endpoints.users.signin, data);
};

/**
 * Yêu cầu cấp lại mật khẩu tạm thời qua Email
 */
export const forgotPassword = async (email: string) => {
  if (!email) {
    throw new Error("Email không được phép để trống.");
  }
  return await post(endpoints.users.forgotPassword, { email });
};

/**
 * Lấy thông tin phiên đăng nhập của người dùng hiện tại (Me)
 */
export const getLoginInfo = async (): Promise<any> => {
  const res = await get(endpoints.users.me);
  if (!res) {
    throw new Error("Không thể lấy thông tin đăng nhập từ hệ thống.");
  }
  return res;
};

/**
 * =========================================================================
 * 2. CÁC API CRUD QUẢN TRỊ USER (Kế thừa từ BaseController)
 * =========================================================================
 */

/**
 * Lấy toàn bộ danh sách người dùng
 */
export const getAllUsers = async () => {
  return await get(endpoints.users.getAll);
};

/**
 * Lấy thông tin chi tiết của người dùng qua ID
 */
export const getUserById = async (id: string) => {
  if (!id) {
    throw new Error("ID người dùng không được phép để trống.");
  }
  return await get(endpoints.users.getById(id));
};

/**
 * Lấy danh sách người dùng phân trang và tìm kiếm
 */
export const getUserPaging = async (requestData: any) => {
  if (!requestData) {
    throw new Error("Tham số yêu cầu phân trang không được phép để trống.");
  }
  return await post(endpoints.users.getPaging, requestData);
};

/**
 * Lưu thông tin người dùng (Thêm mới hoặc Cập nhật)
 */
export const saveUsers = async (users: User[]) => {
  if (!users || users.length === 0) {
    throw new Error("Danh sách người dùng cần lưu không được phép để trống.");
  }
  return await post(endpoints.users.save, users);
};

/**
 * Xóa danh sách người dùng theo IDs
 */
export const deleteUsers = async (ids: string[]) => {
  if (!ids || ids.length === 0) {
    throw new Error("Danh sách ID người dùng cần xóa không được phép trống.");
  }
  return await post(endpoints.users.delete, { ids });
};

/**
 * Thực hiện đổi mật khẩu tài khoản người dùng hiện tại
 */
export const changePassword = async (data: any) => {
  if (!data || !data.currentPassword || !data.newPassword) {
    throw new Error("Thông tin mật khẩu không được phép để trống.");
  }
  return await post(endpoints.users.changePassword, data);
};

/**
 * Lấy toàn bộ danh sách vai trò từ CSDL thực tế
 */
export const getAllRoles = async (): Promise<any> => {
  return await get(endpoints.roles.getAll);
};
