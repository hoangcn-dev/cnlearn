import type { Question, QuestionAnswer, QuestionLevel, QuestionType } from './questions'

export enum ExamAccessType {
  Public = 0,
  Private = 1
}

export interface Exam {
  examId: string
  name: string
  description?: string
  questionCategoryId: string
  durationMin: number
  accessType: ExamAccessType
  isDraft: boolean
  draftData?: string
  learnMsUserId: string
  contributeToBank: boolean
  questions: ExamQuestion[]
}

export interface ExamQuestion {
    examQuestionId: string;
    stringContent?: string;
    orderInExam: number;
    explaination?: string;
    level: QuestionLevel;
    type: QuestionType;
    answers: QuestionAnswer[];
}


export interface ExamDto {
  examId: string
  name: string
  description?: string
  questionCategoryId: string
  questionCategoryName: string
  durationMin: number
  accessType: ExamAccessType
  isDraft: boolean
  learnMsUserId: string
  authorFullname: string
  questionCount?: number
  isMyCreated?: boolean
}
