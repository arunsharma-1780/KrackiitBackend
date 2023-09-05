namespace OnlinePractice.API.Models.Payment.Response
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class PaymentResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }
    public class CustomerDetails
    {
        public string customer_id { get; set; } = string.Empty;
        public string customer_name { get; set; } = string.Empty;
        public string customer_email { get; set; } = string.Empty;
        public string customer_phone { get; set; } = string.Empty;
    }

    public class OrderMeta
    {
        public object return_url { get; set; } = string.Empty;
        public string notify_url { get; set; } = string.Empty;
        public object payment_methods { get; set; } = string.Empty;
    }

    public class Payments
    {
        public string url { get; set; } = string.Empty;
    }

    public class Refunds
    {
        public string url { get; set; } = string.Empty;
    }

    public class OrderResponse
    {
        public int cf_order_id { get; set; }
        public DateTime created_at { get; set; }
        public CustomerDetails customer_details { get; set; } = new();
        public string entity { get; set; } = string.Empty;
        public double order_amount { get; set; }
        public string order_currency { get; set; } = string.Empty;
        public DateTime order_expiry_time { get; set; }
        public string order_id { get; set; } = string.Empty;
        public OrderMeta order_meta { get; set; } = new(); 
        public object order_note { get; set; } = string.Empty;
        public List<object> order_splits { get; set; } = new();
        public string order_status { get; set; } = string.Empty;
        public object order_tags { get; set; } = string.Empty;
        public string payment_session_id { get; set; } = string.Empty;
        public Payments payments { get; set; } = new();
        public Refunds refunds { get; set; } = new();
        public Settlements settlements { get; set; } = new();
        public object terminal_data { get; set; } = new();
    }

    public class Settlements
    {
        public string url { get; set; } = string.Empty;
    }


    public class OrderDetail
    {
        public bool OrderStatus { get; set; }
    }


}
