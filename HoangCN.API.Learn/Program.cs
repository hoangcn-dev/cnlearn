
using HoangCN.BL.Utils;
using HoangCN.Common.Utils;

namespace HoangCN.API.Learn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddBL(builder.Configuration);
            builder.ConfigCommon();

            var app = builder.BuildCommon();
            app.Run();
        }
    }
}
