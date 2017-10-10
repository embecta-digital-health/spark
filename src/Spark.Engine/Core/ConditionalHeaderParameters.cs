using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Spark.Engine.Extensions;

namespace Spark.Engine.Core
{
    public class ConditionalHeaderParameters
    {
        public ConditionalHeaderParameters()
        {
            
        }
        public ConditionalHeaderParameters(HttpRequest request)
        {
            throw new NotImplementedException();
//            IfNoneMatchTags = request.IfNoneMatch();
//            IfModifiedSince = request.IfModifiedSince();
        }

        public IEnumerable<string> IfNoneMatchTags { get; set; }
        public DateTimeOffset? IfModifiedSince { get; set; }
    }
}