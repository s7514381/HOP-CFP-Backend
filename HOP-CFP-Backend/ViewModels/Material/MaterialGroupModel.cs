using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class MaterialGroupModel : MaterialGroup
    {
        public List<MaterialModel> MaterialList { get; set; } = new();
    }

    public class MaterialGroupSearchViewModel : BaseSearchViewModel
    {
    }

    public class MaterialGroupListViewModel : PagingViewModel<MaterialGroupListDataModel>
    {
    }

    public class MaterialGroupListDataModel : BaseListDataModel
    {
        [Display(Name = "群組代號")]
        public string? Code { get; set; }

        [Display(Name = "群組名稱")]
        public string? Name { get; set; }
    }
}
