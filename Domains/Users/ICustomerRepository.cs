using ChillPay.Merchant.Register.Api.Domains.Repositories;
using ChillPay.Merchant.Register.Api.Entities.Customers;
using ChillPay.Merchant.Register.Api.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Users
{
    public interface ICustomerRepository : IRepository<Customer, long>
    {
        //Task<IdentityResult> CreateOrUpdateAsync(Customer customer);
        //Task<IdentityResult> UpdateListAsync(List<Customer> customers);
        //Task<IdentityResult> DeleteAsync(long id);

        //Task<Customer> FindByUserNameAsync(string username);
        //Task<Customer> FindByEmailAsync(string email);

        //Task<long> CountByKeywordAsync(string keyword);
        //Task<List<Customer>> FindByKeywordAsync(string keyword, string orderBy, string orderDir, int start, int pageSize);

        //#region #####  User Merchant Mapping  #####
        //Task<IdentityResult> CreateOrUpdateUserMerchantAsync(UserMerchantMapping userMerchant);
        //Task<IdentityResult> DeleteUserMerchantAsync(long userId, string merchantCode);

        //Task<UserMerchantMapping> FindUserMerchantByIdAsync(long id);
        //Task<UserMerchantMapping> FindUserMerchantByUserIdAsync(long userId);
        //Task<List<UserMerchantMapping>> FindUserMerchantByParentIdAsync(long parentId);
        //Task<List<UserMerchantMapping>> FindUserMerchantByMerchantCodeAsync(string merchantCode);
        //#endregion

        //#region #####  User Profile  #####
        //Task<IdentityResult> CreateOrUpdateUserProfileAsync(CustomerProfile userProfile);

        //Task<CustomerProfile> FindUserProfileByIdAsync(long id);
        //Task<CustomerProfile> FindUserProfileByUserIdAsync(long userId);
        //#endregion
    }
}
