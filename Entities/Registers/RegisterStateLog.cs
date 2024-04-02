namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterStateLog
    {
        public long Id { get; set; }
        public long RegisterMerchantId { get; set; }
        public DateTime AddedDate { get; set; }
        public string OldState { get; set; }
        public byte OldStateValue { get; set; }
        public string NewState { get; set; }
        public byte NewStateValue { get; set; }
        public string StateDescription { get; set; }
        public long AddedBy { get; set; }
        public byte AddedBySystem { get; set; }

    }
}
