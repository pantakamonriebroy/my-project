namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterInstallmentModel
    {
        public long Id { get; set; }
        //public long RegisterMerchantId { get; set; }
        public long ChannelServiceCode { get; set; }
        public long RequireChannelServiceCode { get; set; }
        public string ChannelName { get; set; }
        public bool IsSelected { get; set; }
        public long RouteNo { get; set; }
    }
}
