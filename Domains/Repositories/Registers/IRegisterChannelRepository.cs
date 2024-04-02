using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterChannelRepository : IRepository<RegisterChannel, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterChannel channel);

        Task<List<RegisterChannel>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);
    }
}
