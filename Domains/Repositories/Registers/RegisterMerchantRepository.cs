using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using ChillPay.Merchant.Register.Api.Validate;
using ChillPay.Merchant.Register.Api.Validator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterMerchantRepository : Repository<RegisterMerchant, long>, IRegisterMerchantRepository
    //internal class RegisterMerchantRepository : IRegisterMerchantRepository
    {
        private readonly ILogger _logger;
        private IIdentityValidator<RegisterMerchant> _registerMerchantValidator;

        internal RegisterMerchantRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            _registerMerchantValidator = new RegisterMerchantValidator(this);
            _logger = logger;
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RegisterMerchant regMerchant)
        {
            var result = await _registerMerchantValidator.ValidateAsync(regMerchant);
            if (!result.Succeeded)
            {
                return result;
            }

            try
            {
                if (regMerchant.Id > 0)
                {
                    await UpdateAsync(regMerchant);
                }
                else
                {
                    await AddAsync(regMerchant);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
            }

            return IdentityResult.Success;
        }

        public async Task<RegisterMerchant> FindByUserIdAsync(long userId)
        {
            return await Set.FirstOrDefaultAsync(x => x.UserId.Equals(userId));
        }

        //public async Task<long> CountByKeywordAsync(string keyword, DateTime? tnxDate1, DateTime? tnxDate2, byte state1, byte state2)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword) || keyword.Equals("all"))
        //    {
        //        keyword = string.Empty;
        //    }

        //    var query = Set.Where(m => (m.RegisterState >= state1 && m.RegisterState <= state2));

        //    if (tnxDate1 != null)
        //    {
        //        query = query.Where(m => m.RegisterRequestDate.HasValue && m.RegisterRequestDate.Value >= tnxDate1);
        //    }
        //    if (tnxDate2 != null)
        //    {
        //        query = query.Where(m => m.RegisterRequestDate.HasValue && m.RegisterRequestDate.Value < tnxDate2);
        //    }

        //    query = query.Where(GetSearchFunc(keyword));
        //    return await query.LongCountAsync();
        //}
        //public async Task<List<RegisterMerchantSearchModel>> FindByKeywordAsync(string keyword, DateTime? tnxDate1, DateTime? tnxDate2, byte state1, byte state2, string orderBy, string orderDir, int start, int pageSize)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword) || keyword.Equals("all"))
        //    {
        //        keyword = string.Empty;
        //    }

        //    var query = Set.Where(m => (m.RegisterState >= state1 && m.RegisterState <= state2));

        //    if (tnxDate1 != null)
        //    {
        //        query = query.Where(m => m.RegisterRequestDate.HasValue && m.RegisterRequestDate.Value >= tnxDate1);
        //    }
        //    if (tnxDate2 != null)
        //    {
        //        query = query.Where(m => m.RegisterRequestDate.HasValue && m.RegisterRequestDate.Value < tnxDate2);
        //    }

        //    query = query.Where(GetSearchFunc(keyword));

        //    query = query.OrderBy(string.Format("{0} {1}", orderBy, orderDir))
        //        .Skip(start).Take(pageSize);

        //    var query2 = query.GroupJoin(Context.RegisterContacts,
        //        rm => new { rm.Id, ContactType = (byte)RegisterConstant.ContactType.Normal },
        //        rc => new { Id = rc.RegisterMerchantId, rc.ContactType },
        //        (rm, contactsGroup) => new
        //        {
        //            Merchant = rm,
        //            Contacts = contactsGroup.OrderBy(c => c.Id),
        //        });
        //    var query3 = query2.GroupJoin(Context.RegisterChannels,
        //        g => g.Merchant.Id,
        //        rc => rc.RegisterMerchantId,
        //        (g, channelsGroup) => new RegisterMerchantSearchModel
        //        {
        //            Merchant = g.Merchant,
        //            Contacts = g.Contacts,
        //            Channels = channelsGroup.OrderBy(c => c.Id),
        //        });

        //    return await query3.ToListAsync();
        //}
        //public async Task<List<RegisterMerchantSearch2Model>> FindByKeyword2Async(string keyword, DateTime? tnxDate1, DateTime? tnxDate2, byte state1, byte state2, string orderBy, string orderDir, int start, int pageSize)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword) || keyword.Equals("all"))
        //    {
        //        keyword = string.Empty;
        //    }

        //    var query = Set.Where(m => (m.RegisterState >= state1 && m.RegisterState <= state2));

        //    if (tnxDate1 != null)
        //    {
        //        query = query.Where(m => m.RegisterRequestDate.HasValue && m.RegisterRequestDate.Value >= tnxDate1);
        //    }
        //    if (tnxDate2 != null)
        //    {
        //        query = query.Where(m => m.RegisterRequestDate.HasValue && m.RegisterRequestDate.Value < tnxDate2);
        //    }

        //    query = query.Where(GetSearchFunc(keyword));

        //    query = query.OrderBy(string.Format("{0} {1}", orderBy, orderDir))
        //        .Skip(start).Take(pageSize);

        //    var query2 = query.GroupJoin(Context.RegisterContacts,
        //        rm => new { rm.Id, ContactType = (byte)RegisterConstant.ContactType.Normal },
        //        rc => new { Id = rc.RegisterMerchantId, rc.ContactType },
        //        (rm, contactsGroup) => new
        //        {
        //            Merchant = rm,
        //            Contacts = contactsGroup.OrderBy(c => c.Id),
        //        });
        //    var query3 = query2.GroupJoin(Context.RegisterStateLogs,
        //        g => g.Merchant.Id,
        //        rc => rc.RegisterMerchantId,
        //        (g, logsGroup) => new RegisterMerchantSearch2Model
        //        {
        //            Merchant = g.Merchant,
        //            Contacts = g.Contacts,
        //            StateLogs = logsGroup.OrderBy(l => l.AddedDate),
        //        });

        //    return await query3.ToListAsync();
        //}

        //private Expression<Func<RegisterMerchant, bool>> GetSearchFunc(string keyword)
        //{
        //    Expression<Func<RegisterMerchant, bool>> predicate = null;
        //    predicate = (m => m.RegisterNameEN.Contains(keyword)
        //        || m.RegisterNameTH.Contains(keyword)
        //        //|| m.RegisterFirstnameEN.Contains(keyword)
        //        //|| m.RegisterLastnameEN.Contains(keyword)
        //        //|| m.RegisterFirstnameTH.Contains(keyword)
        //        //|| m.RegisterLastnameTH.Contains(keyword)
        //        //|| m.ProductType.Contains(keyword));

        //    return predicate;
        //}
    }
}