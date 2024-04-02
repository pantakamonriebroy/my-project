using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterInstallmentRepository : Repository<RegisterInstallment, long>, IRegisterInstallmentRepository
    {
        private readonly ILogger _logger;

        internal RegisterInstallmentRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RegisterInstallment ins)
        {
            try
            {
                if (ins.Id > 0)
                {
                    await UpdateAsync(ins);
                }
                else
                {
                    await AddAsync(ins);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<List<RegisterInstallment>> GetAllByRegisterMerchantIdAsync(long registerMerchantId)
        {
            return await Set.Where(m => m.RegisterMerchantId == registerMerchantId).OrderBy(m => m.Id).ToListAsync();
        }
    }
}
