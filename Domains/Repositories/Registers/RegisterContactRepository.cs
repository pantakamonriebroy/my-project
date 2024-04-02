using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterContactRepository : Repository<RegisterContact, long>, IRegisterContactRepository
    {
        private readonly ILogger _logger;

        internal RegisterContactRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RegisterContact contact)
        {
            try
            {
                if (contact.Id > 0)
                {
                    await UpdateAsync(contact);
                }
                else
                {
                    await AddAsync(contact);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        //public async Task<List<RegisterContact>> GetAllByRegisterMerchantIdAsync(long registerMerchantId)
        //{
        //    var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId).OrderBy(m => m.Id);
        //    return await query.ToListAsync();
        //}

        public async Task<RegisterContact> GetLatestByContactTypeAsync(long registerMerchantId, byte contactType)
        {
            Expression<Func<RegisterContact, object>> expr = p => p.ModifiedDate.HasValue ? p.ModifiedDate.Value : p.AddedDate;
            var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId && m.ContactType == contactType).OrderByDescending(expr);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<List<RegisterContact>> GetAllByContactTypeAsync(long registerMerchantId, byte contactType)
        {
            var query = Set.Where(m => m.RegisterMerchantId == registerMerchantId && m.ContactType == contactType).OrderBy(m => m.Id);
            return await query.ToListAsync();
        }
    }
}
