namespace OnlinePractice.API.Models.DBModel
{
    public class Payment : BaseModel
    {
        public Guid OrderId { get; set; }
        public Guid StudentId { get; set; }
        public double OrderAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
    }
}
