using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Domains.Repositories;
using ChillPay.Merchant.Register.Api.Entities.Customers;
using ChillPay.Merchant.Register.Api.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Users
{
    internal class CustomerRepository : Repository<Customer, long>, ICustomerRepository
    {
        private readonly ILogger _logger;
        //private IIdentityValidator<Customer> _customerValidator;

        internal CustomerRepository(ChillPayGlobalDbContext context, ILogger logger) : base(context)
        {
            //_customerValidator = new CustomerValidator(this);
            _logger = logger;
        }

        //public async Task<IdentityResult> CreateOrUpdateAsync(Customer customer)
        //{
        //    //var result = await _customerValidator.ValidateAsync(customer);
        //    //if (!result.Succeeded)
        //    //{
        //    //    return result;
        //    //}

        //    try
        //    {
        //        if (customer.Id > 0)
        //        {
        //            await UpdateAsync(customer);
        //        }
        //        else
        //        {
        //            await AddAsync(customer);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
        //    }

        //    return IdentityResult.Success;

        //}

        //    public async Task<IdentityResult> UpdateListAsync(List<Customer> customers)
        //    {
        //        using (var transaction = Context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                foreach (Customer c in customers)
        //                {
        //                    await UpdateAsync(c);
        //                }

        //                transaction.Commit();
        //            }
        //            catch (Exception ex)
        //            {
        //                // TODO: Handle failure
        //                transaction.Rollback();

        //                _logger.LogError(ex, ex.Message);
        //                return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
        //            }

        //            return IdentityResult.Success;
        //        }
        //    }

        //public async Task<IdentityResult> DeleteAsync(long id)
        //{
        //    try
        //    {
        //        var entity = await FindByIdAsync(id);
        //        await DeleteAsync(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
        //    }

        //    return IdentityResult.Success;
        //}

        //    public async Task<Customer> FindByEmailAsync(string email)
        //    {
        //        return await Set.FirstOrDefaultAsync(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        //    }
        //    public async Task<Customer> FindByUserNameAsync(string username)
        //    {
        //        return await Set.FirstOrDefaultAsync(x => x.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
        //    }

        //    public async Task<long> CountByKeywordAsync(string keyword)
        //    {
        //        var query = Set.Where(GetSearchFunc(keyword));
        //        return await query.LongCountAsync();
        //    }
        //    public async Task<List<Customer>> FindByKeywordAsync(string keyword, string orderBy, string orderDir, int start, int pageSize)
        //    {
        //        var query = Set.Where(GetSearchFunc(keyword))
        //            .OrderBy(string.Format("{0} {1}", orderBy, orderDir))
        //            .Skip(start).Take(pageSize);
        //        return await query.ToListAsync();
        //    }

        //    private Expression<Func<Customer, bool>> GetSearchFunc(string keyword)
        //    {
        //        Expression<Func<Customer, bool>> predicate = null;
        //        if (StringUtil.IsNumberic(keyword, new string[] { ",", "." }))
        //        {
        //            predicate = (m => m.PhoneNumber.Contains(keyword)
        //                   || (m.Status + "").Contains(keyword)
        //                   || (m.Id + "").Contains(keyword)
        //                   || (m.IsActivated + "").Contains(keyword));
        //        }
        //        else
        //        {
        //            predicate = (m => m.UserName.Contains(keyword)
        //                   || m.Email.Contains(keyword)
        //                   || m.FirstName.Contains(keyword)
        //                   || m.LastName.Contains(keyword)
        //                   || m.PhoneNumber.Contains(keyword));
        //        }

        //        return predicate;
        //    }
        //public async Task<IdentityResult> CreateOrUpdateUserMerchantAsync(UserMerchantMapping userMerchant)
        //{
        //    try
        //    {
        //        var user = await FindByIdAsync(userMerchant.UserId);
        //        if (user == null)
        //        {
        //            return IdentityResult.Failed(new IdentityError { Code = "500", Description = "User not found." });
        //        }

        //        var entity = await Context.UserMerchantMappings.Where(x => x.UserId == userMerchant.UserId && x.MerchantCode == userMerchant.MerchantCode).FirstOrDefaultAsync();
        //        if (entity == null)
        //        {
        //            Context.UserMerchantMappings.Add(userMerchant);
        //        }
        //        else
        //        {
        //            var entry = Context.Entry(entity);
        //            if (entry.State == EntityState.Detached)
        //            {
        //                Context.UserMerchantMappings.Attach(userMerchant);
        //                entry = Context.Entry(entity);
        //            }
        //            entry.State = EntityState.Modified;
        //        }

        //        await Context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
        //    }

        //    return IdentityResult.Success;
        //}

        //    public async Task<IdentityResult> DeleteUserMerchantAsync(long userId, string merchantCode)
        //    {
        //        try
        //        {
        //            var entity = await Context.UserMerchantMappings.Where(m => m.UserId == userId && m.MerchantCode == merchantCode).FirstOrDefaultAsync();
        //            Context.UserMerchantMappings.Remove(entity);
        //            await Context.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, ex.Message);
        //            return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
        //        }

        //        return IdentityResult.Success;
        //    }

        //    public async Task<UserMerchantMapping> FindUserMerchantByIdAsync(long id)
        //    {
        //        return await Context.UserMerchantMappings.FirstOrDefaultAsync(x => x.Id == id);
        //    }
        //public async Task<UserMerchantMapping> FindUserMerchantByUserIdAsync(long userId)
        //{
        //    return await Context.UserMerchantMappings.FirstOrDefaultAsync(x => x.UserId == userId);
        //}
        //    public async Task<List<UserMerchantMapping>> FindUserMerchantByParentIdAsync(long parentId)
        //    {
        //        return await Context.UserMerchantMappings.Where(x => x.ParentId == parentId).ToListAsync();
        //    }
        //    public async Task<List<UserMerchantMapping>> FindUserMerchantByMerchantCodeAsync(string merchantCode)
        //    {
        //        return await Context.UserMerchantMappings.Where(x => x.MerchantCode == merchantCode).ToListAsync();
        //    }

        #region #####  User Profile  #####
        //    public async Task<IdentityResult> CreateOrUpdateUserProfileAsync(CustomerProfile userProfile)
        //{
        //    try
        //    {
        //        var user = await FindByIdAsync(userProfile.UserId);
        //        if (user == null)
        //        {
        //            return IdentityResult.Failed(new IdentityError { Code = "500", Description = "User not found." });
        //        }

        //        var profile = await Context.UserProfiles.Where(x => x.UserId == userProfile.UserId).FirstOrDefaultAsync();
        //        if (profile == null)
        //        {
        //            Context.UserProfiles.Add(userProfile);
        //        }
        //        else
        //        {
        //            var entry = Context.Entry(profile);
        //            if (entry.State == EntityState.Detached)
        //            {
        //                Context.UserProfiles.Attach(userProfile);
        //                entry = Context.Entry(profile);
        //            }
        //            entry.State = EntityState.Modified;
        //        }

        //        await Context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return IdentityResult.Failed(new IdentityError { Code = "500", Description = ex.Message });
        //    }

        //    return IdentityResult.Success;
        //}

        //public async Task<CustomerProfile> FindUserProfileByIdAsync(long id)
        //{
        //    return await Context.UserProfiles.FirstOrDefaultAsync(x => x.Id == id);
        //}
        //public async Task<CustomerProfile> FindUserProfileByUserIdAsync(long userId)
        //{
        //    return await Context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        //}
        #endregion
        //}
    }
}
