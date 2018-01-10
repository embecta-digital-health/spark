#region

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Spark.Engine.Core;

#endregion

namespace Spark.Engine.Formatters
{
    public class JsonFhirOutputFormatter : TextOutputFormatter
    {
        public const string ContextOutputLookupKey = "key2497529487";//for loggingmiddleware
        public const string ContextOutputLookupKeyType = "type9470209745";//for loggingmiddleware
        public JsonFhirOutputFormatter()
        {
            foreach (string mediaType in ContentType.JSON_CONTENT_HEADERS)
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            context.HttpContext.Response.ContentType = "application/json";

            var injectedRequestStream = new MemoryStream();
            FhirResponse response = null;
            return Task.Factory.StartNew(() =>
            {
                using (var streamwriter = new StreamWriter(context.HttpContext.Response.Body))
                {
                    using (JsonWriter writer = new JsonTextWriter(streamwriter))
                    {
                        //todo do we need this?  Couldn't seem to find the 'new' version of this
//                    SummaryType summary = requestMessage.RequestSummary();
                        Type type = context.ObjectType;
                        object value = context.Object;

                        if (type == typeof(OperationOutcome))
                        {
                            var resource = (Resource) value;
                            FhirSerializer.SerializeResource(resource, writer);
                            context.HttpContext.Items[ContextOutputLookupKey] = resource;
                            
                        }
                        else if (typeof(Resource).IsAssignableFrom(type))
                        {
                            var resource = (Resource) value;
                            FhirSerializer.SerializeResource(resource, writer);
                            context.HttpContext.Items[ContextOutputLookupKey] = resource;
                        }
                        else if (typeof(FhirResponse).IsAssignableFrom(type))
                        {
                            response = value as FhirResponse;
                            if (response != null && response.HasBody)
                            {
                                FhirSerializer.SerializeResource(response.Resource, writer);
                            }
                            //Set response content for loggingmiddleware processing
                            context.HttpContext.Items[ContextOutputLookupKey] = response;
                        }
                        //Set type for loggingmiddleware processing
                        context.HttpContext.Items[ContextOutputLookupKeyType] = type;
                    }
                }
            });
        }

        
    }
}