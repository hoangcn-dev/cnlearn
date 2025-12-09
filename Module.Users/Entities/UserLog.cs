using Core.Interfaces;

namespace Module.Users.Entities
{
    public class UserLog : IEntity
    {
        public long Id { get; set; }
        public string Log { get; set; }
        public bool IsSystemAction { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}