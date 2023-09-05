namespace OnlinePractice.API.Models.DBModel
{
    public class Wallet : BaseModel
    {
        public Guid StudentId { get; set; }
        public double Amount { get; set; }
    }
}
