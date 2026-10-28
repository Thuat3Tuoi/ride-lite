# Hướng dẫn cài đặt và cấu hình RideLite

Tài liệu dành cho thành viên mới cần chạy dự án trên máy cá nhân. Sau khi hoàn thành, bạn có thể khởi động gateway, ba dịch vụ backend, gọi API mẫu và chạy kiểm thử.

**Phạm vi hiện tại:** mã nguồn mới có bộ khung backend. Chưa có giao diện frontend, nghiệp vụ đặt xe, thanh toán thực tế hoặc cơ sở dữ liệu được kết nối. API dự báo thời tiết là dữ liệu mẫu để kiểm tra ứng dụng.

Các lệnh dưới đây dùng cú pháp **Bash**, chạy trong Git Bash trên Windows hoặc Bash trên Linux. Không cần cài WSL để dùng Git Bash.

## 1. Chuẩn bị phần mềm

| Thành phần | Yêu cầu | Mục đích |
| --- | --- | --- |
| .NET SDK | `10.0.401`, hoặc SDK mới hơn trong cùng nhánh `10.0` đáp ứng `backend/global.json` | Biên dịch và chạy backend |
| Git | Cần nếu tải mã nguồn bằng `git clone` | Lấy và cập nhật mã nguồn |
| Terminal Bash | Git Bash trên Windows; Bash trên Linux | Chạy các lệnh trong tài liệu |
| Trình duyệt hoặc curl | Có ít nhất một công cụ | Mở API và kiểm tra kết quả |
| Trình soạn thảo | Tùy chọn | Xem và chỉnh sửa mã nguồn |

Tải SDK từ [trang .NET 10 của Microsoft](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). Chọn mục **SDK**, vì bộ SDK cung cấp công cụ xây dựng và chạy ứng dụng. Nếu dùng Linux, trang tải có liên kết hướng dẫn cài bằng trình quản lý gói theo hệ điều hành.

`backend/global.json` đặt `version` là `10.0.401` và `rollForward` là `latestFeature`. Vì vậy, chỉ thấy máy có “.NET 10” chưa đủ: SDK cũ hơn như `10.0.100` không đáp ứng cấu hình này. Tham khảo [quy tắc chọn SDK](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json) nếu máy có nhiều phiên bản.

Hiện chưa cần cài PostgreSQL, Docker, Node.js hoặc công cụ frontend. Mã nguồn cũng chưa yêu cầu khóa API hay tài khoản dịch vụ bên ngoài.

## 2. Lấy mã nguồn và mở đúng thư mục

Nhận đường dẫn kho mã nguồn từ nhóm hoặc dùng bản sao dự án đã được chia sẻ. Nếu dùng Git, thay `URL_KHO_MA_NGUON` bằng URL thực tế do nhóm cung cấp:

```bash
git clone URL_KHO_MA_NGUON ride-lite
cd ride-lite
```

Nếu đã có thư mục dự án, mở terminal tại thư mục đó, không cần clone lại. Ví dụ với máy có đường dẫn hiện tại:

**Windows, Git Bash:**

```bash
cd /d/SGU/Nam4/T2-Chieu-KTPM/ride-lite
```

**Linux:** dùng đường dẫn thực tế trên máy, ví dụ nếu dự án nằm trong `~/projects/ride-lite`:

```bash
cd ~/projects/ride-lite
```

Kiểm tra rồi chuyển vào backend:

```bash
pwd
ls
cd backend
ls RideLite.slnx global.json
```

**Từ bước 3 trở đi, chạy các lệnh `dotnet` tại thư mục `ride-lite/backend/`, trừ khi có ghi khác.** Mỗi terminal mới cần mở tại thư mục này để đường dẫn dự án và cấu hình SDK được nhận đúng.

## 3. Kiểm tra môi trường và cài các gói phụ thuộc

### Bước 1: Kiểm tra SDK

```bash
dotnet --list-sdks
dotnet --version
```

Lệnh đầu liệt kê các SDK đã cài. Lệnh sau cho biết SDK được chọn theo `global.json`; với đúng phiên bản gốc, kết quả là `10.0.401`.

Nếu lệnh không chạy được hoặc báo không có SDK phù hợp, xử lý ở [mục 7](#7-các-lỗi-thường-gặp-và-cách-xử-lý) trước khi tiếp tục.

### Bước 2: Tải các gói phụ thuộc

```bash
dotnet restore RideLite.slnx
```

Lệnh này tải các thư viện mà dự án khai báo. Máy cần truy cập được nguồn gói NuGet. Thông báo `All projects are up-to-date for restore.` cũng là kết quả hợp lệ nếu các gói đã có sẵn.

### Bước 3: Biên dịch toàn bộ backend

```bash
dotnet build RideLite.slnx --no-restore
```

Kết quả mong đợi: `Build succeeded.` và `0 Error(s)`. Tham số `--no-restore` dùng các gói đã tải ở bước trước.

Chưa cần chỉnh sửa cấu hình để chạy bản hiện tại. Công cụ `dotnet-ef` là tùy chọn, được hướng dẫn ở [mục 6](#6-cấu-hình-trên-máy-cá-nhân).

## 4. Khởi chạy các ứng dụng

Mở **bốn terminal riêng**, mỗi terminal tại `ride-lite/backend/`, rồi chạy một lệnh tương ứng dưới đây. Giữ các terminal mở trong lúc kiểm tra.

Các ứng dụng hiện chạy độc lập, chưa có yêu cầu gọi qua lại. Có thể chỉ chạy dịch vụ mình đang phụ trách; chạy đủ bốn ứng dụng khi muốn kiểm tra toàn bộ backend.

### Terminal 1: Trip

```bash
dotnet run --project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj --launch-profile http
```

Địa chỉ: <http://localhost:5092/weatherforecast>.

### Terminal 2: MatchingPricing

```bash
dotnet run --project src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.csproj --launch-profile http
```

Địa chỉ: <http://localhost:5286/weatherforecast>.

### Terminal 3: Payment

```bash
dotnet run --project src/Services/Payment/Payment.Presentation/Payment.Presentation.csproj --launch-profile http
```

Địa chỉ: <http://localhost:5219/weatherforecast>.

### Terminal 4: ApiGateway

```bash
dotnet run --project src/Gateway/ApiGateway/ApiGateway.csproj --launch-profile http
```

Địa chỉ: <http://localhost:5221/>.

Mỗi terminal cần hiển thị `Now listening on: http://localhost:...` và `Hosting environment: Development`. Các lệnh chọn rõ profile `http`, nên không cần thiết lập chứng chỉ HTTPS.

Gateway hiện chỉ trả về `Hello World!` tại `/`. Khi kiểm tra các dịch vụ, dùng trực tiếp cổng của chúng vì gateway chưa chuyển tiếp yêu cầu.

**Dừng ứng dụng:** nhấn `Ctrl+C` trong terminal đang chạy ứng dụng đó. Để dừng cả backend, thực hiện ở cả bốn terminal.

## 5. Kiểm tra dự án đã hoạt động

### Kiểm tra bằng trình duyệt hoặc curl

Mở các địa chỉ ở mục 4 bằng trình duyệt. Nếu dùng curl, mở thêm một terminal Bash và chạy:

```bash
curl -i --fail http://localhost:5221/
curl -i --fail http://localhost:5092/weatherforecast
curl -i --fail http://localhost:5286/weatherforecast
curl -i --fail http://localhost:5219/weatherforecast
```

Kết quả mong đợi:

- Cả bốn yêu cầu có trạng thái `200 OK`.
- Gateway trả về `Hello World!`.
- Mỗi dịch vụ trả về mảng JSON gồm 5 bản ghi, có các trường `date`, `temperatureC`, `temperatureF`, `summary`. Dữ liệu được tạo ngẫu nhiên, nên nội dung có thể khác nhau giữa các lần gọi.

### Xem tài liệu API

Khi chạy với profile `http`, ba dịch vụ cung cấp OpenAPI dạng JSON:

```bash
curl -i --fail http://localhost:5092/openapi/v1.json
curl -i --fail http://localhost:5286/openapi/v1.json
curl -i --fail http://localhost:5219/openapi/v1.json
```

Có thể mở các URL này trong trình duyệt. OpenAPI chỉ được bật khi môi trường là `Development`. Hiện chưa có giao diện tại `/swagger`, và gateway chưa cung cấp OpenAPI.

### Chạy kiểm thử

Trong một terminal tại thư mục `backend/`:

```bash
dotnet test --solution RideLite.slnx
```

Kết quả hiện tại mong đợi: **3 kiểm thử thành công, 0 thất bại**. Cả ba đều là bài kiểm thử mẫu `Assert.True(true)`, dùng để xác nhận bộ công cụ chạy được; chưa kiểm tra nghiệp vụ.

Để chạy riêng bộ kiểm thử của Trip:

```bash
dotnet test --project tests/Trip.Domain.Tests/Trip.Domain.Tests.csproj
```

Dự án chọn `Microsoft.Testing.Platform` trong `global.json`, vì vậy dùng cú pháp `--solution` hoặc `--project` như trên.

## 6. Cấu hình trên máy cá nhân

### Vị trí các tệp cấu hình

Đường dẫn dưới đây tính từ thư mục `backend/`:

| Ứng dụng | Thư mục chứa cấu hình | Cổng HTTP mặc định |
| --- | --- | --- |
| ApiGateway | `src/Gateway/ApiGateway/` | `5221` |
| Trip | `src/Services/Trip/Trip.Presentation/` | `5092` |
| MatchingPricing | `src/Services/MatchingPricing/MatchingPricing.Presentation/` | `5286` |
| Payment | `src/Services/Payment/Payment.Presentation/` | `5219` |

Trong mỗi thư mục:

- `Properties/launchSettings.json`: profile chạy trên máy cá nhân. `applicationUrl` đặt địa chỉ và cổng; `ASPNETCORE_ENVIRONMENT` đang là `Development`.
- `appsettings.json`: cấu hình ghi log và `AllowedHosts`.
- `appsettings.Development.json`: cấu hình ghi log dùng trong môi trường phát triển.

### Đổi cổng khi bị trùng

Ví dụ chạy Trip ở cổng `5500`, nếu cổng này còn trống:

```bash
dotnet run --project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj --launch-profile http -- --urls http://localhost:5500
```

Sau đó kiểm tra <http://localhost:5500/weatherforecast>. Cách này chỉ đổi cổng của lần chạy hiện tại, thuận tiện khi máy có ứng dụng khác đang chiếm cổng mặc định.

Nếu nhóm muốn thay đổi cổng mặc định, cập nhật `applicationUrl` trong profile `http` của `Properties/launchSettings.json` tương ứng. Đồng thời cập nhật URL trong tệp `.http` của dịch vụ và các địa chỉ liên quan trong tài liệu.

### Cơ sở dữ liệu và biến môi trường

Ba dự án Infrastructure đã tham chiếu `Npgsql.EntityFrameworkCore.PostgreSQL` phiên bản `10.0.3`. Tuy nhiên, hiện chưa có `DbContext`, migration, chuỗi kết nối hay đăng ký kết nối cơ sở dữ liệu trong ứng dụng. Vì vậy, chưa có bước tạo database hoặc chạy migration để cài đặt.

Dự án cũng chưa có tệp `.env` hoặc đoạn cấu hình đọc tệp `.env`. Các giá trị mặc định trong những tệp nêu trên đã đủ cho các API mẫu.

### Khôi phục công cụ Entity Framework, nếu cần

Nếu cần chuẩn bị công cụ cho công việc phát triển sau này, chạy từ `backend/`:

```bash
dotnet tool restore --tool-manifest dotnet-tools.json
dotnet ef --version
```

Manifest hiện khai báo `dotnet-ef` phiên bản **10.0.14**. Dùng đường dẫn manifest cụ thể vì tệp đang nằm ở `backend/dotnet-tools.json`; tham khảo [tài liệu lệnh khôi phục công cụ](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-restore).

Bước này không bắt buộc để chạy API mẫu. Chưa chạy lệnh tạo hoặc cập nhật cơ sở dữ liệu khi dự án chưa có `DbContext`.

## 7. Các lỗi thường gặp và cách xử lý

### Không tìm thấy lệnh dotnet

Cài .NET SDK ở mục 1, đóng rồi mở lại terminal để nhận đường dẫn mới. Sau đó thử lại:

```bash
dotnet --list-sdks
```

Nếu vẫn không nhận lệnh, kiểm tra lại quá trình cài đặt và cấu hình `PATH` của hệ điều hành.

### Báo không tìm thấy SDK tương thích

Kiểm tra `dotnet --list-sdks` và `backend/global.json`. Cài SDK `10.0.401` hoặc bản mới hơn trong cùng nhánh `10.0` đáp ứng cấu hình. Không đổi `global.json` chỉ để bỏ qua lỗi trên máy cá nhân vì tệp này quy định môi trường dùng chung của nhóm.

### Lệnh không tìm thấy solution hoặc project

Kiểm tra thư mục hiện tại:

```bash
pwd
ls RideLite.slnx
```

Nếu không thấy tệp, chuyển đến `ride-lite/backend/` rồi chạy lại. Khi dùng Linux, chú ý đúng chữ hoa và chữ thường trong tên thư mục, tệp và đường dẫn.

### Restore thất bại hoặc không truy cập được NuGet

Kiểm tra kết nối Internet, proxy của mạng đang dùng và nguồn gói bị báo lỗi. Sau khi khắc phục kết nối, chạy lại:

```bash
dotnet restore RideLite.slnx
```

Giữ lại thông báo lỗi nếu cần nhờ thành viên khác hỗ trợ; thông báo sẽ cho biết nguồn hoặc gói nào không tải được.

### Cổng đã được sử dụng

Nếu gặp `Address already in use` hoặc lỗi không thể mở cổng, kiểm tra có terminal cũ đang chạy cùng ứng dụng hay không. Dừng phiên cũ bằng `Ctrl+C`, hoặc dùng cổng khác theo mục 6.

### Trình duyệt báo không thể kết nối

Kiểm tra terminal của ứng dụng còn mở và đã có dòng `Now listening on`. Dùng đúng `http://localhost` cùng cổng được in trong terminal. Nếu ứng dụng vừa dừng do lỗi, đọc thông báo trong terminal trước khi chạy lại.

### Nhận lỗi 404 hoặc không thấy Swagger

- Với Trip, MatchingPricing và Payment: mở `/weatherforecast`; các dịch vụ chưa có trang tại `/`.
- Với gateway: mở `/`; chưa có đường dẫn chuyển tiếp như `/trip` hoặc `/payment`.
- Với tài liệu API: mở `/openapi/v1.json` trên cổng dịch vụ khi môi trường là `Development`; `/swagger` chưa được cấu hình.

### Lệnh kiểm thử báo sai tham số

Dùng `dotnet test --solution RideLite.slnx` tại `backend/`. Nếu chạy riêng một dự án, dùng `dotnet test --project` kèm đường dẫn như mục 5. Đây là cú pháp của trình chạy kiểm thử được chọn trong `global.json`.

## 8. Danh sách xác nhận trước khi bắt đầu làm việc

- [ ] SDK được chọn phù hợp với `backend/global.json`.
- [ ] `dotnet restore RideLite.slnx` hoàn tất.
- [ ] `dotnet build RideLite.slnx --no-restore` thành công.
- [ ] Các ứng dụng cần dùng khởi động ở đúng cổng.
- [ ] API mẫu của từng ứng dụng đang chạy trả về `200 OK`.
- [ ] OpenAPI của dịch vụ mở được trong môi trường `Development`.
- [ ] `dotnet test --solution RideLite.slnx` có 3 kiểm thử thành công.

Khi hoàn thành các bước trên, môi trường đã sẵn sàng để phát triển phần backend hiện có. Đọc [README tổng quan](../README.md) để nắm cấu trúc và phạm vi của dự án.
