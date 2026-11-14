# Hướng dẫn cài đặt và cấu hình RideLite (Backend)

Tài liệu này hướng dẫn chi tiết quy trình chuẩn bị môi trường, cấu hình cơ sở dữ liệu PostgreSQL cục bộ, chạy migration, khởi động các dịch vụ và thực thi kiểm thử cho toàn bộ hệ thống backend **RideLite**.

---

## 1. Tổng quan kiến trúc Backend

Hệ thống backend của RideLite được xây dựng theo kiến trúc **Microservices** và tuân theo nguyên lý **Clean Architecture** (Domain, Application, Infrastructure, Presentation), chạy trên nền tảng **.NET 10**.

### Cấu trúc dự án:

```text
ride-lite/
├── backend/
│   ├── RideLite.slnx                                   # Solution chứa toàn bộ dự án backend
│   ├── global.json                                     # Định nghĩa phiên bản .NET SDK (10.0.401+)
│   ├── dotnet-tools.json                               # Định nghĩa công cụ EF Core CLI (dotnet-ef)
│   ├── src/
│   │   ├── BuildingBlocks/
│   │   │   └── SharedKernel/                           # Lớp dùng chung: Xử lý ngoại lệ, mã lỗi tập trung
│   │   ├── Gateway/
│   │   │   └── ApiGateway/                             # API Gateway YARP Reverse Proxy (Port 5000)
│   │   └── Services/
│   │       ├── Trip/                                   # Dịch vụ Quản lý chuyến đi & Rider (Port 5001)
│   │       ├── MatchingPricing/                        # Dịch vụ Ghép tài xế & Định giá cước (Port 5002)
│   │       └── Payment/                                # Dịch vụ Thanh toán, Hoàn tiền, Sổ cái (Port 5003)
│   ├── tests/                                          # 9 dự án Unit & Integration tests (241 test cases)
│   ├── Trip_Service.postman_collection.json            # Postman collection test API Trip
│   ├── Matching_Pricing_Service.postman_collection.json# Postman collection test API Matching & Pricing
│   └── Payment_Service.postman_collection.json         # Postman collection test API Payment
├── docs/                                               # Tài liệu kỹ thuật dự án
└── README.md                                           # Giới thiệu tổng quan
```

### Danh mục dịch vụ và cổng mạng mặc định:

| Dịch vụ | Cổng HTTP | Database PostgreSQL | Health Check Endpoint |
| :--- | :--- | :--- | :--- |
| **ApiGateway** | `5000` | _Không có_ | `GET http://localhost:5000/` |
| **Trip Service** | `5001` | `ridelite_trip` | `GET http://localhost:5001/trip/health` |
| **MatchingPricing Service** | `5002` | `ridelite_matching_pricing` | `GET http://localhost:5002/matching/health` |
| **Payment Service** | `5003` | `ridelite_payment` | `GET http://localhost:5003/payment/health` |

---

## 2. Yêu cầu hệ thống và công cụ cần chuẩn bị

| Công cụ | Phiên bản yêu cầu | Mục đích |
| :--- | :--- | :--- |
| **.NET SDK** | **10.0.401** trở lên (nhánh `.NET 10.0`) | Biên dịch, chạy ứng dụng và test |
| **PostgreSQL** | **15.x / 16.x** cài đặt trực tiếp trên máy | Lưu trữ cơ sở dữ liệu cho 3 microservices |
| **Git** | Bản mới nhất | Quản lý mã nguồn |
| **Terminal Bash** | Git Bash trên Windows; Bash trên Linux/macOS | Thực thi các dòng lệnh |
| **Postman / cURL** | Bản mới nhất | Kiểm thử các API nghiệp vụ |

> [!IMPORTANT]
> Tệp `backend/global.json` quy định `rollForward: latestFeature` cho `version: 10.0.401`. Chắc chắn rằng máy bạn đã cài đặt .NET 10 SDK tương thích. Kiểm tra bằng lệnh: `dotnet --version`.

---

## 3. Cài đặt và cấu hình Cơ sở dữ liệu (PostgreSQL)

Hệ thống sử dụng PostgreSQL chạy ở cổng mặc định `5432`. Mỗi microservice sử dụng một cơ sở dữ liệu độc lập.

### Bước 1: Khởi động dịch vụ PostgreSQL trên máy
Đảm bảo PostgreSQL service đang chạy:
- **Linux**:
  ```bash
  sudo systemctl start postgresql
  sudo systemctl status postgresql
  ```
- **macOS**:
  ```bash
  brew services start postgresql
  ```
- **Windows**: Mở `services.msc` và đảm bảo service `postgresql-x64-...` đang ở trạng thái `Running`.

### Bước 2: Tạo 3 cơ sở dữ liệu
Bạn có thể dùng công cụ dòng lệnh `psql` hoặc các công cụ giao diện như **pgAdmin / DBeaver / Navicat**.

#### Cách A: Dùng giao diện psql CLI
Đăng nhập vào psql bằng user `postgres`:
```bash
# Trên Linux/macOS
sudo -u postgres psql

# Hoặc qua kết nối localhost
psql -U postgres -h localhost
```

Chạy 3 câu lệnh SQL sau để tạo cơ sở dữ liệu:
```sql
CREATE DATABASE ridelite_trip;
CREATE DATABASE ridelite_matching_pricing;
CREATE DATABASE ridelite_payment;
\q
```

#### Cách B: Chạy lệnh trực tiếp từ terminal
```bash
psql -U postgres -h localhost -c "CREATE DATABASE ridelite_trip;"
psql -U postgres -h localhost -c "CREATE DATABASE ridelite_matching_pricing;"
psql -U postgres -h localhost -c "CREATE DATABASE ridelite_payment;"
```

### Bước 3: Cấu hình tài khoản & mật khẩu kết nối
Mặc định chuỗi kết nối trong các tệp `appsettings.json` đang để là:
```
Host=localhost;Port=5432;Database=<ten_db>;Username=postgres;Password=01052005
```

> [!TIP]
> Nếu tài khoản `postgres` trên máy bạn có mật khẩu khác `01052005`:
> 1. Mở tệp `appsettings.json` tại thư mục của từng dịch vụ:
>    - `src/Services/Trip/Trip.Presentation/appsettings.json`
>    - `src/Services/MatchingPricing/MatchingPricing.Presentation/appsettings.json`
>    - `src/Services/Payment/Payment.Presentation/appsettings.json`
> 2. Thay đổi giá trị `Password` trong mục `ConnectionStrings:Database` cho phù hợp với máy của bạn.
> 3. Hoặc đặt mật khẩu `postgres` trên máy thành `01052005`:
>    ```sql
>    ALTER USER postgres PASSWORD '01052005';
>    ```

---

## 4. Các bước cài đặt mã nguồn Backend

Mở terminal Bash (Git Bash trên Windows hoặc Bash trên Linux/macOS) tại thư mục `ride-lite`:

### Bước 1: Di chuyển vào thư mục backend và kiểm tra môi trường
```bash
cd backend
dotnet --version
dotnet --list-sdks
```

### Bước 2: Khôi phục công cụ cục bộ (Local Tools)
Dự án quản lý công cụ `dotnet-ef` (Entity Framework Core CLI v10.0.14) qua manifest:
```bash
dotnet tool restore
dotnet ef --version
```

### Bước 3: Tải các gói thư viện phụ thuộc (Restore)
```bash
dotnet restore RideLite.slnx
```

### Bước 4: Áp dụng Migration vào Cơ sở dữ liệu
Chạy các lệnh sau tại thư mục `backend/` để khởi tạo cấu trúc bảng trong PostgreSQL:

1. **Trip Service Migration**:
   ```bash
   dotnet ef database update \
     --project src/Services/Trip/Trip.Infrastructure/Trip.Infrastructure.csproj \
     --startup-project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj
   ```

2. **Matching & Pricing Service Migration**:
   ```bash
   dotnet ef database update \
     --project src/Services/MatchingPricing/MatchingPricing.Infrastructure/MatchingPricing.Infrastructure.csproj \
     --startup-project src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.csproj
   ```

3. **Payment Service Migration**:
   ```bash
   dotnet ef database update \
     --project src/Services/Payment/Payment.Infrastructure/Payment.Infrastructure.csproj \
     --startup-project src/Services/Payment/Payment.Presentation/Payment.Presentation.csproj
   ```

*Kết quả mong đợi:* Dòng thông báo `Applying migration '...'` và `Done.` hiển thị cho từng dịch vụ mà không có lỗi.

### Bước 5: Biên dịch toàn bộ Solution
```bash
dotnet build RideLite.slnx --no-restore
```
*Kết quả mong đợi:* `Build succeeded.` với `0 Error(s)`.

---

## 5. Khởi chạy các ứng dụng

Để chạy toàn bộ hệ sinh thái, mở **4 cửa sổ terminal riêng biệt**, tất cả đều tại thư mục `ride-lite/backend/`:

### Terminal 1: Khởi chạy Trip Service (Port 5001)
```bash
dotnet run --project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj --launch-profile http
```
- Endpoint: `http://localhost:5001`
- Kiểm tra trạng thái: `http://localhost:5001/trip/health`

### Terminal 2: Khởi chạy Matching & Pricing Service (Port 5002)
```bash
dotnet run --project src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.csproj --launch-profile http
```
- Endpoint: `http://localhost:5002`
- Kiểm tra trạng thái: `http://localhost:5002/matching/health`

### Terminal 3: Khởi chạy Payment Service (Port 5003)
```bash
dotnet run --project src/Services/Payment/Payment.Presentation/Payment.Presentation.csproj --launch-profile http
```
- Endpoint: `http://localhost:5003`
- Kiểm tra trạng thái: `http://localhost:5003/payment/health`

### Terminal 4: Khởi chạy ApiGateway (Port 5000)
```bash
dotnet run --project src/Gateway/ApiGateway/ApiGateway.csproj --launch-profile http
```
- Endpoint: `http://localhost:5000`
- Kiểm tra trạng thái: `http://localhost:5000/`

> [!TIP]
> Nhấn `Ctrl+C` tại từng cửa sổ terminal bất kỳ lúc nào để dừng dịch vụ đó.

---

## 6. Kiểm tra và xác nhận hệ thống hoạt động

### 6.1. Kiểm tra sức khỏe các dịch vụ (Health Check)
Mở một terminal khác và dùng lệnh `curl` (hoặc mở trực tiếp trên trình duyệt):

```bash
# Gateway
curl -i http://localhost:5000/
# Kết quả: 200 OK - "Hello World!"

# Trip Service
curl -i http://localhost:5001/trip/health
# Kết quả: {"status":"trip-service active"}

# Matching & Pricing Service
curl -i http://localhost:5002/matching/health
# Kết quả: {"status":"matchingpricing-service active"}

# Payment Service
curl -i http://localhost:5003/payment/health
# Kết quả: {"status":"payment-service active"}
```

### 6.2. Kiểm tra OpenAPI Specification (JSON)
Khi chạy ở môi trường `Development`, từng microservice cung cấp tài liệu OpenAPI tại:
- Trip Service: <http://localhost:5001/openapi/v1.json>
- Matching & Pricing Service: <http://localhost:5002/openapi/v1.json>
- Payment Service: <http://localhost:5003/openapi/v1.json>

### 6.3. Kiểm thử API bằng Postman Collection
Dự án đã đính kèm sẵn 3 bộ sưu tập Postman hoàn chỉnh ngay trong thư mục `backend/`:
1. `Trip_Service.postman_collection.json`: Tạo Rider, quản lý hồ sơ, tạo Trip, cập nhật tài xế đón, bắt đầu chuyến, hoàn tất chuyến, hủy chuyến.
2. `Matching_Pricing_Service.postman_collection.json`: Tạo Driver, cập nhật vị trí, tìm tài xế lân cận, gán cuốc (assignment), chấp nhận/từ chối cuốc, ước tính cước phí (pricing estimate).
3. `Payment_Service.postman_collection.json`: Quản lý Payment Provider, Phương thức thanh toán, Tạo giao dịch thanh toán, Xử lý giao dịch, Hoàn tiền (Refund), Xem sổ cái (Ledger entries).

**Cách sử dụng:**
1. Mở Postman -> Chọn **Import** -> Kéo thả 3 tệp `.json` trên vào Postman.
2. Mỗi collection đã cấu hình sẵn biến môi trường `baseUrl`:
   - Trip: `http://localhost:5001`
   - Matching & Pricing: `http://localhost:5002`
   - Payment: `http://localhost:5003`
3. Tiến hành gọi thử các request trong collection theo thứ tự từ trên xuống dưới.

---

## 7. Chạy kiểm thử tự động (Testing)

Dự án sở hữu **241 bài kiểm thử tự động** bao gồm cả Unit Tests (Domain, Application) và Integration Tests (Controllers, Repositories). Các bài kiểm thử tích hợp sử dụng cơ chế SQLite In-Memory, do đó **không phụ thuộc vào PostgreSQL**.

Từ thư mục `backend/`, chạy:

```bash
dotnet test RideLite.slnx
```

Kết quả mong đợi:
```text
Test run summary: Passed!
  total: 241
  failed: 0
  succeeded: 241
  skipped: 0
```

---

## 8. Cấu hình trên máy cá nhân & Đổi cổng khi bị trùng

### Đổi cổng HTTP khi bị xung đột
Nếu một trong các cổng `5000`, `5001`, `5002`, `5003` bị chiếm dụng bởi ứng dụng khác trên máy bạn:
- Tạm thời chỉ định cổng mới khi chạy:
  ```bash
  dotnet run --project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj --launch-profile http -- --urls http://localhost:5101
  ```
- Hoặc chỉnh sửa trực tiếp trong file `Properties/launchSettings.json` của dịch vụ tương ứng.
- **Lưu ý:** Nếu đổi cổng dịch vụ, cần cập nhật lại địa chỉ đích tương ứng trong `src/Gateway/ApiGateway/appsettings.json` và biến `baseUrl` trong Postman collection.

---

## 9. Các lỗi thường gặp và cách xử lý (Troubleshooting)

| Hiện tượng lỗi | Nguyên nhân | Cách khắc phục |
| :--- | :--- | :--- |
| **`Cannot connect to 127.0.0.1:5432` / `Connection refused`** | PostgreSQL service chưa được bật. | Khởi động PostgreSQL service bằng `sudo systemctl start postgresql` (Linux) hoặc qua `Services` (Windows). |
| **`password authentication failed for user "postgres"`** | Mật khẩu tài khoản PostgreSQL trên máy bạn khác với mật khẩu cấu hình (`01052005`). | Cập nhật lại mật khẩu trong `ConnectionStrings:Database` tại `appsettings.json` của 3 dịch vụ hoặc đổi mật khẩu user `postgres` thành `01052005`. |
| **`database "ridelite_trip" does not exist`** | Chưa tạo database trước khi chạy migration hoặc chạy app. | Chạy lệnh `CREATE DATABASE ridelite_trip;` (tương tự với `ridelite_matching_pricing` và `ridelite_payment`) qua psql hoặc pgAdmin. |
| **`Cannot find SDK 10.0.401`** | Máy chưa cài .NET 10 SDK hoặc phiên bản cũ hơn 10.0.401. | Tải và cài đặt .NET SDK 10 mới nhất từ trang chủ Microsoft (.NET 10). |
| **`Address already in use`** | Cổng mạng (`5000` - `5003`) đang bị chiếm bởi tiến trình khác. | Tìm và tắt tiến trình đang chạy (dùng `lsof -i :<port>` hoặc `netstat`), hoặc dùng tham số `--urls` để đổi cổng. |
| **`dotnet ef: command not found`** | Chưa khôi phục công cụ CLI cục bộ. | Chạy lệnh `dotnet tool restore` tại thư mục `backend/`. |

---

## 10. Checklist kiểm tra nhanh

- [ ] Đã cài đặt .NET 10 SDK (`dotnet --version` hiển thị 10.0.401+).
- [ ] Dịch vụ PostgreSQL đang chạy ở cổng 5432.
- [ ] Đã tạo đủ 3 database (`ridelite_trip`, `ridelite_matching_pricing`, `ridelite_payment`).
- [ ] Đã chạy `dotnet tool restore` thành công.
- [ ] Đã chạy `dotnet ef database update` cho cả 3 microservice.
- [ ] `dotnet build RideLite.slnx` thành công (`0 Error(s)`).
- [ ] `dotnet test RideLite.slnx` vượt qua 241/241 bài kiểm thử.
- [ ] Khởi động thành công 4 terminal (Gateway: 5000, Trip: 5001, MatchingPricing: 5002, Payment: 5003).
- [ ] Gọi thử `health` endpoint của cả 4 ứng dụng trả về HTTP 200 OK.
