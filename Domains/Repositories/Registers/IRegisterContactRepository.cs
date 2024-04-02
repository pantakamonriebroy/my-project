using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterContactRepository : IRepository<RegisterContact, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterContact contact);

        //Task<List<RegisterContact>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);

        Task<RegisterContact> GetLatestByContactTypeAsync(long registerMerchantId, byte contactType);
        Task<List<RegisterContact>> GetAllByContactTypeAsync(long registerMerchantId, byte contactType);
    }
}
