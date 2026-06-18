import { get, post } from '@/api/config/axios';
import { endpoints } from '@/api/config/endpoint';

/**
 * Giao diện đối tượng Mẫu Email (Email Template) lưu trữ trong Database
 */
export interface EmailTemplate {
  emailTemplateId: string;
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
 * Lấy chi tiết mẫu email theo ID
 */
export const getTemplateById = async (id: string): Promise<any> => {
  if (!id) {
    throw new Error('ID template không được phép để trống.');
  }
  return await get(endpoints.emailTemplates.getById(id));
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
