using Core.Exceptions;
using Core.Utilities;
using Hangfire;
using Module.MediaDownloader.Models;
using Module.MediaDownloader.Models.Requests;

namespace Module.MediaDownloader.Services
{
    public interface IDownloadMediaService
    {
        Task<MediaInfo> GetInfoAsync(string url);
        string StartHandleAsync(StartHandleMediaRequest request);
        Task<HandleProgressInfo> GetDownloadInfoAsync(string jobId);
        Task ClearJobAsync(string jobId);
    }

    public class DownloadMediaService : IDownloadMediaService
    {
        private readonly IYtDlpService _ytDlpService;
        private readonly IManageProgressService _manageProgressService;

        public DownloadMediaService(
            IYtDlpService ytDlpService,
            IManageProgressService manageProgressService)
        {
            _ytDlpService = ytDlpService;
            _manageProgressService = manageProgressService;
        }

        public string StartHandleAsync(StartHandleMediaRequest request)
        {
            var ext = request.AudioFormatId is not null && request.VideoFormatId is null? 
                "mp3" : "mp4";
            var (jobId, newProgress) = _manageProgressService.AddProgressAsync(ext, request.MediaFileName);

            BackgroundJob.Enqueue<YtDlpService>(s => s.StartDownloadMediaAsync(
                request.Url,
                request.VideoFormatId,
                request.AudioFormatId,
                newProgress.FilePath,
                jobId));

            return jobId;
        }

        public async Task<MediaInfo> GetInfoAsync(string url)
        {
            return await _ytDlpService.GetMediaInfoAsync(url);
            //string json = await RunCommandAsync($"--dump-json \"{url}\"");
            //var data = JsonDocument.Parse(json).RootElement;

            //var formats = data.GetProperty("formats").EnumerateArray().Select(f =>
            //{
            //    string resolution =
            //        f.TryGetProperty("resolution", out var r) && r.ValueKind == JsonValueKind.String
            //            ? r.GetString()!
            //            : f.TryGetProperty("width", out var w) && f.TryGetProperty("height", out var h)
            //                ? $"{w.GetInt32()}x{h.GetInt32()}"
            //                : "";

            //    return new MediaFormatOption
            //    {
            //        FormatId = f.GetProperty("format_id").GetString(),
            //        Ext = f.TryGetProperty("ext", out var ext) ? ext.GetString() : "",
            //        Note = f.TryGetProperty("format_note", out var n) ? n.GetString() : "",
            //        Width = f.TryGetProperty("width", out var wd) && wd.ValueKind == JsonValueKind.Number ? wd.GetInt32() : null,
            //        Height = f.TryGetProperty("height", out var ht) && ht.ValueKind == JsonValueKind.Number ? ht.GetInt32() : null,
            //        Fps = f.TryGetProperty("fps", out var fps) && fps.ValueKind == JsonValueKind.Number ? fps.GetDouble() : null,
            //        Tbr = f.TryGetProperty("tbr", out var tbr) && tbr.ValueKind == JsonValueKind.Number ? tbr.GetDouble() : null,
            //        FileSize = f.TryGetProperty("filesize", out var fs) && fs.ValueKind == JsonValueKind.Number ? fs.GetInt64() : null,
            //        AudioCodec = f.TryGetProperty("acodec", out var ac) ? ac.GetString() : null,
            //        VideoCodec = f.TryGetProperty("vcodec", out var vc) ? vc.GetString() : null,
            //        AudioBitrate = f.TryGetProperty("abr", out var ab) && ab.ValueKind == JsonValueKind.Number ? ab.GetDouble() : null
            //    };
            //})
            //.Where(o =>
            //{
            //    //if (string.IsNullOrEmpty(o.Note) || o.Note == "storyboard")
            //    //{
            //    //    return false;
            //    //}

            //    //if (o.FileSize is null)
            //    //{
            //    //    return false;
            //    //}

            //    //if (o.Ext != "mp4" && o.Ext != "mp3")
            //    //{
            //    //    return false;
            //    //}

            //    //if (o.AudioCodec == "none")
            //    //{
            //    //    return false;
            //    //}

            //    return true;
            //})
            //.ToList();

            //return new MediaInfo
            //{
            //    Title = data.TryGetProperty("title", out var t) ? t.GetString() : "",
            //    Thumbnail = data.TryGetProperty("thumbnail", out var th) ? th.GetString() : "",
            //    Duration = data.TryGetProperty("duration_string", out var du) ? du.GetString() : "",
            //    Options = formats
            //};
        }

        public async Task<HandleProgressInfo> GetDownloadInfoAsync(string jobId)
        {
            var info = await _manageProgressService.GetProgressAsync(jobId);

            if (info == null)
            {
                throw new NotFoundException(StringUtil.ApiMessages.DownloadProgressNotExist);
            }

            if (info.Progress != 100)
            {
                throw new BadRequestException(StringUtil.ApiMessages.ProgressNotCompleted);
            }

            return info;
        }

        public async Task ClearJobAsync(string jobId)
        {
            await _manageProgressService.RemoveJobAsync(jobId);
        }
    }
}
