namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterBank
    {
        public long Id { get; set; }
        public string NameEN { get; set; }
        public string NameTH { get; set; }
        public string Code { get; set; }
        public byte Status { get; set; }
    }
}
