using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterChannelRepository : Repository<RegisterChannel, long>, IRegisterChannelRepository
    {
        private readonly ILogger _logger;

        internal RegisterChannelRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RegisterChannel channel)
        {
            try
            {
                if (channel.Id > 0)
                {
                    await UpdateAsync(channel);
                }
                else
                {
                    await AddAsync(channel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<List<RegisterChannel>> GetAllByRegisterMerchantIdAsync(long registerMerchantId)
        {
            return await Set.Where(m => m.RegisterMerchantId == registerMerchantId).OrderBy(m => m.Id).ToListAsync();
        }
        //public async Task<List<RegisterChannel>> GetAllActiveByRegisterMerchantIdAsync(long registerMerchantId)
        //{
        //    return await Set.Where(m => m.RegisterMerchantId == registerMerchantId && m.Status == (byte)GenericConstant.DefaultStatus.Active).ToListAsync();
        //}

        //public async Task<List<RegisterChannelGroup>> GetAllChannelGroupByRegisterMerchantIdAsync(long registerMerchantId)
        //{
        //    return await Context.RegisterChannelGroups.Where(m => m.RegisterMerchantId == registerMerchantId).ToListAsync();
        //}
        //public async Task<List<RegisterChannelGroup>> GetAllActiveChannelGroupByRegisterMerchantIdAsync(long registerMerchantId)
        //{
        //    return await Context.RegisterChannelGroups.Where(m => m.RegisterMerchantId == registerMerchantId && m.Status == (byte)GenericConstant.DefaultStatus.Active).ToListAsync();
        //}
    }
}
