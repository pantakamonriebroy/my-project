using ChillPay.Merchant.Register.Api.Entities.Generics;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Generics
{
    public interface IGenericRepository : IDisposable
    {
        //Task<IdentityResult> CreateOrUpdateContactAsync(Contact contact);
        List<Province> GetAllProvince();
        Task<List<Province>> GetAllProvinceAsync();
        Task<List<District>> GetAllDistrictByProvinceIdAsync(int provinceId);
        Task<List<Subdistrict>> GetAllSubdistrictByDistrictIdAsync(int districtId);
        //add
        Task<List<Subdistrict>> GetAllSubdistrictByID(int id);
        void Dispose(bool disposing);
    }
}
