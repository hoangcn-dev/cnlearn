using System;
using System.Collections.Generic;
using HoangCN.LearnMS.Enums;

namespace HoangCN.LearnMS.Requests
{
    public class ExamSessionStartRequest
    {
        public Guid QuizId { get; set; }
    }

    public class ExamCheatLogRequest
    {
        public ExamViolationType ViolationType { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ExamSessionHeartbeatRequest
    {
        public List<ExamCheatLogRequest>? OfflineLogs { get; set; }
    }
}
