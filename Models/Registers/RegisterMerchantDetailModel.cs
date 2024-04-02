namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterMerchantDetailModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RegisterMerchantId { get; set; }
        public byte RegisterState { get; set; }
        //public bool IsRegistered { get; set; }
        //public bool IsApproved { get; set; }

        public byte? RegisterType { get; set; }
        public string RegisterNameEN { get; set; }
        public string RegisterNameTH { get; set; }
        //public string RegisterFirstnameEN { get; set; }
        //public string RegisterLastnameEN { get; set; }
        //public string RegisterFirstnameTH { get; set; }
        //public string RegisterLastnameTH { get; set; }
        public string BrandNameEN { get; set; }
        public string BrandNameTH { get; set; }
        //public string LogoImageUrl { get; set; }
        public string TaxId { get; set; }
        public bool? UseRegisterAddressForBilling { get; set; }
        public RegisterAddressModel RegisterAddress { get; set; }
        public RegisterAddressModel BillingAddress { get; set; }

        public string DomainName { get; set; }
        public string ProductType { get; set; }
        public decimal? ProductMinPrice { get; set; }
        public decimal? ProductMaxPrice { get; set; }
        public decimal? EstimateSales { get; set; }
        public int? BankAccountBrandId { get; set; }
        public string BankAccountBranch { get; set; }
        public int? BankAccountTypeId { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNo { get; set; }

        public RegisterContactModel[] Contacts { get; set; }

        public RegisterContactModel DevContact { get; set; }
        public string MerchantServerPrdIPAddress { get; set; }
        public string MerchanrServerPrdUrl { get; set; }
        public string UrlBackgroundPrd { get; set; }
        public string MerchantServerSandIPAddress { get; set; }
        public string MerchantServerSandUrl { get; set; }
        public string UrlBackgroundSand { get; set; }
        public bool? UseUrlBackground { get; set; }
        public bool? UseSSL { get; set; }
        public string SSLType { get; set; }
        public string SSLBy { get; set; }
        public DateTime? SSLCreateDate { get; set; }
        public DateTime? SSLExpireDate { get; set; }
        public byte? SSLLocationType { get; set; }

        public RegisterChannelModel[] Channels { get; set; }
        public RegisterInstallmentModel[] Installments { get; set; }

        //public decimal? EntranceFeeBase { get; set; }
        public decimal? EntranceFee { get; set; }
        //public decimal? MonthlyFeeBase { get; set; }
        public decimal? MonthlyFee { get; set; }
        //public decimal? MerchantTransferFeeBase { get; set; }
        public decimal? MerchantTransferFee { get; set; }
        public string AdditionNote { get; set; }
    }
}
