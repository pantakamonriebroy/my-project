using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterAddressRepository : IRepository<RegisterAddress, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterAddress address);

        //    Task<List<RegisterAddress>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);

        Task<RegisterAddress> GetLatestByAddressTypeAsync(long registerMerchantId, byte addressType);
        //    Task<List<RegisterAddress>> GetAllByAddressTypeAsync(long registerMerchantId, byte addressType);

        //Task<RegisterAddress2Model> GetLatestByAddressTypeAsync2(long registerMerchantId, byte addressType);
        //}
    }
}
