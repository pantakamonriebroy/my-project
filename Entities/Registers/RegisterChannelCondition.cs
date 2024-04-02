using ChillPay.Merchant.Register.Api.Models;

namespace ChillPay.Merchant.Register.Api.Entities.Registers
{
    public class RegisterChannelCondition
    {
        public long Id { get; set; }
        public long RegisterMerchantId { get; set; }
        public long RegisterChannelId { get; set; }
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
        public DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public byte AddedBySystem { get; set; }
    }
}
