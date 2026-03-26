using System.Collections.Generic;
using HOP_CFP_Backend.Library.Models;

namespace HOP_CFP_Backend.ViewModels
{
    public class BaseSearchViewModel
    {
        public List<DataTablesOrder> order { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public int draw { get; set; }
        public EStatus? Status { get; set; }
    }

    public class DataTablesOrder
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

}
