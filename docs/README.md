# TÀI LIỆU DỰ ÁN RIDELITE (PROJECT DOCUMENTATION)

Thư mục này chứa toàn bộ các văn bản, tài liệu đặc tả yêu cầu, hướng dẫn kỹ thuật và báo cáo đối chiếu kiến trúc của dự án **RideLite** (Môn học: Kiến trúc Phần mềm - SGU).

---

## Danh mục tài liệu

| STT | Tài liệu | Định dạng | Mô tả nội dung |
| :-: | :--- | :-: | :--- |
| 1 | [RideLite-KienTrucPhanMem.pdf](file:///run/media/thuatdv/16AAC624AAC5FFEB/SGU/Nam4/Ki_1/T2-Chieu-KTPM/ride-lite/docs/RideLite-KienTrucPhanMem.pdf) | PDF | **Đề tài & Yêu cầu Đồ án**: Đặc tả yêu cầu kiến trúc hệ thống chia sẻ chuyến đi quy mô nhỏ của giảng viên (Kiến trúc Microservices, Clean Architecture, Design Patterns, Công thức tính giá, Cơ sở dữ liệu). |
| 2 | [BAO_CAO_DOI_CHIEU_YEU_CAU.md](file:///run/media/thuatdv/16AAC624AAC5FFEB/SGU/Nam4/Ki_1/T2-Chieu-KTPM/ride-lite/docs/BAO_CAO_DOI_CHIEU_YEU_CAU.md) | Markdown | **Báo cáo đối chiếu kiến trúc chi tiết**: Đánh giá chi tiết từng yêu cầu trong file PDF với hiện trạng mã nguồn (State Pattern, Factory Pattern, Strategy Pattern, Database-per-Service, CQRS, 499 tests) kèm lộ trình đề xuất cải tiến. |
| 3 | [HUONG_DAN_CAI_DAT.md](file:///run/media/thuatdv/16AAC624AAC5FFEB/SGU/Nam4/Ki_1/T2-Chieu-KTPM/ride-lite/docs/HUONG_DAN_CAI_DAT.md) | Markdown | **Hướng dẫn cài đặt & vận hành**: Hướng dẫn chi tiết thiết lập môi trường .NET 10, khởi tạo 3 cơ sở dữ liệu PostgreSQL, áp dụng Migration EF Core và chạy các Microservices / API Gateway. |

---

## Chi tiết các tài liệu

### 1. [Đề tài yêu cầu: RideLite-KienTrucPhanMem.pdf](file:///run/media/thuatdv/16AAC624AAC5FFEB/SGU/Nam4/Ki_1/T2-Chieu-KTPM/ride-lite/docs/RideLite-KienTrucPhanMem.pdf)
- **Tác giả**: Giảng viên phụ trách môn Kiến trúc Phần mềm - Khoa CNTT - Đại học Sài Gòn (SGU).
- **Phạm vi**: 11 trang đặc tả nghiệp vụ RideLite (Trip, Matching & Pricing, Payment), yêu cầu áp dụng các phong cách kiến trúc và mẫu thiết kế.

### 2. [Báo cáo đối chiếu: BAO_CAO_DOI_CHIEU_YEU_CAU.md](file:///run/media/thuatdv/16AAC624AAC5FFEB/SGU/Nam4/Ki_1/T2-Chieu-KTPM/ride-lite/docs/BAO_CAO_DOI_CHIEU_YEU_CAU.md)
- **Nội dung chính**:
  - Đối chiếu 11 nhóm tiêu chí của đề bài với hiện trạng mã nguồn backend.
  - Phân tích chi tiết việc triển khai **State Pattern** (7 trạng thái vòng đời Trip), **Factory Pattern** (`TripStateFactory` và `PricingStrategyFactory`), **Strategy Pattern** (3 thuật toán tính giá: Regular, Surge, Discounted theo công thức toán học chuẩn).
  - Kết quả kiểm thử tự động: 499/499 tests pass 100%.
  - Lộ trình đề xuất cải tiến (API Gateway YARP, Docker Compose, HTTP Client liên dịch vụ, JWT Auth, MassTransit/RabbitMQ).

### 3. [Hướng dẫn cài đặt: HUONG_DAN_CAI_DAT.md](file:///run/media/thuatdv/16AAC624AAC5FFEB/SGU/Nam4/Ki_1/T2-Chieu-KTPM/ride-lite/docs/HUONG_DAN_CAI_DAT.md)
- **Nội dung chính**:
  - Yêu cầu cấu hình máy chủ/máy trạm (.NET SDK 10.0, PostgreSQL 15+).
  - Hướng dẫn chạy các lệnh migration cho 3 database: `ridelite_trip`, `ridelite_matching_pricing`, `ridelite_payment`.
  - Hướng dẫn chạy 4 dịch vụ độc lập và kiểm thử qua Postman Collection.

