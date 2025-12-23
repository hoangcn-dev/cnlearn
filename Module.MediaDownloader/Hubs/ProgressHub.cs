using Microsoft.AspNetCore.SignalR;
using Module.MediaDownloader.Services;
using Realtime;

namespace Module.MediaDownloader.Hubs
{
    public class ProgressHub : AppHub
    {
        private readonly IManageProgressService _updateProgressService;

        public const string HUB_URL = "/hubs/media-progress";

        public ProgressHub(IManageProgressService updateProgressService)
        {
            _updateProgressService = updateProgressService;
        }

        public override async Task OnConnectedAsync()
        {
            var jobId = GetQueryParam("jobId");
            if (string.IsNullOrEmpty(jobId))
            {
                Context.Abort();
            }

            var progressInfo = await _updateProgressService.GetProgressAsync(jobId!);
            if (progressInfo is null)
            {
                Context.Abort();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(jobId!));
            await Clients.Caller.SendAsync(Methods.OnConnected, progressInfo!.Progress);

            await base.OnConnectedAsync();
        }

        public static string GetGroupName(string jobId) 
            => $"group_{jobId}";

        public class Methods
        {
            public const string OnConnected = nameof(OnConnected);
            public const string OnReceivedProgress = nameof(OnReceivedProgress);
            public const string OnCompleted = nameof(OnCompleted);
        }
    }
}
