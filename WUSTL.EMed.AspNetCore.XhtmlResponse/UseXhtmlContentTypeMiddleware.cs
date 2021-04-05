// <copyright file="UseXhtmlContentTypeMiddleware.cs" company="Washington University in St. Louis">
// Copyright (c) 2021 Washington University in St. Louis. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>

namespace WUSTL.EMed.AspNetCore.XhtmlResponse
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Static class for adding XhtmlContentType middlware.
    /// </summary>
    public static class UseXhtmlContentTypeMiddleware
    {
        private const string TextHtml = "text/html";

        private const string ApplicationXhtmlXml = "application/xhtml+xml";

        private const string ApplicationXml = "application/xml";

        /// <summary>
        /// Adds a response handler that changes html responses into xhtml responses, when available.
        /// </summary>
        /// <param name="app">An <see cref="IApplicationBuilder"/> instance.</param>
        /// <returns>The given <see cref="IApplicationBuilder"/> instance.</returns>
        public static IApplicationBuilder UseXhtmlContentType(this IApplicationBuilder app)
        {
            return app.Use(async (httpContext, next) =>
            {
                httpContext.Response.OnStarting(
                    (state) =>
                    {
                        var contentType = httpContext.Response.GetTypedHeaders()?.ContentType;
                        if (contentType is null)
                            return Task.CompletedTask;

                        var responseContentType = contentType.MediaType.ToString() ?? string.Empty;
                        if (!responseContentType.Equals(TextHtml, StringComparison.OrdinalIgnoreCase))
                            return Task.CompletedTask;

                        var acceptTypes = httpContext.Request.GetTypedHeaders()?.Accept?.Select(_ => _.MediaType.ToString()) ?? Enumerable.Empty<string>();
                        if (acceptTypes.Contains(ApplicationXhtmlXml, StringComparer.OrdinalIgnoreCase))
                            contentType.MediaType = ApplicationXhtmlXml;
                        else if (acceptTypes.Contains(ApplicationXml, StringComparer.OrdinalIgnoreCase))
                            contentType.MediaType = ApplicationXml;

                        httpContext.Response.ContentType = contentType.ToString();
                        return Task.CompletedTask;
                    }, null);

                await next.Invoke();
            });
        }
    }
}
