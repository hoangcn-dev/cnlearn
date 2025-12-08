namespace Module.Users.Models
{
    public class UserListItem
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Note { get; set; }
        public bool IsActived { get; set; }
        public DateTimeOffset LastLogin { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public RoleListItem Role { get; set; }
    }
}
