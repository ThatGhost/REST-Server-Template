﻿using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend
{
    public class AuthorizationHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authentication",
                In = ParameterLocation.Header,
                Description = "acces token",
                Required = false, // set to false if this is optional
                Schema = new OpenApiSchema
                {
                    Type = "string",
                }
            });
        }
    }
}