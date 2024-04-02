namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterChannelModel
    {
        public long Id { get; set; }
        //public long RegisterMerchantId { get; set; }
        public long ChannelServiceCode { get; set; }
        public string GroupName { get; set; }
        public string ChannelName { get; set; }
        public bool IsSelected { get; set; }
        public long RouteNo { get; set; }
        //public RegisterChannelConditionModel[] Conditions { get; set; }
        //public RegisterChannelFeeModel[] Fees { get; set; }
        //public RegisterChannelFeeModel[] BaseFees { get; set; }
        //public RegisterChannelServiceFeeModel[] ServiceFees { get; set; }
        //public RegisterChannelServiceFeeModel[] BaseServiceFees { get; set; }
    }
}
