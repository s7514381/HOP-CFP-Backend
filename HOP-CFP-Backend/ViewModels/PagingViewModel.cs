using System.Collections.Generic;

namespace HOP_CFP_Backend.ViewModels
{
    public class PagingViewModel
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
    }

    public class PagingViewModel<T> : PagingViewModel
    {
        public IEnumerable<T> data { get; set; }
    }

}
