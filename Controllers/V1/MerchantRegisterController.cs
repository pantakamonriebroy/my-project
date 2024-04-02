using ChillPay.Merchant.Register.Api.Configs;
using ChillPay.Merchant.Register.Api.Constant;
using ChillPay.Merchant.Register.Api.Domains;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using ChillPay.Merchant.Register.Api.Models;
using ChillPay.Merchant.Register.Api.Models.MerchantRegisters;
using ChillPay.Merchant.Register.Api.Models.Registers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChillPay.Merchant.Register.Api.Controllers.V1
{
    [ApiController]
    [Route("api/register")]
    public class MerchantRegisterController : BaseController
    {
        private readonly ILogger<MerchantRegisterController> _logger;
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public MerchantRegisterController(IUserService userService, IOptions<AppSettings> settings, ILogger<MerchantRegisterController> logger)
            : base(settings, logger)
        {
            _logger = logger;
            _userService = userService;
            _appSettings = settings.Value;
        }

        [HttpGet("generalInfo/{userId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterGeneralInfoModel>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetGeneralInfo(long userId)
        {
            _logger.LogInformation($"GetGeneralInfo: {userId}");
            var jsonResponse = ApiResponseMessageModel<RegisterGeneralInfoModel>.Failed();

            try
            {
                // validate get info parameter(validate, check user, find or create register data)
                var res = await ValidateAndGetRegisterInfoByUserId(userId);
                if (res.Status != ApiResponseStatus.Success)
                {
                    jsonResponse.Message = res.Message;

                    return BadRequest(jsonResponse);
                }

                // find address
                var regAddr = await _userService.RegisterAddressRepository.GetLatestByAddressTypeAsync(res.Data.UserId, (byte)RegisterConstant.AddressType.Registered); //(byte)RegisterConstant.AddressType.Registered
                var bilAddr = await _userService.RegisterAddressRepository.GetLatestByAddressTypeAsync(res.Data.UserId, (byte)RegisterConstant.AddressType.Billing);//(byte)RegisterConstant.AddressType.Billing
                RegisterAddressModel regAddrModel = null;
                RegisterAddressModel bilAddrModel = null;
                if (regAddr != null)
                {
                    regAddrModel = new RegisterAddressModel
                    {
                        Address = regAddr.Address,
                        ProvinceId = regAddr.ProvinceId,
                        DistrictId = regAddr.DistrictId,
                        SubdistrictId = regAddr.SubdistrictId,
                        ZipCode = regAddr.ZipCode,
                        TelNo = regAddr.TelNo,
                        Fax = regAddr.Fax,
                    };
                }
                if (bilAddr != null)
                {
                    bilAddrModel = new RegisterAddressModel
                    {
                        Address = bilAddr.Address,
                        ProvinceId = bilAddr.ProvinceId,
                        DistrictId = bilAddr.DistrictId,
                        SubdistrictId = bilAddr.SubdistrictId,
                        ZipCode = bilAddr.ZipCode,
                        TelNo = bilAddr.TelNo,
                        Fax = bilAddr.Fax,
                    };
                }

                RegisterGeneralInfoModel model = new RegisterGeneralInfoModel
                {
                    Id = res.Data.Id,
                    UserId = userId,
                    RegisterMerchantId = userId,
                    RegisterState = res.Data.RegisterState,
                    //isregistered = res.data.isregistered,
                    //isapproved = res.data.isapproved,

                    RegisterType = res.Data.RegisterType,
                    RegisterNameEN = res.Data.RegisterNameEN,
                    RegisterNameTH = res.Data.RegisterNameTH,
                    //RegisterFirstnameEN = res.Data.RegisterFirstnameEN,
                    //RegisterLastnameEN = res.Data.RegisterLastnameEN,
                    //RegisterFirstnameTH = res.Data.RegisterFirstnameTH,
                    //RegisterLastnameTH = res.Data.RegisterLastnameTH,
                    BrandNameEN = res.Data.BrandNameEN,
                    BrandNameTH = res.Data.BrandNameTH,
                    //LogoImageUrl = res.Data.LogoImageUrl,
                    TaxId = res.Data.TaxId,
                    UseRegisterAddressForBilling = res.Data.UseRegisterAddressForBilling,
                    RegisterAddress = regAddrModel,
                    BillingAddress = bilAddrModel,

                };

                jsonResponse = ApiResponseMessageModel<RegisterGeneralInfoModel>.Success(model);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("generalInfo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterGeneralInfoModel>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> PostGeneralInfo(RegisterGeneralInfoModel info)
        {
            _logger.LogInformation($"PostGeneralInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterGeneralInfoModel>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateGeneralInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find register merchant data
                var regMData = await _userService.RegisterMerchantRepository.FindByIdAsync(info.Id);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // check regiser merchant data with userId
                jsonError = await ValidateUserWithRegisterInfo(regMData, info.UserId);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // check current state(that available for edit register general data)
                if (regMData.RegisterState != (byte)RegisterConstant.RegisterState.FillGeneralInfo &&
                    regMData.RegisterState != (byte)RegisterConstant.RegisterState.FillDetailInfo &&
                    regMData.RegisterState != (byte)RegisterConstant.RegisterState.FillWebInfo &&
                    regMData.RegisterState != (byte)RegisterConstant.RegisterState.SelectChannel)
                {
                    _logger.LogError("Register state mismatch");
                    jsonResponse.Message = "Register state mismatch";

                    return BadRequest(jsonResponse);
                }

                // register merchant data change
                bool saveChange = false;
                if (RegisterDataChange(regMData, info))
                {
                    regMData.RegisterType = info.RegisterType;
                    regMData.RegisterNameEN = info.RegisterNameEN;
                    regMData.RegisterNameTH = info.RegisterNameTH;
                    //regMData.RegisterFirstnameEN = info.RegisterFirstnameEN;
                    //regMData.RegisterLastnameEN = info.RegisterLastnameEN;
                    //regMData.RegisterFirstnameTH = info.RegisterFirstnameTH;
                    //regMData.RegisterLastnameTH = info.RegisterLastnameTH;
                    regMData.BrandNameEN = info.BrandNameEN;
                    regMData.BrandNameTH = info.BrandNameTH;
                    //regMData.LogoImageUrl = info.LogoImageUrl;
                    regMData.TaxId = info.TaxId;
                    regMData.UseRegisterAddressForBilling = info.UseRegisterAddressForBilling;

                    saveChange = true;
                }
                // check state change
                if (regMData.RegisterState == (byte)RegisterConstant.RegisterState.FillGeneralInfo)
                {
                    regMData.RegisterState = (byte)RegisterConstant.RegisterState.FillDetailInfo;
                    saveChange = true;

                    // add state change log
                    await _userService.RegisterStateLogRepository.CreateOrUpdateAsync(new RegisterStateLog
                    {
                        RegisterMerchantId = info.Id,
                        OldState = RegisterConstant.RegisterState.FillGeneralInfo.ToString(),
                        OldStateValue = (byte)RegisterConstant.RegisterState.FillGeneralInfo,
                        NewState = RegisterConstant.RegisterState.FillDetailInfo.ToString(),
                        NewStateValue = (byte)RegisterConstant.RegisterState.FillDetailInfo,
                        StateDescription = "Applied GeneralInfo, Change to DetailInfo State",
                        AddedDate = DateTime.Now,
                        AddedBy = info.UserId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    });
                }
                if (saveChange)
                {
                    // update modified data
                    regMData.ModifiedDate = DateTime.Now;
                    regMData.ModifiedBy = info.UserId;
                    regMData.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                    // save data
                    var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);
                }

                // save registered/billing address
                bool saveRegAddr = await UpdateSingleRegisterAddress(RegisterConstant.AddressType.Registered, info.RegisterMerchantId, info.UserId, info.RegisterAddress);
                bool saveBilAddr = await UpdateSingleRegisterAddress(RegisterConstant.AddressType.Billing, info.RegisterMerchantId, info.UserId, info.BillingAddress);

                // apply change back to param object
                info.RegisterState = regMData.RegisterState;
                //info.IsRegistered = regMData.IsRegistered;
                //info.IsApproved = regMData.IsApproved;

                jsonResponse = ApiResponseMessageModel<RegisterGeneralInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("generalInfo/add")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterGeneralInfoModel>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> AddGeneralInfo(RegisterGeneralInfoModel info)
        {
            _logger.LogInformation($"PostGeneralInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterGeneralInfoModel>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateAddGeneralInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                RegisterAddress regAddr = new RegisterAddress();
                RegisterAddress bilAddr = new RegisterAddress();

                RegisterAddressModel regAddrModel = null;
                RegisterAddressModel bilAddrModel = null;
                if (regAddr != null)
                {
                    regAddrModel = new RegisterAddressModel
                    {
                        Address = regAddr.Address,
                        ProvinceId = regAddr.ProvinceId,
                        DistrictId = regAddr.DistrictId,
                        SubdistrictId = regAddr.SubdistrictId,
                        ZipCode = regAddr.ZipCode,
                        TelNo = regAddr.TelNo,
                        Fax = regAddr.Fax,
                    };
                }
                if (bilAddr != null)
                {
                    bilAddrModel = new RegisterAddressModel
                    {
                        Address = bilAddr.Address,
                        ProvinceId = bilAddr.ProvinceId,
                        DistrictId = bilAddr.DistrictId,
                        SubdistrictId = bilAddr.SubdistrictId,
                        ZipCode = regAddr.ZipCode,
                        TelNo = bilAddr.TelNo,
                        Fax = bilAddr.Fax,
                    };
                }
                RegisterAddress addDress = new RegisterAddress();
                RegisterMerchant regMData = new RegisterMerchant();
                //regMData.Id = info.Id;
                regMData.UserId = info.UserId;
                regMData.AddedDate = DateTime.Now;
                regMData.RegisterType = info.RegisterType;
                regMData.RegisterNameEN = info.RegisterNameEN;
                regMData.RegisterNameTH = info.RegisterNameTH;
                regMData.BrandNameEN = info.BrandNameEN;
                regMData.BrandNameTH = info.BrandNameTH;
                regMData.TaxId = info.TaxId;
                regAddrModel = info.RegisterAddress;
                bilAddrModel = info.BillingAddress;
                regMData.UseRegisterAddressForBilling = info.UseRegisterAddressForBilling;
                regMData.RegisterState = info.RegisterState;
                addDress.RegisterMerchantId = info.RegisterMerchantId;

                var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);

                // save registered/billing address
                bool saveRegAddr = await AddSingleRegisterAddress(RegisterConstant.AddressType.Registered, info.UserId, info.RegisterMerchantId, info.RegisterAddress);
                bool saveBilAddr = await AddSingleRegisterAddress(RegisterConstant.AddressType.Billing, info.UserId, info.RegisterMerchantId, info.BillingAddress);

                jsonResponse = ApiResponseMessageModel<RegisterGeneralInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("getBankList/{language}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<JsonRegisterBankModel[]>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetBankList(string language)
        {
            _logger.LogInformation($"GetBankList: {language}");
            var jsonResponse = ApiResponseMessageModel<JsonRegisterBankModel[]>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // check parameter
                language = language.ToUpper();
                if (!language.Equals("EN") && !language.Equals("TH"))
                {
                    _logger.LogError("Parameter language is invalid");
                    jsonResponse.Message = "Parameter language is invalid";

                    return BadRequest(jsonResponse);
                }

                var bList = await _userService.RegisterBankRepository.GetAllActiveAsync();

                List<JsonRegisterBankModel> dataList = new List<JsonRegisterBankModel>(bList.Count());
                if (language.Equals("EN"))
                {
                    foreach (var b in bList)
                    {
                        dataList.Add(new JsonRegisterBankModel
                        {
                            Id = b.Id,
                            Name = b.NameEN,
                            Code = b.Code,
                        });
                    }
                }
                else
                {
                    foreach (var b in bList)
                    {
                        dataList.Add(new JsonRegisterBankModel
                        {
                            Id = b.Id,
                            Name = b.NameTH,
                            Code = b.Code,
                        });
                    }
                }

                jsonResponse = ApiResponseMessageModel<JsonRegisterBankModel[]>.Success(dataList.ToArray());
                jsonResponse.TotalRecord = dataList.Count;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("getAccountTypeList/{language}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<JsonRegisterBankAccountTypeModel[]>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetBankAccountTypeList(string language)
        {
            _logger.LogInformation($"GetBankAccountTypeList: {language}");
            var jsonResponse = ApiResponseMessageModel<JsonRegisterBankAccountTypeModel[]>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // check parameter
                language = language.ToUpper();
                if (!language.Equals("EN") && !language.Equals("TH"))
                {
                    _logger.LogError("Parameter language is invalid");
                    jsonResponse.Message = "Parameter language is invalid";

                    return BadRequest(jsonResponse);
                }

                var bList = await _userService.RegisterBankRepository.GetAllActiveAccountTypeAsync();

                List<JsonRegisterBankAccountTypeModel> dataList = new List<JsonRegisterBankAccountTypeModel>(bList.Count());
                if (language.Equals("EN"))
                {
                    foreach (var b in bList)
                    {
                        dataList.Add(new JsonRegisterBankAccountTypeModel
                        {
                            Id = b.Id,
                            Name = b.NameEN,
                            Code = b.Code,
                        });
                    }
                }
                else
                {
                    foreach (var b in bList)
                    {
                        dataList.Add(new JsonRegisterBankAccountTypeModel
                        {
                            Id = b.Id,
                            Name = b.NameTH,
                            Code = b.Code,
                        });
                    }
                }

                jsonResponse = ApiResponseMessageModel<JsonRegisterBankAccountTypeModel[]>.Success(dataList.ToArray());
                jsonResponse.TotalRecord = dataList.Count;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("detailInfo/{userId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterDetailInfoModel>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetDetailInfo(long userId)
        {
            _logger.LogInformation($"GetDetailInfo: {userId}");
            var jsonResponse = ApiResponseMessageModel<RegisterDetailInfoModel>.Failed();

            try
            {
                // validate get info parameter(validate, check user, find or create register data)
                var res = await ValidateAndGetRegisterInfoByUserId(userId);
                if (res.Status != ApiResponseStatus.Success)
                {
                    jsonResponse.Message = res.Message;

                    return BadRequest(jsonResponse);
                }

                // find contact
                var contacts = await _userService.RegisterContactRepository.GetAllByContactTypeAsync(res.Data.UserId /*Id*/, (byte)RegisterConstant.ContactType.Normal);
                List<RegisterContactModel> cList = new List<RegisterContactModel>(contacts.Count());
                foreach (RegisterContact c in contacts)
                {
                    cList.Add(new RegisterContactModel
                    {
                        CorperateName = c.CorperateName,
                        Name = c.Name,
                        Position = c.Position,
                        TelNo = c.TelNo,
                        MobileNo = c.MobileNo,
                        Email = c.Email,
                        LineId = c.LineId,
                    });
                }

                RegisterDetailInfoModel model = new RegisterDetailInfoModel
                {
                    Id = res.Data.Id,
                    UserId = userId,
                    RegisterMerchantId = userId,
                    RegisterState = res.Data.RegisterState,
                    //IsRegistered = res.Data.IsRegistered,
                    //IsApproved = res.Data.IsApproved,

                    DomainName = res.Data.DomainName,
                    ProductType = res.Data.ProductType,
                    ProductMinPrice = decimal.Round(res.Data.ProductMinPrice ?? 0, 2, MidpointRounding.AwayFromZero),
                    ProductMaxPrice = decimal.Round(res.Data.ProductMaxPrice ?? 0, 2, MidpointRounding.AwayFromZero),
                    EstimateSales = decimal.Round(res.Data.EstimateSales ?? 0, 2, MidpointRounding.AwayFromZero),
                    BankAccountBrandId = res.Data.BankAccountBrandId,
                    BankAccountBranch = res.Data.BankAccountBranch,
                    BankAccountTypeId = res.Data.BankAccountTypeId,
                    BankAccountName = res.Data.BankAccountName,
                    BankAccountNo = res.Data.BankAccountNo,
                    Contacts = cList.ToArray(),
                };

                jsonResponse = ApiResponseMessageModel<RegisterDetailInfoModel>.Success(model);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("detailInfo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterDetailInfoModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostDetailInfo(RegisterDetailInfoModel info)
        {
            _logger.LogInformation($"PostDetailInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterDetailInfoModel>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateDetailInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find register merchant data
                var regMData = await _userService.RegisterMerchantRepository.FindByIdAsync(info.Id);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // check regiser merchant data with userId
                jsonError = await ValidateUserWithRegisterInfo(regMData, info.UserId);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // check current state(that available for edit register detail data)
                if (regMData.RegisterState != (byte)RegisterConstant.RegisterState.FillDetailInfo &&
                    regMData.RegisterState != (byte)RegisterConstant.RegisterState.FillWebInfo &&
                    regMData.RegisterState != (byte)RegisterConstant.RegisterState.SelectChannel)
                {
                    _logger.LogError("Register state mismatch");
                    jsonResponse.Message = "Register state mismatch";

                    return BadRequest(jsonResponse);
                }

                // register merchant data change
                bool saveChange = false;
                if (RegisterDataChange(regMData, info))
                {
                    regMData.DomainName = info.DomainName;
                    regMData.ProductType = info.ProductType;
                    regMData.ProductMinPrice = info.ProductMinPrice;
                    regMData.ProductMaxPrice = info.ProductMaxPrice;
                    regMData.EstimateSales = info.EstimateSales;
                    regMData.BankAccountBrandId = info.BankAccountBrandId;
                    regMData.BankAccountBranch = info.BankAccountBranch;
                    regMData.BankAccountTypeId = info.BankAccountTypeId;
                    regMData.BankAccountName = info.BankAccountName;
                    regMData.BankAccountNo = info.BankAccountNo;

                    saveChange = true;
                }
                // check state change
                if (regMData.RegisterState == (byte)RegisterConstant.RegisterState.FillDetailInfo)
                {
                    regMData.RegisterState = (byte)RegisterConstant.RegisterState.FillWebInfo;
                    saveChange = true;

                    // add state change log
                    await _userService.RegisterStateLogRepository.CreateOrUpdateAsync(new RegisterStateLog
                    {
                        RegisterMerchantId = info.Id,
                        OldState = RegisterConstant.RegisterState.FillDetailInfo.ToString(),
                        OldStateValue = (byte)RegisterConstant.RegisterState.FillDetailInfo,
                        NewState = RegisterConstant.RegisterState.FillWebInfo.ToString(),
                        NewStateValue = (byte)RegisterConstant.RegisterState.FillWebInfo,
                        StateDescription = "Applied DetailInfo, Change to WebInfo State",
                        AddedDate = DateTime.Now,
                        AddedBy = info.UserId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    });
                }
                if (saveChange)
                {
                    // update modified data
                    regMData.ModifiedDate = DateTime.Now;
                    regMData.ModifiedBy = info.UserId;
                    regMData.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                    // save data
                    var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);
                }

                // update contact list
                await UpdateListRegisterContact(RegisterConstant.ContactType.Normal, info.RegisterMerchantId, info.UserId, info.Contacts.ToArray());

                // apply change back to param object
                info.RegisterState = regMData.RegisterState;
                //info.IsRegistered = regMData.IsRegistered;
                //info.IsApproved = regMData.IsApproved;

                jsonResponse = ApiResponseMessageModel<RegisterDetailInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("detailInfo/add")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterDetailInfoModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddDetailInfo(RegisterDetailInfoModel info)
        {
            _logger.LogInformation($"PostDetailInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterDetailInfoModel>.Failed();

            try
            {
                RegisterContact conTact = new RegisterContact();
                RegisterMerchant regMData = new RegisterMerchant();

                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateAddDetailInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);

                }

                // find register merchant data
                regMData = await _userService.RegisterMerchantRepository.FindByUserIdAsync(info.UserId);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // check regiser merchant data with userId
                jsonError = await ValidateUserWithRegisterInfo(regMData, info.UserId);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find contact
                var contacts = await _userService.RegisterContactRepository.GetAllByContactTypeAsync(info.UserId /*Id*/, (byte)RegisterConstant.ContactType.Normal);
                List<RegisterContactModel> cList = new List<RegisterContactModel>(contacts.Count());
                foreach (RegisterContact c in contacts)
                {
                    cList.Add(new RegisterContactModel
                    {
                        CorperateName = c.CorperateName,
                        Name = c.Name,
                        Position = c.Position,
                        TelNo = c.TelNo,
                        MobileNo = c.MobileNo,
                        Email = c.Email,
                        LineId = c.LineId,
                    });
                }

                //regMData.Id = info.Id;
                regMData.UserId = info.UserId;
                regMData.DomainName = info.DomainName;
                regMData.ProductType = info.ProductType;
                regMData.ProductMinPrice = info.ProductMinPrice;
                regMData.ProductMaxPrice = info.ProductMaxPrice;
                regMData.EstimateSales = info.EstimateSales;
                regMData.BankAccountBrandId = info.BankAccountBrandId;
                regMData.BankAccountBranch = info.BankAccountBranch;
                regMData.BankAccountTypeId = info.BankAccountTypeId;
                regMData.BankAccountName = info.BankAccountName;
                regMData.BankAccountNo = info.BankAccountNo;
                //Contacts = cList.ToArray();
                regMData.RegisterState = info.RegisterState;
                conTact.RegisterMerchantId = info.RegisterMerchantId;

                // save data
                var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);

                // update contact list
                await UpdateAddListRegisterContact(RegisterConstant.ContactType.Normal, info.RegisterMerchantId, info.UserId, info.Contacts.ToArray());

                //// apply change back to param object
                //info.RegisterState = regMData.RegisterState;
                ////info.IsRegistered = regMData.IsRegistered;
                ////info.IsApproved = regMData.IsApproved;

                jsonResponse = ApiResponseMessageModel<RegisterDetailInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("webInfo/{userId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterWebInfoModel>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetWebInfo(long userId)
        {
            _logger.LogInformation($"GetWebInfo: {userId}");
            var jsonResponse = ApiResponseMessageModel<RegisterWebInfoModel>.Failed();

            try
            {
                // validate get info parameter(validate, check user, find or create register data)
                var res = await ValidateAndGetRegisterInfoByUserId(userId);
                if (res.Status != ApiResponseStatus.Success)
                {
                    jsonResponse.Message = res.Message;

                    return BadRequest(jsonResponse);
                }

                // find dev contact
                var devContact = await _userService.RegisterContactRepository.GetLatestByContactTypeAsync(res.Data.UserId /*Id*/, (byte)RegisterConstant.ContactType.Developer);
                RegisterContactModel devContactModel = null;
                if (devContact != null)
                {
                    devContactModel = new RegisterContactModel
                    {
                        CorperateName = devContact.CorperateName,
                        Name = devContact.Name,
                        Position = devContact.Position,
                        TelNo = devContact.TelNo,
                        MobileNo = devContact.MobileNo,
                        Email = devContact.Email,
                        LineId = devContact.LineId,
                    };
                }


                RegisterWebInfoModel model = new RegisterWebInfoModel
                {
                    Id = res.Data.Id,
                    UserId = userId,
                    RegisterMerchantId = userId,
                    RegisterState = res.Data.RegisterState,
                    //IsRegistered = res.Data.IsRegistered,
                    //IsApproved = res.Data.IsApproved,

                    DevContact = devContactModel,
                    MerchantServerPrdIPAddress = res.Data.MerchantServerPrdIPAddress,
                    MerchanrServerPrdUrl = res.Data.MerchanrServerPrdUrl,
                    UrlBackgroundPrd = res.Data.UrlBackgroundPrd,
                    MerchantServerSandIPAddress = res.Data.MerchantServerSandIPAddress,
                    MerchantServerSandUrl = res.Data.MerchantServerSandUrl,
                    UrlBackgroundSand = res.Data.UrlBackgroundSand,
                    UseUrlBackground = res.Data.UseUrlBackground,
                    UseSSL = res.Data.UseSSL,
                    SSLType = res.Data.SSLType,
                    SSLBy = res.Data.SSLBy,
                    SSLCreateDate = res.Data.SSLCreateDate,
                    SSLExpireDate = res.Data.SSLExpireDate,
                    SSLLocationType = res.Data.SSLLocationType,
                };

                jsonResponse = ApiResponseMessageModel<RegisterWebInfoModel>.Success(model);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("webInfo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterWebInfoModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostWebInfo(RegisterWebInfoModel info)
        {
            _logger.LogInformation($"PostWebInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterWebInfoModel>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateWebInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find register merchant data
                var regMData = await _userService.RegisterMerchantRepository.FindByIdAsync(info.Id);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // check regiser merchant data with userId
                jsonError = await ValidateUserWithRegisterInfo(regMData, info.UserId);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // check current state(that available for edit register web data)
                if (regMData.RegisterState != (byte)RegisterConstant.RegisterState.FillWebInfo &&
                    regMData.RegisterState != (byte)RegisterConstant.RegisterState.SelectChannel)
                {
                    _logger.LogError("Register state mismatch");
                    jsonResponse.Message = "Register state mismatch";

                    return BadRequest(jsonResponse);
                }

                // register merchant data change
                bool saveChange = false;
                if (RegisterDataChange(regMData, info))
                {
                    regMData.MerchantServerPrdIPAddress = info.MerchantServerPrdIPAddress;
                    regMData.MerchanrServerPrdUrl = info.MerchanrServerPrdUrl;
                    regMData.UrlBackgroundPrd = info.UrlBackgroundPrd;
                    regMData.MerchantServerSandIPAddress = info.MerchantServerSandIPAddress;
                    regMData.MerchantServerSandUrl = info.MerchantServerSandUrl;
                    regMData.UrlBackgroundSand = info.UrlBackgroundSand;
                    regMData.UseUrlBackground = info.UseUrlBackground;
                    regMData.UseSSL = info.UseSSL;
                    regMData.SSLType = info.SSLType;
                    regMData.SSLBy = info.SSLBy;
                    regMData.SSLCreateDate = info.SSLCreateDate;
                    regMData.SSLExpireDate = info.SSLExpireDate;
                    regMData.SSLLocationType = info.SSLLocationType;

                    saveChange = true;
                }
                // check state change
                if (regMData.RegisterState == (byte)RegisterConstant.RegisterState.FillWebInfo)
                {
                    regMData.RegisterState = (byte)RegisterConstant.RegisterState.SelectChannel;
                    saveChange = true;

                    // add state change log
                    await _userService.RegisterStateLogRepository.CreateOrUpdateAsync(new RegisterStateLog
                    {
                        RegisterMerchantId = info.Id,
                        OldState = RegisterConstant.RegisterState.FillWebInfo.ToString(),
                        OldStateValue = (byte)RegisterConstant.RegisterState.FillWebInfo,
                        NewState = RegisterConstant.RegisterState.SelectChannel.ToString(),
                        NewStateValue = (byte)RegisterConstant.RegisterState.SelectChannel,
                        StateDescription = "Applied WebInfo, Change to SelectChannel State",
                        AddedDate = DateTime.Now,
                        AddedBy = info.UserId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    });
                }
                if (saveChange)
                {
                    // update modified data
                    regMData.ModifiedDate = DateTime.Now;
                    regMData.ModifiedBy = info.UserId;
                    regMData.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                    // save data
                    var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);
                }

                // save dev contact
                bool saveDevContact = await UpdateSingleRegisterContact(RegisterConstant.ContactType.Developer, info.RegisterMerchantId, info.UserId, info.DevContact);

                // apply change back to param object
                info.RegisterState = regMData.RegisterState;
                //info.IsRegistered = regMData.IsRegistered;
                //info.IsApproved = regMData.IsApproved;

                jsonResponse = ApiResponseMessageModel<RegisterWebInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("webInfo/add")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterWebInfoModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddWebInfo(RegisterWebInfoModel info)
        {
            _logger.LogInformation($"PostWebInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterWebInfoModel>.Failed();

            try
            {

                RegisterContact conTact = new RegisterContact();
                RegisterMerchant regMData = new RegisterMerchant();

                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateAddWebInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find register merchant data
                regMData = await _userService.RegisterMerchantRepository.FindByUserIdAsync(info.UserId);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // check regiser merchant data with userId
                jsonError = await ValidateUserWithRegisterInfo(regMData, info.UserId);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find dev contact
                var devContact = await _userService.RegisterContactRepository.GetLatestByContactTypeAsync(info.UserId /*Id*/, (byte)RegisterConstant.ContactType.Developer);
                RegisterContactModel devContactModel = null;
                if (devContact != null)
                {
                    devContactModel = new RegisterContactModel
                    {
                        CorperateName = devContact.CorperateName,
                        Name = devContact.Name,
                        Position = devContact.Position,
                        TelNo = devContact.TelNo,
                        MobileNo = devContact.MobileNo,
                        Email = devContact.Email,
                        LineId = devContact.LineId,
                    };
                }

                //regMData.Id = info.Id;
                regMData.UserId = info.UserId;
                conTact.RegisterMerchantId = info.RegisterMerchantId;
                regMData.RegisterState = info.RegisterState;

                //DevContact = devContactModel,
                regMData.MerchantServerPrdIPAddress = info.MerchantServerPrdIPAddress;
                regMData.MerchanrServerPrdUrl = info.MerchanrServerPrdUrl;
                regMData.UrlBackgroundPrd = info.UrlBackgroundPrd;
                regMData.MerchantServerSandIPAddress = info.MerchantServerSandIPAddress;
                regMData.MerchantServerSandUrl = info.MerchantServerSandUrl;
                regMData.UrlBackgroundSand = info.UrlBackgroundSand;
                regMData.UseUrlBackground = info.UseUrlBackground;
                regMData.UseSSL = info.UseSSL;
                regMData.SSLType = info.SSLType;
                regMData.SSLBy = info.SSLBy;
                regMData.SSLCreateDate = info.SSLCreateDate;
                regMData.SSLExpireDate = info.SSLExpireDate;
                regMData.SSLLocationType = info.SSLLocationType;

                // save data
                var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);


                // save dev contact
                bool saveDevContact = await UpdateSingleRegisterContact(RegisterConstant.ContactType.Developer, info.RegisterMerchantId, info.UserId, info.DevContact);

                jsonResponse = ApiResponseMessageModel<RegisterWebInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("channelInfo/{userId:int}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterChannelInfoModel>), 200)]
        [ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<IActionResult> GetChannelInfo(long userId)
        {
            _logger.LogInformation($"GetChannelInfo: {userId}");
            var jsonResponse = ApiResponseMessageModel<RegisterChannelInfoModel>.Failed();

            try
            {
                // validate get info parameter(validate, check user, find or create register data)
                var res = await ValidateAndGetRegisterInfoByUserId(userId);
                if (res.Status != ApiResponseStatus.Success)
                {
                    jsonResponse.Message = res.Message;

                    return BadRequest(jsonResponse);
                }

                // create default channel data
                RegisterChannelModel[] channels = (RegisterChannelModel[])_appSettings.RegisterChannelIntialData.Clone();
                RegisterInstallmentModel[] installments = (RegisterInstallmentModel[])_appSettings.RegisterInstallmentInitialData.Clone();
                //RegisterChannelModel[] channels = new RegisterChannelModel[0];
                //RegisterInstallmentModel[] installments = new RegisterInstallmentModel[0];

                List<RegisterChannel> dbChannels = await _userService.RegisterChannelRepository.GetAllByRegisterMerchantIdAsync(res.Data.UserId /*Id*/);
                //List<RegisterChannelCondition> dbConditions = await _userService.RegisterChannelConditionRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                //List<RegisterChannelFee> dbFees = await _userService.RegisterChannelFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.UserId /*Id*/);
                //List<RegisterChannelBaseFee> dbBaseFees = await _userService.RegisterChannelBaseFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                //List<RegisterChannelServiceFee> dbServiceFees = await _userService.RegisterChannelServiceFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                //List<RegisterChannelBaseServiceFee> dbBaseServiceFees = await _userService.RegisterChannelBaseServiceFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                List<RegisterInstallment> dbInstallments = await _userService.RegisterInstallmentRepository.GetAllByRegisterMerchantIdAsync(res.Data.UserId);

                channels = CreateChannelModelList(dbChannels, /*dbConditions,*//* dbFees,*/ /*dbBaseFees,*/ /*dbServiceFees,*/ /*dbBaseServiceFees,*/ channels);
                installments = CreateInstallmentModelList(dbInstallments, installments);

                RegisterChannelInfoModel model = new RegisterChannelInfoModel
                {
                    Id = res.Data.Id,
                    UserId = userId,
                    RegisterMerchantId = userId,
                    RegisterState = res.Data.RegisterState,
                    //IsRegistered = res.Data.IsRegistered,
                    //IsApproved = res.Data.IsApproved,

                    Channels = channels,
                    Installments = installments,

                    //EntranceFeeBase = res.Data.EntranceFeeBase,
                    //EntranceFee = res.Data.EntranceFee,
                    //MonthlyFeeBase = res.Data.MonthlyFeeBase,
                    //MonthlyFee = res.Data.MonthlyFee,
                    //MerchantTransferFeeBase = res.Data.MerchantTransferFeeBase,
                    //MerchantTransferFee = res.Data.MerchantTransferFee,
                    //AdditionNote = res.Data.AdditionNote,
                };

                jsonResponse = ApiResponseMessageModel<RegisterChannelInfoModel>.Success(model);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("channelInfo")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterChannelInfoModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostChannelInfo(RegisterChannelInfoModel info)
        {
            _logger.LogInformation($"PostChannelInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterChannelInfoModel>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateChannelInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find register merchant data
                var regMData = await _userService.RegisterMerchantRepository.FindByUserIdAsync(info.UserId);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // check regiser merchant data with userId
                jsonError = await ValidateUserWithRegisterInfo(regMData, info.UserId);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // check current state(that available for edit register web data)
                if (regMData.RegisterState != (byte)RegisterConstant.RegisterState.SelectChannel)
                {
                    _logger.LogError("Register state mismatch");
                    jsonResponse.Message = "Register state mismatch";

                    return BadRequest(jsonResponse);
                }

                // load channel + installment from db
                List<RegisterChannel> dbChannels = await _userService.RegisterChannelRepository.GetAllByRegisterMerchantIdAsync(regMData.UserId);
                List<RegisterInstallment> dbInstallments = await _userService.RegisterInstallmentRepository.GetAllByRegisterMerchantIdAsync(regMData.UserId);

                // check channel matching
                List<RegisterChannelModel> addCList = new List<RegisterChannelModel>();
                List<RegisterChannel> updateCList = new List<RegisterChannel>();
                foreach (RegisterChannelModel cm in info.Channels)
                {
                    if (cm.Id == 0)
                    {   // add new channel
                        addCList.Add(cm);
                    }
                    else
                    {   // update exist data, check match
                        int idx = dbChannels.FindIndex(m => m.Id == cm.Id && m.ChannelServiceCode == cm.ChannelServiceCode);
                        if (idx < 0)
                        {   // error, mismatch exist data
                            _logger.LogError("Channel data corrupt");
                            jsonResponse.Message = "Channel data corrupt";

                            return BadRequest(jsonResponse);
                        }

                        RegisterChannel c = dbChannels[idx];
                        dbChannels.RemoveAt(idx);

                        // check data change(that this api allow to change)
                        if (c.IsSelected != cm.IsSelected)
                        {
                            c.IsSelected = cm.IsSelected;
                            c.ModifiedDate = DateTime.Now;
                            c.ModifiedBy = info.UserId;
                            c.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                            updateCList.Add(c);
                        }
                    }
                }

                // check installment matching
                List<RegisterInstallmentModel> addIList = new List<RegisterInstallmentModel>();
                List<RegisterInstallment> updateIList = new List<RegisterInstallment>();
                foreach (RegisterInstallmentModel im in info.Installments)
                {
                    if (im.Id == 0)
                    {   // add new channel
                        addIList.Add(im);
                    }
                    else
                    {   // update exist data, check match
                        int idx = dbInstallments.FindIndex(m => m.Id == im.Id && m.ChannelServiceCode == im.ChannelServiceCode);
                        if (idx < 0)
                        {   // error, mismatch exist data
                            _logger.LogError("Installment data corrupt");
                            jsonResponse.Message = "Installment data corrupt";

                            return BadRequest(jsonResponse);
                        }

                        RegisterInstallment ist = dbInstallments[idx];
                        dbInstallments.RemoveAt(idx);

                        // check data change(that this api allow to change)
                        if (ist.IsSelected != im.IsSelected)
                        {
                            ist.IsSelected = im.IsSelected;
                            ist.ModifiedDate = DateTime.Now;
                            ist.ModifiedBy = info.UserId;
                            ist.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                            updateIList.Add(ist);
                        }
                    }
                }

                // update change channel
                foreach (RegisterChannel c in updateCList)
                {   // update ONLY Channel data(has no change in condition,fee,service fee)
                    await _userService.RegisterChannelRepository.CreateOrUpdateAsync(c);
                }
                // add channel
                foreach (RegisterChannelModel cm in addCList)
                {   // add data by DEPEND on data from model
                    await AddChannelDb(regMData.Id, info.UserId, (byte)RegisterConstant.UserSystem.Merchant, cm);
                }
                // delete channel
                foreach (RegisterChannel c in dbChannels)
                {   // delete channel and child of channel(condition, fee, base fee, service fee, base service fee)
                    await DeleteChannelDb(c);
                }
                dbChannels.Clear();

                // update change installment
                foreach (RegisterInstallment ist in updateIList)
                {
                    await _userService.RegisterInstallmentRepository.CreateOrUpdateAsync(ist);
                }
                // add installment
                foreach (RegisterInstallmentModel im in addIList)
                {
                    // add new RegisterInstallment
                    await AddInstallmentDb(regMData.Id, info.UserId, (byte)RegisterConstant.UserSystem.Merchant, im);
                }
                // delete installment
                foreach (RegisterInstallment ist in dbInstallments)
                {
                    await _userService.RegisterInstallmentRepository.DeleteAsync(ist);
                }
                dbInstallments.Clear();

                // register merchant data change
                bool saveChange = false;
                //if (RegisterDataChange(regMData, info))
                //{
                //    //regMData.EntranceFeeBase = info.EntranceFeeBase;
                //    regMData.EntranceFee = info.EntranceFee;
                //    //regMData.MonthlyFeeBase = info.MonthlyFeeBase;
                //    regMData.MonthlyFee = info.MonthlyFee;
                //    //regMData.MerchantTransferFeeBase = info.MerchantTransferFeeBase;
                //    regMData.MerchantTransferFee = info.MerchantTransferFee;
                //    regMData.AdditionNote = info.AdditionNote;

                //    saveChange = true;
                //}
                // check state change
                if (regMData.RegisterState == (byte)RegisterConstant.RegisterState.SelectChannel)
                {
                    regMData.RegisterState = (byte)RegisterConstant.RegisterState.ConfirmInfo;
                    saveChange = true;

                    // add state change log
                    await _userService.RegisterStateLogRepository.CreateOrUpdateAsync(new RegisterStateLog
                    {
                        RegisterMerchantId = info.Id,
                        OldState = RegisterConstant.RegisterState.SelectChannel.ToString(),
                        OldStateValue = (byte)RegisterConstant.RegisterState.SelectChannel,
                        NewState = RegisterConstant.RegisterState.ConfirmInfo.ToString(),
                        NewStateValue = (byte)RegisterConstant.RegisterState.ConfirmInfo,
                        StateDescription = "Selected channel, Change to ConfirmInfo State",
                        AddedDate = DateTime.Now,
                        AddedBy = info.UserId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    });
                }
                if (saveChange)
                {
                    // update modified data
                    regMData.ModifiedDate = DateTime.Now;
                    regMData.ModifiedBy = info.UserId;
                    regMData.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                    // save data
                    var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);
                }

                // apply change back to param object
                info.RegisterState = regMData.RegisterState;
                //info.IsRegistered = regMData.IsRegistered;
                //info.IsApproved = regMData.IsApproved;

                jsonResponse = ApiResponseMessageModel<RegisterChannelInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpPost("channelInfo/add")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterChannelInfoModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddChannelInfo(RegisterChannelInfoModel info)
        {
            _logger.LogInformation($"PostChannelInfo: regMerchantId {info.Id}, userId {info.UserId}");
            var jsonResponse = ApiResponseMessageModel<RegisterChannelInfoModel>.Failed();

            try
            {
                //RegisterChannel chanNel = new RegisterChannel();
                //RegisterInstallment installMent = new RegisterInstallment();
                RegisterMerchant regMData = new RegisterMerchant();

                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                jsonError = ValidateAddChannelInfoModel(info);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find register merchant data
                regMData = await _userService.RegisterMerchantRepository.FindByUserIdAsync(info.UserId);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // check regiser merchant data with userId
                jsonError = await ValidateUserWithRegisterInfo(regMData, info.UserId);
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // create default channel data
                RegisterChannelModel[] channels = (RegisterChannelModel[])_appSettings.RegisterChannelIntialData.Clone();
                RegisterInstallmentModel[] installments = (RegisterInstallmentModel[])_appSettings.RegisterInstallmentInitialData.Clone();
                //RegisterChannelModel[] channels = new RegisterChannelModel[0];
                //RegisterInstallmentModel[] installments = new RegisterInstallmentModel[0];
                //RegisterChannelModel[] channels = info.Channels;
                //RegisterInstallmentModel[] installments = info.Installments;

                List<RegisterChannel> dbChannels = await _userService.RegisterChannelRepository.GetAllByRegisterMerchantIdAsync(info.Id /*Id*/);
                //List<RegisterChannelCondition> dbConditions = await _userService.RegisterChannelConditionRepository.GetAllByRegisterMerchantIdAsync(info.Id);
                //List<RegisterChannelFee> dbFees = await _userService.RegisterChannelFeeRepository.GetAllByRegisterMerchantIdAsync(info.UserId /*Id*/);
                //List<RegisterChannelBaseFee> dbBaseFees = await _userService.RegisterChannelBaseFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                //List<RegisterChannelServiceFee> dbServiceFees = await _userService.RegisterChannelServiceFeeRepository.GetAllByRegisterMerchantIdAsync(info.Id);
                //List<RegisterChannelBaseServiceFee> dbBaseServiceFees = await _userService.RegisterChannelBaseServiceFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                List<RegisterInstallment> dbInstallments = await _userService.RegisterInstallmentRepository.GetAllByRegisterMerchantIdAsync(info.Id/*info.Id*/);

                channels = CreateChannelModelList(dbChannels, /*dbConditions,*/ /*dbFees,*/ /*dbBaseFees,*/ /*dbServiceFees,*/ /*dbBaseServiceFees,*/ channels);
                installments = CreateInstallmentModelList(dbInstallments, installments);

                // check channel matching
                List<RegisterChannelModel> addCList = new List<RegisterChannelModel>();

                // check installment matching
                List<RegisterInstallmentModel> addIList = new List<RegisterInstallmentModel>();

                foreach (RegisterChannelModel im in channels)
                {
                    addCList.Add(im);
                }

                foreach (RegisterInstallmentModel im in installments)
                {
                    addIList.Add(im);
                }

                // add channel
                foreach (RegisterChannelModel cm in addCList)
                {   // add data by DEPEND on data from model
                    int idx = Array.FindIndex(info.Channels, c => c.ChannelServiceCode == cm.ChannelServiceCode);
                    await AddRegisterChannelDb(regMData.Id, info.UserId, (byte)RegisterConstant.UserSystem.Merchant, cm, info, idx); /*regMData.Id*/
                }
                dbChannels.Clear();

                // add installment
                foreach (RegisterInstallmentModel im in addIList)
                {
                    // add new RegisterInstallment
                    int idx = Array.FindIndex(info.Installments, i => i.ChannelServiceCode == im.ChannelServiceCode);
                    await AddRegisterInstallmentDb(regMData.Id, info.UserId, (byte)RegisterConstant.UserSystem.Merchant, im, info, idx); /*info.RegisterMerchantId*/
                }
                dbInstallments.Clear();

                //regMData.Id = info.Id;
                regMData.UserId = info.UserId;
                //regMData.RegisterMerchantId = info.RegisterMerchantId;
                regMData.UserId = info.RegisterMerchantId;
                regMData.RegisterState = info.RegisterState;
                //channels = info.Channels ;
                //installments = info.Installments;
                //EntranceFeeBase = res.Data.EntranceFeeBase;
                //regMData.EntranceFee = info.EntranceFee;
                //MonthlyFeeBase = res.Data.MonthlyFeeBase;
                //regMData.MonthlyFee = info.MonthlyFee;
                //MerchantTransferFeeBase = res.Data.MerchantTransferFeeBase,
                //regMData.MerchantTransferFee = info.MerchantTransferFee;
                //regMData.AdditionNote = info.AdditionNote;
                
                // save data
                var res = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(regMData);

                jsonResponse = ApiResponseMessageModel<RegisterChannelInfoModel>.Success(info);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        [HttpGet("merchant/{registerMerchantId}")] /*{userId:int}*/ /*{registerMerchantId}*/
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponseMessageModel<RegisterMerchantDetailModel>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMerchantDetail(long registerMerchantId)
        {
            _logger.LogInformation($"GetMerchantDetail: regMerchantId {registerMerchantId}");
            var jsonResponse = ApiResponseMessageModel<RegisterMerchantDetailModel>.Failed();

            try
            {
                var jsonError = ValidateHeader();
                if (jsonError.Status != ApiResponseStatus.Success)
                {
                    _logger.LogError(jsonError.Message);
                    jsonResponse.Message = jsonError.Message;

                    return BadRequest(jsonResponse);
                }

                // find register merchant data
                var regMData = await _userService.RegisterMerchantRepository.FindByUserIdAsync(registerMerchantId);
                if (regMData == null)
                {
                    _logger.LogError("Not found register data");
                    jsonResponse.Message = "Not found register data";

                    return NotFound(jsonResponse);
                }

                // find address
                var regAddr = await _userService.RegisterAddressRepository.GetLatestByAddressTypeAsync(regMData.UserId, (byte)RegisterConstant.AddressType.Registered); //(byte)RegisterConstant.AddressType.Registered
                var bilAddr = await _userService.RegisterAddressRepository.GetLatestByAddressTypeAsync(regMData.UserId, (byte)RegisterConstant.AddressType.Billing);//(byte)RegisterConstant.AddressType.Billing
                RegisterAddressModel regAddrModel = null;
                RegisterAddressModel bilAddrModel = null;
                if (regAddr != null)
                {
                    regAddrModel = new RegisterAddressModel
                    {
                        Address = regAddr.Address,
                        ProvinceId = regAddr.ProvinceId,
                        DistrictId = regAddr.DistrictId,
                        SubdistrictId = regAddr.SubdistrictId,
                        ZipCode = regAddr.ZipCode,
                        TelNo = regAddr.TelNo,
                        Fax = regAddr.Fax,
                    };
                }
                if (bilAddr != null)
                {
                    bilAddrModel = new RegisterAddressModel
                    {
                        Address = bilAddr.Address,
                        ProvinceId = bilAddr.ProvinceId,
                        DistrictId = bilAddr.DistrictId,
                        SubdistrictId = bilAddr.SubdistrictId,
                        ZipCode = bilAddr.ZipCode,
                        TelNo = bilAddr.TelNo,
                        Fax = bilAddr.Fax,
                    };
                }

                // find contact
                var contacts = await _userService.RegisterContactRepository.GetAllByContactTypeAsync(regMData.UserId /*Id*/, (byte)RegisterConstant.ContactType.Normal);
                List<RegisterContactModel> cList = new List<RegisterContactModel>(contacts.Count());
                foreach (RegisterContact c in contacts)
                {
                    cList.Add(new RegisterContactModel
                    {
                        CorperateName = c.CorperateName,
                        Name = c.Name,
                        Position = c.Position,
                        TelNo = c.TelNo,
                        MobileNo = c.MobileNo,
                        Email = c.Email,
                        LineId = c.LineId,
                    });
                }

                // find dev contact
                var devContact = await _userService.RegisterContactRepository.GetLatestByContactTypeAsync(regMData.UserId /*Id*/, (byte)RegisterConstant.ContactType.Developer);
                RegisterContactModel devContactModel = null;
                if (devContact != null)
                {
                    devContactModel = new RegisterContactModel
                    {
                        CorperateName = devContact.CorperateName,
                        Name = devContact.Name,
                        Position = devContact.Position,
                        TelNo = devContact.TelNo,
                        MobileNo = devContact.MobileNo,
                        Email = devContact.Email,
                        LineId = devContact.LineId,
                    };
                }

                // create default channel data
                RegisterChannelModel[] channels = (RegisterChannelModel[])_appSettings.RegisterChannelIntialData.Clone();
                RegisterInstallmentModel[] installments = (RegisterInstallmentModel[])_appSettings.RegisterInstallmentInitialData.Clone();
                //RegisterChannelModel[] channels = new RegisterChannelModel[0];
                //RegisterInstallmentModel[] installments = new RegisterInstallmentModel[0];

                List<RegisterChannel> dbChannels = await _userService.RegisterChannelRepository.GetAllByRegisterMerchantIdAsync(regMData.UserId /*Id*/);
                //List<RegisterChannelCondition> dbConditions = await _userService.RegisterChannelConditionRepository.GetAllByRegisterMerchantIdAsync(regMData.Id);
                //List<RegisterChannelFee> dbFees = await _userService.RegisterChannelFeeRepository.GetAllByRegisterMerchantIdAsync(regMData.UserId /*Id*/);
                //List<RegisterChannelBaseFee> dbBaseFees = await _userService.RegisterChannelBaseFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                //List<RegisterChannelServiceFee> dbServiceFees = await _userService.RegisterChannelServiceFeeRepository.GetAllByRegisterMerchantIdAsync(regMData.Id);
                //List<RegisterChannelBaseServiceFee> dbBaseServiceFees = await _userService.RegisterChannelBaseServiceFeeRepository.GetAllByRegisterMerchantIdAsync(res.Data.Id);
                List<RegisterInstallment> dbInstallments = await _userService.RegisterInstallmentRepository.GetAllByRegisterMerchantIdAsync(regMData.UserId);

                channels = CreateChannelModelList(dbChannels, /*dbConditions,*/ /*dbFees,*/ /*dbBaseFees,*/ /*dbServiceFees,*/ /*dbBaseServiceFees,*/ channels);
                installments = CreateInstallmentModelList(dbInstallments, installments);

                RegisterMerchantDetailModel resp = new RegisterMerchantDetailModel
                {
                    Id = regMData.Id,
                    UserId = regMData.UserId,
                    RegisterMerchantId = regMData.UserId,
                    RegisterState = regMData.RegisterState,
                    //IsRegistered = regMData.IsRegistered,
                    //IsApproved = regMData.IsApproved,

                    RegisterType = regMData.RegisterType,
                    RegisterNameEN = regMData.RegisterNameEN,
                    RegisterNameTH = regMData.RegisterNameTH,
                    //RegisterFirstnameEN = regMData.RegisterFirstnameEN,
                    //RegisterLastnameEN = regMData.RegisterLastnameEN,
                    //RegisterFirstnameTH = regMData.RegisterFirstnameTH,
                    //RegisterLastnameTH = regMData.RegisterLastnameTH,
                    BrandNameEN = regMData.BrandNameEN,
                    BrandNameTH = regMData.BrandNameTH,
                    //LogoImageUrl = regMData.LogoImageUrl,
                    TaxId = regMData.TaxId,
                    UseRegisterAddressForBilling = regMData.UseRegisterAddressForBilling,
                    RegisterAddress = regAddrModel,
                    BillingAddress = bilAddrModel,

                    DomainName = regMData.DomainName,
                    ProductType = regMData.ProductType,
                    ProductMinPrice = regMData.ProductMinPrice,
                    ProductMaxPrice = regMData.ProductMaxPrice,
                    EstimateSales = regMData.EstimateSales,
                    BankAccountBrandId = regMData.BankAccountBrandId,
                    BankAccountBranch = regMData.BankAccountBranch,
                    BankAccountTypeId = regMData.BankAccountTypeId,
                    BankAccountName = regMData.BankAccountName,
                    BankAccountNo = regMData.BankAccountNo,
                    Contacts = cList.ToArray(),

                    DevContact = devContactModel,
                    MerchantServerPrdIPAddress = regMData.MerchantServerPrdIPAddress,
                    MerchanrServerPrdUrl = regMData.MerchanrServerPrdUrl,
                    UrlBackgroundPrd = regMData.UrlBackgroundPrd,
                    MerchantServerSandIPAddress = regMData.MerchantServerSandIPAddress,
                    MerchantServerSandUrl = regMData.MerchantServerSandUrl,
                    UrlBackgroundSand = regMData.UrlBackgroundSand,
                    UseUrlBackground = regMData.UseUrlBackground,
                    UseSSL = regMData.UseSSL,
                    SSLType = regMData.SSLType,
                    SSLBy = regMData.SSLBy,
                    SSLCreateDate = regMData.SSLCreateDate,
                    SSLExpireDate = regMData.SSLExpireDate,
                    SSLLocationType = regMData.SSLLocationType,
                    Channels = channels,
                    Installments = installments,

                    //EntranceFeeBase = regMData.EntranceFeeBase,
                    //EntranceFee = regMData.EntranceFee,
                    //MonthlyFeeBase = regMData.MonthlyFeeBase,
                    //MonthlyFee = regMData.MonthlyFee,
                    //MerchantTransferFeeBase = regMData.MerchantTransferFeeBase,
                    //MerchantTransferFee = regMData.MerchantTransferFee,
                    //AdditionNote = regMData.AdditionNote,
                };

                jsonResponse = ApiResponseMessageModel<RegisterMerchantDetailModel>.Success(resp);
                jsonResponse.TotalRecord = 1;

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                jsonResponse.Status = ApiResponseStatus.SystemError;
                jsonResponse.Message = "Exception : " + ex.ToString();

                return BadRequest(jsonResponse);
            }
        }

        private async Task<ApiResponseMessageModel<RegisterMerchant>> ValidateAndGetRegisterInfoByUserId(long userId)//long
        {
            ApiResponseMessageModel<RegisterMerchant> res = ApiResponseMessageModel<RegisterMerchant>.Failed();

            var jsonError = ValidateHeader();
            if (jsonError.Status != ApiResponseStatus.Success)
            {
                _logger.LogError(jsonError.Message);

                res.Status = jsonError.Status;
                res.Message = jsonError.Message;

                return res;
            }

            // check parameter
            if (userId == 0)
            {
                _logger.LogError("Parameter userId is empty");
                res.Message = "Parameter userId is empty";

                return res;
            }

            ////check user
            //var u = await _userService.CustomerRepository.FindUserMerchantByUserIdAsync(userId);
            //if (u == null || u.Customer == null)
            //{
            //    _logger.LogError("Not found user id " + userId);
            //    res.Message = "Not found user id " + userId;

            //    return res;
            //}
            // find parentUserId
            //long parentUserId = 0;
            //if (u.ParentId == 0)
            //    parentUserId = userId;
            //else
            //    parentUserId = u.ParentId;

            //find / create register merchant data
            //var rm = await _userService.RegisterMerchantRepository.FindByUserIdAsync(parentUserId);
            var rm = await _userService.RegisterMerchantRepository.FindByUserIdAsync(userId); //parentUserId
            if (rm == null)
            {
                // not found data, create new data
                rm = new RegisterMerchant
                {
                    UserId = userId,//parentUserId
                    RegisterState = (byte)RegisterConstant.RegisterState.FillGeneralInfo,
                    //IsRegistered = false,
                    //IsApproved = false,
                    AddedDate = DateTime.Now,
                    AddedBy = userId,
                    AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                };

                var createRes = await _userService.RegisterMerchantRepository.CreateOrUpdateAsync(rm);
                if (!createRes.Succeeded)
                {
                    _logger.LogError("Create RegisterMerchant failed");
                    res.Message = "Create RegisterMerchant failed";

                    return res;
                }
            }
            //var rm = new RegisterMerchant(); //เพิ่มเข้ามาทีหลัง
            //found data
            res = ApiResponseMessageModel<RegisterMerchant>.Success(rm);
            return res;
        }

        private ApiResponseMessageModel<string> ValidateGeneralInfoModel(RegisterGeneralInfoModel model)
        {

            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            if (model.Id == 0)
            {
                res.Message = "Parameter Id has empty";
                _logger.LogError("ERROR: Parameter Id has empty");

                return res;
            }

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.RegisterAddress != null)
            {
                if (model.RegisterAddress.Address == null)
                {
                    res.Message = "Parameter RegisterAddress.Address is null";
                    _logger.LogError("ERROR: Parameter RegisterAddress.Address is null");

                    return res;
                }
                if (model.RegisterAddress.TelNo == null)
                {
                    res.Message = "Parameter RegisterAddress.TelNo is null";
                    _logger.LogError("ERROR: Parameter RegisterAddress.TelNo is null");

                    return res;
                }
            }

            if (model.BillingAddress != null)
            {
                if (model.BillingAddress.Address == null)
                {
                    res.Message = "Parameter BillingAddress.Address is null";
                    _logger.LogError("ERROR: Parameter BillingAddress.Address is null");

                    return res;
                }
                if (model.BillingAddress.TelNo == null)
                {
                    res.Message = "Parameter BillingAddress.TelNo is null";
                    _logger.LogError("ERROR: Parameter BillingAddress.TelNo is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private ApiResponseMessageModel<string> ValidateAddGeneralInfoModel(RegisterGeneralInfoModel model)
        {

            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            //if (model.Id == 0)
            //{
            //    res.Message = "Parameter Id has empty";
            //    _logger.LogError("ERROR: Parameter Id has empty");

            //    return res;
            //}

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.RegisterAddress != null)
            {
                if (model.RegisterAddress.Address == null)
                {
                    res.Message = "Parameter RegisterAddress.Address is null";
                    _logger.LogError("ERROR: Parameter RegisterAddress.Address is null");

                    return res;
                }
                if (model.RegisterAddress.TelNo == null)
                {
                    res.Message = "Parameter RegisterAddress.TelNo is null";
                    _logger.LogError("ERROR: Parameter RegisterAddress.TelNo is null");

                    return res;
                }
            }

            if (model.BillingAddress != null)
            {
                if (model.BillingAddress.Address == null)
                {
                    res.Message = "Parameter BillingAddress.Address is null";
                    _logger.LogError("ERROR: Parameter BillingAddress.Address is null");

                    return res;
                }
                if (model.BillingAddress.TelNo == null)
                {
                    res.Message = "Parameter BillingAddress.TelNo is null";
                    _logger.LogError("ERROR: Parameter BillingAddress.TelNo is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private ApiResponseMessageModel<string> ValidateDetailInfoModel(RegisterDetailInfoModel model)
        {

            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            if (model.Id == 0)
            {
                res.Message = "Parameter Id has empty";
                _logger.LogError("ERROR: Parameter Id has empty");

                return res;
            }

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.Contacts == null)
            {
                res.Message = "Parameter Contact is null";
                _logger.LogError("ERROR: Parameter Contact is null");

                return res;
            }
            foreach (var c in model.Contacts)
            {
                if (c.Name == null)
                {
                    res.Message = "Parameter Contact.Name is null";
                    _logger.LogError("ERROR: Parameter Contact.Name is null");

                    return res;
                }
                if (c.TelNo == null)
                {
                    res.Message = "Parameter Contact.TelNo is null";
                    _logger.LogError("ERROR: Parameter Contact.TelNo is null");

                    return res;
                }
                if (c.Email == null)
                {
                    res.Message = "Parameter Contact.Email is null";
                    _logger.LogError("ERROR: Parameter Contact.Email is null");

                    return res;
                }
                if (c.LineId == null)
                {
                    res.Message = "Parameter Contact.LineId is null";
                    _logger.LogError("ERROR: Parameter Contact.LineId is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private ApiResponseMessageModel<string> ValidateAddDetailInfoModel(RegisterDetailInfoModel model)
        {

            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            //if (model.Id == 0)
            //{
            //    res.Message = "Parameter Id has empty";
            //    _logger.LogError("ERROR: Parameter Id has empty");

            //    return res;
            //}

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.Contacts == null)
            {
                res.Message = "Parameter Contact is null";
                _logger.LogError("ERROR: Parameter Contact is null");

                return res;
            }
            foreach (var c in model.Contacts)
            {
                if (c.Name == null)
                {
                    res.Message = "Parameter Contact.Name is null";
                    _logger.LogError("ERROR: Parameter Contact.Name is null");

                    return res;
                }
                if (c.TelNo == null)
                {
                    res.Message = "Parameter Contact.TelNo is null";
                    _logger.LogError("ERROR: Parameter Contact.TelNo is null");

                    return res;
                }
                if (c.Email == null)
                {
                    res.Message = "Parameter Contact.Email is null";
                    _logger.LogError("ERROR: Parameter Contact.Email is null");

                    return res;
                }
                if (c.LineId == null)
                {
                    res.Message = "Parameter Contact.LineId is null";
                    _logger.LogError("ERROR: Parameter Contact.LineId is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private ApiResponseMessageModel<string> ValidateWebInfoModel(RegisterWebInfoModel model)
        {

            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            if (model.Id == 0)
            {
                res.Message = "Parameter Id has empty";
                _logger.LogError("ERROR: Parameter Id has empty");

                return res;
            }

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.DevContact != null)
            {
                if (model.DevContact.Name == null)
                {
                    res.Message = "Parameter DevContact.Name is null";
                    _logger.LogError("ERROR: Parameter DevContact.Name is null");

                    return res;
                }
                if (model.DevContact.TelNo == null)
                {
                    res.Message = "Parameter DevContact.TelNo is null";
                    _logger.LogError("ERROR: Parameter DevContact.TelNo is null");

                    return res;
                }
                if (model.DevContact.Email == null)
                {
                    res.Message = "Parameter DevContact.Email is null";
                    _logger.LogError("ERROR: Parameter DevContact.Email is null");

                    return res;
                }
                if (model.DevContact.LineId == null)
                {
                    res.Message = "Parameter DevContact.LineId is null";
                    _logger.LogError("ERROR: Parameter DevContact.LineId is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private ApiResponseMessageModel<string> ValidateAddWebInfoModel(RegisterWebInfoModel model)
        {

            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            //if (model.Id == 0)
            //{
            //    res.Message = "Parameter Id has empty";
            //    _logger.LogError("ERROR: Parameter Id has empty");

            //    return res;
            //}

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.DevContact != null)
            {
                if (model.DevContact.Name == null)
                {
                    res.Message = "Parameter DevContact.Name is null";
                    _logger.LogError("ERROR: Parameter DevContact.Name is null");

                    return res;
                }
                if (model.DevContact.TelNo == null)
                {
                    res.Message = "Parameter DevContact.TelNo is null";
                    _logger.LogError("ERROR: Parameter DevContact.TelNo is null");

                    return res;
                }
                if (model.DevContact.Email == null)
                {
                    res.Message = "Parameter DevContact.Email is null";
                    _logger.LogError("ERROR: Parameter DevContact.Email is null");

                    return res;
                }
                if (model.DevContact.LineId == null)
                {
                    res.Message = "Parameter DevContact.LineId is null";
                    _logger.LogError("ERROR: Parameter DevContact.LineId is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private ApiResponseMessageModel<string> ValidateChannelInfoModel(RegisterChannelInfoModel model)
        {
            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            if (model.Id == 0)
            {
                res.Message = "Parameter Id has empty";
                _logger.LogError("ERROR: Parameter Id has empty");

                return res;
            }

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.Channels == null)
            {
                res.Message = "Parameter Channels is null";
                _logger.LogError("ERROR: Parameter Channels is null");

                return res;
            }
            foreach (RegisterChannelModel c in model.Channels)
            {
                if (c.ChannelServiceCode == null)
                {
                    res.Message = "Parameter Channels.ChannelServiceCode is null";
                    _logger.LogError("ERROR: Parameter Channels.ChannelServiceCode is null");

                    return res;
                }

                //if (c.Fees == null)
                //{
                //    res.Message = "Parameter Channels.Fees is null";
                //    _logger.LogError("ERROR: Parameter Channels.Fees is null");

                //    return res;
                //}
                //foreach (RegisterChannelFeeModel f in c.Fees)
                //{
                //    if (f.FeeOfferMode == null)
                //    {
                //        res.Message = "Parameter Channels.Fees.FeeOfferMode is null";
                //        _logger.LogError("ERROR: Parameter Channels.Fees.FeeOfferMode is null");

                //        return res;
                //    }

                //    if (f.FeeType == null)
                //    {
                //        res.Message = "Parameter Channels.Fees.FeeType is null";
                //        _logger.LogError("ERROR: Parameter Channels.Fees.FeeType is null");

                //        return res;
                //    }
                //}

                //if (c.ServiceFees == null)
                //{
                //    res.Message = "Parameter Channels.ServiceFees is null";
                //    _logger.LogError("ERROR: Parameter Channels.ServiceFees is null");

                //    return res;
                //}
                //foreach (RegisterChannelServiceFeeModel f in c.ServiceFees)
                //{
                //    if (f.ServiceRateType == null)
                //    {
                //        res.Message = "Parameter Channels.ServiceFees.ServiceRateType is null";
                //        _logger.LogError("ERROR: Parameter Channels.ServiceFees.ServiceRateType is null");

                //        return res;
                //    }

                //    if (f.ServiceRateInVat == null)
                //    {
                //        res.Message = "Parameter Channels.ServiceFees.ServiceRateInVat is null";
                //        _logger.LogError("ERROR: Parameter Channels.ServiceFees.ServiceRateInVat is null");

                //        return res;
                //    }

                //    if (f.ServiceConditionBy == null)
                //    {
                //        res.Message = "Parameter Channels.ServiceFees.ServiceConditionBy is null";
                //        _logger.LogError("ERROR: Parameter Channels.ServiceFees.ServiceConditionBy is null");

                //        return res;
                //    }
                //}
            }

            if (model.Installments == null)
            {
                res.Message = "Parameter Installments is null";
                _logger.LogError("ERROR: Parameter Installments is null");

                return res;
            }
            foreach (RegisterInstallmentModel i in model.Installments)
            {
                if (i.ChannelServiceCode == null)
                {
                    res.Message = "Parameter Installments.ChannelServiceCode is null";
                    _logger.LogError("ERROR: Parameter Installments.ChannelServiceCode is null");

                    return res;
                }

                if (i.RequireChannelServiceCode == null)
                {
                    res.Message = "Parameter Installments.RequireChannelServiceCode is null";
                    _logger.LogError("ERROR: Parameter Installments.RequireChannelServiceCode is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private ApiResponseMessageModel<string> ValidateAddChannelInfoModel(RegisterChannelInfoModel model)
        {
            ApiResponseMessageModel<string> res = ApiResponseMessageModel.Failed();

            if (model == null)
            {
                //res.Message = "Parameter token has empty";
                _logger.LogError("ERROR: model is null.");

                return res;
            }

            //if (model.Id == 0)
            //{
            //    res.Message = "Parameter Id has empty";
            //    _logger.LogError("ERROR: Parameter Id has empty");

            //    return res;
            //}

            if (model.UserId == 0)
            {
                res.Message = "Parameter UserId has empty";
                _logger.LogError("ERROR: Parameter UserId has empty");

                return res;
            }

            if (model.Channels == null)
            {
                res.Message = "Parameter Channels is null";
                _logger.LogError("ERROR: Parameter Channels is null");

                return res;
            }
            foreach (RegisterChannelModel c in model.Channels)
            {
                if (c.ChannelServiceCode == null)
                {
                    res.Message = "Parameter Channels.ChannelServiceCode is null";
                    _logger.LogError("ERROR: Parameter Channels.ChannelServiceCode is null");

                    return res;
                }

                if (c.GroupName == null)
                {
                    res.Message = "Parameter Channels.GroupName is null";
                    _logger.LogError("ERROR: Parameter Channels.GroupName is null");

                    return res;
                }

                if (c.ChannelName == null)
                {
                    res.Message = "Parameter Channels.ChannelName is null";
                    _logger.LogError("ERROR: Parameter Channels.ChannelName is null");

                    return res;
                }

                //if (c.Fees == null)
                //{
                //    res.Message = "Parameter Channels.Fees is null";
                //    _logger.LogError("ERROR: Parameter Channels.Fees is null");

                //    return res;
                //}
                //foreach (RegisterChannelFeeModel f in c.Fees)
                //{
                //    if (f.FeeOfferMode == null)
                //    {
                //        res.Message = "Parameter Channels.Fees.FeeOfferMode is null";
                //        _logger.LogError("ERROR: Parameter Channels.Fees.FeeOfferMode is null");

                //        return res;
                //    }

                //    if (f.FeeType == null)
                //    {
                //        res.Message = "Parameter Channels.Fees.FeeType is null";
                //        _logger.LogError("ERROR: Parameter Channels.Fees.FeeType is null");

                //        return res;
                //    }
                //}

                //if (c.ServiceFees == null)
                //{
                //    res.Message = "Parameter Channels.ServiceFees is null";
                //    _logger.LogError("ERROR: Parameter Channels.ServiceFees is null");

                //    return res;
                //}
                //foreach (RegisterChannelServiceFeeModel f in c.ServiceFees)
                //{
                //    if (f.ServiceRateType == null)
                //    {
                //        res.Message = "Parameter Channels.ServiceFees.ServiceRateType is null";
                //        _logger.LogError("ERROR: Parameter Channels.ServiceFees.ServiceRateType is null");

                //        return res;
                //    }

                //    if (f.ServiceRateInVat == null)
                //    {
                //        res.Message = "Parameter Channels.ServiceFees.ServiceRateInVat is null";
                //        _logger.LogError("ERROR: Parameter Channels.ServiceFees.ServiceRateInVat is null");

                //        return res;
                //    }

                //    if (f.ServiceConditionBy == null)
                //    {
                //        res.Message = "Parameter Channels.ServiceFees.ServiceConditionBy is null";
                //        _logger.LogError("ERROR: Parameter Channels.ServiceFees.ServiceConditionBy is null");

                //        return res;
                //    }
                //}
            }

            if (model.Installments == null)
            {
                res.Message = "Parameter Installments is null";
                _logger.LogError("ERROR: Parameter Installments is null");

                return res;
            }
            foreach (RegisterInstallmentModel i in model.Installments)
            {
                if (i.ChannelServiceCode == null)
                {
                    res.Message = "Parameter Installments.ChannelServiceCode is null";
                    _logger.LogError("ERROR: Parameter Installments.ChannelServiceCode is null");

                    return res;
                }

                if (i.RequireChannelServiceCode == null)
                {
                    res.Message = "Parameter Installments.RequireChannelServiceCode is null";
                    _logger.LogError("ERROR: Parameter Installments.RequireChannelServiceCode is null");

                    return res;
                }

                if (i.ChannelName == null)
                {
                    res.Message = "Parameter Channels.ChannelName is null";
                    _logger.LogError("ERROR: Parameter Channels.ChannelName is null");

                    return res;
                }
            }

            // no error
            res = ApiResponseMessageModel<string>.Success("Success");

            return res;
        }

        private async Task<ApiResponseMessageModel<string>> ValidateUserWithRegisterInfo(RegisterMerchant regData, long userId)
        {
            var res = ApiResponseMessageModel.Failed();

            //// get user data
            //var u = await _userService.CustomerRepository.FindUserMerchantByUserIdAsync(userId);
            //if (u == null || u.Customer == null)
            //{
            //    _logger.LogError("Not found user id " + userId);
            //    res.Message = "Not found user id " + userId;

            //    return res;
            //}

            //// find parentUserId
            //long parentUserId = 0;
            //if (u.ParentId == 0)
            //    parentUserId = userId;
            //else
            //    parentUserId = u.ParentId;

            // check user match
            if (regData.UserId != userId)//parentUserId
            {
                _logger.LogError("RegisterData mismatch with user");
                res.Message = "RegisterData mismatch with user";

                return res;
            }

            // no error
            res = ApiResponseMessageModel.Success("Success");

            return res;
        }

        private async Task<bool> UpdateSingleRegisterAddress(RegisterConstant.AddressType type, long regMerchantId, long userId, RegisterAddressModel addrModel)
        {
            if (regMerchantId == 0)
            {
                return false;
            }

            var addr = await _userService.RegisterAddressRepository.GetLatestByAddressTypeAsync(regMerchantId, (byte)type);
            if (addrModel != null)
            {
                if (addr == null)
                {   // create new
                    addr = new RegisterAddress
                    {
                        RegisterMerchantId = regMerchantId,
                        AddressType = (byte)type,
                        Address = addrModel.Address,
                        ProvinceId = addrModel.ProvinceId,
                        DistrictId = addrModel.DistrictId,
                        SubdistrictId = addrModel.SubdistrictId,
                        ZipCode = addrModel.ZipCode,
                        TelNo = addrModel.TelNo,
                        Fax = addrModel.Fax,

                        AddedDate = DateTime.Now,
                        AddedBy = userId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    };

                    await _userService.RegisterAddressRepository.CreateOrUpdateAsync(addr);

                    return true;
                }
                else
                {   // check changeing
                    if (AddressDataChange(addr, addrModel))
                    {   // has changed, update
                        addr.Address = addrModel.Address;
                        addr.ProvinceId = addrModel.ProvinceId;
                        addr.DistrictId = addrModel.DistrictId;
                        addr.SubdistrictId = addrModel.SubdistrictId;
                        addr.ZipCode = addrModel.ZipCode;
                        addr.TelNo = addrModel.TelNo;
                        addr.Fax = addrModel.Fax;

                        addr.ModifiedDate = DateTime.Now;
                        addr.ModifiedBy = userId;
                        addr.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                        await _userService.RegisterAddressRepository.CreateOrUpdateAsync(addr);

                        return true;
                    }
                }
            }
            else
            {
                if (addr != null)
                {
                    await _userService.RegisterAddressRepository.DeleteAsync(addr);

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> AddSingleRegisterAddress(RegisterConstant.AddressType type, long regMerchantId, long userId, RegisterAddressModel addrModel)
        {
            if (regMerchantId == 0)
            {
                return false;
            }

            var addr = await _userService.RegisterAddressRepository.GetLatestByAddressTypeAsync(regMerchantId, (byte)type);
            if (addrModel != null)
            {
                if (addr == null)
                {   // create new
                    addr = new RegisterAddress
                    {
                        RegisterMerchantId = regMerchantId,
                        AddressType = (byte)type,
                        Address = addrModel.Address,
                        ProvinceId = addrModel.ProvinceId,
                        DistrictId = addrModel.DistrictId,
                        SubdistrictId = addrModel.SubdistrictId,
                        ZipCode = addrModel.ZipCode,
                        TelNo = addrModel.TelNo,
                        Fax = addrModel.Fax,

                        AddedDate = DateTime.Now,
                        AddedBy = userId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    };

                    await _userService.RegisterAddressRepository.CreateOrUpdateAsync(addr);

                    return true;
                }
                else
                {   // check changeing
                    if (AddressDataChange(addr, addrModel))
                    {   // has changed, update
                        addr.Address = addrModel.Address;
                        addr.ProvinceId = addrModel.ProvinceId;
                        addr.DistrictId = addrModel.DistrictId;
                        addr.SubdistrictId = addrModel.SubdistrictId;
                        addr.ZipCode = addrModel.ZipCode;
                        addr.TelNo = addrModel.TelNo;
                        addr.Fax = addrModel.Fax;

                        addr.ModifiedDate = DateTime.Now;
                        addr.ModifiedBy = userId;
                        addr.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                        await _userService.RegisterAddressRepository.CreateOrUpdateAsync(addr);

                        return true;
                    }
                }
            }
            else
            {
                if (addr != null)
                {
                    await _userService.RegisterAddressRepository.DeleteAsync(addr);

                    return true;
                }
            }

            return false;
        }

        private async Task<bool> UpdateListRegisterContact(RegisterConstant.ContactType type, long regMerchantId, long userId, RegisterContactModel[] conModels)
        {
            if (regMerchantId == 0)
            {
                return false;
            }

            // load current contact list
            var contacts = await _userService.RegisterContactRepository.GetAllByContactTypeAsync(regMerchantId, (byte)type);

            // update contact list
            int i = 0;
            for (; i < conModels.Length; ++i)
            {
                RegisterContactModel rcm = conModels[i];
                if (i < contacts.Count)
                {   // has entity, check changed and update
                    RegisterContact rc = contacts[i];
                    if (ContactDataChange(rc, rcm))
                    {
                        rc.CorperateName = rcm.CorperateName;
                        rc.Name = rcm.Name;
                        rc.Position = rcm.Position;
                        rc.TelNo = rcm.TelNo;
                        rc.MobileNo = rcm.MobileNo;
                        rc.Email = rcm.Email;
                        rc.LineId = rcm.LineId;

                        rc.ModifiedDate = DateTime.Now;
                        rc.ModifiedBy = userId;
                        rc.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                        await _userService.RegisterContactRepository.CreateOrUpdateAsync(rc);
                    }
                }
                else
                {   // no entity, add new
                    RegisterContact rc = new RegisterContact
                    {
                        RegisterMerchantId = regMerchantId,
                        ContactType = (byte)RegisterConstant.ContactType.Normal,
                        CorperateName = rcm.CorperateName,
                        Name = rcm.Name,
                        Position = rcm.Position,
                        TelNo = rcm.TelNo,
                        MobileNo = rcm.MobileNo,
                        Email = rcm.Email,
                        LineId = rcm.LineId,

                        AddedDate = DateTime.Now,
                        AddedBy = userId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    };

                    await _userService.RegisterContactRepository.CreateOrUpdateAsync(rc);

                    // add new created entity to contact list
                    contacts.Add(rc);
                }
            }

            // check for delete contact
            int i2 = i;
            for (; i2 < contacts.Count; ++i2)
            {
                await _userService.RegisterContactRepository.DeleteAsync(contacts[i2]);
            }
            contacts.RemoveRange(i, contacts.Count - i);

            return true;
        }

        private async Task<bool> UpdateAddListRegisterContact(RegisterConstant.ContactType type, long regMerchantId, long userId, RegisterContactModel[] conModels)
        {
            if (regMerchantId == 0)
            {
                return false;
            }

            // load current contact list
            var contacts = await _userService.RegisterContactRepository.GetAllByContactTypeAsync(regMerchantId, (byte)type);

            // update contact list
            int i = 0;
            for (; i < conModels.Length; ++i)
            {
                RegisterContactModel rcm = conModels[i];
                if (i < contacts.Count)
                {   // has entity, check changed and update
                    RegisterContact rc = contacts[i];
                    if (ContactDataChange(rc, rcm))
                    {
                        rc.CorperateName = rcm.CorperateName;
                        rc.Name = rcm.Name;
                        rc.Position = rcm.Position;
                        rc.TelNo = rcm.TelNo;
                        rc.MobileNo = rcm.MobileNo;
                        rc.Email = rcm.Email;
                        rc.LineId = rcm.LineId;

                        rc.ModifiedDate = DateTime.Now;
                        rc.ModifiedBy = userId;
                        rc.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                        await _userService.RegisterContactRepository.CreateOrUpdateAsync(rc);
                    }
                }
                else
                {   // no entity, add new
                    RegisterContact rc = new RegisterContact
                    {
                        RegisterMerchantId = regMerchantId,
                        ContactType = (byte)RegisterConstant.ContactType.Normal,
                        CorperateName = rcm.CorperateName,
                        Name = rcm.Name,
                        Position = rcm.Position,
                        TelNo = rcm.TelNo,
                        MobileNo = rcm.MobileNo,
                        Email = rcm.Email,
                        LineId = rcm.LineId,

                        AddedDate = DateTime.Now,
                        AddedBy = userId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    };

                    await _userService.RegisterContactRepository.CreateOrUpdateAsync(rc);

                    // add new created entity to contact list
                    contacts.Add(rc);
                }
            }

            // check for delete contact
            int i2 = i;
            for (; i2 < contacts.Count; ++i2)
            {
                await _userService.RegisterContactRepository.DeleteAsync(contacts[i2]);
            }
            contacts.RemoveRange(i, contacts.Count - i);

            return true;
        }

        private async Task<bool> UpdateSingleRegisterContact(RegisterConstant.ContactType type, long regMerchantId, long userId, RegisterContactModel conModel)
        {
            if (regMerchantId == 0)
            {
                return false;
            }

            var con = await _userService.RegisterContactRepository.GetLatestByContactTypeAsync(regMerchantId, (byte)type);
            if (conModel != null)
            {
                if (con == null)
                {   // create new
                    con = new RegisterContact
                    {
                        RegisterMerchantId = regMerchantId,
                        ContactType = (byte)type,
                        CorperateName = conModel.CorperateName,
                        Name = conModel.Name,
                        Position = conModel.Position,
                        TelNo = conModel.TelNo,
                        MobileNo = conModel.MobileNo,
                        Email = conModel.Email,
                        LineId = conModel.LineId,

                        AddedDate = DateTime.Now,
                        AddedBy = userId,
                        AddedBySystem = (byte)RegisterConstant.UserSystem.Merchant,
                    };

                    await _userService.RegisterContactRepository.CreateOrUpdateAsync(con);

                    return true;
                }
                else
                {   // check changing
                    if (ContactDataChange(con, conModel))
                    {   // has changed, update
                        con.CorperateName = conModel.CorperateName;
                        con.Name = conModel.Name;
                        con.Position = conModel.Position;
                        con.TelNo = conModel.TelNo;
                        con.MobileNo = conModel.MobileNo;
                        con.Email = conModel.Email;
                        con.LineId = conModel.LineId;

                        con.ModifiedDate = DateTime.Now;
                        con.ModifiedBy = userId;
                        con.ModifiedBySystem = (byte)RegisterConstant.UserSystem.Merchant;

                        await _userService.RegisterContactRepository.CreateOrUpdateAsync(con);

                        return true;
                    }
                }
            }
            else
            {
                if (con != null)
                {
                    await _userService.RegisterContactRepository.DeleteAsync(con);

                    return true;
                }
            }

            return false;
        }

        private RegisterChannelModel[] CreateChannelModelList(List<RegisterChannel> dbChannels, /*List<RegisterChannelCondition> dbConditions,*/
            /*List<RegisterChannelFee> dbFees,*/ /*List<RegisterChannelBaseFee> dbBaseFees,*/ /*List<RegisterChannelServiceFee> dbServiceFees,*/
            /*List<RegisterChannelBaseServiceFee> dbBaseServiceFees,*/ RegisterChannelModel[] iniModel)
        {
            //Dictionary<long, List<RegisterChannelCondition>> chConditions = CreateChannelConditionMapping(dbConditions);
            //Dictionary<long, List<RegisterChannelFee>> chFees = CreateChannelFeeMapping(dbFees);
            //Dictionary<long, List<RegisterChannelBaseFee>> chBaseFees = CreateChannelBaseFeeMapping(dbBaseFees);
            //Dictionary<long, List<RegisterChannelServiceFee>> chServiceFees = CreateChannelServiceFeeMapping(dbServiceFees);
            //Dictionary<long, List<RegisterChannelBaseServiceFee>> chBaseServiceFees = CreateChannelBaseServiceFeeMapping(dbBaseServiceFees);

            List<RegisterChannelModel> dataList = new List<RegisterChannelModel>();

            // loop all initial data to create initial list of data
            foreach (RegisterChannelModel im in iniModel)
            {
                int idx = dbChannels.FindIndex(m => m.ChannelServiceCode == im.ChannelServiceCode);
                if (idx >= 0)
                {   // FOUND match data between db and initial data, use model from database
                    RegisterChannel c = dbChannels[idx];
                    //List<RegisterChannelCondition> conditions = null;
                    //List<RegisterChannelFee> fees = null;
                    //List<RegisterChannelBaseFee> baseFees = null;
                    //List<RegisterChannelServiceFee> serviceFees = null;
                    //List<RegisterChannelBaseServiceFee> baseServiceFees = null;

                    //chConditions.TryGetValue(c.Id, out conditions);
                    //chFees.TryGetValue(c.Id, out fees);
                    //chBaseFees.TryGetValue(c.Id, out baseFees);
                    //chServiceFees.TryGetValue(c.Id, out serviceFees);
                    //chBaseServiceFees.TryGetValue(c.Id, out baseServiceFees);

                    dataList.Add(CreateRegisterChannelModel(c/*, conditions, fees, baseFees, serviceFees, baseServiceFees*/));

                    // delete from db list
                    dbChannels.RemoveAt(idx);
                }
                else
                {   // NOT FOUND match data between db and initial data, use initial data
                    dataList.Add(im);
                }
            }

            // loop all remain db data to add to list(data that remaim in list is not match with initial data)
            foreach (RegisterChannel c in dbChannels)
            {
                //List<RegisterChannelCondition> conditions = null;
                //List<RegisterChannelFee> fees = null;
                //List<RegisterChannelBaseFee> baseFees = null;
                //List<RegisterChannelServiceFee> serviceFees = null;
                //List<RegisterChannelBaseServiceFee> baseServiceFees = null;

                //chConditions.TryGetValue(c.Id, out conditions);
                //chFees.TryGetValue(c.Id, out fees);
                //chBaseFees.TryGetValue(c.Id, out baseFees);
                //chServiceFees.TryGetValue(c.Id, out serviceFees);
                //chBaseServiceFees.TryGetValue(c.Id, out baseServiceFees);

                dataList.Add(CreateRegisterChannelModel(c/*, conditions, fees, baseFees, serviceFees, baseServiceFees*/));
            }


            return dataList.ToArray();
        }


        private Dictionary<long, List<RegisterChannelCondition>> CreateChannelConditionMapping(List<RegisterChannelCondition> dbDatas)
        {
            Dictionary<long, List<RegisterChannelCondition>> maps = new Dictionary<long, List<RegisterChannelCondition>>();
            foreach (RegisterChannelCondition data in dbDatas)
            {
                if (!maps.ContainsKey(data.RegisterChannelId))
                {   // create list
                    maps[data.RegisterChannelId] = new List<RegisterChannelCondition>();
                }

                maps[data.RegisterChannelId].Add(data);
            }

            return maps;
        }

        private Dictionary<long, List<RegisterChannelFee>> CreateChannelFeeMapping(List<RegisterChannelFee> dbDatas)
        {
            Dictionary<long, List<RegisterChannelFee>> maps = new Dictionary<long, List<RegisterChannelFee>>();
            foreach (RegisterChannelFee data in dbDatas)
            {
                if (!maps.ContainsKey(data.RegisterChannelId))
                {   // create list
                    maps[data.RegisterChannelId] = new List<RegisterChannelFee>();
                }

                maps[data.RegisterChannelId].Add(data);
            }

            return maps;
        }

        //private Dictionary<long, List<RegisterChannelBaseFee>> CreateChannelBaseFeeMapping(List<RegisterChannelBaseFee> dbDatas)
        //{
        //    Dictionary<long, List<RegisterChannelBaseFee>> maps = new Dictionary<long, List<RegisterChannelBaseFee>>();
        //    foreach (RegisterChannelBaseFee data in dbDatas)
        //    {
        //        if (!maps.ContainsKey(data.RegisterChannelId))
        //        {   // create list
        //            maps[data.RegisterChannelId] = new List<RegisterChannelBaseFee>();
        //        }

        //        maps[data.RegisterChannelId].Add(data);
        //    }

        //    return maps;
        //}

        private Dictionary<long, List<RegisterChannelServiceFee>> CreateChannelServiceFeeMapping(List<RegisterChannelServiceFee> dbDatas)
        {
            Dictionary<long, List<RegisterChannelServiceFee>> maps = new Dictionary<long, List<RegisterChannelServiceFee>>();
            foreach (RegisterChannelServiceFee data in dbDatas)
            {
                if (!maps.ContainsKey(data.RegisterChannelId))
                {   // create list
                    maps[data.RegisterChannelId] = new List<RegisterChannelServiceFee>();
                }

                maps[data.RegisterChannelId].Add(data);
            }

            return maps;
        }

        //private Dictionary<long, List<RegisterChannelBaseServiceFee>> CreateChannelBaseServiceFeeMapping(List<RegisterChannelBaseServiceFee> dbDatas)
        //{
        //    Dictionary<long, List<RegisterChannelBaseServiceFee>> maps = new Dictionary<long, List<RegisterChannelBaseServiceFee>>();
        //    foreach (RegisterChannelBaseServiceFee data in dbDatas)
        //    {
        //        if (!maps.ContainsKey(data.RegisterChannelId))
        //        {   // create list
        //            maps[data.RegisterChannelId] = new List<RegisterChannelBaseServiceFee>();
        //        }

        //        maps[data.RegisterChannelId].Add(data);
        //    }

        //    return maps;
        //}

        private RegisterChannelModel CreateRegisterChannelModel(RegisterChannel entity/*, List<RegisterChannelCondition> conditions,*/
            //List<RegisterChannelFee> fees, List<RegisterChannelBaseFee> baseFees, List<RegisterChannelServiceFee> serviceFees,
            /*List<RegisterChannelBaseServiceFee> baseServiceFees*/)
        {
            return new RegisterChannelModel
            {
                Id = entity.Id,
                //RegisterMerchantId = entity.RegisterMerchantId,
                ChannelServiceCode = entity.ChannelServiceCode,
                GroupName = entity.GroupName,
                ChannelName = entity.ChannelName,
                IsSelected = entity.IsSelected,
                RouteNo = entity.RouteNo,
                //Conditions = CreateRegisterChannelConditionModel(conditions),
                //Fees = CreateRegisterChannelFeeModel(fees),
                //BaseFees = CreateRegisterChannelFeeModel(baseFees),
                //ServiceFees = CreateRegisterChannelServiceFeeModel(serviceFees),
                //BaseServiceFees = CreateRegisterChannelServiceFeeModel(baseServiceFees),
            };
        }

        private RegisterInstallmentModel[] CreateInstallmentModelList(List<RegisterInstallment> dbList, RegisterInstallmentModel[] iniModel)
        {
            List<RegisterInstallmentModel> dataList = new List<RegisterInstallmentModel>();

            // loop all initial data to create initial list of data
            foreach (RegisterInstallmentModel im in iniModel)
            {
                int idx = dbList.FindIndex(m => m.ChannelServiceCode == im.ChannelServiceCode);
                if (idx >= 0)
                {   // FOUND match data between db and initial data, use model from database
                    dataList.Add(CreateRegisterInstallmentModel(dbList[idx]));

                    // delete from db list
                    dbList.RemoveAt(idx);
                }
                else
                {   // NOT FOUND match data between db and initial data, use initial data
                    dataList.Add(im);
                }
            }

            // loop all remain db data to add to list(data that remaim in list is not match with initial data)
            foreach (RegisterInstallment e in dbList)
            {
                dataList.Add(CreateRegisterInstallmentModel(e));
            }

            return dataList.ToArray();
        }

        //private RegisterChannelConditionModel[] CreateRegisterChannelConditionModel(List<RegisterChannelCondition> entities)
        //{
        //    if (entities == null || entities.Count <= 0)
        //    {
        //        return new RegisterChannelConditionModel[0];
        //    }

        //    int length = entities.Count;
        //    RegisterChannelConditionModel[] res = new RegisterChannelConditionModel[length];
        //    for (int i = 0; i < length; ++i)
        //    {
        //        RegisterChannelCondition entity = entities[i];
        //        res[i] = new RegisterChannelConditionModel
        //        {
        //            ConditionType = entity.ConditionType,
        //            ValueLong1 = entity.ValueLong1,
        //            ValueLong2 = entity.ValueLong2,
        //            ValueLong3 = entity.ValueLong3,
        //            ValueLong4 = entity.ValueLong4,
        //            ValueDec1 = entity.ValueDec1,
        //            ValueDec2 = entity.ValueDec2,
        //            ValueDec3 = entity.ValueDec3,
        //            ValueDec4 = entity.ValueDec4,
        //            Description = entity.Description,
        //            Status = entity.Status,
        //        };
        //    }

        //    return res;
        //}

        //private RegisterChannelFeeModel[] CreateRegisterChannelFeeModel(List<RegisterChannelFee> entities)
        //{
        //    if (entities == null || entities.Count <= 0)
        //    {
        //        return new RegisterChannelFeeModel[0];
        //    }

        //    int length = entities.Count;
        //    RegisterChannelFeeModel[] res = new RegisterChannelFeeModel[length];
        //    for (int i = 0; i < length; ++i)
        //    {
        //        RegisterChannelFee entity = entities[i];
        //        res[i] = new RegisterChannelFeeModel
        //        {
        //            FeeAmount = entity.FeeAmount,
        //            FeeMinAmount = entity.FeeMinAmount,
        //            FeeOfferMode = entity.FeeOfferMode,
        //            FeeType = entity.FeeType,
        //            FeeMaxChargePrice = entity.FeeMaxChargePrice,
        //            PaymentMinPrice = entity.PaymentMinPrice,
        //            PaymentMaxPrice = entity.PaymentMaxPrice,
        //            SortNo = entity.SortNo,
        //            CurrencyCode = entity.CurrencyCode,
        //            CurrencyName = entity.CurrencyName,
        //            UpdateDate = entity.ModifiedDate.HasValue ? entity.ModifiedDate.Value : entity.AddedDate,
        //        };
        //    }

        //    return res;
        //}

        //private RegisterChannelFeeModel[] CreateRegisterChannelFeeModel(List<RegisterChannelBaseFee> entities)
        //{
        //    if (entities == null || entities.Count <= 0)
        //    {
        //        return new RegisterChannelFeeModel[0];
        //    }

        //    int length = entities.Count;
        //    RegisterChannelFeeModel[] res = new RegisterChannelFeeModel[length];
        //    for (int i = 0; i < length; ++i)
        //    {
        //        RegisterChannelBaseFee entity = entities[i];
        //        res[i] = new RegisterChannelFeeModel
        //        {
        //            FeeAmount = entity.FeeAmount,
        //            FeeMinAmount = entity.FeeMinAmount,
        //            FeeOfferMode = entity.FeeOfferMode,
        //            FeeType = entity.FeeType,
        //            FeeMaxChargePrice = entity.FeeMaxChargePrice,
        //            PaymentMinPrice = entity.PaymentMinPrice,
        //            PaymentMaxPrice = entity.PaymentMaxPrice,
        //            SortNo = entity.SortNo,
        //            CurrencyCode = entity.CurrencyCode,
        //            CurrencyName = entity.CurrencyName,
        //            UpdateDate = entity.ModifiedDate.HasValue ? entity.ModifiedDate.Value : entity.AddedDate,
        //        };
        //    }

        //    return res;
        //}

        //private RegisterChannelServiceFeeModel[] CreateRegisterChannelServiceFeeModel(List<RegisterChannelServiceFee> entities)
        //{
        //    if (entities == null || entities.Count <= 0)
        //    {
        //        return new RegisterChannelServiceFeeModel[0];
        //    }

        //    int length = entities.Count;
        //    RegisterChannelServiceFeeModel[] res = new RegisterChannelServiceFeeModel[length];
        //    for (int i = 0; i < length; ++i)
        //    {
        //        RegisterChannelServiceFee entity = entities[i];
        //        res[i] = new RegisterChannelServiceFeeModel
        //        {
        //            ServiceRateType = entity.ServiceRateType,
        //            ServiceRateAmount = entity.ServiceRateAmount,
        //            ServiceRateVat = entity.ServiceRateVat,
        //            ServiceRateInVat = entity.ServiceRateInVat,
        //            MinTxAmount = entity.MinTxAmount,
        //            MaxTxAmount = entity.MaxTxAmount,
        //            ServiceWHTRate = entity.ServiceWHTRate,
        //            ServiceMinAmount = entity.ServiceMinAmount,
        //            ServiceConditionBy = entity.ServiceConditionBy,
        //            SortNo = entity.SortNo,
        //            UpdateDate = entity.ModifiedDate.HasValue ? entity.ModifiedDate.Value : entity.AddedDate,
        //        };
        //    }

        //    return res;
        //}

        //private RegisterChannelServiceFeeModel[] CreateRegisterChannelServiceFeeModel(List<RegisterChannelBaseServiceFee> entities)
        //{
        //    if (entities == null || entities.Count <= 0)
        //    {
        //        return new RegisterChannelServiceFeeModel[0];
        //    }

        //    int length = entities.Count;
        //    RegisterChannelServiceFeeModel[] res = new RegisterChannelServiceFeeModel[length];
        //    for (int i = 0; i < length; ++i)
        //    {
        //        RegisterChannelBaseServiceFee entity = entities.ElementAt(i);
        //        res[i] = new RegisterChannelServiceFeeModel
        //        {
        //            ServiceRateType = entity.ServiceRateType,
        //            ServiceRateAmount = entity.ServiceRateAmount,
        //            ServiceRateVat = entity.ServiceRateVat,
        //            ServiceRateInVat = entity.ServiceRateInVat,
        //            MinTxAmount = entity.MinTxAmount,
        //            MaxTxAmount = entity.MaxTxAmount,
        //            ServiceWHTRate = entity.ServiceWHTRate,
        //            ServiceMinAmount = entity.ServiceMinAmount,
        //            ServiceConditionBy = entity.ServiceConditionBy,
        //            SortNo = entity.SortNo,
        //            UpdateDate = entity.ModifiedDate.HasValue ? entity.ModifiedDate.Value : entity.AddedDate,
        //        };
        //    }

        //    return res;
        //}

        private RegisterInstallmentModel CreateRegisterInstallmentModel(RegisterInstallment entity)
        {
            return new RegisterInstallmentModel
            {
                Id = entity.Id,
                //RegisterMerchantId = entity.RegisterMerchantId,
                ChannelServiceCode = entity.ChannelServiceCode,
                RequireChannelServiceCode = entity.RequireChannelServiceCode,
                ChannelName = entity.ChannelName,
                IsSelected = entity.IsSelected,
                RouteNo = entity.RouteNo,
            };
        }

        private async Task AddRegisterChannelDb(long registerMerchantId, long userId, byte userSystem, RegisterChannelModel cm, RegisterChannelInfoModel info, int idx)
        {
            if (cm == null)
            {
                return;
            }

            // add new RegisterChannel
            RegisterChannel c = new RegisterChannel
            {
                RegisterMerchantId = userId /*registerMerchantId*/,
                ChannelServiceCode = cm.ChannelServiceCode,
                GroupName = cm.GroupName,
                ChannelName = cm.ChannelName,
                IsSelected = info.Channels[idx].IsSelected,
                AddedDate = DateTime.Now,
                AddedBy = userId,
                AddedBySystem = userSystem,
                RouteNo = cm.RouteNo,
            };
            var res = await _userService.RegisterChannelRepository.CreateOrUpdateAsync(c);

            cm.Id = c.Id;
        }

        private async Task AddChannelDb(long registerMerchantId, long userId, byte userSystem, RegisterChannelModel cm)
        {
            if (cm == null)
            {
                return;
            }

            // add new RegisterChannel
            RegisterChannel c = new RegisterChannel
            {
                RegisterMerchantId = userId /*registerMerchantId*/,
                ChannelServiceCode = cm.ChannelServiceCode,
                GroupName = cm.GroupName,
                ChannelName = cm.ChannelName,
                IsSelected = cm.IsSelected,
                AddedDate = DateTime.Now,
                AddedBy = userId,
                AddedBySystem = userSystem,
                RouteNo = cm.RouteNo,
            };
            var res = await _userService.RegisterChannelRepository.CreateOrUpdateAsync(c);

            // set id back
            cm.Id = c.Id;

            //// add condition
            //foreach (RegisterChannelConditionModel ccm in cm.Conditions)
            //{
            //    var res2 = await _userService.RegisterChannelConditionRepository.CreateOrUpdateAsync(new RegisterChannelCondition
            //    {
            //        RegisterMerchantId = c.RegisterMerchantId,
            //        RegisterChannelId = c.Id,
            //        ConditionType = ccm.ConditionType,
            //        ValueLong1 = ccm.ValueLong1,
            //        ValueLong2 = ccm.ValueLong2,
            //        ValueLong3 = ccm.ValueLong3,
            //        ValueLong4 = ccm.ValueLong4,
            //        ValueDec1 = ccm.ValueDec1,
            //        ValueDec2 = ccm.ValueDec2,
            //        ValueDec3 = ccm.ValueDec3,
            //        ValueDec4 = ccm.ValueDec4,
            //        Description = ccm.Description,
            //        Status = ccm.Status,
            //        AddedDate = DateTime.Now,
            //        AddedBy = userId,
            //        AddedBySystem = userSystem,
            //    });
            //}

            //// add fee
            //foreach (RegisterChannelFeeModel cfm in cm.Fees)
            //{
            //    var res2 = await _userService.RegisterChannelFeeRepository.CreateOrUpdateAsync(new RegisterChannelFee
            //    {
            //        RegisterMerchantId = c.RegisterMerchantId,
            //        RegisterChannelId = c.Id,
            //        FeeAmount = cfm.FeeAmount,
            //        FeeMinAmount = cfm.FeeMinAmount,
            //        FeeOfferMode = cfm.FeeOfferMode,
            //        FeeType = cfm.FeeType,
            //        FeeMaxChargePrice = cfm.FeeMaxChargePrice,
            //        PaymentMinPrice = cfm.PaymentMinPrice,
            //        PaymentMaxPrice = cfm.PaymentMaxPrice,
            //        SortNo = cfm.SortNo,
            //        AddedDate = DateTime.Now,
            //        AddedBy = userId,
            //        AddedBySystem = userSystem,
            //        CurrencyCode = cfm.CurrencyCode,
            //        CurrencyName = cfm.CurrencyName,
            //    });
            //}

            //// add base fee
            //foreach (RegisterChannelFeeModel cfm in cm.BaseFees)
            //{
            //    var res2 = await _userService.RegisterChannelBaseFeeRepository.CreateOrUpdateAsync(new RegisterChannelBaseFee
            //    {
            //        RegisterMerchantId = c.RegisterMerchantId,
            //        RegisterChannelId = c.Id,
            //        FeeAmount = cfm.FeeAmount,
            //        FeeMinAmount = cfm.FeeMinAmount,
            //        FeeOfferMode = cfm.FeeOfferMode,
            //        FeeType = cfm.FeeType,
            //        FeeMaxChargePrice = cfm.FeeMaxChargePrice,
            //        PaymentMinPrice = cfm.PaymentMinPrice,
            //        PaymentMaxPrice = cfm.PaymentMaxPrice,
            //        SortNo = cfm.SortNo,
            //        AddedDate = DateTime.Now,
            //        AddedBy = userId,
            //        AddedBySystem = userSystem,
            //        CurrencyCode = cfm.CurrencyCode,
            //        CurrencyName = cfm.CurrencyName,
            //    });
            //}

            //// add service fee
            //foreach (RegisterChannelServiceFeeModel csfm in cm.ServiceFees)
            //{
            //    var res2 = await _userService.RegisterChannelServiceFeeRepository.CreateOrUpdateAsync(new RegisterChannelServiceFee
            //    {
            //        RegisterMerchantId = c.RegisterMerchantId,
            //        RegisterChannelId = c.Id,
            //        ServiceRateType = csfm.ServiceRateType,
            //        ServiceRateAmount = csfm.ServiceRateAmount,
            //        ServiceRateVat = csfm.ServiceRateVat,
            //        ServiceRateInVat = csfm.ServiceRateInVat,
            //        MinTxAmount = csfm.MinTxAmount,
            //        MaxTxAmount = csfm.MaxTxAmount,
            //        ServiceWHTRate = csfm.ServiceWHTRate,
            //        ServiceMinAmount = csfm.ServiceMinAmount,
            //        ServiceConditionBy = csfm.ServiceConditionBy,
            //        SortNo = csfm.SortNo,
            //        AddedDate = DateTime.Now,
            //        AddedBy = userId,
            //        AddedBySystem = userSystem,
            //    });
            //}

            //// add base service fee
            //foreach (RegisterChannelServiceFeeModel csfm in cm.BaseServiceFees)
            //{
            //    var res2 = await _userService.RegisterChannelBaseServiceFeeRepository.CreateOrUpdateAsync(new RegisterChannelBaseServiceFee
            //    {
            //        RegisterMerchantId = c.RegisterMerchantId,
            //        RegisterChannelId = c.Id,
            //        ServiceRateType = csfm.ServiceRateType,
            //        ServiceRateAmount = csfm.ServiceRateAmount,
            //        ServiceRateVat = csfm.ServiceRateVat,
            //        ServiceRateInVat = csfm.ServiceRateInVat,
            //        MinTxAmount = csfm.MinTxAmount,
            //        MaxTxAmount = csfm.MaxTxAmount,
            //        ServiceWHTRate = csfm.ServiceWHTRate,
            //        ServiceMinAmount = csfm.ServiceMinAmount,
            //        ServiceConditionBy = csfm.ServiceConditionBy,
            //        SortNo = csfm.SortNo,
            //        AddedDate = DateTime.Now,
            //        AddedBy = userId,
            //        AddedBySystem = userSystem,
            //    });
            //}
        }

        private async Task<ApiResponseMessageModel<string>> UpdateChannelDb(long registerMerchantId, long userId, byte userSystem, RegisterChannelModel cm)
        {
            ApiResponseMessageModel<string> ret = ApiResponseMessageModel.Failed("Unknown error");
            if (cm == null)
            {
                ret.Message = "Model is null";
                return ret;
            }

            RegisterChannel c = await _userService.RegisterChannelRepository.FindByIdAsync(cm.Id);
            if (c == null)
            {
                ret.Message = "Not found channel";
                return ret;
            }
            if (c.RegisterMerchantId != registerMerchantId)
            {
                ret.Message = "Mismatch RegisterMerchantId";
                return ret;
            }
            if (c.ChannelServiceCode != cm.ChannelServiceCode)
            {
                ret.Message = "Mismatch ChannelServiceCode";
                return ret;
            }

            if (ChannelDataChange(c, cm))
            {
                c.IsSelected = cm.IsSelected;
                c.RouteNo = cm.RouteNo;
                c.ModifiedDate = DateTime.Now;
                c.ModifiedBy = userId;
                c.ModifiedBySystem = userSystem;

                await _userService.RegisterChannelRepository.CreateOrUpdateAsync(c);
            }

            //// channel condition comparing
            //if (cm.Conditions == null)
            //    cm.Conditions = new RegisterChannelConditionModel[0];
            //List<RegisterChannelCondition> entityList = await _userService.RegisterChannelConditionRepository.GetAllByRegisterChannelIdAsync(c.Id);
            //int dbCnt = entityList.Count;
            //int mCnt = cm.Conditions.Length;
            //int loopCnt = (dbCnt >= mCnt) ? dbCnt : mCnt;
            //for (int i = 0; i < loopCnt; ++i)
            //{
            //    RegisterChannelCondition entity = (i < dbCnt) ? entityList[i] : null;
            //    RegisterChannelConditionModel model = (i < mCnt) ? cm.Conditions[i] : null;

            //    if (entity == null)
            //    {
            //        if (model != null)
            //        {   // add new
            //            RegisterChannelCondition newEntity = new RegisterChannelCondition
            //            {
            //                RegisterMerchantId = registerMerchantId,
            //                RegisterChannelId = c.Id,
            //                ConditionType = model.ConditionType,
            //                ValueLong1 = model.ValueLong1,
            //                ValueLong2 = model.ValueLong2,
            //                ValueLong3 = model.ValueLong3,
            //                ValueLong4 = model.ValueLong4,
            //                ValueDec1 = model.ValueDec1,
            //                ValueDec2 = model.ValueDec2,
            //                ValueDec3 = model.ValueDec3,
            //                ValueDec4 = model.ValueDec4,
            //                Description = model.Description,
            //                Status = model.Status,
            //                AddedDate = DateTime.Now,
            //                AddedBy = userId,
            //                AddedBySystem = userSystem,
            //            };
            //            await _userService.RegisterChannelConditionRepository.CreateOrUpdateAsync(newEntity);
            //        }
            //    }
            //    else
            //    {
            //        if (model != null)
            //        {   // check changing for update
            //            if (ChannelConditionDataChange(entity, model))
            //            {
            //                entity.ConditionType = model.ConditionType;
            //                entity.ValueLong1 = model.ValueLong1;
            //                entity.ValueLong2 = model.ValueLong2;
            //                entity.ValueLong3 = model.ValueLong3;
            //                entity.ValueLong4 = model.ValueLong4;
            //                entity.ValueDec1 = model.ValueDec1;
            //                entity.ValueDec2 = model.ValueDec2;
            //                entity.ValueDec3 = model.ValueDec3;
            //                entity.ValueDec4 = model.ValueDec4;
            //                entity.Description = model.Description;
            //                entity.Status = model.Status;
            //                entity.ModifiedDate = DateTime.Now;
            //                entity.ModifiedBy = userId;
            //                entity.ModifiedBySystem = userSystem;

            //                await _userService.RegisterChannelConditionRepository.CreateOrUpdateAsync(entity);
            //            }
            //        }
            //        else
            //        {   // delete
            //            await _userService.RegisterChannelConditionRepository.DeleteAsync(entity);
            //        }
            //    }
            //}



            ret = ApiResponseMessageModel.Success("Success");
            return ret;
        }

        private async Task DeleteChannelDb(RegisterChannel c)
        {
            if (c == null)
            {
                return;
            }

            // delete condition
            List<RegisterChannelCondition> conditions = await _userService.RegisterChannelConditionRepository.GetAllByRegisterChannelIdAsync(c.Id);
            foreach (RegisterChannelCondition entity in conditions)
            {
                await _userService.RegisterChannelConditionRepository.DeleteAsync(entity);
            }

            // delete fee
            List<RegisterChannelFee> fees = await _userService.RegisterChannelFeeRepository.GetAllByRegisterChannelIdAsync(c.Id);
            foreach (RegisterChannelFee entity in fees)
            {
                await _userService.RegisterChannelFeeRepository.DeleteAsync(entity);
            }

            //// delete base fee
            //List<RegisterChannelBaseFee> baseFees = await _userService.RegisterChannelBaseFeeRepository.GetAllByRegisterChannelIdAsync(c.Id);
            //foreach (RegisterChannelBaseFee entity in baseFees)
            //{
            //    await _userService.RegisterChannelBaseFeeRepository.DeleteAsync(entity);
            //}

            // delete service fee
            List<RegisterChannelServiceFee> serviceFees = await _userService.RegisterChannelServiceFeeRepository.GetAllByRegisterChannelIdAsync(c.Id);
            foreach (RegisterChannelServiceFee entity in serviceFees)
            {
                await _userService.RegisterChannelServiceFeeRepository.DeleteAsync(entity);
            }

            //// delete base service fee
            //List<RegisterChannelBaseServiceFee> baseServiceFees = await _userService.RegisterChannelBaseServiceFeeRepository.GetAllByRegisterChannelIdAsync(c.Id);
            //foreach (RegisterChannelBaseServiceFee entity in baseServiceFees)
            //{
            //    await _userService.RegisterChannelBaseServiceFeeRepository.DeleteAsync(entity);
            //}

            await _userService.RegisterChannelRepository.DeleteAsync(c);
        }

        private async Task AddRegisterInstallmentDb(long registerMerchantId, long userId, byte userSystem, RegisterInstallmentModel im, RegisterChannelInfoModel info, int idx)
        {
            if (im == null)
            {
                return;
            }

            // add new RegisterInstallment
            RegisterInstallment i = new RegisterInstallment
            {
                RegisterMerchantId = userId /*registerMerchantId*/,
                ChannelServiceCode = im.ChannelServiceCode,
                RequireChannelServiceCode = im.RequireChannelServiceCode,
                ChannelName = im.ChannelName,
                IsSelected = info.Installments[idx].IsSelected,
                AddedDate = DateTime.Now,
                AddedBy = userId,
                AddedBySystem = userSystem,
                RouteNo = im.RouteNo,
            };

            var res = await _userService.RegisterInstallmentRepository.CreateOrUpdateAsync(i);

            // set id back
            im.Id = i.Id;
        }

        private async Task AddInstallmentDb(long registerMerchantId, long userId, byte userSystem, RegisterInstallmentModel im)
        {
            if (im == null)
            {
                return;
            }

            // add new RegisterInstallment
            RegisterInstallment i = new RegisterInstallment
            {
                RegisterMerchantId = userId /*registerMerchantId*/,
                ChannelServiceCode = im.ChannelServiceCode,
                RequireChannelServiceCode = im.RequireChannelServiceCode,
                ChannelName = im.ChannelName,
                IsSelected = im.IsSelected,
                AddedDate = DateTime.Now,
                AddedBy = userId,
                AddedBySystem = userSystem,
                RouteNo = im.RouteNo,
            };

            var res = await _userService.RegisterInstallmentRepository.CreateOrUpdateAsync(i);

            // set id back
            im.Id = i.Id;
        }

        private bool RegisterDataChange(RegisterMerchant entity, RegisterGeneralInfoModel model)
        {
            return !entity.RegisterType.Equals(model.RegisterType) ||
                entity.RegisterNameEN != model.RegisterNameEN ||
                entity.RegisterNameTH != model.RegisterNameTH ||
                //entity.RegisterFirstnameEN != model.RegisterFirstnameEN ||
                //entity.RegisterLastnameEN != model.RegisterLastnameEN ||
                //entity.RegisterFirstnameTH != model.RegisterFirstnameTH ||
                //entity.RegisterLastnameTH != model.RegisterLastnameTH ||
                entity.BrandNameEN != model.BrandNameEN ||
                entity.BrandNameTH != model.BrandNameTH ||
                //entity.LogoImageUrl != model.LogoImageUrl ||
                entity.TaxId != model.TaxId ||
                entity.UseRegisterAddressForBilling.Equals(model.UseRegisterAddressForBilling);
        }
        
        private bool AddressDataChange(RegisterAddress entity, RegisterAddressModel model)
        {
            return entity.Address != model.Address ||
                !entity.ProvinceId.Equals(model.ProvinceId) ||
                !entity.DistrictId.Equals(model.DistrictId) ||
                !entity.SubdistrictId.Equals(model.SubdistrictId) ||
                !entity.ZipCode.Equals(model.ZipCode) ||
                entity.TelNo != model.TelNo ||
                entity.Fax != model.Fax;
        }
        
        private bool RegisterDataChange(RegisterMerchant entity, RegisterDetailInfoModel model)
        {
            return entity.DomainName != model.DomainName ||
                entity.ProductType != model.ProductType ||
                !entity.ProductMinPrice.Equals(model.ProductMinPrice) ||
                !entity.ProductMaxPrice.Equals(model.ProductMaxPrice) ||
                !entity.EstimateSales.Equals(model.EstimateSales) ||
                !entity.BankAccountBrandId.Equals(model.BankAccountBrandId) ||
                entity.BankAccountBranch != model.BankAccountBranch ||
                !entity.BankAccountTypeId.Equals(model.BankAccountTypeId) ||
                entity.BankAccountName != model.BankAccountName ||
                entity.BankAccountNo != model.BankAccountNo;
        }
        
        private bool RegisterDataChange(RegisterMerchant entity, RegisterWebInfoModel model)
        {
            return entity.MerchantServerPrdIPAddress != model.MerchantServerPrdIPAddress ||
                entity.MerchanrServerPrdUrl != model.MerchanrServerPrdUrl ||
                !entity.UseUrlBackground.Equals(model.UseUrlBackground) ||
                entity.UrlBackgroundPrd != model.UrlBackgroundPrd ||
                !entity.UseSSL.Equals(model.UseSSL) ||
                entity.SSLType != model.SSLType ||
                entity.SSLBy != model.SSLBy ||
                !entity.SSLCreateDate.Equals(model.SSLCreateDate) ||
                !entity.SSLExpireDate.Equals(model.SSLExpireDate) ||
                !entity.SSLLocationType.Equals(model.SSLLocationType);
        }

        private bool ContactDataChange(RegisterContact entity, RegisterContactModel model)
        {
            return entity.CorperateName != model.CorperateName ||
                entity.Name != model.Name ||
                entity.Position != model.Position ||
                entity.TelNo != model.TelNo ||
                entity.MobileNo != model.MobileNo ||
                entity.Email != model.Email ||
                entity.LineId != model.LineId;
        }

        private bool ChannelDataChange(RegisterChannel entity, RegisterChannelModel model)
        {
            return entity.ChannelServiceCode != model.ChannelServiceCode ||
                entity.IsSelected != model.IsSelected ||
                entity.RouteNo != model.RouteNo;
        }

        //private bool RegisterDataChange(RegisterMerchant entity, RegisterChannelInfoModel model)
        //{
        //    return !entity.EntranceFeeBase.Equals(model.EntranceFeeBase) ||
        //        !entity.EntranceFee.Equals(model.EntranceFee) ||
        //        !entity.MonthlyFeeBase.Equals(model.MonthlyFeeBase) ||
        //        !entity.MonthlyFee.Equals(model.MonthlyFee) ||
        //        !entity.MerchantTransferFeeBase.Equals(model.MerchantTransferFeeBase) ||
        //        !entity.MerchantTransferFee.Equals(model.MerchantTransferFee) ||
        //        entity.AdditionNote != model.AdditionNote;
        //}
    }
}
