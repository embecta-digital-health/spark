using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Spark.Engine.Core;

namespace Spark.Engine.Formatters
{
    /// <summary>
    /// inspiration: https://www.codefluff.com/write-your-own-asp-net-core-mvc-formatters/
    /// and https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-formatters.
    /// </summary>
    public class JsonFhirInputFormatter : TextInputFormatter
    {
        private static FhirJsonParser _fhirJsonParser;

        public JsonFhirInputFormatter()
        {
            foreach (string mediaType in ContentType.JSON_CONTENT_HEADERS)
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            
            _fhirJsonParser = new FhirJsonParser();
        }

        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            string contentType = context.HttpContext.Request.ContentType;
            if (contentType == null || contentType == "application/json")
                return true;
            return false;
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            Debugger.Break();
            //todo do we need this?
            HttpRequest request = context.HttpContext.Request;
            if (request.ContentLength == 0)
            {
                if (context.ModelType.GetTypeInfo().IsValueType)
                {
                    return InputFormatterResult.SuccessAsync(Activator.CreateInstance(context.ModelType));
                }
                return InputFormatterResult.SuccessAsync(null);
            }

            try
            {
                string body = context.HttpContext.Request.Body.ToString();

                if (typeof(Resource).IsAssignableFrom(context.ModelType))
                {
                    var resource = _fhirJsonParser.Parse<Resource>(body);
                    return InputFormatterResult.SuccessAsync(resource);
                }
                throw Error.Internal("Cannot read unsupported type {0} from body", context.ModelName);
            }
            catch (FormatException exception)
            {
                throw Error.BadRequest("Body parsing failed: " + exception.Message);
            }
        }
    }
//        }
//            headers.ContentType = FhirMediaType.GetMediaTypeHeaderValue(type, ResourceFormat.Json);
//            base.SetDefaultContentHeaders(type, headers, mediaType);
//        {

//        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
}