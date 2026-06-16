import { get, post } from '@/api/config/axios';
import { endpoints } from '@/api/config/endpoint';

/**
 * Giao diện đối tượng Mẫu Email (Email Template) lưu trữ trong Database
 */
export interface EmailTemplate {
  fileResourceId: string;
  templateCode: string;
  subject: string;
  content: string;
  createdBy?: string;
  createdDate?: string;
  modifiedBy?: string;
  modifiedDate?: string;
  isDeleted?: boolean;
}

/**
 * Lấy toàn bộ danh sách mẫu email
 */
export const getAllTemplates = async (): Promise<any> => {
  return await get(endpoints.emailTemplates.getAll);
};

/**
 * Lấy chi tiết mẫu email theo mã code
 */
export const getTemplateByCode = async (code: string): Promise<any> => {
  if (!code) {
    throw new Error('Mã template không được phép để trống.');
  }
  return await get(endpoints.emailTemplates.getByCode(code));
};

/**
 * Lưu mẫu email (Tự động thêm mới hoặc cập nhật)
 */
export const saveTemplate = async (template: EmailTemplate): Promise<any> => {
  if (!template) {
    throw new Error('Dữ liệu template không được phép để trống.');
  }
  return await post(endpoints.emailTemplates.save, template);
};
