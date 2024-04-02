using ChillPay.Merchant.Register.Api.Entities.Registers;

namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterDetailInfoModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public byte RegisterState { get; set; }
        public long RegisterMerchantId { get; set; }
        //public bool IsRegistered { get; set; }
        //public bool IsApproved { get; set; }

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

    }
}
