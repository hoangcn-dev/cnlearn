namespace Module.MediaDownloader.Models.Requests
{
    public class StartHandleMediaRequest
    {
        public string Url { get; set; }
        public string? VideoFormatId { get; set; }
        public string? AudioFormatId { get; set; }
        public string MediaFileName { get; set; }
    }
}
