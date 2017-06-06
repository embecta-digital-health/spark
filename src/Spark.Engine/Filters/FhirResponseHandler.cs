using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Spark.Engine.Core;

namespace Spark.Engine.Filters
{

    public class FhirResponseHandler : DelegatingHandler
    {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    FhirResponse fhirResponse;
                    if (task.IsCompleted)
                    {
                        if (task.Result.TryGetContentValue(out fhirResponse))//what is this line doing?
                        {
                            return request.CreateResponse(fhirResponse);
                        }
                        else
                        {
                            return task.Result;
                        }
                    } 
                    else
                    {
                        return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                        //return task.Result;
                    }
                    
                }, 
                cancellationToken
            );
             
        }

    }

    
}
