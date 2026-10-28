# RideLite

## 1. Giới thiệu dự án

RideLite hiện là bộ khung backend cho dự án đặt xe, được tổ chức thành các dịch vụ riêng: `Trip`, `MatchingPricing`, `Payment` và `ApiGateway`. Cấu trúc này giúp các thành viên chia phần việc và chạy thử từng dịch vụ trên máy cá nhân.

**Trạng thái hiện tại:** dự án đang ở bước khởi tạo. Các dịch vụ có API mẫu để kiểm tra môi trường; chưa có chức năng đặt xe, ghép tài xế, tính giá hoặc thanh toán thực tế. Thư mục `frontend/` còn trống.

Thành viên mới nên đọc [Hướng dẫn cài đặt và cấu hình](docs/HUONG_DAN_CAI_DAT.md) để chuẩn bị máy, chạy các dịch vụ và xử lý lỗi.

## 2. Tính năng chính hiện có

- Chạy độc lập ba dịch vụ backend bằng ASP.NET Core trên .NET 10.
- Gọi API mẫu `GET /weatherforecast` ở mỗi dịch vụ để xác nhận ứng dụng hoạt động.
- Xem tài liệu OpenAPI dạng JSON trong môi trường phát triển.
- Chạy ứng dụng gateway với endpoint `GET /` trả về `Hello World!`.
- Chạy ba dự án kiểm thử mẫu bằng xUnit và Microsoft.Testing.Platform.

Gateway đã có gói YARP nhưng chưa cấu hình chuyển tiếp yêu cầu đến các dịch vụ. Các lớp Infrastructure đã có gói hỗ trợ PostgreSQL nhưng chưa có kết nối cơ sở dữ liệu.

### Cấu trúc thư mục

```text
ride-lite/
├── backend/
│   ├── RideLite.slnx       # Tệp tập hợp các dự án backend
│   ├── global.json         # Phiên bản SDK và trình chạy kiểm thử
│   ├── dotnet-tools.json   # Công cụ dotnet-ef của dự án
│   ├── src/
│   │   ├── Gateway/ApiGateway/
│   │   └── Services/
│   │       ├── Trip/
│   │       ├── MatchingPricing/
│   │       └── Payment/
│   └── tests/              # Các dự án kiểm thử mẫu
├── docs/                   # Tài liệu cho thành viên
├── frontend/               # Chưa có mã nguồn
└── scripts/                # Chưa có tập lệnh
```

Mỗi dịch vụ có bốn phần: `Domain` chứa mô hình nghiệp vụ, `Application` xử lý các trường hợp sử dụng, `Infrastructure` tích hợp lưu trữ và hạ tầng, `Presentation` tiếp nhận yêu cầu HTTP. Hiện ba phần đầu mới có lớp khung.

## 3. Yêu cầu hệ thống

- Windows với Git Bash hoặc Linux với Bash.
- .NET SDK **10.0.401** theo `backend/global.json`. Cấu hình `latestFeature` cho phép chọn SDK mới hơn trong cùng nhánh `10.0` đáp ứng phiên bản này; xem [quy tắc chọn SDK của Microsoft](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json).
- Kết nối Internet khi tải các gói phụ thuộc lần đầu.
- Git nếu lấy mã nguồn bằng `git clone`; trình duyệt hoặc `curl` để kiểm tra API.

Tải bộ **SDK** tại [trang .NET 10 chính thức](https://dotnet.microsoft.com/en-us/download/dotnet/10.0). Bản hiện tại chưa cần PostgreSQL, Docker hoặc Node.js để khởi chạy.

## 4. Hướng dẫn cài đặt

Mở Git Bash trên Windows hoặc Bash trên Linux tại thư mục `ride-lite`, sau đó chạy:

```bash
cd backend
dotnet --version
dotnet restore RideLite.slnx
dotnet build RideLite.slnx --no-restore
```

Khi xuất hiện `Build succeeded.`, có thể khởi chạy dịch vụ. Các bước chuẩn bị chi tiết và công cụ tùy chọn được trình bày trong [hướng dẫn cài đặt](docs/HUONG_DAN_CAI_DAT.md).

## 5. Hướng dẫn sử dụng

Ví dụ chạy dịch vụ Trip từ thư mục `backend/`:

```bash
dotnet run --project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj --launch-profile http
```

Giữ terminal này mở và truy cập địa chỉ tương ứng bằng trình duyệt:

| Ứng dụng | Địa chỉ kiểm tra | Kết quả mong đợi |
| --- | --- | --- |
| ApiGateway | <http://localhost:5221/> | Chuỗi `Hello World!` |
| Trip | <http://localhost:5092/weatherforecast> | Mảng JSON gồm 5 bản ghi mẫu |
| MatchingPricing | <http://localhost:5286/weatherforecast> | Mảng JSON gồm 5 bản ghi mẫu |
| Payment | <http://localhost:5219/weatherforecast> | Mảng JSON gồm 5 bản ghi mẫu |

Mỗi địa chỉ chỉ hoạt động khi ứng dụng tương ứng đang chạy. Làm theo [cách chạy đủ bốn ứng dụng](docs/HUONG_DAN_CAI_DAT.md#4-khởi-chạy-các-ứng-dụng) nếu cần kiểm tra toàn bộ backend. Nhấn `Ctrl+C` tại từng terminal để dừng.

Ba dịch vụ cung cấp tài liệu API tại đường dẫn `/openapi/v1.json` trên cổng của mình khi chạy với profile `http`. Dự án chưa có giao diện Swagger UI.

Chạy bộ kiểm thử từ thư mục `backend/`:

```bash
dotnet test --solution RideLite.slnx
```

Hiện có 3 kiểm thử mẫu với `Assert.True(true)`; kết quả đạt chỉ xác nhận bộ công cụ kiểm thử chạy được, chưa chứng minh tính đúng đắn của nghiệp vụ.

## 6. Cấu hình

Các giá trị mặc định đã đủ để chạy thử. Chưa cần tạo `.env`, tài khoản hoặc chuỗi kết nối cơ sở dữ liệu.

- `Properties/launchSettings.json` của từng ứng dụng: cổng và môi trường khi chạy trên máy cá nhân.
- `appsettings.json`: cấu hình ghi log và `AllowedHosts`.
- `appsettings.Development.json`: cấu hình ghi log cho môi trường phát triển.

Xem [hướng dẫn cấu hình](docs/HUONG_DAN_CAI_DAT.md#6-cấu-hình-trên-máy-cá-nhân) nếu cần đổi cổng hoặc khôi phục công cụ Entity Framework.

## 7. Các lỗi thường gặp và cách xử lý

| Hiện tượng | Cách xử lý nhanh |
| --- | --- |
| Không tìm thấy `dotnet` hoặc SDK không phù hợp | Cài SDK đáp ứng `global.json`, mở lại terminal và kiểm tra trong `backend/`. |
| Không tải được các gói NuGet | Kiểm tra Internet và quyền truy cập nguồn gói, rồi chạy lại `dotnet restore RideLite.slnx`. |
| Cổng đang được sử dụng | Dừng phiên chạy cũ bằng `Ctrl+C` hoặc dùng cổng khác theo hướng dẫn. |
| Nhận lỗi 404 khi mở dịch vụ | Dùng `/weatherforecast` cho ba dịch vụ; đường dẫn `/` chỉ có ở gateway. |
| Không thấy `/swagger` | Dùng `/openapi/v1.json`; hiện chưa cài Swagger UI. |

Tham khảo [phần xử lý lỗi chi tiết](docs/HUONG_DAN_CAI_DAT.md#7-các-lỗi-thường-gặp-và-cách-xử-lý).
