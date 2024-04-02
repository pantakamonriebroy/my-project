using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterStateLogRepository : Repository<RegisterStateLog, long>, IRegisterStateLogRepository
    {
        private readonly ILogger _logger;

        internal RegisterStateLogRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RegisterStateLog log)
        {
            try
            {
                if (log.Id > 0)
                {
                    await UpdateAsync(log);
                }
                else
                {
                    await AddAsync(log);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<List<RegisterStateLog>> GetAllByRegisterMerchantIdAsync(long registerMerchantId)
        {
            var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId).OrderBy(m => m.AddedDate);
            return await query.ToListAsync();
        }
    }
}
