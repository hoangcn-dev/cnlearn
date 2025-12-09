using System.Data.Common;
using Module.Users.Entities;

namespace Module.Users.Models
{
    public class UserDetail : UserListItem
    {
        public string AvatarUrl { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

        public static UserDetail ConvertFromUserToDetail(User user)
        {
            return new UserDetail
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Note = user.Note,
                PhoneNumber = user.PhoneNumber,
                IsActived = user.IsActived,
                IsOnline = user.IsOnline,
                LastLogin = user.LastLogin,
                Role = new RoleListItem
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name
                }
            };
        }
    }
}