using HOP_CFP_Backend.Library.Models;
using HOP_CFP_Backend.Library.Models.Manager;
using HOP_CFP_Backend.Library.Utility;
using HOP_CFP_Backend.Utility;
using HOP_CFP_Backend.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using SmartExpoIoT.ViewModels.Api;
using System.Security.Cryptography;
using System.Text;

namespace HOP_CFP_Backend.Services
{
    public class ManagerService : _StandardService<Manager, ManagerModel, ManagerSearchViewModel, ManagerListViewModel, ManagerListDataModel>
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// 由 ApiFilter 在每次請求時設定，代表目前已驗證的使用者
        /// </summary>
        public ManagerSessionModel? CurrentManager { get; set; }

        public ManagerService(BaseServiceArgument argument, IMemoryCache cache) : base(argument)
        {
            _cache = cache;
        }

        protected override async Task SetModel(ManagerModel viewModel)
        {
            await base.SetModel(viewModel);

            viewModel.RoleId = await _lazy.ManyToManyService.Value.GetTargetId<Role>(viewModel.Id);
        }

        protected override async Task ModelSave(ManagerModel viewModel)
        {
            await base.ModelSave(viewModel);

            if (viewModel.RoleId.HasValue) 
            {
                await _lazy.ManyToManyService.Value.SaveById(viewModel, new List<Guid> { viewModel.RoleId.Value }, nameof(Role));
            }
        }

        /// <summary>
        /// 檢查Manager最後登入時間跟權限修改時間比對
        /// </summary>
        /// <param name="managerId"></param>
        /// <returns></returns>
        public async Task<DateTime?> CheckManagerRoleUpdateDate(Guid managerId)
        {
            DateTime? result = null;
            string sql = $@" SELECT MAX(LastDate)
                               FROM (
                                   SELECT UpdateDate AS LastDate
                                   FROM Manager
                                   WHERE ManagerId = @ManagerId

                                   UNION ALL

                                   SELECT r.UpdateDate
                                   FROM ManagerByRole mbr
                                   JOIN [Role] r ON mbr.RoleId = r.RoleId
                                   WHERE mbr.ManagerId = @ManagerId
                               ) AS AllDates";

            //result = await QueryFirstAsync<DateTime?>(sql, new { ManagerId = managerId });
            return DateTime.Now.AddDays(-1);
        }

        public async Task SetSessionManagerInfo(Guid managerId)
        {
            // List<Task> tasks = new List<Task>();
            // LoginManagerInfo managerInfo = new LoginManagerInfo();
            // List<AdminMenuByRoleViewModel> roleMenuList = new List<AdminMenuByRoleViewModel>();
            // List<AdminMenuViewModel> adminMenus = new List<AdminMenuViewModel>();
            // List<Guid> manageAccounts = new();
            // EEventRole? eventRole = null;
            // Guid? eventId = null;
            //
            // tasks.AddTask(async () =>
            // {
            //     managerInfo = await GetLoginManagerInfoAsync(managerId);
            //
            //     if (managerInfo.CompanyId.HasValue)
            //     {
            //         manageAccounts = await GetManageAccountsByCompany(managerInfo.CompanyId.Value);
            //     }
            // });
            // tasks.AddTask(async () =>
            // {
            //     roleMenuList = await GetRoleMenuListByManager(managerId);
            //     adminMenus = await GetAdminMenuListByManager(roleMenuList);
            // });
            // tasks.AddTask(async () =>
            // {
            //     eventRole = await GetEventRoleByManager(managerId);
            // });
            // tasks.AddTask(async () =>
            // {
            //     IEnumerable<Guid> eventIds = await _lazy.ManyToManyService.Value.GetIdsBySource(managerId, nameof(Events));
            //     if (eventIds.Count() > 0)
            //     {
            //         eventId = eventIds.FirstOrDefault();
            //     }
            // });
            // await Task.WhenAll(tasks);
            //
            // SessionManagerInfo = new SessionManagerInfo
            // {
            //     ManagerId = managerId,
            //     Name = managerInfo.Name,
            //     RoleMenuList = roleMenuList,
            //     AdminMenuList = adminMenus,
            //     LastPasswordChangeDate = managerInfo.LastPasswordChangeDate,
            //     LastApplyRoleDate = SystemVariable.Now,
            //     CompanyId = managerInfo.CompanyId,
            //     ManageAccounts = manageAccounts,
            //     EventRole = eventRole,
            //     EventId = eventId
            // };
        }

        /// <summary>
        /// 註冊新的 Manager 帳號
        /// </summary>
        public async Task<ApiResult<object>> Register(ManagerRegisterViewModel viewModel)
        {
            string checkAccountSql = "SELECT COUNT(*) FROM Manager WHERE Account = @Account AND Status != -1";
            int accountCount = await QueryFirstAsync<int>(checkAccountSql, new { viewModel.Account });
            if (accountCount > 0)
                return new ApiResult<object>().SetError("帳號已存在");

            string checkEmailSql = "SELECT COUNT(*) FROM Manager WHERE Email = @Email AND Status != -1";
            int emailCount = await QueryFirstAsync<int>(checkEmailSql, new { viewModel.Email });
            if (emailCount > 0)
                return new ApiResult<object>().SetError("Email 已被使用");

            byte[] passwordHash = GenPasswordHash(viewModel.Password);

            Guid id = Guid.NewGuid();

            Manager manager = new Manager
            {
                Id = id,
                Account = viewModel.Account,
                Email = viewModel.Email,
                Name = viewModel.Name,
                TaxID = viewModel.TaxID,
                PasswordHash = passwordHash,
                EmailConfirm = false,
                LastPasswordChangeDate = DateTime.Now,
                Status = EStatus.Enable,
                CreateUserId = id
            };
            await InsertAsync(manager);

            await CheckInsertRoleByTaxID(manager.Id, manager.TaxID);

            return new ApiResult<object>().SetSuccess(null, "註冊成功");
        }

        /// <summary>
        /// Manager 帳號登入
        /// </summary>
        public async Task<ApiResult<LoginInfoModel>> Login(ManagerLoginViewModel viewModel)
        {
            ApiResult<LoginInfoModel> result = new();

            string ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            string sql = "SELECT * FROM Manager WHERE Account = @Account AND Status != -1";
            Manager manager = await QueryFirstAsync<Manager>(sql, new { viewModel.Account });

            if (manager == null)
            {
                await InsertAsync(new Log_ManagerLoginFail { Id = Guid.NewGuid(), Account = viewModel.Account, IPAddress = ip });
                return result.SetError("帳號或密碼錯誤");
            }

            byte[] passwordHash = GenPasswordHash(viewModel.Password);
            if (!passwordHash.SequenceEqual(manager.PasswordHash ?? Array.Empty<byte>()))
            {
                await InsertAsync(new Log_ManagerLoginFail { Id = Guid.NewGuid(), Account = viewModel.Account, IPAddress = ip });
                return result.SetError("帳號或密碼錯誤");
            }

            if (manager.Status == EStatus.Disable)
                return result.SetError("帳號已停用，請聯絡系統管理人員。");


            IEnumerable<Guid> roleIds = await _lazy.ManyToManyService.Value.GetIdsBySource(manager.Id, nameof(Role));
            if (!roleIds.Any()) 
            {
                await CheckInsertRoleByTaxID(manager.Id, manager.TaxID);
            }

            Log_ManagerLogin log_ManagerLogin = new Log_ManagerLogin
            {
                Id = Guid.NewGuid(),
                ManagerId = manager.Id,
                ActionType = ELogin.Login,
                IPAddress = ip
            };
            await InsertAsync(log_ManagerLogin);

            ManagerSessionModel managerSession = manager.CastBy<ManagerSessionModel>();
            managerSession.AdminMenus = (await _lazy.RoleService.Value.GetRoleAdminMenus(manager.Id)).ToList();

            _cache.Set(log_ManagerLogin.Id, managerSession
            , new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(20)
            });

            LoginInfoModel loginInfo = new LoginInfoModel
            {
                Token = log_ManagerLogin.Id,
                Name = manager.Name,
                AdminMenus = managerSession.AdminMenus
            };

            return result.SetSuccess(loginInfo);
        }

        /// <summary>
        /// 產生數學驗證碼，儲存答案於 cache 並回傳題目
        /// </summary>
        public CaptchaViewModel GenerateCaptcha()
        {
            var rng = Random.Shared;
            int a = rng.Next(1, 10);
            int b = rng.Next(1, 10);
            string captchaId = Guid.NewGuid().ToString("N");
            _cache.Set($"captcha_{captchaId}", a + b, TimeSpan.FromMinutes(10));
            return new CaptchaViewModel { CaptchaId = captchaId, Question = $"{a} + {b} = ?" };
        }

        /// <summary>
        /// 忘記密碼：驗證 email、產生 token、寄送重設連結
        /// </summary>
        public async Task<ApiResult<object>> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            string sql = "SELECT * FROM Manager WHERE Email = @Email AND Status != -1";
            Manager manager = await QueryFirstAsync<Manager>(sql, new { viewModel.Email });

            // 不論 email 是否存在，都回傳成功（避免洩漏帳號資訊）
            if (manager != null)
            {
                string token = Guid.NewGuid().ToString("N");
                _cache.Set($"reset_pw_{token}", manager.Id, TimeSpan.FromMinutes(30));

                string frontendUrl = _configuration.GetValue<string>("SiteSettings:FrontendUrl") ?? "https://localhost:3000";
                string resetLink = $"{frontendUrl}/reset-password?token={token}";
                string mailBody = $@"
                    <p>您好 {manager.Name}，</p>
                    <p>我們收到您的密碼重設請求，請點擊以下連結重設密碼（30 分鐘內有效）：</p>
                    <p><a href=""{resetLink}"">{resetLink}</a></p>
                    <p>若您未申請重設密碼，請忽略此郵件。</p>";

                await _mailSender.SentAsync(viewModel.Email, "重設密碼", mailBody);
            }

            return new ApiResult<object>().SetSuccess(null, "若此 Email 已註冊，重設連結已發送至您的信箱");
        }

        /// <summary>
        /// 重設密碼：驗證 token，更新密碼
        /// </summary>
        public async Task<ApiResult<object>> ResetPassword(ResetPasswordViewModel viewModel)
        {
            string cacheKey = $"reset_pw_{viewModel.Token}";
            if (!_cache.TryGetValue(cacheKey, out Guid managerId))
                return new ApiResult<object>().SetError("連結已失效或不存在，請重新申請忘記密碼");

            byte[] passwordHash = GenPasswordHash(viewModel.NewPassword);
            string sql = "UPDATE Manager SET PasswordHash = @PasswordHash, LastPasswordChangeDate = @Now WHERE Id = @Id";
            await ExecuteAsync(sql, new { PasswordHash = passwordHash, Now = DateTime.Now, Id = managerId });

            _cache.Remove(cacheKey);
            return new ApiResult<object>().SetSuccess(null, "密碼重設成功，請重新登入");
        }

        private byte[] GenPasswordHash(string password)
        {
            string newPassword = password + "|4568A7FF-B35B-4F20-922A-AB2D2B56AECF"; // 加入固定鹽值
            return SHA256.HashData(Encoding.UTF8.GetBytes(newPassword));
        }

        private async Task CheckInsertRoleByTaxID(Guid managerId, string? taxID) 
        {
            string checkTaxIDSql = $@"select RowNum 
                                        from ( SELECT M.Id, ROW_NUMBER() OVER (ORDER BY CreateDate) AS RowNum
                                                 FROM Manager M
                                                where TaxID = @TaxID AND [Status] != -1
                                                ) a
                                      where Id = @Id";
            int? taxIDRownum = await QueryFirstAsync<int?>(checkTaxIDSql, new { TaxID = taxID, Id = managerId });

            RoleType? roleType = RoleType.新註冊;
            if (!roleType.HasValue
                || taxIDRownum == 1) { roleType = RoleType.公司管理員; }

            Role role = await _lazy.RoleService.Value.GetDataByType(roleType.Value);
            if (role == null) { return; }

            ManyToMany many = new ManyToMany
            {
                SourceTable = nameof(Manager),
                SourceId = managerId,
                TargetTable = nameof(Role),
                TargetId = role.Id
            };
            await InsertAsync(many);
        }

    }
}
