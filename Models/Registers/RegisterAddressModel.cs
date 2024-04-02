namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterAddressModel
    {
        public string Address { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int SubdistrictId { get; set; }
        public string ZipCode { get; set; }
        public string TelNo { get; set; }
        public string Fax { get; set; }
    }
}
