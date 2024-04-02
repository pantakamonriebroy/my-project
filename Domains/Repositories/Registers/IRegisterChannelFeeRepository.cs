using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterChannelFeeRepository : IRepository<RegisterChannelFee, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterChannelFee fee);

        Task<List<RegisterChannelFee>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);
        Task<List<RegisterChannelFee>> GetAllByRegisterChannelIdAsync(long registerChannelId);
    }
}
