import { get, post } from "./config/axios"
import { endpoints } from "./config/endpoint"

// Lấy danh sách danh mục
export const getAllCate = async () => {
    return await get(endpoints.categories.getAll);
}

// Lưu danh sách danh mục (Thêm mới/Cập nhật)
export const saveCategories = async (categories: any[]) => {
    return await post(endpoints.categories.save, categories);
}

// Xóa danh mục
export const deleteCategories = async (ids: string[]) => {
    return await post(endpoints.categories.delete, { ids });
}