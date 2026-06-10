using HoangCN.Core.Common.Attributes;
using HoangCN.MainSystem.Entities;

namespace HoangCN.MainSystem.DTOs
{
    public class UserAuthDto : User
    {
        [ForeignTable(EntityType = typeof(Role))]
        public string RoleName { get; set; }
    }
}

