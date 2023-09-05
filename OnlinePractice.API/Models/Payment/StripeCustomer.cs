namespace OnlinePractice.API.Models.Stripe
{
    public class StripeCustomer
    {
     public   string? Name { get; set; }

      public string? Email { get; set; }

        public string? CustomerId { get; set; }

        public string? token { get; set;}
    }
}
