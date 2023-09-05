using Stripe;

namespace OnlinePractice.API.Models.Response
{

    public class ShowMyCartList
    {
        public List<ShowMyCart> ShowMyCart { get; set; } = new();
        public int TotalRecords { get; set; }
        public double CartTotalPrice { get; set; }
    }
    public class ShowMyCart : MyCartDetails
    {
        public Guid Id { get; set; }
        public string ProductCategory { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public double Price { get; set; }
    }

    public class MyCartDetails
    {
        public string ProductName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;

    }
}

