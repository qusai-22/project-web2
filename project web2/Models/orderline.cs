namespace project_web2.Models
{
    public class orderline
    {
        public int Id { get; set; }
        public string itemname { get; set; }
        public int itemquant { get; set; }
        public Decimal itemprice { get; set; }
        public int orderid { get; set; }
    }
}
