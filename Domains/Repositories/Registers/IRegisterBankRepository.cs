using ChillPay.Merchant.Register.Api.Entities.Registers;

namespace ChillPay.Merchant.Register.Api.Domains.Repositories.Registers
{
    public interface IRegisterBankRepository : IRepository<RegisterBank, int>
    {
        Task<List<RegisterBank>> GetAllActiveAsync();

        //Task<List<RegisterBankAccountType>> GetAllAccountTypeAsync();
        Task<List<RegisterBankAccountType>> GetAllActiveAccountTypeAsync();
    }
}
