using Core.Interfaces;

namespace Modules.Users.Entities
{
    public class Roles
    {
        public const string USER = "user";
        public const string ADMIN = "admin";
    }

    public class Role : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public List<User> Users { get; set; } = new();
    }
}
