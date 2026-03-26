using HOP_CFP_Backend.Library.Models.Manager;

namespace HOP_CFP_Backend.ViewModels
{
    public class AdminMenuModel : AdminMenu
    {
    }

    public class AdminMenuSearchViewModel : BaseSearchViewModel
    {
    }

    public class AdminMenuListViewModel : PagingViewModel<AdminMenuListDataModel>
    {
    }

    public class AdminMenuListDataModel : BaseListDataModel
    {

    }
}
