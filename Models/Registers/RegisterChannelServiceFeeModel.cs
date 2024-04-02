namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterChannelServiceFeeModel
    {
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
        public DateTime UpdateDate { get; set; }
    }
}
