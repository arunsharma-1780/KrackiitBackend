using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Stripe;
using OnlinePractice.API.Validator.Interfaces;
using Stripe;
using Stripe.Checkout;
using Stripe.FinancialConnections;
using Stripe.Issuing;
using System.Text.Json.Serialization;
using CardCreateOptions = Stripe.CardCreateOptions;

namespace OnlinePractice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     [ApiExplorerSettings(IgnoreApi = true)]
    public class StripeController : ControllerBase
    {
        private readonly IStripeAppService _stripeService;
        private readonly IConfiguration _configuration;


        public StripeController(IStripeAppService stripeService, IConfiguration configuration)
        {
            _stripeService = stripeService;
            _configuration = configuration;
        }
        //public StripeController(IOptions<StripeSettings> stripeSettings)
        //{
        //    _stripeSettings = stripeSettings.Value;
        //}

        //[HttpPost("create-checkout-session")]
        //public ActionResult CreateCheckoutSession()
        //{
        //    var options = new SessionCreateOptions
        //    {
        //        LineItems = new List<SessionLineItemOptions>
        //{
        //  new SessionLineItemOptions
        //  {
        //    PriceData = new SessionLineItemPriceDataOptions
        //    {
        //      UnitAmount = 2000,
        //      Currency = "usd",
        //      ProductData = new SessionLineItemPriceDataProductDataOptions
        //      {
        //        Name = "T-shirt",
        //      },
        //    },
        //    Quantity = 1,
        //  },
        //},
        //        Mode = "payment",
        //        SuccessUrl = "http://localhost:4242/success",
        //        CancelUrl = "http://localhost:4242/cancel",
        //    };

        //    var service = new SessionService();
        //    Session session = service.Create(options);

        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //}
        [HttpPost]
        [Route("create-payment-intent")]
        public string Create(PaymentIntentCreateRequest request)
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = 500,
                Currency = "inr",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },

            };
            var service = new PaymentIntentService();
            var x = service.Create(options);
            return x.Id;
        }
        [HttpPost]
        [Route("create-payment-intent1")]
        public ActionResult Create1(string id)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 500,
                Currency = "inr",
                PaymentMethod = "pm_card_visa",
            };
            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);
            // Return the client_secret value to the client-side code
            var clientSecret = paymentIntent.ClientSecret;
            // In your client-side code, confirm the payment intent
            var options1 = new PaymentIntentConfirmOptions
            {
                PaymentMethod = "pm_card_visa",

            };
            var service1 = new PaymentIntentService();
            var paymentIntent1 = service1.Confirm(paymentIntent.Id, options1);
            // Check if the payment is successful
            if (paymentIntent1.Status == "succeeded")
            {
                // Update your records and confirm the payment
                var paymentIntentId = paymentIntent.Id;
                // your code here
            }
            return Ok(paymentIntent);
        }


        //[HttpPost]
        //[Route("create-payment-intent")]
        //public string Create(PaymentIntentCreateRequest request)
        //{
        //    var paymentIntentService = new PaymentIntentService();
        //    var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
        //    {
        //        Amount = CalculateOrderAmount(request.Items),
        //        Currency = "inr",
        //        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
        //        {
        //            Enabled = true,
        //        },
        //    });

        //    return paymentIntent.ClientSecret;
        //}
        private int CalculateOrderAmount(Item[] items)
        {
            // Replace this constant with a calculation of the order's amount
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            return 1400;
        }

        public class Item
        {
            [JsonProperty("id")]
            public string Id { get; set; }
        }

        public class PaymentIntentCreateRequest
        {
            [JsonProperty("items")]
            public Item[] Items { get; set; }
        }

        [HttpPost("customer/add")]
        public async Task<ActionResult<StripeCustomer>> AddStripeCustomer(
            [FromBody] AddStripeCustomer customer,
            CancellationToken ct)
        {
            StripeCustomer createdCustomer = await _stripeService.AddStripeCustomerAsync(
                customer,
                ct);

            return StatusCode(StatusCodes.Status200OK, createdCustomer);
        }

        [HttpPost("payment/add")]
        public async Task<ActionResult<StripePayment>> AddStripePayment(
            [FromBody] AddStripePayment payment,
            CancellationToken ct)
        {
            StripePayment createdPayment = await _stripeService.AddStripePaymentAsync(
                payment,
                ct);

            return StatusCode(StatusCodes.Status200OK, createdPayment);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayments(decimal amount, string currency)
        {
            //  var paymentService = new _stripeService.pa.(Configuration);
            var charge = await _stripeService.CreateChargeAsync(amount, currency);
            if (charge.Paid)
            {
                // Payment successful
                return Ok();
            }
            else
            {
                // Payment failed
                return BadRequest(charge.FailureMessage);
            }
        }


        [HttpPost]
        [Route("pay")]
        public async Task<IActionResult> Processing(string stripeToken, string stripeEmail)
        {
            var charge = await _stripeService.ProcessPayment(stripeToken, stripeEmail);
            if (charge.Id != null)
            {
                // Payment successful
                return Ok();
            }
            else
            {
                // Payment failed
                return BadRequest(charge.PaymentMethod);
            }
        }

        private readonly StripeSettings _stripeSettings;




        [HttpPost]
        [Route("pay2")]
        public IActionResult Charge(string stripeEmail, string stripeToken1)
        {
            var customers = new CustomerService();
            var options = new TokenCreateOptions
            {
                Card = new TokenCardOptions()
                {

                    Number = "4242424242424242",
                    ExpYear = "2023",
                    ExpMonth = "12",
                    Cvc = "123",
                    Name = "John Doe",
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Apt 4",
                    AddressCity = "Anytown",
                    AddressState = "CA",
                    AddressZip = "12345",
                    AddressCountry = "US",
                },
            };

            var service1 = new TokenService();
            Token stripeToken = service1.Create(options);

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken.Id
            });
            var options2 = new PaymentIntentCreateOptions
            {
                Customer = customer.Id,
                Amount = 500,
                Currency = "inr",
                PaymentMethod = "pm_card_visa",
            };
            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options2);
            // Return the client_secret value to the client-side code
            var clientSecret = paymentIntent.ClientSecret;
            // In your client-side code, confirm the payment intent
            var options1 = new PaymentIntentConfirmOptions
            {
                ClientSecret = clientSecret,
                PaymentMethod = "pm_card_visa",

            };
            //   var uri = new Uri(_configuration.GetValue<string>("StripeSettings:SecretKey"));

            var service3 = new PaymentIntentService();
            //var servic = new PaymentIntentService();
            //service.Get("pi_1GszSj2eZvKYlo2CtS2raUvO");
            var E = new RequestOptions
            {
                ApiKey = "sk_test_51MqY6BSEup9pVxFetPrW8ibaqnJ0gwWBAv2YstfPA4gXUTefJIftCBxPtv7IIMHmftaun66l5ZowIvHpCNY1hDGF00Jn2JMDtU"
            };
            var paymentIntent1 = service3.Confirm(paymentIntent.Id, options1, E);
            // HttpMethod method, String path, BaseOptions options, RequestOptions requestOptions, CancellationToken cancellationToken)


            //            var paymentIntent1 = service.Confirm(
            //  paymentIntent.Id,
            //  new PaymentIntentConfirmOptions
            //  {
            //      PaymentMethod = paymentIntent.PaymentMethodId,
            //      ClientSecret = paymentIntent.ClientSecret
            //  }
            //);
            // Check if the payment is successful
            if (paymentIntent1.Status == "succeeded")
            {
                // Update your records and confirm the payment
                var paymentIntentId = paymentIntent.Id;
                // your code here
            }
            return Ok(paymentIntent1);

            //var charge = charges.Create(new ChargeCreateOptions
            //{
            //    Amount = 500,
            //    Description = "Sample Charge",
            //    Currency = "usd",
            //    Customer = customer.Id,
            //    ReceiptEmail = stripeEmail
            //});


        }

        [HttpPost]
        [Route("createpaymentintent")]
        public async Task<IActionResult> CreatePayment()
        {
            StripeConfiguration.ApiKey = "sk_test_51MqY6BSEup9pVxFetPrW8ibaqnJ0gwWBAv2YstfPA4gXUTefJIftCBxPtv7IIMHmftaun66l5ZowIvHpCNY1hDGF00Jn2JMDtU";

            var options = new PaymentIntentCreateOptions
            {
                ReceiptEmail = "yoourmail@gmail.com",
                Customer = "cus_NiA8ZPYqoTmR4x",
                Amount = 2000,
                Currency = "inr",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var service = new PaymentIntentService();
            var result = service.Create(options);
            return Ok(result);
        }

        [HttpPost]
        [Route("retrivepaymentintent")]
        public async Task<IActionResult> Retrivepaymentintent()
        {
            StripeConfiguration.ApiKey = "sk_test_51MqY6BSEup9pVxFetPrW8ibaqnJ0gwWBAv2YstfPA4gXUTefJIftCBxPtv7IIMHmftaun66l5ZowIvHpCNY1hDGF00Jn2JMDtU";


            var service = new PaymentIntentService();
            var result = service.Get("pi_1GszTl2eZvKYlo2CEWegepx2");
            return Ok(result);
        }
        [HttpPost]
        [Route("updatepaymentintent")]
        public async Task<IActionResult> Updatepaymentintent()
        {
            StripeConfiguration.ApiKey = "sk_test_51MqY6BSEup9pVxFetPrW8ibaqnJ0gwWBAv2YstfPA4gXUTefJIftCBxPtv7IIMHmftaun66l5ZowIvHpCNY1hDGF00Jn2JMDtU";

            var options = new PaymentIntentUpdateOptions
            {
                Metadata = new Dictionary<string, string>
  {
    { "order_id", "6735" },
  },
            };
            var service = new PaymentIntentService();
            var result = service.Update(
               "pi_1GszTl2eZvKYlo2CEWegepx2",
               options);
            return Ok(result);
        }

        [HttpPost]
        [Route("confirmpayment")]
        public async Task<IActionResult> ConfirPaymentIntent(string id,string clientSecret)
        {
            StripeConfiguration.ApiKey = "sk_test_51MqY6BSEup9pVxFetPrW8ibaqnJ0gwWBAv2YstfPA4gXUTefJIftCBxPtv7IIMHmftaun66l5ZowIvHpCNY1hDGF00Jn2JMDtU";

            // To create a PaymentIntent for confirmation, see our guide at: https://stripe.com/docs/payments/payment-intents/creating-payment-intents#creating-for-automatic
            var options = new PaymentIntentConfirmOptions
            {
                
                PaymentMethod = "pm_card_visa",
               //ClientSecret= clientSecret,
                ReturnUrl= "https://onlinepractice-omega.vercel.app/mocktest"
            };
            var service = new PaymentIntentService();
         var result =   service.Confirm(
              id,
              options);
            return Ok(result);
        }




    }

}
