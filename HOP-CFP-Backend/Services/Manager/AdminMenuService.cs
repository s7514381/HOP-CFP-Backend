using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;

namespace HOP_CFP_Backend.Services
{
    public class AdminMenuService : _StandardService<AdminMenu, AdminMenuModel, AdminMenuSearchViewModel, AdminMenuListViewModel, AdminMenuListDataModel>
    {
        public AdminMenuService(BaseServiceArgument argument) : base(argument) { }

    }
}
