using Core.Exceptions;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Module.MediaDownloader.Hubs;
using Module.MediaDownloader.Models;
using System;
using System.Collections.Concurrent;

namespace Module.MediaDownloader.Services
{
    public interface IManageProgressService
    {
        Task UpdateProgressAsync(string jobId, double progress);
        Task RemoveJobAsync(string jobId);
        Task CleanAsync();
        Task CompleteProgressAsync(string jobId);
        (string, HandleProgressInfo) AddProgressAsync(string ext, string mediaFileName);
        Task<HandleProgressInfo?> GetProgressAsync(string jobId);
    }

    public class ManageProgressService : IManageProgressService
    {
        private readonly ILogger<ManageProgressService> _logger;
        private readonly IHubContext<ProgressHub> _hubContext;
        private static readonly ConcurrentDictionary<string, HandleProgressInfo> _progress = new();
        private readonly string _tempFolderPath = Path.Combine(AppContext.BaseDirectory, "temps");

        public ManageProgressService(
            IHubContext<ProgressHub> hubContext, 
            ILogger<ManageProgressService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task UpdateProgressAsync(string jobId, double progress)
        {
            if (_progress.TryGetValue(jobId, out HandleProgressInfo? value))
            {
                value.Progress = progress;
                await _hubContext.Clients.Group(ProgressHub.GetGroupName(jobId))
                    .SendAsync(ProgressHub.Methods.OnReceivedProgress, progress);
            }
        }

        public (string, HandleProgressInfo) AddProgressAsync(string ext, string mediaFileName)
        {
            string filePath = Path.Combine(_tempFolderPath, $"{Guid.NewGuid()}.{ext}");
            string jobId = Guid.NewGuid().ToString();

            var progress = new HandleProgressInfo
            {
                FilePath = filePath,
                Progress = 0,
                MediaFileName = mediaFileName,
            };
            _progress.TryAdd(jobId, progress);

            return (jobId, progress);
        }

        public async Task<HandleProgressInfo?> GetProgressAsync(string jobId)
        {
            if (_progress.TryGetValue(jobId, out var p))
            {
                return await Task.FromResult(p);
            }
            return null;
        }

        public Task RemoveJobAsync(string jobId)
        {
            if (_progress.TryRemove(jobId, out var info))
            {
                if (File.Exists(info.FilePath))
                {
                    File.Delete(info.FilePath);
                }
            }

            return Task.CompletedTask;
        }

        public async Task CompleteProgressAsync(string jobId)
        {
            if (!_progress.ContainsKey(jobId))
            {
                _logger.LogError($"End unknown progress for job {jobId}");
                return;
            }

            await _hubContext.Clients.Group(ProgressHub.GetGroupName(jobId))
                    .SendAsync(ProgressHub.Methods.OnCompleted, _progress[jobId].Progress == 100);
        }

        public async Task CleanAsync()
        {
            var files = Directory.GetFiles(_tempFolderPath);
            var executingFiles = _progress.Values
                .Select(p => Path.GetFileNameWithoutExtension(p.FilePath)).ToHashSet();
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file).Split('.').First();
                if (!executingFiles.Contains(fileName))
                {
                    File.Delete(file);
                    _logger.LogInformation($"Deleted " + file);
                }
            }

            await Task.CompletedTask;
        }
    }
}
