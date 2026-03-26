# HDP-CFP-Backend 專案索引

> **維護規則**：每次新增 / 修改 / 刪除 Controller、Service、Model、ViewModel、Filter、Utility 時，
> 請同步更新本文件對應章節，以確保索引準確。

---

## 目錄
1. [專案概覽](#1-專案概覽)
2. [架構分層](#2-架構分層)
3. [DI 注入模式](#3-di-注入模式)
4. [驗證與授權流程](#4-驗證與授權流程)
5. [Base Classes 繼承鏈](#5-base-classes-繼承鏈)
6. [資料庫 & ORM](#6-資料庫--orm)
7. [Models（Library）](#7-modelslibrary)
8. [Enums](#8-enums)
9. [ViewModels（主專案）](#9-viewmodels主專案)
10. [Controllers](#10-controllers)
11. [Services](#11-services)
12. [Filters & Attributes](#12-filters--attributes)
13. [Utilities](#13-utilities)
14. [新增功能 SOP](#14-新增功能-sop)
15. [變更紀錄](#15-變更紀錄)

---

## 1. 專案概覽

| 項目 | 說明 |
|---|---|
| Solution | HDP-CFP-Backend |
| 語言版本 | C# 14.0 |
| Target Framework | .NET 10 |
| 資料庫 | SQL Server |
| ORM | Dapper.Contrib（查詢）+ EF Core（Migration 產生 DB schema） |
| Session 過期時間 | 20 分鐘 |

### 專案結構
```
HDP-CFP-Backend.Library/   ← 共用 Library（Models、Repositories、Attributes、Utility）
HDP-CFP-Backend/            ← 主 Web API 專案（Controllers、Services、ViewModels、Filters）
```

---

## 2. 架構分層

```
[HTTP Request]
      │
[Filter: AuthorizeFilter]          ← 驗證登入 & 權限
      │
[Controller]                       ← 路由、ModelState 驗證、回傳 JSON
      │
[Service]                          ← 商業邏輯、SQL 查詢
      │
[Repository: DapperRepository]     ← DB 連線、Transaction
      │
[SQL Server]
```

**呼叫慣例：**
- Controller 直接呼叫 Service（透過 `_lazy.XxxService.Value` 或建構子注入）
- Service 透過 `_repository` 執行 SQL（`QueryAsync`、`QueryFirstAsync`、`InsertAsync`、`ExecuteAsync`）
- 不直接在 Controller 寫 SQL

---

## 3. DI 注入模式

### 3.1 自動注冊規則（Program.cs）
命名空間為 `HDP_CFP_Backend.Services` 或 `HDP_CFP_Backend.Argument` 的**公開非抽象類別**，
均自動以 **Scoped** 方式注入，**不需手動 AddScoped**。

### 3.2 Argument 物件（避免建構子爆炸）

| 類別 | 路徑 | 用途 |
|---|---|---|
| `BaseControllerArgument` | （尚未列出，DI 打包入口） | Controller 層 DI 統一入口 |
| `BaseServiceArgument` | （尚未列出，DI 打包入口） | Service 層 DI 統一入口 |
| `LazyServiceArgument` | `Services/Argument/LazyServiceArgument.cs` | 所有 Service 的 `Lazy<T>` 包裝，防循環 DI |

### 3.3 LazyServiceArgument 目前包含的 Services

| 屬性 | 型別 |
|---|---|
| `BaseService` | `Lazy<BaseService>` |
| `ManagerService` | `Lazy<ManagerService>` |
| `SysConfigService` | `Lazy<SysConfigService>` |
| `ManyToManyService` | `Lazy<ManyToManyService>` |
| `DbConnectionFactory` | `Lazy<IDbConnectionFactory>` |
| `UploadFileService` | `Lazy<UploadFileService>` |

> **新增 Service 時**：在 `LazyServiceArgument` 新增屬性與建構子參數，
> 並更新本文件 § 3.3 與 § 11。

### 3.4 Singleton 服務（手動注冊）

| 服務 | 說明 |
|---|---|
| `IMailSender / MailSender` | 寄信 |
| `MailSenderConfig` | 寄信設定 |
| `IDbConnectionFactory / SqlConnectionFactory` | DB 連線工廠 |
| `IDapperRepository / DapperRepository` | Scoped，每 Request 一個連線 |

---

## 4. 驗證與授權流程

```
Request
  │
  ├─ [AllowAnonymous]   → 直接放行（完全不驗證）
  │
  ├─ 無 Session         → Redirect → /Manager/Login
  │
  ├─ [IgnoreAuthorize]  → 需登入，但不驗權限（如 Register、GetManagerSession）
  │
  └─ [AuthorizeAs("ActionName")]  → 以指定 Action 的權限為準
```

**Session Key**：`"ManagerInfo"` → 反序列化為 `SessionManagerInfo`

**權限更新時機**：每次 Request 時，
`AuthorizeFilter` 比對 `SessionManagerInfo.LastApplyRoleDate` 與 DB 的 `Role.UpdateDate`，
若有變動則重新呼叫 `ManagerService.SetSessionManagerInfo()`。

---

## 5. Base Classes 繼承鏈

### 5.1 Controller

```
Controller（ASP.NET Core）
  └── BaseController
        ├── 持有：IWebHostEnvironment, IConfiguration, IMailSender,
        │        IDapperRepository, BaseService, ILogger,
        │        LazyServiceArgument, UploadFileService
        ├── SessionManagerInfo（get/set from Session）
        ├── CurrentController / CurrentAction
        ├── TransactionFunc(Func<Task>) → (bool isSuccess, string message)
        └── GetInvalidModelStateEntry() → 回傳 Invalid 的 ModelState 給前端
              └── AuthorizedController  [ServiceFilter(AuthorizeFilter)]
                    └── StandardController<DBModel, ViewModel, SearchViewModel,
                                           ListViewModel, ListDataModel>
                          ├── _service（_StandardService 實例）
                          ├── GET  Index()
                          ├── POST GetList(SearchViewModel)
                          ├── GET  Create() / Edit(Guid) / Copy(Guid) / Detail(Guid)
                          ├── POST GetModel(Guid?) / GetNewModel() / GetCopyModel(Guid)
                          ├── POST CreateModel(IFormCollection, ViewModel)
                          ├── POST EditModel(IFormCollection, ViewModel)
                          ├── POST CopyModel(IFormCollection, ViewModel)
                          ├── POST Save(IFormCollection, ViewModel)
                          ├── POST Delete(Guid)
                          └── POST SwitchStatus(Guid)
```

### 5.2 Service

```
BaseService
  ├── 持有：IDapperRepository, IHttpContextAccessor, IMailSender,
  │        IConfiguration, IWebHostEnvironment, LazyServiceArgument, UploadFileService
  ├── QueryAsync<T>(sql, param)
  ├── QueryFirstAsync<T>(sql, param)
  ├── ExecuteAsync(sql, param)
  ├── InsertAsync<T>(model)         ← 自動填 CreateDate / UpdateDate
  ├── TransactionFunc(Func<Task>)   → (bool, Exception)
  └── _ModelService<DBModel, ViewModel>
        ├── _tableName / _keyField（從 [Table] / [ExplicitKey] Attribute 讀取）
        ├── GetData(Guid) / GetModel(Guid) / GetModelList(IEnumerable<Guid>)
        ├── CreateModel() / GetCopyModel(Guid)
        ├── Insert(ViewModel) / Update(ViewModel) / Delete(Guid) / Copy(ViewModel)
        └── _StandardService<DBModel, ViewModel, SearchViewModel, ListViewModel, ListDataModel>
              ├── GetList(SearchViewModel) → ListViewModel（含分頁）
              ├── GetListQueryString() → (mainSql, baseWhere, sqlWhere)
              └── SwitchStatus(Guid)
```

---

## 6. 資料庫 & ORM

| 項目 | 說明 |
|---|---|
| 連線字串設定 | `appsettings.json` → `ConnectionStrings:DefaultConnection` |
| Soft Delete | `Status = -1 (EStatus.Deleted)`，查詢預設加 `WHERE Status != -1` |
| 主鍵 | `Guid Id`（`[ExplicitKey]`，預設 `Guid.NewGuid()`） |
| Password 儲存 | `byte[]`，使用 `SHA256.HashData(Encoding.UTF8.GetBytes(plainText))` |
| Migration | EF Core，指令：`dotnet ef migrations add <名稱>` |
| DBContext 路徑 | `HDP-CFP-Backend.Library/Models/DBContext.cs` |

### DBContext 中的 DbSets

**Manager 相關**
`AdminMenu`, `AdminFunction`, `AdminMenuByRole`, `Log_ManagerLogin`,
`Log_ManagerLoginFail`, `Manager`, `Role`, `Log_ManagerWatch`

**System 相關**
`Log_BackendPageRequest`, `SysConfig`, `DataChange`, `ManyToMany`, `KeyValueSetting`

---

## 7. Models（Library）

路徑：`HDP-CFP-Backend.Library/Models/`

### 7.1 Base

| 類別 | 欄位 |
|---|---|
| `ModelBase` | `CreateDate`, `CreateUserId`, `UpdateDate`, `UpdateUserId`, `Status(EStatus?)`, `Sequence` |
| `IdModelBase : ModelBase` | `Id (Guid, [ExplicitKey])` |

### 7.2 Manager 領域（`Models/Manager/`）

| 類別 | 資料表 | 重要欄位 |
|---|---|---|
| `Manager` | `Manager` | `Account(string?)`, `Email(string?)`, `Password(byte[]?)`, `Name(string?)`, `EmailConfirm(bool)`, `PauseDate(DateTime?)`, `LastPasswordChangeDate(DateTime)` |
| `Role` | `Role` | `Name(string?)` |
| `AdminMenu` | `AdminMenu` | — |
| `AdminFunction` | `AdminFunction` | — |
| `AdminMenuByRole` | `AdminMenuByRole` | — |
| `Log_ManagerLogin` | `Log_ManagerLogin` | （無主鍵，HasNoKey） |
| `Log_ManagerLoginFail` | `Log_ManagerLoginFail` | — |
| `Log_ManagerWatch` | `Log_ManagerWatch` | — |

### 7.3 Events 領域（`Models/Events/`）

| 類別 | 資料表 |
|---|---|
| `Events` | `Events` |
| `AttendeeList` | `AttendeeList` |
| `EventAward` | `EventAward` |
| `Companies` | `Companies` |
| `LotteryWinner` | `LotteryWinner` |

### 7.4 News 領域（`Models/News/`）

| 類別 | 資料表 |
|---|---|
| `News` | `News` |
| `NewsCategory` | `NewsCategory` |
| `NewsImage` | `NewsImage` |

### 7.5 System 領域（`Models/System/`）

| 類別 | 資料表 | 說明 |
|---|---|---|
| `SysConfig` | `SysConfig` | Value, Note, TypeName |
| `ManyToMany` | `ManyToMany` | SourceTable, SourceId, TargetTable, TargetId, RelationType, Params |
| `DataChange` | `DataChange` | 欄位變更紀錄 |
| `Log_BackendPageRequest` | `Log_BackendPageRequest` | 後台頁面 request log |
| `KeyValueSetting` | `KeyValueSetting` | — |

### 7.6 其他（`Models/`）

| 類別 | 資料表 |
|---|---|
| `UploadFile` | `UploadFile` |
| `OriginalImage` | `OriginalImage` |
| `CropData` | `CropData` |
| `DynamicForm` | `DynamicForm` |
| `DynamicFormField` | `DynamicFormField` |
| `DynamicFormFieldOption` | `DynamicFormFieldOption` |

---

## 8. Enums

路徑：`HDP-CFP-Backend.Library/Models/Enums/`

| Enum | 值 |
|---|---|
| `EStatus` | `Deleted = -1`, `Disable = 0`, `Enable = 1` |
| `EFileType` | — |
| `EPageFormFieldType` | — |
| `EDataChange` | — |
| `ELotteryWinner` | — |
| `EEventRole` | — |
| `ELogin` | — |

---

## 9. ViewModels（主專案）

路徑：`HDP-CFP-Backend/ViewModels/`

### 9.1 Base

| 類別 | 路徑 | 說明 |
|---|---|---|
| `BaseSearchViewModel` | `Base/Paging.cs` | `start`, `length`, `draw`, `Status`, `OrderType`, `order` |
| `BaseListDataModel` | `Base/BaseListDataModel.cs` | `Id(Guid)`, `Status(EStatus)` |
| `PagingViewModel<T>` | `PagingViewModel.cs` | `draw`, `recordsTotal`, `recordsFiltered`, `data(List<T>)` |

### 9.2 Manager

| 類別 | 路徑 | 繼承 / 說明 |
|---|---|---|
| `ManagerModel` | `Manager/ManagerModel.cs` | `: Manager`（無額外欄位） |
| `ManagerSearchViewModel` | `Manager/ManagerModel.cs` | `: BaseSearchViewModel`，`Account(string?)` |
| `ManagerListViewModel` | `Manager/ManagerModel.cs` | `: PagingViewModel<ManagerListDataModel>` |
| `ManagerListDataModel` | `Manager/ManagerModel.cs` | `: BaseListDataModel`，`Account(string?)` |
| `ManagerRegisterViewModel` | `Manager/ManagerModel.cs` | `Account`, `Email`, `Name`, `Password`, `ConfirmPassword`（含 Required/Compare 驗證） |

### 9.3 Session

| 類別 | 路徑 | 重要欄位 |
|---|---|---|
| `SessionManagerInfo` | `LoginManagerInfo.cs` | `ManagerId`, `Name`, `LastApplyRoleDate`, `LastPasswordChangeDate`, `Watching`, `NullableRealManagerId` |
| `LoginManagerInfo` | `LoginManagerInfo.cs` | `ManagerId`, `Account`, `Name`, `Status`, `LastPasswordChangeDate`, `CompanyId` |

---

## 10. Controllers

路徑：`HDP-CFP-Backend/Controllers/`

| 類別 | 路由 | 繼承 | 說明 |
|---|---|---|---|
| `ManagerController` | `/Manager` | `StandardController<Manager, ManagerModel, ManagerSearchViewModel, ManagerListViewModel, ManagerListDataModel>` | Manager CRUD + Register |
| `WeatherForecastController` | `/WeatherForecast` | — | 預設範例，待移除 |

### ManagerController 自訂 Actions

| HTTP | 路由 | Auth | 說明 |
|---|---|---|---|
| POST | `/Manager/Register` | `[IgnoreAuthorize]` | 新帳號註冊，接受 `ManagerRegisterViewModel` JSON body |

---

## 11. Services

路徑：`HDP-CFP-Backend/Services/`

| 類別 | 路徑 | 繼承 | 說明 |
|---|---|---|---|
| `ManagerService` | `Services/ManagerService.cs` | `_StandardService<Manager, ManagerModel, ManagerSearchViewModel, ManagerListViewModel, ManagerListDataModel>` | Manager 業務邏輯 |
| `SysConfigService` | `Services/SysConfigService.cs` | — | 系統參數 |
| `ManyToManyService` | `Services/ManyToManyService.cs` | — | 多對多關聯通用操作 |
| `UploadFileService` | `Services/UploadFileService.cs` | — | 檔案上傳 |

### ManagerService 方法

| 方法 | 說明 |
|---|---|
| `CheckManagerRoleUpdateDate(Guid managerId)` | 比對 Manager 與其 Role 最後更新時間，供 AuthorizeFilter 使用 |
| `SetSessionManagerInfo(Guid managerId)` | 重新讀取並寫入 Session 的使用者資訊 |
| `Register(ManagerRegisterViewModel)` | 檢查帳號/Email 是否重複 → SHA256 hash 密碼 → InsertAsync |

---

## 12. Filters & Attributes

路徑：`HDP-CFP-Backend/Filter/`

| 類別 | 用法 | 說明 |
|---|---|---|
| `AuthorizeFilter` | `[ServiceFilter(typeof(AuthorizeFilter))]`（套在 `AuthorizedController`） | 驗證 Session、檢查權限更新 |
| `[IgnoreAuthorize]` | 加在 Action 或 Controller | 需登入，但跳過功能權限驗證 |
| `[AuthorizeAs("ActionName")]` | 加在 Action | 借用另一個 Action 的權限 |
| `[AllowAnonymous]` | 加在 Action 或 Controller | 完全不驗證（無需登入） |
| `GlobalExceptionsFilter` | 全域例外攔截 | — |
| `BaseFilter` | Filter 基類 | `GetSession<T>()` helper |

---

## 13. Utilities

路徑：`HDP-CFP-Backend/Utility/`

| 類別 | 說明 |
|---|---|
| `PasswordValidator` | 長度 8–32，至少包含大寫、小寫、數字、特殊符號中的 3 種 |
| `BaseFunction` | `GetDataTableColumns<T>()`, `GetOrderSql<T>(searchModel)` |
| `IMailSender / MailSender` | 寄信，Singleton |
| `MailSenderConfig` | 寄信設定，Singleton |
| `UploadFileService` | 上傳檔案（含 5MB 限制） |
| `SystemVariable` | 全域系統變數（如 `SystemVariable.Now`） |
| `HtmlSanitizerConverter` | HTML 清理 |
| `ModelComparer` | 比對兩個 Model 的差異（用於 DataChange） |
| `MimeSniffer` | 偵測上傳檔案 MIME |
| `Logging` | Log 工具 |
| `LineNotifySender` | Line Notify 發送（目前已停用，已被 Singleton 但 commented out） |

---

## 14. 新增功能 SOP

### 新增一個完整 CRUD 模組（例：`News`）

1. **Model**（Library）
   - 建立 `HDP-CFP-Backend.Library/Models/News/News.cs`
   - 繼承 `IdModelBase`，加上 `[Table("News")]`
   - 在 `DBContext.cs` 加上 `DbSet<News>`

2. **ViewModel**（主專案）
   - 建立 `ViewModels/News/NewsModel.cs`
   - 定義 `NewsModel : News`、`NewsSearchViewModel : BaseSearchViewModel`、
     `NewsListViewModel : PagingViewModel<NewsListDataModel>`、`NewsListDataModel : BaseListDataModel`

3. **Service**
   - 建立 `Services/NewsService.cs`
   - 繼承 `_StandardService<News, NewsModel, NewsSearchViewModel, NewsListViewModel, NewsListDataModel>`
   - 覆寫 `GetListQueryString()` 加入搜尋條件
   - 在 `LazyServiceArgument` 新增 `Lazy<NewsService>` 屬性與建構子參數

4. **Controller**
   - 建立 `Controllers/NewsController.cs`
   - 繼承 `StandardController<News, NewsModel, NewsSearchViewModel, NewsListViewModel, NewsListDataModel>`
   - 建構子：`base(argument, argument.LazyServiceArgument.NewsService.Value)`

5. **Migration**
   ```
   dotnet ef migrations add Add_News --project HDP-CFP-Backend.Library --startup-project HDP-CFP-Backend
   dotnet ef database update
   ```

6. **更新本文件** §7、§9、§10、§11、§3.3、§15

---

### 新增自訂 Action

1. 確認是否需要 `[IgnoreAuthorize]`、`[AuthorizeAs]` 或 `[AllowAnonymous]`
2. 在對應 ViewModel 檔新增 Request/Response ViewModel
3. 在 Service 新增商業邏輯方法
4. 在 Controller 新增 Action，使用 `TransactionFunc` 包裝，回傳 `Json(GetInvalidModelStateEntry())`
5. 更新本文件 §9、§10、§11

---

## 15. 變更紀錄

| 日期 | 變更內容 | 影響檔案 |
|---|---|---|
| 2026-03-19 | 初始 Migration（Init, Init2, Tim_20260319） | `Migrations/` |
| （初版索引建立）| 新增 `ManagerRegisterViewModel`；`ManagerService.Register()`；`ManagerController` 移除 WeatherForecast 殘留，新增 `POST /Manager/Register` | `ViewModels/Manager/ManagerModel.cs`, `Services/ManagerService.cs`, `Controllers/ManagerController.cs` |
