using API.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Module.MediaDownloader.Models.Requests;
using Module.MediaDownloader.Services;

namespace API.Controllers
{
    [Route("api/v{version:apiVersion}/media-downloaders")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MediaDownloadersController : ControllerBase
    {
        private readonly IDownloadMediaService _mediaService;

        public MediaDownloadersController(IDownloadMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        #region YTB

        [HttpGet("ytb/info")]
        public async Task<IActionResult> CheckMediaInfo([FromQuery] string url)
        {
            var info = await _mediaService.GetInfoAsync(url);
            return Ok(ApiResponse.Success(info));
        }

        [HttpGet("ytb/handle")]
        public IActionResult StartHandling([FromQuery] StartHandleMediaRequest request)
        {
            var jobId = _mediaService.StartHandleAsync(request);
            return Ok(ApiResponse.Success(data: jobId));
        }

        [HttpGet("ytb/download")]
        public async Task DownloadVideo([FromQuery] string jobId)
        {
            var info = await _mediaService.GetDownloadInfoAsync(jobId);

            try
            {
                var stream = new FileStream(info.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var ext = Path.GetExtension(info.FilePath).ToLowerInvariant();
                string contentType = ext switch
                {
                    ".mp4" => "video/mp4",
                    ".mp3" => "audio/mpeg", 
                    ".webm" => "video/webm",
                    ".m4a" => "audio/mp4",
                    ".opus" => "audio/opus",
                    ".ogg" => "audio/ogg",
                    _ => "application/octet-stream" 
                };

                Response.ContentType = contentType;

                var encodedFileName = Uri.EscapeDataString(info.MediaFileName);
                Response.Headers.Add(
                    "Content-Disposition",
                    $"attachment; filename*=UTF-8''{encodedFileName}.{ext}"
                );
                Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");

                byte[] buffer = new byte[1024 * 1024];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await Response.Body.WriteAsync(buffer, 0, bytesRead);
                    await Response.Body.FlushAsync();
                }

                stream.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await _mediaService.ClearJobAsync(jobId);
            }
        }

        #endregion
    }
}
