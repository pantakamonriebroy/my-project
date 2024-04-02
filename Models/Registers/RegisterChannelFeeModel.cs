namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterChannelFeeModel
    {
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
        public DateTime UpdateDate { get; set; }
    }
}
