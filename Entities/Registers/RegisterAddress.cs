using System.ComponentModel.DataAnnotations;

namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterAddress
    {
        [Key]
        public long Id { get; set; }
        public long RegisterMerchantId { get; set; }
        public byte AddressType { get; set; }
        public string Address { get; set; }
        public int ProvinceId { get; set; }
        public int DistrictId { get; set; }
        public int SubdistrictId { get; set; }
        public string ZipCode { get; set; }
        public string TelNo { get; set; }
        public string Fax { get; set; }
        public DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public byte AddedBySystem { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public byte? ModifiedBySystem { get; set; }
    }
}
