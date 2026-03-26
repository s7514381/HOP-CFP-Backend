using HOP_CFP_Backend.Library.Models;
using System.ComponentModel.DataAnnotations;

namespace HOP_CFP_Backend.ViewModels
{
    public class BaseListDataModel
    {
        public Guid Id { get; set; }

        [Display(Name = "狀態")]
        public EStatus Status { get; set; }
    }

}
