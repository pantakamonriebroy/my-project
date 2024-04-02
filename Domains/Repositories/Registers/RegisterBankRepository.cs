using ChillPay.Merchant.Register.Api.Constant;
using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    internal class RegisterBankRepository : Repository<RegisterBank, int>, IRegisterBankRepository
    {
        internal RegisterBankRepository(ChillPayGlobalDbContext context) : base(context)
        {
        }

        public async Task<List<RegisterBank>> GetAllActiveAsync()
        {
            return await Set.Where(m => m.Status == (byte)GenericConstant.DefaultStatus.Active).ToListAsync();
        }

        //public async Task<List<RegisterBankAccountType>> GetAllAccountTypeAsync()
        //{
        //    return await Context.RegisterBankAccountTypes.ToListAsync();
        //}
        public async Task<List<RegisterBankAccountType>> GetAllActiveAccountTypeAsync()
        {
            return await Context.RegisterBankAccountTypes.Where(m => m.Status == (byte)GenericConstant.DefaultStatus.Active).ToListAsync();
        }
    }
}
