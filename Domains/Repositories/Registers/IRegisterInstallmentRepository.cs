using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterInstallmentRepository : IRepository<RegisterInstallment, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterInstallment ins);

        Task<List<RegisterInstallment>> GetAllByRegisterMerchantIdAsync(long registerMerchantId);
    }
}
