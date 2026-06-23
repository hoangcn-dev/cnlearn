export interface Quiz {
  quizId: string
  title: string
  targetGroup?: string
  sourceType: string // "exam" | "direct"
  examId?: string | null
  startDate?: string | null
  endDate?: string | null
  isDraft: boolean
  userId: string
  lockBrowser: boolean
  shuffleQuestions: boolean
  disableCopyPaste: boolean
  fullscreen: boolean
  webcam: boolean
  ipLimit: boolean
  allowLateJoin: boolean
  allowLateSubmit: boolean
  publicLeaderboard: boolean
  sendEmailReport: boolean
  directCategoryId?: string | null
  directTotalQuestions?: number | null
  directEasyCount: number
  directMediumCount: number
  directHardCount: number
  isMyCreated: boolean
}
