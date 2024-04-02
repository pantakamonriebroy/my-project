namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterChannelInfoModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RegisterMerchantId { get; set; }
        public byte RegisterState { get; set; }
        //public bool IsRegistered { get; set; }
        //public bool IsApproved { get; set; }

        public RegisterChannelModel[] Channels { get; set; }
        public RegisterInstallmentModel[] Installments { get; set; }

        //public decimal? EntranceFeeBase { get; set; }
        //public decimal? EntranceFee { get; set; }
        //public decimal? MonthlyFeeBase { get; set; }
        //public decimal? MonthlyFee { get; set; }
        //public decimal? MerchantTransferFeeBase { get; set; }
        //public decimal? MerchantTransferFee { get; set; }
        //public string AdditionNote { get; set; }
    }
}
