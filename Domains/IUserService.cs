using ChillPay.Merchant.Register.Api.Domains.Repositories.Generics;
using ChillPay.Merchant.Register.Api.Domains.Repositories.Registers;
using ChillPay.Merchant.Register.Api.Domains.Users;

namespace ChillPay.Merchant.Register.Api.Domains
{
    public interface IUserService : IDisposable
    {
        ICustomerRepository CustomerRepository { get; }
        IGenericRepository GenericRepository { get; }
        //IUserRoleRepository UserRoleRepository { get; }

        IRegisterAddressRepository RegisterAddressRepository { get; }
        IRegisterBankRepository RegisterBankRepository { get; }
        //IRegisterChannelBaseFeeRepository RegisterChannelBaseFeeRepository { get; }
        //IRegisterChannelBaseServiceFeeRepository RegisterChannelBaseServiceFeeRepository { get; }
        IRegisterChannelConditionRepository RegisterChannelConditionRepository { get; }
        IRegisterChannelFeeRepository RegisterChannelFeeRepository { get; }
        IRegisterChannelRepository RegisterChannelRepository { get; }
        IRegisterChannelServiceFeeRepository RegisterChannelServiceFeeRepository { get; }
        IRegisterContactRepository RegisterContactRepository { get; }
        //IRegisterDocumentRepository RegisterDocumentRepository { get; }
        IRegisterInstallmentRepository RegisterInstallmentRepository { get; }
        IRegisterMerchantRepository RegisterMerchantRepository { get; }
        IRegisterStateLogRepository RegisterStateLogRepository { get; }
        //IRegisterDataChangeLogRepository RegisterDataChangeLogRepository { get; }
    }
}
