using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Spark.Engine.Core;
using Spark.Engine.Service;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Spark.Engine.Middleware
{
    /// <summary>
    ///     read response body: https://stackoverflow.com/questions/43403941/how-to-read-asp-net-core-response-body
    ///     write response body:
    ///     https://stackoverflow.com/questions/33178983/how-to-create-a-response-message-and-add-content-string-to-it-in-asp-net-5-mvc
    ///     main article on writing middleware:
    ///     https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware#writing-middleware
    /// </summary>
    public class FhirResponseMiddleware
    {
        private readonly RequestDelegate _next;

//        private const string ContentType = "text/plain";
        private FhirJsonParser _fhirJsonParser;

        // Must have constructor with this signature, otherwise exception at run time
        public FhirResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private static string ReadBody(HttpResponse response)
        {
            //process context.request
            string body = new StreamReader(response.Body).ReadToEnd();
            response.Body.Position = 0;

            return body;
        }


        /// <summary>
        ///     Attempts to retrieve the value of the content for the
        ///     <see cref="T:System.Net.Http.HttpResponseMessageExtensions" />.
        /// </summary>
        /// <returns>The result of the retrieval of value of the content.</returns>
        /// <param name="response"></param>
        /// <param name="value">The value of the content.</param>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// formerly in HttpResponseMessageExtensions
//        private bool TryGetContentValue<T>(HttpResponse response, out T value)
//        {
//            string content = ReadBody(response);
//
//            if (content == null || response== null)
//                throw Error.BadRequest("null response");
////            typeof(T).Hl7.Fhir.Utility.ReflectionHelper.CanBeTreatedAsType(typeof(Resource));
//            typeof(T).CanBeTreatedAsType(typeof(T));
////            var fhirjson = _fhirJsonParser.Parse<Resource>(body);
//
////            var content = response as ObjectContent;
//            if (content is T)
//            {
//                value = content;
//                return true;
//            }
////            value = default(T);
//            return false;
//        }
        private static bool TryGetContentValueExplicit(HttpResponse response, out FhirResponse value)
        {
            dynamic dynamicContent = ReadBody(response);
            //todo refactor and put in try/catch or error handling
            if (string.IsNullOrEmpty(dynamicContent.code) || string.IsNullOrEmpty(dynamicContent.key) || string.IsNullOrEmpty(dynamicContent.resource)) //todo remove resource check?
            {
                value = default(FhirResponse);
                return false;
            }
            var content = new FhirResponse(dynamicContent.code, dynamicContent.key, dynamicContent.resource);
            if (content == null || response == null)
                throw Error.BadRequest("null response");
            value = content;
            return true;
        }

        // ...

        /// <summary>
        ///     //https://stackoverflow.com/questions/33178983/how-to-create-a-response-message-and-add-content-string-to-it-in-asp-net-5-mvc
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task GenerateResponse(HttpContext context)
        {
//            string title = context.Request.Query["title"];
//            return string.Format("Title of the report: {0}", title);

            HttpResponse response = context.Response;
            FhirResponse fhirResponse;

//            if (!TryGetContentValue(response, out fhirResponse))
            if (!TryGetContentValueExplicit(response, out fhirResponse))
            {
                return _next(context);
            }

            byte[] data = Encoding.UTF8.GetBytes(fhirResponse.ToString());
            response.ContentType = Const.ContentType;
            return response.Body.WriteAsync(data, 0, data.Length);
        }

//        private string GetContentType()
//        {
//            return ContentType;
//        }

        /// <summary>
        ///     automatically called by the framework on incoming and outgoing http messages
        /// </summary>
        /// <param name="context"></param>
        /// <param name="fhirJsonParser"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, FhirJsonParser fhirJsonParser)
        {
            Debugger.Break();
            _fhirJsonParser = fhirJsonParser;

            await _next(context);

            Debugger.Break();
            //Start process context.Response

            //            context.Response.ContentType = GetContentType();//todo do we need this?

            await GenerateResponse(context);
        }
    }

    public static class MyHandlerExtensions
    {
        public static IApplicationBuilder UseFhirResponseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FhirResponseMiddleware>();
        }
    }
}