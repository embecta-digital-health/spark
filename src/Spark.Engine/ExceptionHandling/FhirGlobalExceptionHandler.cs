using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;

namespace Spark.Engine.ExceptionHandling
{
    public class FhirGlobalExceptionHandler : IExceptionFilter, IDisposable
    {
        private readonly IExceptionResponseMessageFactory _exceptionResponseMessageFactory;

        public FhirGlobalExceptionHandler(IExceptionResponseMessageFactory exceptionResponseMessageFactory)
        {
            _exceptionResponseMessageFactory = exceptionResponseMessageFactory;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool AllowMultiple { get; }

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        //ORIGINAL METHODS:
//        public override bool ShouldHandle(ExceptionHandlerContext context)
//        {
//            return true;
//        }

//        public override void Handle(ExceptionHandlerContext context)
//        {
//            HttpResponseMessage responseMessage = _exceptionResponseMessageFactory.GetResponseMessage(context.Exception,
//                context.Request);
//            context.Result = new ResponseMessageResult(responseMessage);
//        }
//END ORIGINAL METHODS

        /// <summary>
        ///     I am not convinced this is right.  I had to take some liberties with converting this method to .net core 2.0
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            HttpResponseMessage responseMessage = _exceptionResponseMessageFactory.GetResponseMessage(context.Exception,
                context.Request);
            context.Response = new HttpResponseMessage(responseMessage.StatusCode);
        }
    }
}