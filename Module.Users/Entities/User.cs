using Core.Interfaces;

namespace Module.Users.Entities
{
    public class User : IChangableEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public bool IsActived { get; set; }
        public bool IsOnline { get; set; }
        public DateTimeOffset LastLogin { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public Role Role { get; set; }
        public Guid RoleId { get; set; }

        public List<UserLog> Logs { get; set; } = new ();

        public void AddLog(string log, bool isSystemAction = false)
        {
            Logs.Add(new UserLog
            {
                IsSystemAction = isSystemAction,
                Log = log,
                Timestamp = DateTimeOffset.UtcNow,
                User = this
            });
        }
    }
}
