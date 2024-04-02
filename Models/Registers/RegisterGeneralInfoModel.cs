using ChillPay.Merchant.Register.Api.Models.Registers;
using System.ComponentModel.DataAnnotations;

namespace ChillPay.Merchant.Register.Api.Models.MerchantRegisters
{
    public class RegisterGeneralInfoModel
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RegisterMerchantId { get; set; }
        public byte? RegisterType { get; set; }
        public string RegisterNameEN { get; set; }
        public string RegisterNameTH { get; set; }
        public string BrandNameEN { get; set; }
        public string BrandNameTH { get; set; }
        public string TaxId { get; set; }
        public bool? UseRegisterAddressForBilling { get; set; }
        public RegisterAddressModel RegisterAddress { get; set; }
        public RegisterAddressModel BillingAddress { get; set; }
        public byte RegisterState { get; set; }
        
    }
}
