using HoangCN.Core.BL.Base;
using HoangCN.Core.BL.Interfaces;
using HoangCN.Core.Common.Exceptions;
using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using HoangCN.LearnMS.Interfaces;
using HoangCN.LearnMS.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace HoangCN.LearnMS.Services
{
    public class ExamSessionService : BaseBL<ExamSession>, IExamSessionService
    {
        private readonly IBaseBL<ExamCheatLog> _cheatLogBL;

        public ExamSessionService(IBaseReadDL readDL, IBaseWriteDL writeDL, IBaseBL<ExamCheatLog> cheatLogBL, IHttpContextAccessor httpContextAccessor) 
            : base(readDL, writeDL, httpContextAccessor)
        {
            _cheatLogBL = cheatLogBL;
        }

        public async Task<Guid> StartSessionAsync(Guid candidateId, ExamSessionStartRequest request)
        {
            var session = new ExamSession
            {
                SessionId = Guid.NewGuid(),
                CandidateId = candidateId,
                QuizId = request.QuizId,
                StartedAt = DateTime.UtcNow,
                LastHeartbeatAt = DateTime.UtcNow,
                IsActive = true,
                Status = ExamSessionStatus.InProgress,
                BlurCount = 0,
                FullscreenExitCount = 0
            };

            await InsertEntities(new List<ExamSession> { session });
            return session.SessionId;
        }

        public async Task ProcessHeartbeatAsync(Guid sessionId, Guid candidateId, ExamSessionHeartbeatRequest request)
        {
            var session = await GetFirstByCondition<ExamSession>(es => es.SessionId == sessionId);
            if (session == null || session.CandidateId != candidateId)
            {
                throw new NotFoundException("Không tìm thấy phiên làm bài");
            }

            if (!session.IsActive)
            {
                throw new ForbiddenException("Phiên làm bài này đã kết thúc hoặc bị hủy");
            }

            var now = DateTime.UtcNow;
            if ((now - session.LastHeartbeatAt).TotalSeconds > 60)
            {
                session.Status = ExamSessionStatus.Disconnected;
                session.IsActive = false;
                await UpdateEntities(new List<ExamSession> { session });
                throw new ForbiddenException("Phiên làm bài đã mất kết nối quá thời gian cho phép (1 phút). Hệ thống sẽ tự động nộp bài.");
            }

            // Xử lý vi phạm offline
            if (request.OfflineLogs != null && request.OfflineLogs.Count > 0)
            {
                var newLogs = new List<ExamCheatLog>();
                foreach (var log in request.OfflineLogs)
                {
                    if (log.ViolationType == ExamViolationType.BlurWindow) session.BlurCount++;
                    else if (log.ViolationType == ExamViolationType.ExitFullscreen) session.FullscreenExitCount++;

                    newLogs.Add(new ExamCheatLog
                    {
                        LogId = Guid.NewGuid(),
                        SessionId = sessionId,
                        Timestamp = log.Timestamp,
                        ViolationType = log.ViolationType
                    });
                }
                await _cheatLogBL.InsertEntities(newLogs);
            }

            if (session.FullscreenExitCount >= 3)
            {
                session.Status = ExamSessionStatus.Disqualified;
                session.IsActive = false;
                await UpdateEntities(new List<ExamSession> { session });
                throw new ForbiddenException("Bạn đã thoát toàn màn hình 3 lần. Bài thi bị hủy tự động.");
            }

            session.LastHeartbeatAt = now;
            await UpdateEntities(new List<ExamSession> { session });
        }

        public async Task LogCheatAsync(Guid sessionId, Guid candidateId, ExamCheatLogRequest request)
        {
            var session = await GetFirstByCondition<ExamSession>(es => es.SessionId == sessionId);
            if (session == null || session.CandidateId != candidateId)
            {
                throw new NotFoundException("Không tìm thấy phiên làm bài");
            }

            if (!session.IsActive)
            {
                return;
            }

            if (request.ViolationType == ExamViolationType.BlurWindow) session.BlurCount++;
            else if (request.ViolationType == ExamViolationType.ExitFullscreen) session.FullscreenExitCount++;

            var newLog = new ExamCheatLog
            {
                LogId = Guid.NewGuid(),
                SessionId = sessionId,
                Timestamp = request.Timestamp != default ? request.Timestamp : DateTime.UtcNow,
                ViolationType = request.ViolationType
            };

            await _cheatLogBL.InsertEntities(new List<ExamCheatLog> { newLog });

            if (session.FullscreenExitCount >= 3)
            {
                session.Status = ExamSessionStatus.Disqualified;
                session.IsActive = false;
            }

            await UpdateEntities(new List<ExamSession> { session });

            if (session.Status == ExamSessionStatus.Disqualified)
            {
                throw new ForbiddenException("Bạn đã thoát toàn màn hình 3 lần. Bài thi bị hủy tự động.");
            }
        }

        public async Task SubmitSessionAsync(Guid sessionId, Guid candidateId)
        {
            var session = await GetFirstByCondition<ExamSession>(es => es.SessionId == sessionId);
            if (session == null || session.CandidateId != candidateId)
            {
                throw new NotFoundException("Không tìm thấy phiên làm bài");
            }

            if (session.IsActive)
            {
                session.IsActive = false;
                session.Status = ExamSessionStatus.Submitted;
                await UpdateEntities(new List<ExamSession> { session });
            }
        }
    }
}
