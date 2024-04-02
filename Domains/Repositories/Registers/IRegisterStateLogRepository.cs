using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterStateLogRepository : IRepository<RegisterStateLog, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterStateLog log);

        Task<List<RegisterStateLog>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);
    }
}
