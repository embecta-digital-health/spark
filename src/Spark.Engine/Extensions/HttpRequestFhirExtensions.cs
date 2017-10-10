/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using Spark.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Spark.Engine.Core;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Spark.Engine.Extensions
{
    public static class HttpRequestFhirExtensions
    {
        private const string IfMatch = "If-Match";
        // Deprecated: only used for Mailbox
        //public static void SaveBody(this HttpRequest request, string contentType, byte[] data)
        //{
        //    Binary b = new Binary { Content = data, ContentType = contentType };

        //    request.Properties.Add(Const.UNPARSED_BODY, b);
        //}

        // Deprecated: only used for Mailbox
        //public static Binary GetBody(this HttpRequest request)
        //{
        //    if (request.Properties.ContainsKey(Const.UNPARSED_BODY))
        //        return request.Properties[Const.UNPARSED_BODY] as Binary;
        //    else
        //        return null;
        //}

        /// <summary>
        /// Temporary hack!
        /// Adds a resourceEntry to the request property bag. To be picked up by the MediaTypeFormatters for adding http headers.
        /// </summary>
        /// <param name="entry">The resource entry with information to generate headers</param>
        /// <remarks> 
        /// The SendAsync is called after the headers are set. The SetDefaultHeaders have no access to the content object.
        /// The only solution is to give the information through the Request Property Bag.
        /// </remarks>
        public static void SaveEntry(this HttpRequest request, Entry entry)
        {
//            request.Properties.Add(Const.RESOURCE_ENTRY, entry);//pre .net core
            request.HttpContext.Items.Add(Const.RESOURCE_ENTRY, entry);
        }

        public static Entry GetEntry(this HttpRequest request)
        {
            //pre .net core
            //            if (request.Properties.ContainsKey(Const.RESOURCE_ENTRY))
            //                return request.Properties[Const.RESOURCE_ENTRY] as Entry;
            //            else
            //                return null;

            if (request.HttpContext.Items.ContainsKey(Const.RESOURCE_ENTRY))
                return request.HttpContext.Items[Const.RESOURCE_ENTRY] as Entry;
            else
                return null;
        }

        //public static HttpResponseMessage HttpResponse(this HttpRequest request, HttpStatusCode code, Entry entry)
        //{
        //    request.SaveEntry(entry);

        //    HttpResponseMessage msg;
        //    msg = request.CreateResponse<Resource>(code, entry.Resource);
            
        //    // DSTU2: tags
        //    //msg.Headers.SetFhirTags(entry.Tags);
        //    return msg;
        //}

       

        public static void AcquireHeaders(this HttpResponse response, FhirResponse fhirResponse)
        {
            // http.StatusCode = fhir.StatusCode;
            if (fhirResponse.Key != null)
            {
                // perhaps another way if this doesn't work: https://stackoverflow.com/questions/35458737/implement-http-cache-etag-in-asp-net-core-web-api
                response.Headers.Add("ETag",ETag.Create(fhirResponse.Key.VersionId).ToString());

//                Uri location = fhirResponse.Key.ToUri();

//                if (response.HttpContext != null)
//                {
//                    response.Headers.ContentLocation = location;
//
//                    if (fhirResponse.Resource != null && fhirResponse.Resource.Meta != null)
//                    {
//                        response.Headers = fhirResponse.Resource.Meta.LastUpdated;
//                    }
//                }
//                else
//                {
//                    response.Headers.Location = location;
//                }
            }
        }
       
        private static HttpResponse CreateBareFhirResponse(this HttpRequest request, FhirResponse fhir)
        {
            bool includebody = request.PreferRepresentation();

            if (fhir.Resource != null)
            {
                if (includebody)
                {
                    throw new NotImplementedException();
//                    Binary binary = fhir.Resource as Binary;
//                    if (binary != null)
//                    {
//                        return request.CreateResponse(fhir.StatusCode, binary);
//                    }
//                    else
//                    {
//                        return request.CreateResponse(fhir.StatusCode, fhir.Resource);
//                    }
                }
//                else
//                {
//                    return request.CreateResponse(fhir.StatusCode);
//                }
            }
            else
            {
                return request.CreateResponse(fhir.StatusCode);
            }
            throw new NotImplementedException();
        }

        private static HttpResponse CreateResponse(this HttpRequest request, HttpStatusCode fhirStatusCode)
        {
            throw new NotImplementedException();
        }

        public static HttpResponse CreateResponse(this HttpRequest request, FhirResponse fhir)
        {
            HttpResponse message = request.CreateBareFhirResponse(fhir);
            message.AcquireHeaders(fhir);
            return message;
        }

        public static HttpResponse CreateResponse(this HttpRequest request, HttpStatusCode exceptionStatusCode, OperationOutcome outcome)
        {
            throw new NotImplementedException();
        }

        /*
        public static HttpResponseMessage HttpResponse(this HttpRequest request, Entry entry)
        {
            return request.HttpResponse(HttpStatusCode.OK, entry);
        }
        */

        //public static HttpResponseMessage Error(this HttpRequest request, int code, OperationOutcome outcome)
        //{
        //    return request.CreateResponse((HttpStatusCode)code, outcome);
        //}

        //public static HttpResponseMessage StatusResponse(this HttpRequest request, Entry entry, HttpStatusCode code)
        //{
        //    request.SaveEntry(entry);
        //    HttpResponseMessage msg = request.CreateResponse(code);
        //    // DSTU2: tags
        //    // msg.Headers.SetFhirTags(entry.Tags); // todo: move to model binder
        //    msg.Headers.Location = entry.Key.ToUri(Localhost.Base);
        //    return msg;
        //}

        /*
        public static ICollection<Tag> GetFhirTags(this HttpRequest request)
        {
            return request.Headers.GetFhirTags();
        }
        */

        public static DateTimeOffset? GetDateParameter(this HttpRequest request, string name)
        {
            string param = request.GetParameter(name);
            if (param == null) return null;
            return DateTimeOffset.Parse(param);
        }

        public static int? GetIntParameter(this HttpRequest request, string name)
        {
            string s = request.GetParameter(name);
            int n;
            return (int.TryParse(s, out n)) ? n : (int?)null;
        }

        public static bool? GetBooleanParameter(this HttpRequest request, string name)
        {
            string s = request.GetParameter(name);           
            if(s == null) return null;

            try
            {
                bool b = PrimitiveTypeConverter.ConvertTo<bool>(s);
                return (bool.TryParse(s, out b)) ? b : (bool?)null;
            }
            catch
            {
                return null;
            }
        }

        public static DateTimeOffset? IfModifiedSince(this HttpRequest request)
        {
            if (!request.Headers.TryGetValue("If-Modified-Since", out StringValues valueSet))
            {
                return null;
            }
            if(DateTimeOffset.TryParse(valueSet.FirstOrDefault(), out var dateTimeOffset))
            {
                return dateTimeOffset;
            }
            return null;
            //string s = request.Header("If-Modified-Since");
            //DateTimeOffset date;
            //if (DateTimeOffset.TryParse(s, out date))
            //{
            //    return date;
            //}
            //{ 
            //    return null;
            //}
        }

        public static IEnumerable<string> IfNoneMatch(this HttpRequest request)
        {
            throw new NotImplementedException();
            // The if-none-match can be either '*' or tags. This needs further implementation.
//            return request.Headers.IfNoneMatch.Select(h => h.Tag);
        }

        private static string WithoutQuotes(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            else
            {
                return s.Trim('"');
            }
        }

        public static string GetValue(this HttpRequest request, string key)
        {
            if (request.Headers.Any())
            {
                if (request.Headers.TryGetValue(key, out StringValues values))
                {
                    string value = values.FirstOrDefault();
                    return value;
                }
                return null;
            }
            return null;
        }

        public static bool PreferRepresentation(this HttpRequest request)
        {
            string value = request.GetValue("Prefer");
            return (value == "return=representation" || value == null);
        }

        public static string IfMatchVersionId(this HttpRequest request)
        {
            string tag;
            if (request.Headers.Any())
            {
//                var tag = request.Headers.IfMatch.FirstOrDefault();//pre .net Core
                if (request.Headers.TryGetValue(IfMatch, out StringValues value))
                {
                    return WithoutQuotes(value.FirstOrDefault());
                }
                //                if (tag != null)
                //                {
                //                    return WithoutQuotes(tag.Tag);
                //                }

                return null;
                // todo: validate missing quotes
                //else 
                //{
                //    throw Error.Create(HttpStatusCode.BadRequest, "The If-Match (version id) is not properly formatted: '{0}'. You might have forgot the quotes", request.Headers.IfMatch.ToString());
                //}
            }
            return null;
            //return string.IsNullOrEmpty(versionid) ? null : versionid;


            //string versionid = (tag != null) ? tag.Tag : null;
        }

        public static SummaryType RequestSummary(this HttpRequest request)
        {

            return (request.GetParameter("_summary") == "true") ? SummaryType.True : SummaryType.False;
        }


    }
}