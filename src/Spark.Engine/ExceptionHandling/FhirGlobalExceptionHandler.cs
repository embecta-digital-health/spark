using System;
using System.Web.Http.ExceptionHandling;

namespace Spark.Engine.ExceptionHandling
{
    [Obsolete("handlers no longer used in .net core")]
    public class FhirGlobalExceptionHandler : ExceptionHandler
    {
        private readonly IExceptionResponseMessageFactory exceptionResponseMessageFactory;

        public FhirGlobalExceptionHandler(IExceptionResponseMessageFactory exceptionResponseMessageFactory)
        {
            this.exceptionResponseMessageFactory = exceptionResponseMessageFactory;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            throw new NotImplementedException();
//            HttpResponse responseMessage = exceptionResponseMessageFactory.GetResponseOutcome(context.Exception,
//                context);
//            context.Result = new ResponseMessageResult(responseMessage);
        }

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
    }
}