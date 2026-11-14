# RideLite

## 1. Giới thiệu dự án

RideLite là hệ thống backend cho ứng dụng đặt xe công nghệ (ride-hailing), được kiến trúc theo mô hình **Microservices** và tuân thủ nguyên lý **Clean Architecture** (Domain, Application, Infrastructure, Presentation) chạy trên nền tảng **.NET 10**.

Hệ thống bao gồm 3 dịch vụ nghiệp vụ độc lập, 1 cổng API Gateway và thư viện hạt nhân dùng chung:
- **ApiGateway**: Đóng vai trò là cổng truy cập tập trung, sử dụng YARP (Yet Another Reverse Proxy) để định tuyến yêu cầu tới các microservice.
- **Trip Service**: Quản lý thông tin hành khách (Rider), chu trình tạo và cập nhật trạng thái chuyến đi (Trip lifecycle: Requested, Accepted, DriverArrived, InProgress, Completed, Cancelled).
- **Matching & Pricing Service**: Quản lý tài xế (Driver), tìm kiếm tài xế lân cận theo vị trí GPS, gán cuốc (Assignment) và tính toán ước tính cước phí (Pricing).
- **Payment Service**: Xử lý giao dịch thanh toán chuyến đi, hỗ trợ nhiều cổng thanh toán (Payment Provider) & phương thức thanh toán, hoàn tiền (Refund), ghi chép sổ cái giao dịch (Ledger Entries).
- **SharedKernel**: Thư viện hạt nhân dùng chung định nghĩa lỗi tập trung (`ErrorCode`, `ErrorType`, `AppException`).

Thành viên mới vui lòng xem [Hướng dẫn cài đặt và cấu hình chi tiết](docs/HUONG_DAN_CAI_DAT.md) để chuẩn bị môi trường, cơ sở dữ liệu và chạy toàn bộ dịch vụ.

---

## 2. Cấu trúc thư mục

```text
ride-lite/
├── backend/
│   ├── RideLite.slnx                                   # Tập hợp toàn bộ dự án backend
│   ├── global.json                                     # Phiên bản .NET SDK (10.0.401+)
│   ├── dotnet-tools.json                               # Khai báo công cụ dotnet-ef (v10.0.14)
│   ├── src/
│   │   ├── BuildingBlocks/
│   │   │   └── SharedKernel/                           # Lớp dùng chung: Xử lý ngoại lệ chuẩn hóa
│   │   ├── Gateway/
│   │   │   └── ApiGateway/                             # API Gateway YARP (Port 5000)
│   │   └── Services/
│   │       ├── Trip/                                   # Dịch vụ Chuyến đi (Port 5001)
│   │       ├── MatchingPricing/                        # Dịch vụ Ghép tài xế & Giá (Port 5002)
│   │       └── Payment/                                # Dịch vụ Thanh toán & Hoàn tiền (Port 5003)
│   ├── tests/                                          # 9 dự án kiểm thử xUnit (241 test cases)
│   ├── Trip_Service.postman_collection.json            # Collection Postman cho Trip Service
│   ├── Matching_Pricing_Service.postman_collection.json# Collection Postman cho Matching & Pricing Service
│   └── Payment_Service.postman_collection.json         # Collection Postman cho Payment Service
├── docs/
│   └── HUONG_DAN_CAI_DAT.md                            # Hướng dẫn cài đặt & cấu hình chi tiết
└── README.md                                           # Giới thiệu tổng quan dự án
```

---

## 3. Yêu cầu hệ thống

- **Hệ điều hành**: Linux (Bash), macOS, hoặc Windows (Git Bash).
- **.NET SDK**: Phiên bản **10.0.401** trở lên (nhánh `.NET 10.0` theo `global.json`).
- **PostgreSQL**: Phiên bản 15.x / 16.x cài trên máy (cổng mặc định `5432`).
- **Git** & **Postman**: Lấy mã nguồn và kiểm thử API nghiệp vụ.

---

## 4. Hướng dẫn cài đặt nhanh

### Bước 1: Khởi động PostgreSQL và tạo 3 Cơ sở dữ liệu
Đảm bảo PostgreSQL đang chạy ở cổng `5432`. Tạo 3 database cho các dịch vụ (mật khẩu mặc định trong code: `01052005`):
```sql
CREATE DATABASE ridelite_trip;
CREATE DATABASE ridelite_matching_pricing;
CREATE DATABASE ridelite_payment;
```
*(Nếu dùng mật khẩu khác, hãy cập nhật lại `ConnectionStrings:Database` trong `appsettings.json` của từng service).*

### Bước 2: Khôi phục công cụ và gói phụ thuộc
Chuyển vào thư mục `backend/`:
```bash
cd backend
dotnet tool restore
dotnet restore RideLite.slnx
```

### Bước 3: Áp dụng Migration vào Cơ sở dữ liệu
Chạy migration cho cả 3 microservice từ thư mục `backend/`:
```bash
# 1. Trip Service
dotnet ef database update --project src/Services/Trip/Trip.Infrastructure/Trip.Infrastructure.csproj --startup-project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj

# 2. Matching & Pricing Service
dotnet ef database update --project src/Services/MatchingPricing/MatchingPricing.Infrastructure/MatchingPricing.Infrastructure.csproj --startup-project src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.csproj

# 3. Payment Service
dotnet ef database update --project src/Services/Payment/Payment.Infrastructure/Payment.Infrastructure.csproj --startup-project src/Services/Payment/Payment.Presentation/Payment.Presentation.csproj
```

### Bước 4: Biên dịch mã nguồn
```bash
dotnet build RideLite.slnx --no-restore
```

---

## 5. Khởi chạy và kiểm tra các dịch vụ

Mở **4 terminal riêng biệt** tại thư mục `backend/` và khởi chạy các dịch vụ:

```bash
# Terminal 1: Trip Service
dotnet run --project src/Services/Trip/Trip.Presentation/Trip.Presentation.csproj --launch-profile http

# Terminal 2: MatchingPricing Service
dotnet run --project src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.csproj --launch-profile http

# Terminal 3: Payment Service
dotnet run --project src/Services/Payment/Payment.Presentation/Payment.Presentation.csproj --launch-profile http

# Terminal 4: ApiGateway
dotnet run --project src/Gateway/ApiGateway/ApiGateway.csproj --launch-profile http
```

### Bảng đối chiếu cổng và Health Check:

| Dịch vụ | Cổng | URL Kiểm tra (Health Check) | Kết quả mong đợi |
| :--- | :--- | :--- | :--- |
| **ApiGateway** | `5000` | <http://localhost:5000/> | `Hello World!` |
| **Trip** | `5001` | <http://localhost:5001/trip/health> | `{"status":"trip-service active"}` |
| **MatchingPricing** | `5002` | <http://localhost:5002/matching/health> | `{"status":"matchingpricing-service active"}` |
| **Payment** | `5003` | <http://localhost:5003/payment/health> | `{"status":"payment-service active"}` |

### Tài liệu OpenAPI & Postman:
- **OpenAPI JSON**: Truy cập `<service-url>/openapi/v1.json` khi chạy ở profile `http` (môi trường Development).
- **Postman Collections**: Import 3 tệp collection có sẵn trong thư mục `backend/` để test toàn diện các luồng nghiệp vụ.

---

## 6. Chạy bộ kiểm thử (Testing)

Dự án sở hữu 241 bài kiểm thử tự động (Unit Test & Integration Test với In-memory SQLite, không phụ thuộc vào PostgreSQL):

```bash
cd backend
dotnet test RideLite.slnx
```

Kết quả mong đợi: Toàn bộ 241 bài test đều vượt qua (`Passed! 241 passed, 0 failed`).

---

## 7. Tài liệu chi tiết
Chi tiết về cấu hình nâng cao, đổi cổng, biến môi trường và xử lý lỗi cụ thể xem tại:
👉 [**Hướng dẫn cài đặt chi tiết (docs/HUONG_DAN_CAI_DAT.md)**](docs/HUONG_DAN_CAI_DAT.md)
