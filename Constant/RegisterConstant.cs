namespace ChillPay.Merchant.Register.Api.Constant
{
    public class RegisterConstant
    {
        public enum RegisterState
        {
            None = 0,
            FillGeneralInfo = 1,
            FillDetailInfo = 2,
            FillWebInfo = 3,
            SelectChannel = 4,
            ConfirmInfo = 5,
            SaleAssign = 6,
            SetChannelFee = 7,
            ApproveChannelFee = 8,
            ContractInfo = 9,
            PreApproveContract = 10,
            ApproveContract = 11,
            Registerd = 12,
        }

        public enum UserSystem
        {
            Merchant = 0,
            //AdminMain = 1,
            //AdminSand = 2,
        }

        //public enum IsSelected
        //{ 
        //    False = 0,
        //    True = 1
        //}

        //public enum RegisterType
        //{
        //    None = 0,
        //    Corperate = 1,
        //    Personal = 2,
        //}

        public enum AddressType
        {
            Undefined = 0,
            Registered = 1,
            Billing = 2,
        }

        public enum ContactType
        {
            Undefined = 0,
            Normal = 1,
            Developer = 2,
        }

        public enum SSLLocationType
        {
            None = 0,
            MerchantOwner = 1,
            WebHostingShare = 2,
        }

        public enum ChannelConditionType
        {
            Undefined = 0,
            ChangeServiceFeeBySalesPrice = 1,   // ValueDec1 is "Monthly Sales Price", ValueDec2 is "Service Fee Value"
        }

        public const string FEE_TYPE_FIX = "Fix";
        public const string FEE_TYPE_PERCENT = "Percent";
        public const string FEE_OFFER_CUSTOMER = "Customer";
        public const string FEE_OFFER_MERCHANT = "Merchant";

        public enum DocumentType
        {
            Undefined = 0,
            Contract = 1,
            DBD = 2,
            PO20 = 3,
            CommercialRegistration = 4,
            IdCard = 5,
            HouseRegister = 6,
            BookBank = 7,
            PersonImage = 8,

            AdditionContract = 101,
        }
    }
}
