using Module.MediaDownloader.Services;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var s = new YtbService();
            var url = "https://v.douyin.com/-WcetgwlcDk/";
            var info = await s.GetYtbVideoInfoAsync(url);
            await s.DownloadVideoAsync(url, info.Options.Last().FormatId);
        }
    }
}
