using HoangCN.Common.Attributes;
using HoangCN.Common.Model.Entities;

namespace HoangCN.UserManagement.DTOs
{
    public class LoginSessionInfoDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string? DisplayName { get; set; }
        public string Email { get; set; }

        [ForeignTable(EntityType = typeof(Role))]
        public string RoleName { get; set; }
    }
}
