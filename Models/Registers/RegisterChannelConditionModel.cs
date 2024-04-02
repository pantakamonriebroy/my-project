namespace ChillPay.Merchant.Register.Api.Models.Registers
{
    public class RegisterChannelConditionModel
    {
        public byte ConditionType { get; set; }
        public long ValueLong1 { get; set; }
        public long ValueLong2 { get; set; }
        public long ValueLong3 { get; set; }
        public long ValueLong4 { get; set; }
        public long ValueDec1 { get; set; }
        public long ValueDec2 { get; set; }
        public long ValueDec3 { get; set; }
        public long ValueDec4 { get; set; }
        public string Description { get; set; }
        public ApiResponseStatus Status { get; set; }
    }
}
