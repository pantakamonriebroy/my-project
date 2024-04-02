namespace ChillPay.Merchant.Register.Api.Models.Generics
{
    public class JsonSubdistrictModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ZipCode { get; set; }
        public int DistrictId { get; set; }

    }
}
