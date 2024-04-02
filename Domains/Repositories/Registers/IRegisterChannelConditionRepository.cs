using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterChannelConditionRepository : IRepository<RegisterChannelCondition, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterChannelCondition condition);

        Task<List<RegisterChannelCondition>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);
        Task<List<RegisterChannelCondition>> GetAllByRegisterChannelIdAsync(long registerChannelId);
    }
}
