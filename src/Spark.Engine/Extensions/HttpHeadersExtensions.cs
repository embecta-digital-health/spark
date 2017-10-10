/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Spark.Engine.Extensions
{
    public static class HttpRequestExtensions
    {
        public static void ClearNamedHeaders(HttpRequest request, string headerName)
        {
            IHeaderDictionary newHeaders = new HeaderDictionary();
            foreach (KeyValuePair<string, StringValues> keyValuePair in request.Headers)
            {
                if (keyValuePair.Key != headerName)
                {
                    newHeaders.Add(keyValuePair);
                }
            }
            request.Headers.Clear();
            foreach (KeyValuePair<string, StringValues> keyValuePair in newHeaders)
            {
                request.Headers.Add(keyValuePair);
            }
        }

        public static bool Exists(this HttpHeaders headers, string key)
        {
            IEnumerable<string> values;
            if (headers.TryGetValues(key, out values))
            {
                return values.Count() > 0;
            }
            else return false;

        }
        
        public static void Replace(this HttpHeaders headers, string header, string value)
        {
            //if (headers.Exists(header)) 
            headers.Remove(header);
            headers.Add(header, value);
        }
        
        public static string Value(this HttpHeaders headers, string key)
        {
            IEnumerable<string> values;
            if (headers.TryGetValues(key, out values))
            {
                return values.FirstOrDefault();
            }
            else return null;
        }
        
        public static void ReplaceHeader(this HttpRequest request, string header, string value)
        {
            throw new NotImplementedException();
//            request.Headers.Replace(header, value);
        }

        public static string Header(this HttpRequest request, string key)
        {
            if (request.Headers.TryGetValue(key, out StringValues values))
            {
                return values.FirstOrDefault();
            }
            return null;
        }
        
        public static string GetParameter(this HttpRequest request, string key)
        {
//            foreach (var param in request.GetQueryNameValuePairs())
//            {
//                if (param.Key == key) return param.Value;
//            }
//            return null;
            if (!request.Query.Keys.Contains(key))
            {
                return null;
            }
            if (request.Query.TryGetValue(key, out StringValues values))
            {
                return values.FirstOrDefault();
            }
            return null;

        }

        public static List<Tuple<string, string>> TupledParameters(this HttpRequest request)
        {
            var list = new List<Tuple<string, string>>();

            foreach (var pair in request.Query)
            {
                list.Add(new Tuple<string, string>(pair.Key, pair.Value));
            }
            return list;
        }

        public static SearchParams GetSearchParams(this HttpRequest request)
        {
            var parameters = request.TupledParameters().Where(tp => tp.Item1 != "_format");
            UriParamList actualParameters = new UriParamList(parameters);
            var searchCommand = SearchParams.FromUriParamList(parameters);
            return searchCommand;
        }
    }

    public static class HttpHeadersFhirExtensions
    {
        public static bool IsSummary(this HttpHeaders headers)
        {
            string summary = headers.Value("_summary");
            return (summary != null) ? summary.ToLower() == "true" : false;
        }
    }
}