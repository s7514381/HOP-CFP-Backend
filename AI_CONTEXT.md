# AI Context

這份檔案提供每次重新開始工作時快速恢復脈絡的固定資訊。

## Project
- Project path: `C:\Users\s7514\source\repos\HOP-CFP-Backend`
- Backend tech: `.NET 10`
- Database: `MSSQL`
- API index: `C:\Users\s7514\source\repos\HOP-CFP-Backend\HOP-CFP-Backend\BACKEND_PROJECT_INDEX.md`
- Login account for verification: `Tim / !Qaz2wsx`
- Local dev backend should be started with the `https` launch profile so it listens on `https://localhost:7007`.

## Notes
- 這個專案的主索引仍以 `BACKEND_PROJECT_INDEX.md` 為準。
- 若之後要直接修改後端程式，請先讀這份檔案與主索引，再進入對應 Controller / Service / ViewModel。
- 若使用者要求我驗證前後端整合，預設以 `Tim / !Qaz2wsx` 登入測試。
- 任何後端修改完成後，都要先重啟 `https` launch profile，再重新測試受影響的 endpoint 或 template，確認結果後才算完成。
- SellerCompare 已有自己的 `Import` / `DownloadImportTemplate` 端點，模板下載驗證時要檢查 xlsx 第一列欄位是否與需求一致。
