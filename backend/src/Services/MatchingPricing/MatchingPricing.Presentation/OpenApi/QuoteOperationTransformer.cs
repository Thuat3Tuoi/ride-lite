using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace MatchingPricing.Presentation.OpenApi;

// Cung cap request mau hop le de thu bon chinh sach gia trong Swagger UI
public sealed class QuoteOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor { ControllerName: "Quote" }
            || operation.RequestBody?.Content is not { } content)
            return Task.CompletedTask;

        // Vi du chi chon ma demo tren server, khong chen don gia hay he so vao request
        foreach (var mediaType in content.Values)
        {
            mediaType.Examples = new Dictionary<string, IOpenApiExample>
            {
                ["standard"] = Example("Standard"),
                ["surge"] = Example("Surge", "busy-demo"),
                ["promo"] = Example("Promo", promotionCode: "DEMO20"),
                ["surge-promo"] = Example("Surge + Promo", "busy-demo", "DEMO20")
            };
        }

        return Task.CompletedTask;
    }

    private static OpenApiExample Example(string summary, string pickupZone = "default", string? promotionCode = null)
        => new()
        {
            Summary = summary,
            Value = new JsonObject
            {
                ["distanceKm"] = 10,
                ["durationMinutes"] = 20,
                ["pickupZone"] = pickupZone,
                ["promotionCode"] = promotionCode
            }
        };
}
