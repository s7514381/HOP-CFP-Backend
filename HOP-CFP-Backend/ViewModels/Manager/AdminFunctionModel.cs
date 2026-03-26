using HOP_CFP_Backend.Library.Models.Manager;

namespace HOP_CFP_Backend.ViewModels
{
    public class AdminFunctionModel : AdminFunction
    {
        public List<AdminFunctionModel> ChildList { get; set; } = new List<AdminFunctionModel>();
    }

    public class AdminFunctionSearchViewModel : BaseSearchViewModel
    {
    }

    public class AdminFunctionListViewModel : PagingViewModel<AdminFunctionListDataModel>
    {
    }

    public class AdminFunctionListDataModel : BaseListDataModel
    {

    }
}
