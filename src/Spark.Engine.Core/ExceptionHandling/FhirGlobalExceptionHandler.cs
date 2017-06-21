using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Http;
using Spark.Engine.Core;

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

        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }

        public override void Handle(ExceptionHandlerContext context)
        {
            throw new NotImplementedException();
//            HttpResponse responseMessage = exceptionResponseMessageFactory.GetResponseOutcome(context.Exception,
//                context);
//            context.Result = new ResponseMessageResult(responseMessage);
        }
    }
}