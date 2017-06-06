using System;
using Microsoft.AspNetCore.Http;

namespace Spark.Engine.Core
{
    public class HistoryParameters
    {
        public HistoryParameters()
        {
            
        }
        public HistoryParameters(HttpRequest request)
        {
            throw new NotImplementedException();
            //todo figure out how to get the parameters below from the request in .NET core
//            Count = request.GetIntParameter(FhirParameter.COUNT);
//            Since = request.GetDateParameter(FhirParameter.SINCE);
//            SortBy = request.GetParameter(FhirParameter.SORT);
        }

        public int? Count { get; set; }
        public DateTimeOffset? Since { get; set; }
        public string Format { get; set; }
        public string SortBy { get; set; }
    }
}