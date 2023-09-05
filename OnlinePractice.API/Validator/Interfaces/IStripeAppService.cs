using OnlinePractice.API.Models.Stripe;
using Stripe;

namespace OnlinePractice.API.Validator.Interfaces
{
    public interface IStripeAppService
    {
        Task<StripeCustomer> AddStripeCustomerAsync(AddStripeCustomer customer, CancellationToken ct);
        Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct);
        public  Task<Charge> CreateChargeAsync(decimal amount, string currency);

        public Task<PaymentIntent> ProcessPayment(string stripeToken, string stripeEmail);
    }
}
