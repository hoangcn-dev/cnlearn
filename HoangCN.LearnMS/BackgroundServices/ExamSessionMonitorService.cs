using HoangCN.Core.DL.Interfaces;
using HoangCN.LearnMS.Entities;
using HoangCN.LearnMS.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HoangCN.LearnMS.BackgroundServices
{
    public class ExamSessionMonitorService : BackgroundService
    {
        private readonly ILogger<ExamSessionMonitorService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExamSessionMonitorService(ILogger<ExamSessionMonitorService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExamSessionMonitorService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckExpiredSessionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing CheckExpiredSessionsAsync.");
                }

                // Check every 15 seconds
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }

            _logger.LogInformation("ExamSessionMonitorService is stopping.");
        }

        private async Task CheckExpiredSessionsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var examSessionBL = scope.ServiceProvider.GetRequiredService<HoangCN.LearnMS.Interfaces.IExamSessionBL>();

            // Get active sessions
            var activeSessions = await examSessionBL.GetByCondition<ExamSession>(s => s.IsActive);

            if (activeSessions == null || activeSessions.Count == 0)
                return;

            var now = DateTime.UtcNow;
            var expiredSessions = activeSessions
                .Where(s => (now - s.LastHeartbeatAt).TotalSeconds > 60)
                .ToList();

            if (expiredSessions.Any())
            {
                foreach (var session in expiredSessions)
                {
                    session.IsActive = false;
                    session.Status = ExamSessionStatus.Disconnected;
                    session.State = HoangCN.Core.Common.Enums.ModelState.Update;
                }

                await examSessionBL.Save(expiredSessions);
                _logger.LogInformation($"Auto-disconnected {expiredSessions.Count} expired exam sessions.");
            }
        }
    }
}
