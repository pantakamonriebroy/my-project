namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterContact
    {
        public long Id { get; set; }
        public long RegisterMerchantId { get; set; }
        public byte ContactType { get; set; }
        public string CorperateName { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string TelNo { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string LineId { get; set; }

        //add
        public DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public byte AddedBySystem { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public byte? ModifiedBySystem { get; set; }
    }
}
