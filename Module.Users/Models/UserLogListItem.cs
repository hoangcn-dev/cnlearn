using Module.Users.Entities;

namespace Module.Users.Models
{
    public class UserLogListItem
    {
        public string Log { get; set; }
        public bool IsSystemAction { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public Guid UserId { get; set; }

        public static UserLogListItem ConvertFromUserLogToItem(UserLog log)
        {
            return new UserLogListItem
            {
                Log = log.Log,
                Timestamp = log.Timestamp,
                IsSystemAction = log.IsSystemAction,
                UserId = log.UserId
            };
        }
    }
}