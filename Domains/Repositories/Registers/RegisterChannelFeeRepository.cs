using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterChannelFeeRepository : Repository<RegisterChannelFee, long>, IRegisterChannelFeeRepository
    {
        private readonly ILogger _logger;

        internal RegisterChannelFeeRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RegisterChannelFee fee)
        {
            try
            {
                if (fee.Id > 0)
                {
                    await UpdateAsync(fee);
                }
                else
                {
                    await AddAsync(fee);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<List<RegisterChannelFee>> GetAllByRegisterMerchantIdAsync(long registerMerchantId)
        {
            return await Set.Where(m => m.RegisterMerchantId == registerMerchantId).OrderBy(m => m.Id).ToListAsync();
        }
        public async Task<List<RegisterChannelFee>> GetAllByRegisterChannelIdAsync(long registerChannelId)
        {
            return await Set.Where(m => m.RegisterChannelId == registerChannelId).OrderBy(m => m.Id).ToListAsync();
        }
    }
}
