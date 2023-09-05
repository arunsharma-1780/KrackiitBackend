using OnlinePractice.API.Models.Common;
using OnlinePractice.API.Models.Enum;
using System.ComponentModel;

namespace OnlinePractice.API.Models.Payment.Request
{
    public class ResData
    {
        public string encResp { get; set; } = string.Empty;
        public string orderNo { get; set; } = string.Empty;
    }
    public class PaymentRequest :CurrentUser
    {
        public double Amount { get; set; }

        [DefaultValue(false)]
        public bool IsMobile { get; set; }
        public ServerType ServerType { get; set; }
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
        public string notify_url { get; set; } = string.Empty;
        //public string return_url { get; set; } = string.Empty;

    }

    public class Order
    {
        public double order_amount { get; set; }
        public string order_id { get; set; } = string.Empty;
        public string order_currency { get; set; } = string.Empty;
        public CustomerDetails customer_details { get; set; } = new();
        public OrderMeta order_meta { get; set; } = new();
        public string order_note { get; set; } = string.Empty;
    }

    public class OderAmount :CurrentUser
    {
        public double Amount { get; set; }
       // public Mode Mode { get; set; }
    }

    public class PaymentStatus : CurrentUser
    {
        [DefaultValue(false)]
        public bool IsSuccess { get; set; } 
        public string OrderId { get; set; } = string.Empty;
    }

}
