using HoangCN.Core.Common.Attributes;
using HoangCN.MainSystem.Entities;

namespace HoangCN.MainSystem.DTOs
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

