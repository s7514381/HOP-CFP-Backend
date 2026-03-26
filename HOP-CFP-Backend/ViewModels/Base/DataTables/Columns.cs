namespace HOP_CFP_Backend.Models.DataTables
{
    public class Column
    {
        public string data { get; set; }
        public string title { get; set; }
        public string render { get; set; }
        public bool orderable { get; set; }
        public bool visible { get; set; }
        public string tip { get; set; }

        public Column()
        {
            orderable = true;
            visible = true;
        }
    }
}
