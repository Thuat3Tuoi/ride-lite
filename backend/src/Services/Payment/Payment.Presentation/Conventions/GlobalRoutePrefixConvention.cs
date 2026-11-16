using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Payment.Presentation.Conventions;

public sealed class GlobalRoutePrefixConvention(string routePrefix) : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix = new(new RouteAttribute(routePrefix.Trim('/')));

    public void Apply(ApplicationModel application)
    {
        foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
        {
            if (selector.AttributeRouteModel != null)
            {
                var template = selector.AttributeRouteModel.Template;
                if (!string.IsNullOrEmpty(template))
                {
                    if (template.StartsWith("api/", StringComparison.OrdinalIgnoreCase))
                    {
                        var remaining = template["api/".Length..];
                        selector.AttributeRouteModel.Template = $"{_routePrefix.Template}/{remaining}";
                        continue;
                    }
                }

                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
            }
            else
            {
                selector.AttributeRouteModel = _routePrefix;
            }
        }
    }
}
