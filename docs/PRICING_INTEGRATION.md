# Tích hợp Pricing cho Trip

Pricing chạy độc lập, không tham chiếu Trip Domain và không cần database. Đơn vị: km, phút, VND. Contract JSON dùng camelCase và số decimal, không dùng chuỗi tiền tệ.

## Cấu trúc module sau khi đồng bộ mẫu từ main

Module dùng cùng cách chia Application theo feature, tách HTTP contracts và đăng ký DI trong Infrastructure như Payment. Báo giá không ghi trạng thái nên được tổ chức thành query; handler chạy đồng bộ vì hiện chưa gọi I/O bên ngoài.

```text
MatchingPricing.Domain/
  ValueObjects/                         # FareParameters, PricingContext, FareBreakdown
  Services/                             # FareFormula
  Strategies/                           # IPricingStrategy, StandardPricing, SurgePricing, PromoPricing
  Factories/                            # PricingStrategyFactory
MatchingPricing.Application/
  Abstractions/Pricing/                  # ITripMetricsEstimator
  Configuration/                        # PricingOptions, PricingOptionsValidator
  Exceptions/                           # QuoteValidationException
  Features/Quote/Queries/GetQuote/       # GetQuoteQuery, GetQuoteHandler, GetQuoteResult
  Models/Pricing/                       # TripMetrics
  Services/Pricing/                     # FareRoundingPolicy
MatchingPricing.Infrastructure/
  DependencyInjection.cs                # AddInfrastructure
  Services/Pricing/                     # ConfiguredTripMetricsEstimator
MatchingPricing.Presentation/
  Contracts/Requests/                    # QuoteRequest
  Contracts/Responses/                   # QuoteResponse
  Controllers/                          # QuoteController
  ExceptionHandlers/                    # QuoteExceptionHandler
```

[QuoteController](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/Controllers/QuoteController.cs) ánh xạ request HTTP sang query và result sang response. [GetQuoteHandler](../backend/src/Services/MatchingPricing/MatchingPricing.Application/Features/Quote/Queries/GetQuote/GetQuoteHandler.cs) chứa luồng xử lý trước đây của QuoteService; Application không dùng DTO từ Presentation. [AddInfrastructure](../backend/src/Services/MatchingPricing/MatchingPricing.Infrastructure/DependencyInjection.cs) đăng ký Options, estimator, handler và các thành phần giá. Route, JSON, công thức, rounding và định dạng lỗi HTTP giữ nguyên sau refactor.

## Endpoint và route

| Cách gọi | Method | URL mặc định |
| --- | --- | --- |
| Gateway công khai | POST | `http://localhost:5000/matching-pricing/quote` |
| Service trực tiếp | POST | `http://localhost:5002/quote` |
| Service với tiền tố | POST | `http://localhost:5002/matching-pricing/quote` |
| Health qua Gateway | GET | `http://localhost:5000/matching-pricing/health` |
| Health nội bộ | GET | `http://localhost:5002/health` |

Các cổng lấy từ launchSettings thực tế. YARP đã chuyển `/matching-pricing/{**catch-all}` tới cổng 5002 và giữ nguyên tiền tố; không cần sửa Gateway. Hai route báo giá dùng chung một action. Health trả `{"status":"Healthy","module":"MatchingPricing"}` cùng HTTP 200.

## Request

```json
{
  "distanceKm": 10,
  "durationMinutes": 20,
  "pickupZone": "default",
  "promotionCode": null
}
```

| Field | Quy tắc |
| --- | --- |
| `distanceKm` | Bắt buộc trong adapter hiện tại, >= 0; thiếu/null trả 400 vì chưa có nguồn dữ liệu tuyến đường |
| `durationMinutes` | Tùy chọn, >= 0; thiếu/null thì tính `distanceKm * 60 / AverageSpeedKmPerHour` |
| `pickupZone` | Tùy chọn, mặc định `default`; phải thuộc danh sách `Pricing:SurgeZones` trên server |
| `promotionCode` | Tùy chọn/null; phải thuộc `Pricing:Promotions`; mã không hợp lệ trả 400 |

Giá trị 0 là hợp lệ và không bị xem là thiếu. Không gửi `distanceRate`, `timeRate`, `surgeMultiplier`, `tollFee`, `promotionFactor`, `taxRate` hoặc field ngoài contract; server từ chối field lạ bằng 400. Mã vùng và mã ưu đãi phân biệt chữ hoa/thường.

## Cấu hình minh họa và bốn tổ hợp

Section `Pricing` trong [appsettings.json](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/appsettings.json) chứa `DistanceRate=10000`, `TimeRate=1500`, `TollFee=7000`, `TaxRate=0`, `AverageSpeedKmPerHour=30`, vùng `default=1`, `busy-demo=2`, ưu đãi `DEMO20=0.8`.

Đây là dữ liệu minh họa cho bài tập, **không phải bảng giá chính thức**. Toll hiện là khoản cố định do server cấu hình; vì vậy chuyến có hai chỉ số bằng 0 vẫn có toll. Nếu nghiệp vụ sau này yêu cầu toll theo tuyến, bổ sung resolver đáng tin cậy tại server.

| Request bổ sung cho chuyến 10 km, 20 phút | Tổng VND |
| --- | ---: |
| Không có | 137000 |
| `"pickupZone":"busy-demo"` | 267000 |
| `"promotionCode":"DEMO20"` | 109600 |
| Cả hai | 213600 |

`busy-demo` là vùng giá cố định trong cấu hình, chưa phải dữ liệu cung/cầu thời gian thực. `DEMO20` là chương trình demo công khai cho mọi người gọi, chưa kiểm tra rider, thời hạn hay lượt sử dụng. Trip phải chọn vùng theo điểm đón đã xác minh; khi triển khai thật cần resolver theo tọa độ và cơ chế kiểm tra điều kiện ưu đãi, thay cho việc tin mã vùng do người dùng khai báo. Chưa có cơ chế xác thực trong repository và phần này không bổ sung hệ thống xác thực.

## Response ví dụ

```json
{
  "distanceKm": 10,
  "durationMinutes": 20,
  "distanceFare": 100000,
  "timeFare": 30000,
  "subtotal": 130000,
  "surgeAmount": 0,
  "tollFee": 7000,
  "promotionDiscount": 0,
  "taxAmount": 0,
  "roundingAdjustment": 0,
  "totalFare": 137000,
  "currency": "VND"
}
```

Công thức giữ đúng mục 2.3 PDF:

```text
p = ((DistanceRate * DistanceKm + TimeRate * DurationMinutes)
     * SurgeMultiplier + TollFee) * PromotionFactor * (1 + TaxRate)
```

Surge chỉ áp dụng vào quãng đường và thời gian. Toll được cộng trước ưu đãi; thuế tính sau ưu đãi. Domain và các strategy không làm tròn. Application chỉ làm tròn `totalFare` tới 0 chữ số thập phân với `MidpointRounding.AwayFromZero`; các thành phần giữ độ chính xác decimal.

Đối chiếu tổng bằng `subtotal + surgeAmount + tollFee - promotionDiscount + taxAmount + roundingAdjustment = totalFare`. Trường `roundingAdjustment` công khai chênh lệch làm tròn, ví dụ giá thô 100.5 cho tổng 101 và điều chỉnh 0.5. Trip nên lưu quote do server trả, không tự tính lại bằng `double` hoặc dùng quote làm chứng từ thanh toán chính thức.

## Trạng thái và lỗi

| HTTP | Ý nghĩa / cách xử lý |
| --- | --- |
| 200 | Báo giá thành công; dùng `totalFare`, `currency` và breakdown |
| 400 | Thiếu dữ liệu, số âm, overflow, JSON sai, field lạ hoặc mã chính sách không hợp lệ; sửa request, không retry tự động |
| 500 | Lỗi nội bộ, ProblemDetails không chứa stack trace; báo lỗi báo giá và ghi correlation/trace nếu có |
| 502/503/504 | Gateway hoặc dịch vụ chưa sẵn sàng; không xem là báo giá thành công |

Ví dụ lỗi nghiệp vụ:

```json
{
  "title": "Invalid quote request",
  "status": 400,
  "detail": "DistanceKm is required because no route data is available to estimate distance",
  "instance": "/matching-pricing/quote"
}
```

Trip dùng HttpClient từ DI với timeout **5 giây** và truyền CancellationToken của request. Repository chưa có convention HttpClient chung; đây là đề xuất tích hợp, không thay đổi Trip. Quote không ghi trạng thái nên có thể retry tối đa một lần khi lỗi mạng hoặc 502/503/504 với backoff và jitter; không retry 400 hoặc khi caller đã hủy. Nếu hết timeout/retry, báo lỗi tạm thời, không tạo giá giả hoặc mặc định tổng bằng 0. Trước khi chấp nhận giá, kiểm tra response thành công và deserialize đúng contract.

## Chạy và gọi thử

Từ thư mục `backend`, chạy hai lệnh trong hai terminal:

```powershell
dotnet run --project src/Services/MatchingPricing/MatchingPricing.Presentation --launch-profile http
dotnet run --project src/Gateway/ApiGateway --launch-profile http
```

Mở [MatchingPricing.Presentation.http](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.http) và gửi request bằng REST Client hỗ trợ định dạng `.http`. File có 12 request gồm health, bốn tổ hợp, dữ liệu thiếu/sai, ví dụ 137000 và kiểm tra chống ghi đè hệ số. Nếu dịch vụ chạy trên cổng khác, sửa hai biến đầu file và cấu hình destination của Gateway tương ứng trong môi trường chạy.

Kiểm thử backend bằng runner hiện có:

```powershell
dotnet build RideLite.slnx
dotnet test --solution RideLite.slnx
```

Nếu phiên API khác đang khóa đầu ra build, dùng `--artifacts-path "$env:TEMP\RideLite-Pricing-validation"` cho cả hai lệnh; dừng phiên API trước khi build lại vào cùng đầu ra đang được sử dụng.
