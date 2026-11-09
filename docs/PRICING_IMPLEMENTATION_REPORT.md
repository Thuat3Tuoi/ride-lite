# Báo cáo triển khai Pricing – SwiftRide

## A. Files changed

Chỉ thay đổi Pricing, các project test tương ứng, solution để đăng ký test và tài liệu. Không sửa mã nguồn Trip, Payment, Matching tìm tài xế hoặc Gateway.

| Thay đổi | File |
| --- | --- |
| Chỉnh sửa | [backend/RideLite.slnx](../backend/RideLite.slnx) |
| Xóa lớp mẫu | `backend/src/Services/MatchingPricing/MatchingPricing.Application/Class1.cs` |
| Chỉnh sửa | [backend/src/Services/MatchingPricing/MatchingPricing.Application/MatchingPricing.Application.csproj](../backend/src/Services/MatchingPricing/MatchingPricing.Application/MatchingPricing.Application.csproj) |
| Xóa lớp mẫu | `backend/src/Services/MatchingPricing/MatchingPricing.Domain/Class1.cs` |
| Xóa lớp mẫu | `backend/src/Services/MatchingPricing/MatchingPricing.Infrastructure/Class1.cs` |
| Chỉnh sửa | [backend/src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.http](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.http) |
| Chỉnh sửa | [backend/src/Services/MatchingPricing/MatchingPricing.Presentation/Program.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/Program.cs) |
| Chỉnh sửa | [backend/src/Services/MatchingPricing/MatchingPricing.Presentation/appsettings.json](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/appsettings.json) |
| Xóa lớp mẫu | `backend/tests/MatchingPricing.Domain.Tests/UnitTest1.cs` |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/FareRoundingPolicy.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/FareRoundingPolicy.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/IQuoteService.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/IQuoteService.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/ITripMetricsEstimator.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/ITripMetricsEstimator.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/PricingOptions.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/PricingOptions.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/PricingOptionsValidator.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/PricingOptionsValidator.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteRequest.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteRequest.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteResponse.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteResponse.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteService.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteService.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteValidationException.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Application/QuoteValidationException.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/FareBreakdown.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/FareBreakdown.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/FareFormula.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/FareFormula.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/FareParameters.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/FareParameters.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/IPricingStrategy.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/IPricingStrategy.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/PricingContext.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/PricingContext.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/PricingStrategyFactory.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/PricingStrategyFactory.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/PromoPricing.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/PromoPricing.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/StandardPricing.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/StandardPricing.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Domain/SurgePricing.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Domain/SurgePricing.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Infrastructure/ConfiguredTripMetricsEstimator.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Infrastructure/ConfiguredTripMetricsEstimator.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Presentation/Controllers/QuoteController.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/Controllers/QuoteController.cs) |
| Tạo mới | [backend/src/Services/MatchingPricing/MatchingPricing.Presentation/QuoteExceptionHandler.cs](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/QuoteExceptionHandler.cs) |
| Tạo mới | [backend/tests/MatchingPricing.Application.Tests/MatchingPricing.Application.Tests.csproj](../backend/tests/MatchingPricing.Application.Tests/MatchingPricing.Application.Tests.csproj) |
| Tạo mới | [backend/tests/MatchingPricing.Application.Tests/QuoteServiceTests.cs](../backend/tests/MatchingPricing.Application.Tests/QuoteServiceTests.cs) |
| Tạo mới | [backend/tests/MatchingPricing.Domain.Tests/FareFormulaTests.cs](../backend/tests/MatchingPricing.Domain.Tests/FareFormulaTests.cs) |
| Tạo mới | [backend/tests/MatchingPricing.Domain.Tests/PricingStrategyTests.cs](../backend/tests/MatchingPricing.Domain.Tests/PricingStrategyTests.cs) |
| Tạo mới | [backend/tests/MatchingPricing.Presentation.Tests/MatchingPricing.Presentation.Tests.csproj](../backend/tests/MatchingPricing.Presentation.Tests/MatchingPricing.Presentation.Tests.csproj) |
| Tạo mới | [backend/tests/MatchingPricing.Presentation.Tests/QuoteApiTests.cs](../backend/tests/MatchingPricing.Presentation.Tests/QuoteApiTests.cs) |
| Tạo mới | [docs/PRICING_INTEGRATION.md](../docs/PRICING_INTEGRATION.md) |
| Tạo mới | [docs/PRICING_IMPLEMENTATION_REPORT.md](./PRICING_IMPLEMENTATION_REPORT.md) |

Các lớp mẫu và test `Assert.True(true)` của Pricing đã được thay thế bằng mô hình và kiểm thử thực tế.

## B. Implementation

- **Domain:** FareParameters và PricingContext bất biến, kiểm tra số âm và phạm vi hệ số. FareFormula thuần C#, dùng decimal, trả đủ breakdown, không phụ thuộc ASP.NET, database hoặc configuration
- **Strategy:** StandardPricing tạo tham số trung tính; SurgePricing và PromoPricing là decorator cấu hình hệ số theo context. Factory ghép bốn tổ hợp. Các decorator gán hệ số thay vì nhân kết quả đã có thuế/toll; công thức chỉ chạy một lần sau khi ghép, tránh điều chỉnh lặp và sai thứ tự
- **Application:** QuoteService kiểm tra request, tra chính sách trong Options, dùng trực tiếp hai chỉ số đã có hoặc gọi estimator khi thiếu, chọn strategy và hoàn thiện quote. Overflow từ dữ liệu chuyến được đổi thành lỗi validation
- **Infrastructure:** ConfiguredTripMetricsEstimator chỉ tính thời gian từ quãng đường và vận tốc cấu hình. Không đủ dữ liệu xác định distance thì từ chối
- **Presentation:** QuoteController dùng chung action cho route công khai và nội bộ. ProblemDetails cho 400/500; lỗi 500 không lộ stack trace. JSON có field ngoài contract bị từ chối. ValidateOnStart ngăn service khởi động với cấu hình sai

## C. Pricing formula

`p = ((cd * d + ct * t) * s + cm) * eta * (1 + T)`

Đã đối chiếu mục 2.3, 2.4, 3, 5.2, 6, 7 và 8 của PDF được đính kèm. Không thêm BaseFare. Toll không bị nhân surge; khuyến mãi áp dụng sau toll và trước thuế.

Cấu hình minh họa: `DistanceRate=10000`, `TimeRate=1500`, `TollFee=7000`, `TaxRate=0`, `AverageSpeedKmPerHour=30`, vùng `default=1`, `busy-demo=2`, mã `DEMO20=0.8`. Đây không phải bảng giá chính thức.

Chỉ làm tròn total tại Application: decimal, 0 chữ số thập phân, MidpointRounding.AwayFromZero. Thành phần giữ độ chính xác, thêm RoundingAdjustment để đối chiếu:

`Subtotal + SurgeAmount + TollFee - PromotionDiscount + TaxAmount + RoundingAdjustment = TotalFare`

## D. Test results

Thực thi trên Windows với SDK 10.0.403, phù hợp global.json. Do PATH chưa có dotnet, dùng executable tại `C:\Program Files\dotnet\dotnet.exe`. Một phiên API có sẵn đang khóa output mặc định nên build và kiểm tra cuối dùng thư mục artifacts tạm riêng.

Các lệnh chạy từ thư mục backend:

```powershell
& 'C:\Program Files\dotnet\dotnet.exe' build RideLite.slnx --artifacts-path "$env:TEMP\RideLite-Pricing-validation"
& 'C:\Program Files\dotnet\dotnet.exe' test --solution RideLite.slnx --no-build --artifacts-path "$env:TEMP\RideLite-Pricing-validation"
```

Build cuối: **thành công, 0 warning, 0 error**. Test cuối: **75/75 pass, 0 fail, 0 skip**. Pricing có 73 case: Domain 25, Application 22, Presentation 26. Trip và Payment mỗi module có một test mẫu sẵn có; hai test này không xác minh nghiệp vụ của các module đó.

Đã viết test trước logic từng phần và thực thi trạng thái đỏ trước khi triển khai: Domain thiếu các model, Strategy thiếu abstraction, Application thiếu service; API ban đầu trả 404/health chưa đúng. Test ước lượng phát hiện sai số 19.999999999999999999999999998 khi chia trước đổi đơn vị; đã sửa thứ tự phép tính, giữ expected là 20, rồi chạy lại thành công.

Bảng nhóm theo test method; các dòng nhiều case ghi số biến thể được runner thực thi:

| Test name | Expected | Actual | Pass/Fail |
| --- | --- | --- | --- |
| Calculate_IllustrativeTrip_Returns137000Vnd | Đủ 8 thành phần, total 137000, VND | Khớp toàn bộ breakdown | Pass |
| Calculate_ZeroMetrics_AreValid (3) | 37000 / 107000 / 7000 | 37000 / 107000 / 7000 | Pass |
| Context_NegativeMetrics_AreRejected (2) | ArgumentOutOfRangeException | Đúng exception | Pass |
| Parameters_InvalidValues_AreRejected (7) | Từ chối rate, surge, toll, promo, tax sai | Đúng exception | Pass |
| Calculate_Adjustments_PreserveDecimalPrecisionAndOrder | Total 0.2068, breakdown không bị làm tròn | 0.2068 và đúng từng thành phần | Pass |
| Calculate_Overflow_IsRejected | OverflowException | Đúng exception | Pass |
| Factory_AllCombinations_RespectFormula (5) | Với thuế 10%: 150700 / 293700 / 120560 / 234960 / 0 | Khớp total, breakdown và giá không âm | Pass |
| Standard_IgnoresAdjustments_AndKeepsTollAndTax | 150700, surge và discount bằng 0 | Đúng | Pass |
| Decorators_DuplicateOrReordered_DoNotCompoundAdjustments | 234960, không điều chỉnh lặp | 234960 | Pass |
| Context_InvalidAdjustments_AreRejected (3) | Từ chối surge < 1, promo ngoài [0,1] | Đúng exception | Pass |
| CompleteMetrics_DoNotCallEstimator_AndReturn137000 | 10 km / 20 phút / 137000; estimator không gọi | Đúng cả dữ liệu và Moq verification | Pass |
| Policies_AreResolvedFromServerConfiguration (4) | 137000 / 267000 / 109600 / 213600 | Khớp | Pass |
| MissingDuration_IsEstimatedFromConfiguredSpeed | 20 phút, 137000 | Khớp | Pass |
| InvalidOrInsufficientMetrics_AreRejected (4) | Từ chối thiếu distance, thiếu cả hai, chỉ số âm | QuoteValidationException | Pass |
| ZeroMetrics_ArePreserved (3) | Không thay 0 bằng giá trị ước lượng | 37000 / 107000 / 7000 | Pass |
| ZeroDistance_WithMissingDuration_EstimatesZero | 0 phút | 0 phút | Pass |
| UnknownPolicies_AreRejected (3) | Từ chối vùng/mã lạ hoặc vùng rỗng | QuoteValidationException | Pass |
| Finalization_RoundsOnce_AndExposesReconciliation (3) | 100.4999→100, 100.5→101, 100.5001→101; đối chiếu tổng | Đúng total và roundingAdjustment | Pass |
| ExtremeMetrics_ReturnValidationInsteadOfOverflow | Lỗi validation rõ ràng | QuoteValidationException | Pass |
| InvalidServerConfiguration_IsRejected | 9 cấu hình sai bị từ chối; cấu hình đúng được nhận | Đúng cả 10 assertion cấu hình | Pass |
| Quote_ValidRequests_ReturnReconciledBreakdown (7) | HTTP 200, total và breakdown đối chiếu được | Đúng cả alias, 4 tổ hợp, ước lượng, số 0 | Pass |
| Quote_InvalidRequests_Return400 (15) | HTTP 400, application/problem+json | Đúng với thiếu/sai dữ liệu, field nhạy cảm, JSON sai, overflow | Pass |
| Health_ReturnsStructuredStatus (2) | HTTP 200, Healthy / MatchingPricing | Khớp cả 2 route | Pass |
| UnexpectedFailure_Returns500_WithoutInternalDetails | HTTP 500, không có private marker, exception type hoặc stack trace | Đúng | Pass |
| InvalidConfiguration_PreventsStartup | OptionsValidationException | Đúng | Pass |
| Trip.Domain.Tests.UnitTest1.Test1 | Test mẫu sẵn có pass | Pass | Pass |
| Payment.Domain.Tests.UnitTest1.Test1 | Test mẫu sẵn có pass | Pass | Pass |

## E. API results

Đã khởi động Kestrel của Pricing ở `http://localhost:15002` và YARP ở `http://localhost:15000` bằng artifacts vừa build; override destination chỉ trong tham số tiến trình sang cổng 15002. Không sửa cấu hình Gateway trong repository hoặc dừng phiên API có sẵn. Cổng mặc định trong launchSettings và file HTTP vẫn là 5002/5000.

Đã đọc và gửi trực tiếp **12/12 request trong file HTTP**, chỉ thay hai biến base URL cho phiên kiểm thử. Tất cả response thành công đều được đối chiếu từng field JSON với phép tính decimal độc lập.

| Request | Expected | Actual | Pass/Fail |
| --- | --- | --- | --- |
| GET /matching-pricing/health qua YARP | 200, Healthy / MatchingPricing | Khớp JSON | Pass |
| Standard quote | 200, 137000 VND | Khớp toàn bộ breakdown | Pass |
| Surge quote | 200, 267000 VND | Subtotal 130000, surge 130000, toll 7000 | Pass |
| Promo quote | 200, 109600 VND | Subtotal 130000, toll 7000, discount 27400 | Pass |
| Surge + Promo quote | 200, 213600 VND | Subtotal 130000, surge 130000, toll 7000, discount 53400 | Pass |
| Thiếu distance | 400 | 400, ProblemDetails | Pass |
| Thiếu duration | 200, 20 phút, 137000 | Khớp | Pass |
| Distance âm | 400 | 400 | Pass |
| POST /quote nội bộ, ví dụ 137000 | 200, 137000 | Khớp | Pass |
| Thiếu cả hai chỉ số | 400 | 400 | Pass |
| Client chèn promotionFactor | 400 | 400 | Pass |
| Hai chỉ số bằng 0 | 200, toll 7000 | 7000 | Pass |

Response thực tế cho ví dụ 137000:

```json
{"distanceKm":10,"durationMinutes":20,"distanceFare":100000,"timeFare":30000,"subtotal":130000,"surgeAmount":0,"tollFee":7000,"promotionDiscount":0,"taxAmount":0,"roundingAdjustment":0,"totalFare":137000,"currency":"VND"}
```

Cả ba kiểm tra bổ sung đều HTTP 200: API mẫu `/weatherforecast`, health alias `/health`, và Gateway `/`. Lỗi 500 đã được kiểm tra bằng fault injection trong WebApplicationFactory, không phát sinh lỗi cố ý trên Kestrel.

## F. Trip integration

Xem [PRICING_INTEGRATION.md](./PRICING_INTEGRATION.md) cho endpoint, field bắt buộc/tùy chọn, request/response, lỗi và cách gửi [file HTTP](../backend/src/Services/MatchingPricing/MatchingPricing.Presentation/MatchingPricing.Presentation.http).

Trip gọi POST `http://localhost:5002/quote` hoặc qua Gateway tại `http://localhost:5000/matching-pricing/quote`, sử dụng DTO, km/phút/VND. Đề xuất HttpClient timeout 5 giây, truyền CancellationToken, không retry 400 và không dùng giá giả khi lỗi. Chưa sửa Trip hoặc tạo lời gọi HTTP trong module Trip.

## G. Issues

- Chưa có bảng giá chính thức trong repository/PDF; các mức giá và vùng/mã ưu đãi hiện là dữ liệu minh họa
- Surge theo vùng cấu hình tĩnh, chưa tích hợp cung/cầu thời gian thực; chương trình DEMO20 công khai, chưa kiểm tra điều kiện theo rider
- Mã vùng hiện do caller cung cấp; Trip cần xác minh điểm đón, hệ thống thật cần resolver đáng tin cậy thay vì tin mã do người dùng khai báo
- Thiếu distance không thể ước lượng vì chưa có dữ liệu tuyến đường hoặc nhà cung cấp bản đồ; duration chỉ là ước lượng theo vận tốc trung bình
- Toll cố định lấy từ cấu hình, không phải resolver toll theo tuyến
- Chưa triển khai xác thực, Matching tài xế, MongoDB, lưu quote hoặc tích hợp gọi từ Trip, đúng phạm vi Pricing
- README và hướng dẫn cài đặt cũ có mô tả/cổng không còn khớp code hiện tại; tài liệu tích hợp này dùng launchSettings và YARP thực tế
- Chưa có kiểm thử nghiệp vụ thực sự cho Trip/Payment; chỉ xác nhận build toàn solution và test mẫu của hai module vẫn pass
- Build mặc định có thể bị khóa nếu API đang chạy; đã kiểm chứng đầy đủ bằng artifacts riêng, không sửa global.json

Không còn lỗi build hoặc test Pricing đã phát hiện chưa xử lý.

## H. Comment review

Đã quét 28 file C# thuộc Pricing và các test: 35 dòng comment, 0 dòng có ký tự ngoài ASCII và 0 comment kết thúc bằng dấu chấm. Đã đọc lại nội dung để giữ tiếng Việt không dấu, giải thích mục đích nghiệp vụ, validation và xử lý lỗi. `git diff --check` không báo lỗi khoảng trắng.

