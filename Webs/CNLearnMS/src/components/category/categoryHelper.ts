export interface QuestionCategory {
  questionCategoryId: string
  parentId?: string | null
  questionCategoryName: string
  questionCategorySlug?: string
}

export interface CategoryNode {
  key: string
  title: string
  slug?: string
  children?: CategoryNode[]
  originalItem?: QuestionCategory
}

/**
 * Phân tích danh sách phẳng thành cấu trúc cây dựa trên parentId
 */
export function buildCategoryTree(flatList: QuestionCategory[]): CategoryNode[] {
  const build = (parentId: string | null): CategoryNode[] => {
    return flatList
      .filter(x => (x.parentId || null) === parentId)
      .map(x => {
        const children = build(x.questionCategoryId)
        const node: CategoryNode = {
          key: x.questionCategoryId,
          title: x.questionCategoryName.split(' - ').pop() || x.questionCategoryName, // Hiển thị nhãn cục bộ
          slug: x.questionCategorySlug,
          originalItem: x
        }
        if (children.length > 0) {
          node.children = children
        }
        return node
      })
  }
  return build(null)
}

/**
 * Tìm tất cả danh mục con đệ quy (Recursive Children - bao gồm cả con, cháu, chắt)
 */
export function getRecursiveChildIds(
  parentCategory: QuestionCategory,
  flatList: QuestionCategory[]
): string[] {
  const ids: string[] = []
  const traverse = (parentId: string) => {
    const children = flatList.filter(c => c.parentId === parentId)
    children.forEach(c => {
      ids.push(c.questionCategoryId)
      traverse(c.questionCategoryId)
    })
  }
  traverse(parentCategory.questionCategoryId)
  return ids
}

/**
 * Khởi tạo danh sách dropdown thụt đầu dòng (Indented List) để render vào các select thông thường
 */
export interface IndentedOption {
  value: string
  label: string
  rawName: string
  level: number
  hasChildren: boolean
}

export function buildIndentedOptions(flatList: QuestionCategory[]): IndentedOption[] {
  const options: IndentedOption[] = []
  
  const traverse = (parentId: string | null, level: number) => {
    const children = flatList
      .filter(x => (x.parentId || null) === parentId)
      .sort((a, b) => a.questionCategoryName.localeCompare(b.questionCategoryName))
      
    children.forEach(cat => {
      const currentPath = cat.questionCategoryName
      const localName = cat.questionCategoryName.split(' - ').pop() || cat.questionCategoryName
      const indent = '\u00A0\u00A0'.repeat(level)
      const prefix = level > 0 ? '└─ ' : ''
      const hasChildren = flatList.some(child => (child.parentId || null) === cat.questionCategoryId)
      
      options.push({
        value: cat.questionCategoryId,
        label: `${indent}${prefix}${localName}`,
        rawName: currentPath,
        level,
        hasChildren
      })
      
      traverse(cat.questionCategoryId, level + 1)
    })
  }
  
  traverse(null, 0)
  return options
}

/**
 * Tự động chuyển đổi tiếng Việt có dấu sang slug URL
 */
export function generateSlug(name: string): string {
  if (!name) return ''
  let str = name.toLowerCase()
  str = str.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
  str = str.replace(/[đĐ]/g, 'd')
  str = str.replace(/[^a-z0-9\s-]/g, '')
  return str.trim().replace(/\s+/g, '-')
}

/**
 * Lấy các danh mục con trực tiếp
 */
export function getDirectChildren(
  parentCategory: QuestionCategory,
  flatList: QuestionCategory[]
): QuestionCategory[] {
  return flatList.filter(c => c.parentId === parentCategory.questionCategoryId)
}

