namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterChannelBaseServiceFee
    {
        public long Id { get; set; }
        public long RegisterMerchantId { get; set; }
        public long RegisterChannelId { get; set; }
        public byte ServiceRateType { get; set; }
        public string ServiceRateAmount { get; set; }
        public string ServiceRateVat { get; set; }
        public string ServiceRateInVat { get; set; }
        public string MinTxAmount { get; set; }
        public string MaxTxAmount { get; set; }
        public string ServiceWHTRate { get; set; }
        public string ServiceMinAmount { get; set; }
        public long ServiceConditionBy { get; set; }
        public long SortNo { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
