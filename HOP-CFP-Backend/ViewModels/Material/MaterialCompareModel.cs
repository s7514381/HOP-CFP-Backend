using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class MaterialCompareModel : MaterialCompare
    {
        [Display(Name = "供應商名稱")]
        public string? SupplierName { get; set; }

        [Display(Name = "供應商統編")]
        public string? SupplierTaxID { get; set; }
    }
}
