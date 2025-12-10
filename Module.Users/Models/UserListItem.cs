using Module.Users.Entities;

namespace Module.Users.Models
{
    public class UserListItem
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsOnline { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActived { get; set; }
        public DateTimeOffset LastLogin { get; set; }
        public RoleListItem Role { get; set; }

        public static UserListItem ConvertFromUserToItem(User user)
        {
            return new UserListItem
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                IsActived = user.IsActived,
                IsOnline = user.IsOnline,
                LastLogin = user.LastLogin,
                // Role = new RoleListItem
                // {
                //     Id = user.Role.Id,
                //     Name = user.Role.Name
                // }
            };
        }
    }
}
