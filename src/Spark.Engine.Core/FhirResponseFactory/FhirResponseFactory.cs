using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Spark.Core;
using Spark.Engine.Core;
using Spark.Engine.Extensions;
using Spark.Engine.Interfaces;
using Spark.Service;

namespace Spark.Engine.FhirResponseFactory
{
    public class FhirResponseFactory : IFhirResponseFactory
    {
        protected IFhirStore fhirStore;
        protected Transfer transfer;
        private readonly IFhirResponseInterceptorRunner interceptorRunner;

        public FhirResponseFactory(IFhirStore fhirStore, Transfer transfer, IFhirResponseInterceptorRunner interceptorRunner)
        {
            this.fhirStore = fhirStore;
            this.transfer = transfer;
            this.interceptorRunner = interceptorRunner;
        }

        public FhirResponse GetFhirResponse(Key key, ClaimsPrincipal principal, IEnumerable<object> parameters = null)
        {
            Entry entry = fhirStore.Get(key, principal);

            if (entry == null)
                return Respond.NotFound(key);
            return GetFhirResponse(entry, parameters);
        }

        public FhirResponse GetFhirResponse(Entry entry, IEnumerable<object> parameters = null)
        {
            if (entry.IsDeleted())
            {
                return Respond.Gone(entry);
            }

            FhirResponse response = null;

            if (parameters != null)
            {
                response = interceptorRunner.RunInterceptors(entry, parameters);
            }

            return response ?? Respond.WithResource(entry);
        }

        public FhirResponse GetFhirResponse(Key key, ClaimsPrincipal principal, params object[] parameters)
        {
            return GetFhirResponse(key, principal, parameters.ToList());
        }

        public FhirResponse GetFhirResponse(Entry entry, params object[] parameters)
        {
            return GetFhirResponse(entry, parameters.ToList());
        }
    }
}