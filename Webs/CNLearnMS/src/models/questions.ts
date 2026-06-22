export enum QuestionLevel {
  Easy = 0,
  Medium = 1,
  Hard = 2
}

export const QuestionLevelLabel: Record<QuestionLevel, string> = {
  [QuestionLevel.Easy]: 'Dễ',
  [QuestionLevel.Medium]: 'Trung bình',
  [QuestionLevel.Hard]: 'Khó',
}

export enum QuestionType {
  SingleChoice = 0,
  MultipleChoice = 1
}

export const QuestionTypeLabel: Record<QuestionType, string> = {
  [QuestionType.SingleChoice]: 'Chọn 1 đáp án',
  [QuestionType.MultipleChoice]: 'Chọn nhiều đáp án',
}

export enum QuestionAccessType {
  Public = 0,
  Private = 1
}

export const QuestionAccessTypeLabel: Record<QuestionAccessType, string> = {
  [QuestionAccessType.Public]: 'Công khai',
  [QuestionAccessType.Private]: 'Cá nhân',
}

// ----------------- ENTITIES -----------------

export interface Question {
  questionId: string
  questionSlug?: string
  stringContent?: string
  explaination?: string
  attemptCount: number
  level: QuestionLevel
  type: QuestionType
  learnMsUserId: string
  accessType: QuestionAccessType
  isInBank: boolean
  questionCategoryId: string
  source?: string
  sourceExamId?: string | null
  answers: QuestionAnswer[]
}

export interface QuestionAnswer {
  questionAnswerId: string
  stringContent?: string
  isCorrectAnswer: boolean
  questionId: string
  orderInList: number
}

// ----------------- DTOS -----------------

export interface QuestionDto {
  questionId: string
  questionSlug?: string
  stringContent?: string
  explaination?: string
  level: QuestionLevel
  type: QuestionType
  accessType: QuestionAccessType
  questionCategoryName: string
  questionCategoryId: string
  attemptCount: number
  learnMsUserId: string
  fullName: string
  isInBank: boolean
  source?: string
  modifiedDate: string | Date
}

export interface QuestionAnswerDto {
  questionAnswerId: string
  stringContent?: string
  questionId: string
  orderInList: number
}

export interface QuestionCorrectAnswerMappingDto {
  correctMap: Record<string, string[]>
}
