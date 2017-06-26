using System.Collections.Generic;
using System.Security.Claims;
using Spark.Engine.Core;

namespace Spark.Engine.Interfaces
{
    public interface IFhirResponseFactory
    {
        FhirResponse GetFhirResponse(Key key, ClaimsPrincipal principal, IEnumerable<object> parameters =  null);
        FhirResponse GetFhirResponse(Entry entry, IEnumerable<object> parameters = null);
        FhirResponse GetFhirResponse(Key key, ClaimsPrincipal principal, params object[] parameters);
        FhirResponse GetFhirResponse(Entry entry, params object[] parameters);
    }
}