namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterWebInfoModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RegisterMerchantId { get; set; }
        public byte RegisterState { get; set; }
        //public bool IsRegistered { get; set; }
        //public bool IsApproved { get; set; }

        public RegisterContactModel DevContact { get; set; }
        public string? MerchantServerPrdIPAddress { get; set; }
        public string? MerchanrServerPrdUrl { get; set; }
        public string? UrlBackgroundPrd { get; set; }
        public string? MerchantServerSandIPAddress { get; set; }
        public string? MerchantServerSandUrl { get; set; }
        public string? UrlBackgroundSand { get; set; }
        public bool? UseUrlBackground { get; set; }
        public bool? UseSSL { get; set; }
        public string SSLType { get; set; }
        public string SSLBy { get; set; }
        public DateTime? SSLCreateDate { get; set; }
        public DateTime? SSLExpireDate { get; set; }
        public byte? SSLLocationType { get; set; }
    }
}
