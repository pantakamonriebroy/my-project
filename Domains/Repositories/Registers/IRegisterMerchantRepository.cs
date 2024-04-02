using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.AspNetCore.Identity;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterMerchantRepository : IRepository<RegisterMerchant, long>
    {
        Task<IdentityResult> CreateOrUpdateAsync(RegisterMerchant regMerchant);

        Task<RegisterMerchant> FindByUserIdAsync(long userId);

        //Task<long> CountByKeywordAsync(string keyword, DateTime? tnxDate1, DateTime? tnxDate2, byte state1, byte state2);
        //Task<List<RegisterMerchantSearchModel>> FindByKeywordAsync(string keyword, DateTime? tnxDate1, DateTime? tnxDate2, byte state1, byte state2, string orderBy, string orderDir, int start, int pageSize);
        //Task<List<RegisterMerchantSearch2Model>> FindByKeyword2Async(string keyword, DateTime? tnxDate1, DateTime? tnxDate2, byte state1, byte state2, string orderBy, string orderDir, int start, int pageSize);
    }
}