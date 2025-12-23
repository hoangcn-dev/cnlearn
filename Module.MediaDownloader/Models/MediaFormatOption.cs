using System.Text.Json.Serialization;

namespace Module.MediaDownloader.Models
{
    public enum MediaType
    {
        VideoOnly,
        AudioOnly,
        Progressive
    }

    public class MediaFormatOption
    {
        public string FormatId { get; set; }
        public string Ext { get; set; }
        public string Note { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public double? Fps { get; set; }
        public double? Tbr { get; set; }
        public int? FileSize { get; set; } 
        public string AudioCodec { get; set; }
        public string VideoCodec { get; set; }
        public double? AudioBitrate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MediaType Type =>
            VideoCodec != "none" && AudioCodec == "none" ? MediaType.VideoOnly :
            VideoCodec == "none" && AudioCodec != "none" ? MediaType.AudioOnly :
            MediaType.Progressive;
    }
}
