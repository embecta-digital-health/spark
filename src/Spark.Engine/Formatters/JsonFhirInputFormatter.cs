using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    ///     inspiration: https://www.codefluff.com/write-your-own-asp-net-core-mvc-formatters/
    ///     and https://docs.microsoft.com/en-us/aspnet/core/mvc/advanced/custom-formatters.
    /// </summary>
    public class JsonFhirInputFormatter : TextInputFormatter
    {
        public JsonFhirInputFormatter()
        {
            foreach (string contentType in ContentType.JSON_CONTENT_HEADERS)
            {
                SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(contentType));
            }
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            string contentType = context.HttpContext.Request.ContentType;
            if (contentType == null || ContentType.JSON_CONTENT_HEADERS.Contains(contentType))
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IReadOnlyList<string> GetSupportedContentTypes(string contentType, Type objectType)
        {
            return base.GetSupportedContentTypes(contentType, objectType);
        }

        public override Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

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
                if (typeof(Resource).IsAssignableFrom(context.ModelType))
                {
                    using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                    {
                        string bodyStr = reader.ReadToEnd();
                        var resource = FhirParser.ParseResourceFromJson(bodyStr);
                        return InputFormatterResult.SuccessAsync(resource);
                    }
                }
                throw Error.Internal("Cannot read unsupported type {0} from body", context.ModelName);
            }
            catch (FormatException exception)
            {
                throw Error.BadRequest("Body parsing failed: " + exception.Message);
            }
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding e)
        {
            return ReadAsync(context);
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
//        }
//            headers.ContentType = FhirMediaType.GetMediaTypeHeaderValue(type, ResourceFormat.Json);
//            base.SetDefaultContentHeaders(type, headers, mediaType);
//        {

//        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
}