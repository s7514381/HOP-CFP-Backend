using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class AdminMenuByRoleService : _ModelService<AdminMenuByRole, AdminMenuByRoleModel>
    {
        public AdminMenuByRoleService(BaseServiceArgument argument) : base(argument) { }

    }
}
