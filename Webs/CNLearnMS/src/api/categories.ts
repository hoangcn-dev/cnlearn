import { get, post, put } from "./config/axios"
import { endpoints } from "./config/endpoint"

// Lấy danh sách danh mục
export const getAllCate = async () => {
    return await get(endpoints.categories.getAll);
}

// Lấy danh mục phân trang
export const getCategoriesPaging = async (request: any) => {
    return await post(endpoints.categories.paging, request);
}

// Thêm mới danh mục
export const addCategories = async (categories: any[]) => {
    return await post(endpoints.categories.save, categories);
}

// Cập nhật danh mục
export const updateCategories = async (categories: any[]) => {
    return await put(endpoints.categories.save, categories);
}

// Xóa danh mục
export const deleteCategories = async (ids: string[]) => {
    return await post(endpoints.categories.delete, { ids });
}