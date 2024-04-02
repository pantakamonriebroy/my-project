namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterInstallment
    {
        public long Id { get; set; }
        public long RegisterMerchantId { get; set; }
        public long ChannelServiceCode { get; set; }
        public long RequireChannelServiceCode { get; set; }
        public string ChannelName { get; set; }
        public bool IsSelected { get; set; }
        public long RouteNo { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public byte? ModifiedBySystem { get; set; }
        public DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public byte AddedBySystem { get; set; }
    }
}
