using ChillPay.Merchant.Register.Api.Data;
using ChillPay.Merchant.Register.Api.Domains.Repositories.Generics;
using ChillPay.Merchant.Register.Api.Domains.Repositories.Registers;
using ChillPay.Merchant.Register.Api.Domains.Users;

namespace ChillPay.Merchant.Register.Api.Domains
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ChillPayGlobalDbContext _context;

        public UserService(ChillPayGlobalDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private ICustomerRepository _customerRepository;
        private IGenericRepository _genericRepository;
        //private IUserRoleRepository _userRoleRepository;

        private IRegisterAddressRepository _registerAddressRepository;
        private IRegisterBankRepository _registerBankRepository;
        //private IRegisterChannelBaseFeeRepository _registerChannelBaseFeeRepository;
        //private IRegisterChannelBaseServiceFeeRepository _registerChannelBaseServiceFeeRepository;
        private IRegisterChannelConditionRepository _registerChannelConditionRepository;
        private IRegisterChannelFeeRepository _registerChannelFeeRepository;
        private IRegisterChannelRepository _registerChannelRepository;
        private IRegisterChannelServiceFeeRepository _registerChannelServiceFeeRepository;
        private IRegisterContactRepository _registerContactRepository;
        //private IRegisterDocumentRepository _registerDocumentRepository;
        private IRegisterInstallmentRepository _registerInstallmentRepository;
        private IRegisterMerchantRepository _registerMerchantRepository;
        private IRegisterStateLogRepository _registerStateLogRepository;
        //private IRegisterDataChangeLogRepository _registerDataChangeLogRepository;

        public ICustomerRepository CustomerRepository
        {
            get => _customerRepository ?? (_customerRepository = new CustomerRepository(_context, _logger));
        }

        public IGenericRepository GenericRepository
        {
            get => _genericRepository ?? (_genericRepository = new GenericRepository(_context, _logger));
        }

        //public IUserRoleRepository UserRoleRepository
        //{
        //    get => _userRoleRepository ?? (_userRoleRepository = new UserRoleRepository(_context, _logger));
        //}

        public IRegisterAddressRepository RegisterAddressRepository
        {
            get => _registerAddressRepository ?? (_registerAddressRepository = new RegisterAddressRepository(_context, _logger));
        }
        public IRegisterBankRepository RegisterBankRepository
        {
            get => _registerBankRepository ?? (_registerBankRepository = new RegisterBankRepository(_context));
        }
        //public IRegisterChannelBaseFeeRepository RegisterChannelBaseFeeRepository
        //{
        //    get => _registerChannelBaseFeeRepository ?? (_registerChannelBaseFeeRepository = new RegisterChannelBaseFeeRepository(_context, _logger));
        //}

        //public IRegisterChannelBaseServiceFeeRepository RegisterChannelBaseServiceFeeRepository
        //{
        //    get => _registerChannelBaseServiceFeeRepository ?? (_registerChannelBaseServiceFeeRepository = new RegisterChannelBaseServiceFeeRepository(_context, _logger));
        //}

        public IRegisterChannelConditionRepository RegisterChannelConditionRepository
        {
            get => _registerChannelConditionRepository ?? (_registerChannelConditionRepository = new RegisterChannelConditionRepository(_context, _logger));
        }

        public IRegisterChannelFeeRepository RegisterChannelFeeRepository
        {
            get => _registerChannelFeeRepository ?? (_registerChannelFeeRepository = new RegisterChannelFeeRepository(_context, _logger));
        }

        public IRegisterChannelRepository RegisterChannelRepository
        {
            get => _registerChannelRepository ?? (_registerChannelRepository = new RegisterChannelRepository(_context, _logger));
        }
        public IRegisterChannelServiceFeeRepository RegisterChannelServiceFeeRepository
        {
            get => _registerChannelServiceFeeRepository ?? (_registerChannelServiceFeeRepository = new RegisterChannelServiceFeeRepository(_context, _logger));
        }
        public IRegisterContactRepository RegisterContactRepository
        {
            get => _registerContactRepository ?? (_registerContactRepository = new RegisterContactRepository(_context, _logger));
        }
        //public IRegisterDocumentRepository RegisterDocumentRepository
        //{
        //    get => _registerDocumentRepository ?? (_registerDocumentRepository = new RegisterDocumentRepository(_context, _logger));
        //}
        public IRegisterInstallmentRepository RegisterInstallmentRepository
        {
            get => _registerInstallmentRepository ?? (_registerInstallmentRepository = new RegisterInstallmentRepository(_context, _logger));
        }
        public IRegisterMerchantRepository RegisterMerchantRepository
        {
            get => _registerMerchantRepository ?? (_registerMerchantRepository = new RegisterMerchantRepository(_context, _logger));
        }
        public IRegisterStateLogRepository RegisterStateLogRepository
        {
            get => _registerStateLogRepository ?? (_registerStateLogRepository = new RegisterStateLogRepository(_context, _logger));
        }
        //public IRegisterDataChangeLogRepository RegisterDataChangeLogRepository
        //{
        //    get => _registerDataChangeLogRepository ?? (_registerDataChangeLogRepository = new RegisterDataChangeLogRepository(_context, _logger));
        //}

        public void Dispose()
        {
            _customerRepository?.Dispose();
            _genericRepository?.Dispose();
            //_userRoleRepository?.Dispose();
            _registerAddressRepository?.Dispose();
            _registerBankRepository?.Dispose();

            //_registerChannelBaseFeeRepository?.Dispose();
            //_registerChannelBaseServiceFeeRepository?.Dispose();
            _registerChannelConditionRepository?.Dispose();
            _registerChannelFeeRepository?.Dispose();
            _registerChannelRepository?.Dispose();
            _registerChannelServiceFeeRepository?.Dispose();

            _registerContactRepository?.Dispose();
            //_registerDocumentRepository?.Dispose();
            _registerInstallmentRepository?.Dispose();
            _registerMerchantRepository?.Dispose();
            _registerStateLogRepository?.Dispose();
            //_registerDataChangeLogRepository?.Dispose();

            _context?.Dispose();
        }
    }
}