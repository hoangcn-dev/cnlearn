/**
 * Danh sách tập trung tất cả các API Endpoints của hệ thống HoangCN
 */
export const endpoints = {
  users: {
    // Các endpoint CRUD chuẩn kế thừa từ BaseController
    getAll: "/api/users",
    getById: (id: string) => `/api/users/${id}`,
    getPaging: "/api/users/paging",
    save: "/api/users",
    delete: "/api/users/delete",

    // Các endpoint Xác thực thực tế trong UsersController
    signup: "/api/users/signup",
    signin: "/api/users/signin",
    me: "/api/users/me",
    forgotPassword: "/api/users/forgot-password",
    changePassword: "/api/users/change-password",
  },
  emailTemplates: {
    // Các endpoint Quản lý Email Template
    getAll: "/api/emailtemplates",
    getByCode: (code: string) => `/api/emailtemplates/${code}`,
    save: "/api/emailtemplates",
  },
  roles: {
    // Các endpoint Quản lý Roles
    getAll: "/api/roles"
  }
};
