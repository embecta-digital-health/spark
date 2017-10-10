using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Spark.Engine.Auxiliary;
using Spark.Engine.Extensions;

namespace Spark.Engine.Middleware
{
    public class FhirMediaTypeMiddleware
    {
        private readonly RequestDelegate _nextDelegate;

        public FhirMediaTypeMiddleware(RequestDelegate nextDelegate)
        {
            _nextDelegate = nextDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            Debug.WriteLine("");
            // Call the nextDelegate delegate/middleware in the pipeline
            await _nextDelegate.Invoke(context);

            //await SendAsync(context, new CancellationToken(false));
        }

        private bool isBinaryRequest(HttpRequest request)
        {
//            var ub = new UriBuilder(request.RequestUri);//pre.netCore
            var ub = new UriBuilder(request.GetDisplayUrl());
            return ub.Path.Contains("Binary");
            // HACK: replace quick hack by solid solution.
        }

        private bool isTagRequest(HttpRequest request)
        {
//            var ub = new UriBuilder(request.RequestUri);//pre.netCore
            var ub = new UriBuilder(request.GetDisplayUrl());
            return ub.Path.Contains("_tags");
            // HACK: replace quick hack by solid solution.
        }

        protected async Task SendAsync(HttpContext context, CancellationToken cancellationToken)
        {
            HttpRequest request = context.Request;

            string formatParam = request.GetParameter("_format");
            if (!string.IsNullOrEmpty(formatParam))
            {
                var accepted = ContentType.GetResourceFormatFromFormatParam(formatParam);
                if (accepted != ResourceFormat.Unknown)
                {
                    //                    request.Headers.Accept.Clear();//pre.netCore
                    HttpRequestExtensions.ClearNamedHeaders(request, HttpKnownHeaderNames.Accept);

                    if (accepted == ResourceFormat.Json)
                    {
//                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.JSON_CONTENT_HEADER));//pre.netCore
                        request.Headers.Add(HttpKnownHeaderNames.Accept, new MediaTypeWithQualityHeaderValue(ContentType.JSON_CONTENT_HEADER).ToString());
                    }
                    else
                    {
//                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.XML_CONTENT_HEADER));//pre.netCore
                        request.Headers.Add(HttpKnownHeaderNames.Accept, new MediaTypeWithQualityHeaderValue(ContentType.XML_CONTENT_HEADER).ToString());
                    }
                }
            }

            // BALLOT: binary upload should be determined by the Content-Type header, instead of the Rest url?
            // HACK: passes to BinaryFhirFormatter
            if (isBinaryRequest(request))
            {
                throw new NotImplementedException();
                //pre.netCore
//                if (request.Content.Headers.ContentType != null)
//                {
//                    var format = request.Content.Headers.ContentType.MediaType;
//                    request.Content.Headers.Replace("X-Content-Type", format);
//                }
//
//                request.Content.Headers.ContentType = new MediaTypeHeaderValue(FhirMediaType.BinaryResource);
//                if (request.Headers.Accept.Count == 0)
//                {
//                    request.Headers.Replace("Accept", FhirMediaType.BinaryResource);
//                }
            }

            await context.Response.WriteAsync(context.Response.ToString(), cancellationToken);
        }
    }

    // Instead of using the general purpose DelegatingHandler, could we use IContentNegotiator?
//    public class FhirContentNegotiator : IContentNegotiator
//    {
//        public ContentNegotiationResult Negotiate(Type type, HttpRequest request, IEnumerable<MediaTypeFormatter> formatters)
//        {
//            throw new NotImplementedException();
//        }
//    }
}