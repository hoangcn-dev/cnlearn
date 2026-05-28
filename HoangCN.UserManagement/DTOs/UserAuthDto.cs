using HoangCN.Common.Attributes;
using HoangCN.Common.Model.Entities;

namespace HoangCN.UserManagement.DTOs
{
    public class UserAuthDto : User
    {
        [ForeignTable(EntityType = typeof(Role))]
        public string RoleName { get; set; }
    }
}
