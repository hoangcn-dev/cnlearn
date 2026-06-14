import { post, get } from './config/axios'

export interface SubmitAttemptRequest {
  examId: string
  quizId?: string
  attemptType: 'practice' | 'exam' | 'quiz'
  timeSpentSeconds: number
  answers: {
    questionId: string
    selectedAnswerIds: string[]
  }[]
}

export const submitAttempt = (data: SubmitAttemptRequest) => {
  return post('/api/attempts/submit', data)
}

export const getExamHistory = (page: number = 1, size: number = 10) => {
  return get('/api/attempts/history/exams', { page, size })
}

export const getQuestionHistory = (page: number = 1, size: number = 10) => {
  return get('/api/attempts/history/questions', { page, size })
}
