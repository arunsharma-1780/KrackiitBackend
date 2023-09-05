using Microsoft.AspNetCore.Identity;
using OnlinePractice.API.Models.Stripe;
using OnlinePractice.API.Validator.Interfaces;
using Stripe;
using Stripe.FinancialConnections;

namespace OnlinePractice.API.Validator.Services
{
    public class StripeAppService : IStripeAppService
    {
        private readonly ChargeService _chargeService;
        private readonly CustomerService _customerService;
        private readonly TokenService _tokenService;

        public StripeAppService(
            ChargeService chargeService,
            CustomerService customerService,
            TokenService tokenService)
        {
            _chargeService = chargeService;
            _customerService = customerService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Create a new customer at Stripe through API using customer and card details from records.
        /// </summary>
        /// <param name="customer">Stripe Customer</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Stripe Customer</returns>
        //public Task<StripeCustomer> AddStripeCustomerAsync(AddStripeCustomer customer, CancellationToken ct)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Add a new payment at Stripe using Customer and Payment details.
        /// Customer has to exist at Stripe already.
        /// </summary>
        /// <param name="payment">Stripe Payment</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns><Stripe Payment/returns>
        //public Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct)
        //{
        //    throw new NotImplementedException();
        //}
        public async Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct)
        {
            // Set the options for the payment we would like to create at Stripe
            ChargeCreateOptions paymentOptions = new ChargeCreateOptions
            {
                Customer = payment.CustomerId,
                ReceiptEmail = payment.ReceiptEmail,
                Description = payment.Description,
                Currency = payment.Currency,
                Amount = payment.Amount
            };

            // Create the payment
            var createdPayment = await _chargeService.CreateAsync(paymentOptions, null, ct);


            // Return the payment to requesting method
            return new StripePayment()
            {

                CustomerId = createdPayment.CustomerId,
                ReceiptEmail = createdPayment.ReceiptEmail,
                Description = createdPayment.Description,
                Currency = createdPayment.Currency,
                Amount = createdPayment.Amount,
                PaymentId = createdPayment.Id
            };
        }

        public async Task<StripeCustomer> AddStripeCustomerAsync(AddStripeCustomer customer, CancellationToken ct)
        {
            // Set Stripe Token options based on customer data
            TokenCreateOptions tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = customer.Name,
                    Number = customer.CreditCard.CardNumber,
                    ExpYear = customer.CreditCard.ExpirationYear,
                    ExpMonth = customer.CreditCard.ExpirationMonth,
                    Cvc = customer.CreditCard.Cvc
                }
            };

            // Create new Stripe Token
            Token stripeToken = await _tokenService.CreateAsync(tokenOptions, null, ct);

            //var service = new StripeCustomerService();
            //StripeCustomer customer = service.Create(options);

            //var options = new CustomerCreateOptions
            //{
            //    Email = customer.Email,
            //    Name= customer.Name,                                                                                                                        
            //    Description = "New customer",
            //    Source = new CardCreateNestedOptions
            //    {                                                                                                                                                                           
            //            Number = "4242424242424242",
            //            ExpYear = 2025,
            //            ExpMonth = 1,
            //            Cvc = "123"                    
            //    }
            //};
            // Set Customer options using
            CustomerCreateOptions customerOptions = new CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
                Source = stripeToken.Id
            };

            // Create customer at Stripe
            Customer createdCustomer = await _customerService.CreateAsync(customerOptions, null, ct);

            // Return the created customer at stripe
            return new StripeCustomer()
            {
                Name = createdCustomer.Name,
                Email = createdCustomer.Email,
                CustomerId = createdCustomer.Id,
                token = stripeToken.Id
            };
        }

        public async Task<Charge> CreateChargeAsync(decimal amount, string currency)
        {
            TokenCreateOptions tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = "Christian Schou",
                    Number = "4242424242424242",
                    ExpYear = "2024",
                    ExpMonth = "12",
                    Cvc = "999"
                }
            };
            CancellationToken ct = CancellationToken.None;
            // Create new Stripe Token
            Token stripeToken = await _tokenService.CreateAsync(tokenOptions, null, ct);
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = (int)(amount * 100),
                Currency = currency,
                Source = stripeToken.Id
            };
            var service = new ChargeService();
            var charge = await service.CreateAsync(chargeOptions);
            return charge;
        }

        public async Task<PaymentIntent> ProcessPayment(string stripeToken, string stripeEmail)
        {

            StripeConfiguration.ApiKey = "sk_test_51MqY6BSEup9pVxFetPrW8ibaqnJ0gwWBAv2YstfPA4gXUTefJIftCBxPtv7IIMHmftaun66l5ZowIvHpCNY1hDGF00Jn2JMDtU";

            var options = new PaymentIntentCreateOptions
            {
                Amount = 1099,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };
            var service = new PaymentIntentService();
          var x=  service.Create(options);
            //TokenCreateOptions tokenOptions = new TokenCreateOptions
            //{
            //    Card = new TokenCardOptions
            //    {
            //        Name = "Christian Schou",
            //        Number = "4242424242424242",
            //        ExpYear = "2024",
            //        ExpMonth = "12",
            //        Cvc = "999"
            //    }
            //};
            //CancellationToken ct = CancellationToken.None;
            //Token stripeTokens = await _tokenService.CreateAsync(tokenOptions, null, ct);
            //Dictionary<string, string> Metadata =  new Dictionary<string, string>();
            //Metadata.Add("Product", "RubberDuck");
            //Metadata.Add("Quantity", "10");
            //var options = new ChargeCreateOptions
            //{
            //    Amount = 200,
            //    Currency = "INR",
            //    Description = "Buying 10 rubber ducks",
            //    Source = stripeTokens.Id,
            //    ReceiptEmail = stripeEmail,
            //    Metadata = Metadata
            //};
            //var service = new ChargeService();
            //Charge charge =  service.Create(options);
            return null;
        }
    }
}
