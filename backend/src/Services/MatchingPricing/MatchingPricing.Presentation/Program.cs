using System.Text.Json.Serialization;
using MatchingPricing.Infrastructure;
using MatchingPricing.Presentation.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Tu choi field la de client khong tu chen don gia hoac he so nhay cam
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow);
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<QuoteExceptionHandler>();

// Gom dang ky ha tang va query handler theo cung mau voi cac service khac
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Middleware tra ProblemDetails an toan ke ca trong moi truong Development
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

// YARP hien giu nguyen tien to, alias noi bo tuong ung hop dong PDF
app.MapGet("/matching-pricing/health", () => new { status = "Healthy", module = "MatchingPricing" });
app.MapGet("/health", () => new { status = "Healthy", module = "MatchingPricing" });
app.UseAuthorization();
app.MapControllers();
app.Run();

// Cho phep WebApplicationFactory kiem tra pipeline HTTP cua service
public partial class Program;
