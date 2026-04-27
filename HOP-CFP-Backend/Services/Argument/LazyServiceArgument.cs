using System;
using HOP_CFP_Backend.Library.Repositories;
using HOP_CFP_Backend.Services;

namespace HOP_CFP_Backend.Argument
{
    public class LazyServiceArgument
    {
        public Lazy<BaseService> BaseService { get; set; }
        public Lazy<ManagerService> ManagerService { get; set; }
        public Lazy<SysConfigService> SysConfigService { get; set; }
        public Lazy<IDbConnectionFactory> DbConnectionFactory { get; set; }
        public Lazy<ManyToManyService> ManyToManyService { get; set; }
        public Lazy<UploadFileService> UploadFileService { get; set; }
        public Lazy<SupplierService> SupplierService { get; set; }
        public Lazy<MaterialService> MaterialService { get; set; }
        public Lazy<MaterialGroupService> MaterialGroupService { get; set; }
        public Lazy<MaterialNotifyService> MaterialNotifyService { get; set; }
        public Lazy<AdminFunctionService> AdminFunctionService { get; set; }
        public Lazy<AdminMenuService> AdminMenuService { get; set; }
        public Lazy<MaterialCompareService> MaterialCompareService { get; set; }
        public Lazy<MaterialSpecService> MaterialSpecService { get; set; }
        public Lazy<StatusQueryService> StatusQueryService { get; set; }
        public Lazy<NotifyStatusReportService> NotifyStatusReportService { get; set; }
        public Lazy<SellerCompareService> SellerCompareService { get; set; }
        public Lazy<BuyerCompareService> BuyerCompareService { get; set; }
        public Lazy<RoleService> RoleService { get; set; }
        //

        public LazyServiceArgument(
             Lazy<BaseService> baseService,
             Lazy<ManagerService> managerService,
             Lazy<SysConfigService> sysConfigService,
             Lazy<ManyToManyService> manyToManyService,
             Lazy<IDbConnectionFactory> dbConnectionFactory,
             Lazy<UploadFileService> uploadFileService,
             Lazy<SupplierService> supplierService,
             Lazy<MaterialService> materialService,
             Lazy<MaterialGroupService> materialGroupService,
             Lazy<MaterialNotifyService> materialNotifyService,
             Lazy<AdminFunctionService> adminFunctionService,
             Lazy<MaterialCompareService> materialCompareService,
             Lazy<MaterialSpecService> materialSpecService,
             Lazy<StatusQueryService> statusQueryService,
             Lazy<NotifyStatusReportService> notifyStatusReportService,
             Lazy<SellerCompareService> sellerCompareService,
             Lazy<BuyerCompareService> buyerCompareService,
             Lazy<RoleService> roleService,
             Lazy<AdminMenuService> adminMenuService)
        {
            BaseService = baseService;
            ManagerService = managerService;
            SysConfigService = sysConfigService;
            DbConnectionFactory = dbConnectionFactory;
            ManyToManyService = manyToManyService;
            UploadFileService = uploadFileService;
            SupplierService = supplierService;
            MaterialService = materialService;
            MaterialGroupService = materialGroupService;
            MaterialNotifyService = materialNotifyService;
            AdminFunctionService = adminFunctionService;
            AdminMenuService = adminMenuService;
            MaterialCompareService = materialCompareService;
            MaterialSpecService = materialSpecService;
            StatusQueryService = statusQueryService;
            NotifyStatusReportService = notifyStatusReportService;
            SellerCompareService = sellerCompareService;
            BuyerCompareService = buyerCompareService;
            RoleService = roleService;
        }

    }
}