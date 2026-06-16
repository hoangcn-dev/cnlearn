namespace Module.MediaDownloader.Models
{
    public class MediaInfo
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Duration { get; set; }
        public List<MediaFormatOption> Options { get; set; }
    }
}
