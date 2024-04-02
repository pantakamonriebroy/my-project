using ChillPay.Merchant.Register.Api.Models.Registers;

namespace ChillPay.Merchant.Register.Api.Configs
{
    public class AppSettings
    {
        public string CHILLPAY_TOKEN { get; set; }
        public RegisterChannelModel[] RegisterChannelIntialData { get; set; }
        public RegisterInstallmentModel[] RegisterInstallmentInitialData { get; set; }
    }
}
