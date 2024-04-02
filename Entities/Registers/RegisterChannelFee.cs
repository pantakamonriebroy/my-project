namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterChannelFee
    {
        public long Id { get; set; }
        public long RegisterMerchantId { get; set; }
        public long RegisterChannelId { get; set; }
        public string FeeAmount { get; set; }
        public string FeeMinAmount { get; set; }
        public byte FeeOfferMode { get; set; }
        public byte FeeType { get; set; }
        public string FeeMaxChargePrice { get; set; }
        public string PaymentMinPrice { get; set; }
        public string PaymentMaxPrice { get; set; }
        public long SortNo { get; set; }
        public long CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public byte AddedBySystem { get; set; }

    }
}
