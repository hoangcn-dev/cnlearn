import axiosInstance from './config/axios'

export const examSessionApi = {
  startSession: (quizId: string) => {
    return axiosInstance.post('/ExamSessions/start', { quizId })
  },
  heartbeat: (sessionId: string, offlineLogs: any[] = []) => {
    return axiosInstance.post(`/ExamSessions/${sessionId}/heartbeat`, { offlineLogs })
  },
  logCheat: (sessionId: string, violationType: number) => {
    // violationType: 1 = BlurWindow, 2 = ExitFullscreen
    return axiosInstance.post(`/ExamSessions/${sessionId}/cheat-log`, { 
      violationType, 
      timestamp: new Date().toISOString() 
    })
  },
  submitSession: (sessionId: string) => {
    return axiosInstance.post(`/ExamSessions/${sessionId}/submit`)
  }
}
