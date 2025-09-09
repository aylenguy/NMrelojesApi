// Request: lo que recibe el endpoint
namespace Application.Model.Request
{
    public class ApplyCouponRequest
    {
        public string Code { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}

// Response: lo que devuelve al frontend
namespace Application.Model.Response
{
    public class ApplyCouponResponse
    {
        public decimal Discount { get; set; }
        public decimal NewTotal { get; set; }
        public string CouponCode { get; set; } = string.Empty;
    }
}
