namespace ChillPay.Merchant.Register.Api.Entities.Generics
{
    public class District
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string NameInThai { get; set; }
        public string NameInEnglish { get; set; }
        public int ProvinceId { get; set; }

    }
}
