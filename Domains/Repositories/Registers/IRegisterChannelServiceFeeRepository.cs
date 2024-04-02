using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterChannelServiceFeeRepository : IRepository<RegisterChannelServiceFee, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterChannelServiceFee serviceFee);

        Task<List<RegisterChannelServiceFee>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);
        Task<List<RegisterChannelServiceFee>> GetAllByRegisterChannelIdAsync(long registerChannelId);
    }
}
