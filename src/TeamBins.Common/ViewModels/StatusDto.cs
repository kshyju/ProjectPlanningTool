


    public class ItemCount
{
        public string ItemName { set; get; }
        public int Count { set; get; }
        public int ItemId { get; set; }
    }

public class ChartItem : ItemCount
{
    public string Color { set; get; }
}