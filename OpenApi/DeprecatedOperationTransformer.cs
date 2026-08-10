using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ApiEcommerce.OpenApi;

internal sealed class DeprecatedOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        if (metadata.Any(m => m is ObsoleteAttribute))
        {
            operation.Deprecated = true;
        }

        return Task.CompletedTask;
    }
}
