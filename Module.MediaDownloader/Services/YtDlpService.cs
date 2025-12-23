using Core.Exceptions;
using Core.Utilities;
using Hangfire;
using Module.MediaDownloader.Models;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Module.MediaDownloader.Services
{
    public interface IYtDlpService
    {
        Task<MediaInfo> GetMediaInfoAsync(string url);
        Task StartDownloadMediaAsync(
            string mediaUrl, string? videoFormatId, 
            string? audioFormatId,
            string saveFilePath, string jobId);
    }

    public class YtDlpService : IYtDlpService
    {
        private readonly string _ytDlpPath;
        private readonly string _ffmpegPath;
        private readonly IManageProgressService _manageProgressService;

        public YtDlpService(IManageProgressService manageProgressService)
        {
            _manageProgressService = manageProgressService;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _ytDlpPath = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe");
                _ffmpegPath = Path.Combine(AppContext.BaseDirectory, "ffmpeg", "ffmpeg.exe");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _ytDlpPath = Path.Combine(AppContext.BaseDirectory, "yt-dlp");
                _ffmpegPath = "/usr/bin/ffmpeg";
            }
            else
            {
                throw new ServerErrorException("OS Platform not supported");
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task StartDownloadMediaAsync(
            string mediaUrl, string? videoFormatId, string? audioFormatId,
            string saveFilePath,string jobId)
        {
            try
            {
                if (await _manageProgressService.GetProgressAsync(jobId) is null)
                {
                    throw new ServerErrorException($"Update unknown progress for job {jobId}");
                }

                if (string.IsNullOrEmpty(videoFormatId) && string.IsNullOrEmpty(audioFormatId))
                {
                    throw new BadRequestException(StringUtil.ApiMessages.RequiredFormatId);
                }

                string cmd;
                if (!string.IsNullOrEmpty(videoFormatId) && !string.IsNullOrEmpty(audioFormatId))
                {
                    cmd = 
                        $"-f {videoFormatId}+{audioFormatId} " +
                        $"--ffmpeg-location {_ffmpegPath} " +
                        $"--merge-output-format mp4 " +
                        $"-o {saveFilePath} {mediaUrl}";
                }
                else if (!string.IsNullOrEmpty(audioFormatId))
                {
                    cmd = 
                        $"-f {audioFormatId} " +
                        $"--ffmpeg-location {_ffmpegPath} " +
                        $"--extract-audio --audio-format mp3 " +
                        $"-o {saveFilePath} {mediaUrl}";
                }
                else
                {
                    cmd =
                        $"-f {videoFormatId} " +
                        $"--ffmpeg-location {_ffmpegPath} " +
                        $"--merge-output-format mp4 " +
                        $"-o {saveFilePath} {mediaUrl}";
                }
                await RunCommandWithTrackingAsync(cmd, async progressLine =>
                {
                    Console.WriteLine(progressLine);
                    var match = Regex.Match(progressLine, @"(\d+(?:\.\d+)?)%");
                    if (match.Success)
                    {
                        double progress = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                        await _manageProgressService.UpdateProgressAsync(jobId, progress);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await _manageProgressService.CompleteProgressAsync(jobId);
            }
        }

        public async Task<MediaInfo> GetMediaInfoAsync(string url)
        {
            string json = await RunCommandAsync($"--dump-json \"{url}\"");
            var data = JsonDocument.Parse(json).RootElement;

            var formats = data.GetProperty("formats").EnumerateArray()
                .Select(f =>
                {
                    string GetString(string prop) => f.TryGetProperty(prop, out var v) ? v.GetString() ?? "" : "";
                    int? GetInt(string prop) => f.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.Number ? (int)v.GetDouble() : null;
                    double? GetDouble(string prop) => f.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetDouble() : null;

                    return new MediaFormatOption
                    {
                        FormatId = GetString("format_id"),
                        Ext = GetString("ext"),
                        Note = GetString("format_note"),
                        Width = GetInt("width"),
                        Height = GetInt("height"),
                        Fps = GetDouble("fps"),
                        Tbr = GetDouble("tbr"),
                        FileSize = GetInt("filesize"),
                        AudioCodec = GetString("acodec"),
                        VideoCodec = GetString("vcodec"),
                        AudioBitrate = GetDouble("abr"),
                    };
                })
                .Where(o => o.Note != "storyboard")
                .ToList();

            return new MediaInfo
            {
                Title = data.TryGetProperty("title", out var t) ? t.GetString() : "",
                Thumbnail = data.TryGetProperty("thumbnail", out var th) ? th.GetString() : "",
                Duration = data.TryGetProperty("duration_string", out var du) ? du.GetString() : "",
                Options = formats
            };
        }

        private async Task<string> RunCommandAsync(string args)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ytDlpPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };

            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception("yt-dlp error: " + error);

            return output;
        }

        private async Task RunCommandWithTrackingAsync(string args, Action<string>? onProgress = null)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _ytDlpPath,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    onProgress?.Invoke(e.Data);
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    onProgress?.Invoke(e.Data);
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
        }
    }
}
