using HoangCN.BL.Base;
using HoangCN.Common.Model.Entities;
using HoangCN.DL.Interfaces;
using HoangCN.UserManagement.Interfaces;

namespace HoangCN.UserManagement.Services
{
    public class RoleService : BaseBL<Role>, IRoleService
    {
        public RoleService(IBaseDL baseDL) : base(baseDL)
        {
        }
    }
}
