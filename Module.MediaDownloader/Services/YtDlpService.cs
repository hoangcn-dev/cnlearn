using Module.MediaDownloader.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Module.MediaDownloader.Services
{
    public interface IYtDlpService
    {
        Task<MediaInfo> GetMediaInfoAsync(string url, string? cookiesPath = null);
        Task StartDownloadMediaAsync(
            string mediaUrl, string? videoFormatId, 
            string? audioFormatId,
            string saveFilePath);
    }

    public class YtDlpService : IYtDlpService
    {
        private readonly string _ytDlpPath;
        private readonly string _ffmpegPath;

        public YtDlpService()
        {
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
                throw new Exception("OS Platform not supported");
            }
        }

        public async Task StartDownloadMediaAsync(
            string mediaUrl, string? videoFormatId, string? audioFormatId,
            string saveFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(videoFormatId) && string.IsNullOrEmpty(audioFormatId))
                {
                    throw new Exception("Format không hợp lệ");
                }

                string cmd = "--cookies-from-browser chrome ";
                if (!string.IsNullOrEmpty(videoFormatId) && !string.IsNullOrEmpty(audioFormatId))
                {
                    cmd += 
                        $"-f {videoFormatId}+{audioFormatId} " +
                        $"--ffmpeg-location {_ffmpegPath} " +
                        $"--merge-output-format mp4 " +
                        $"-o {saveFilePath} {mediaUrl}";
                }
                else if (!string.IsNullOrEmpty(audioFormatId))
                {
                    cmd +=
                        $"-f {audioFormatId} " +
                        $"--ffmpeg-location {_ffmpegPath} " +
                        $"--extract-audio --audio-format mp3 " +
                        $"-o {saveFilePath} {mediaUrl}";
                }
                else
                {
                    cmd +=
                        $"-f {videoFormatId} " +
                        $"--ffmpeg-location {_ffmpegPath} " +
                        $"--merge-output-format mp4 " +
                        $"-o {saveFilePath} {mediaUrl}";
                }
                await RunCommandWithTrackingAsync(cmd, async progressLine =>
                {
                    Console.WriteLine(progressLine);
                    //var match = Regex.Match(progressLine, @"(\d+(?:\.\d+)?)%");
                    //if (match.Success)
                    //{
                    //    double progress = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    //    await _manageProgressService.UpdateProgressAsync(jobId, progress);
                    //}
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<MediaInfo> GetMediaInfoAsync(string url, string? cookiesPath = null)
        {
            var args = new List<string>
            {
                "--dump-json",
                "--no-warnings",           
                "--retries", "3",            
                "--fragment-retries", "3",
                "--sleep-requests", "1",
                "--user-agent", "\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36\"",  // Sửa ở đây: thêm \"...\"                "--extractor-args", "douyin:api_retry_sleep=2"
            };

            if (!string.IsNullOrWhiteSpace(cookiesPath) && File.Exists(cookiesPath))
            {
                args.Add("--cookies");
                args.Add($"\"{cookiesPath}\"");
            }

            args.Add($"\"{url}\"");

            string cmd = string.Join(" ", args);

            string json = await RunCommandAsync(cmd);
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
                EnableRaisingEvents = true
            };

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Console.WriteLine("[yt-dlp] " + e.Data); // log ra console
                    outputBuilder.AppendLine(e.Data);       // lưu vào string
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Console.WriteLine("[yt-dlp ERROR] " + e.Data); // log lỗi
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
                throw new Exception("yt-dlp error: " + errorBuilder.ToString());

            return outputBuilder.ToString();
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
