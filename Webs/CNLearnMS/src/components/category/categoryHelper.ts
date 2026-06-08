export interface QuestionCategory {
  questionCategoryId: string
  name: string
  slug: string
}

export interface CategoryNode {
  key: string
  title: string
  slug?: string
  children?: CategoryNode[]
  originalItem?: QuestionCategory
}

/**
 * Phân tích danh sách phẳng thành cấu trúc cây dựa trên ký tự phân tách " - "
 */
export function buildCategoryTree(flatList: QuestionCategory[]): CategoryNode[] {
  const rootNodes: CategoryNode[] = []

  flatList.forEach(item => {
    const parts = item.name.split(' - ').map(p => p.trim())
    let currentLevel = rootNodes

    parts.forEach((part, index) => {
      let node = currentLevel.find(n => n.title === part)

      if (!node) {
        node = {
          key: index === parts.length - 1 ? item.questionCategoryId : `virtual-${part}-${index}`,
          title: part,
          slug: index === parts.length - 1 ? item.slug : undefined,
          children: [],
          originalItem: index === parts.length - 1 ? item : undefined
        }
        currentLevel.push(node)
      } else if (index === parts.length - 1) {
        node.key = item.questionCategoryId
        node.slug = item.slug
        node.originalItem = item
      }
      currentLevel = node.children!
    })
  })

  // Đệ quy dọn dẹp mảng children trống
  const pruneChildren = (nodes: CategoryNode[]) => {
    nodes.forEach(n => {
      if (n.children && n.children.length === 0) {
        delete n.children
      } else if (n.children) {
        pruneChildren(n.children)
      }
    })
  }

  pruneChildren(rootNodes)
  return rootNodes
}

/**
 * Tìm tất cả danh mục con trực tiếp (Direct Children) của một danh mục hiện tại
 */
export function getDirectChildren(
  parentCategory: QuestionCategory,
  flatList: QuestionCategory[]
): QuestionCategory[] {
  const prefix = parentCategory.name + ' - '
  return flatList.filter(c => {
    if (c.name.startsWith(prefix)) {
      const remainingName = c.name.slice(prefix.length)
      // Không chứa dấu " - " chứng tỏ đây là con trực tiếp, không phải cháu chắt
      return !remainingName.includes(' - ')
    }
    return false
  })
}

/**
 * Tìm tất cả danh mục con đệ quy (Recursive Children - bao gồm cả con, cháu, chắt)
 */
export function getRecursiveChildIds(
  parentCategory: QuestionCategory,
  flatList: QuestionCategory[]
): string[] {
  const prefix = parentCategory.name + ' - '
  return flatList
    .filter(c => c.name.startsWith(prefix))
    .map(c => c.questionCategoryId)
}

/**
 * Kiểm tra xem danh mục hiện tại có phải là danh mục lá (Leaf Category) hay không
 */
export function isLeafCategory(
  category: QuestionCategory,
  flatList: QuestionCategory[]
): boolean {
  const prefix = category.name + ' - '
  return !flatList.some(c => c.name.startsWith(prefix))
}

/**
 * Khởi tạo danh sách dropdown thụt đầu dòng (Indented List) để render vào các select thông thường
 */
export interface IndentedOption {
  value: string
  label: string
  rawName: string
  level: number
}

export function buildIndentedOptions(flatList: QuestionCategory[]): IndentedOption[] {
  const options: IndentedOption[] = []
  
  // Sắp xếp danh mục theo bảng chữ cái để hiển thị khoa học
  const sorted = [...flatList].sort((a, b) => a.name.localeCompare(b.name))
  
  sorted.forEach(cat => {
    const parts = cat.name.split(' - ')
    const level = parts.length - 1
    const localName = parts[parts.length - 1]
    
    // Tạo khoảng trống thụt lề trực quan
    const indent = '  '.repeat(level)
    const prefix = level > 0 ? '└─ ' : ''
    
    options.push({
      value: cat.questionCategoryId,
      label: `${indent}${prefix}${localName}`,
      rawName: cat.name,
      level
    })
  })
  
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
