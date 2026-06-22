using HoangCN.Core.BL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Requests;
using System;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.Interfaces
{
    public interface IExamSessionService : IBaseBL<ExamSession>
    {
        Task<Guid> StartSessionAsync(Guid candidateId, ExamSessionStartRequest request);
        Task ProcessHeartbeatAsync(Guid sessionId, Guid candidateId, ExamSessionHeartbeatRequest request);
        Task LogCheatAsync(Guid sessionId, Guid candidateId, ExamCheatLogRequest request);
        Task SubmitSessionAsync(Guid sessionId, Guid candidateId);
    }
}
