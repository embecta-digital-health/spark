﻿using System;
using System.Net;
using System.Web.Http;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Spark.Engine.Core;
using Spark.Engine.Extensions;

namespace Spark.Engine.ExceptionHandling
{
    public class ExceptionResponseMessageFactory : IExceptionResponseMessageFactory
    {
        private SparkException ex;

        public IActionResult GetResponseOutcome(Exception exception)
        {
            if (exception == null)
                return new BadRequestResult();

            return InternalCreateResponse(exception as SparkException) ??
                   InternalCreateResponse(exception as HttpResponseException) ??
                   InternalCreateResponse(exception);
        }

        private IActionResult InternalCreateResponse(SparkException exception)
        {
            if (exception == null)
                return new BadRequestResult();
            OperationOutcome outcome = exception.Outcome ?? new OperationOutcome();
            outcome.AddAllInnerErrors(exception);

            //            return exceptionContext.Result.CreateResponse(exception.StatusCode, outcome);
            return new ObjectResult(outcome) {StatusCode = exception.StatusCode.GetHashCode()};
        }

        private IActionResult InternalCreateResponse(HttpResponseException exception)
        {
            if (exception == null)
                return new BadRequestResult();

            OperationOutcome outcome = new OperationOutcome().AddError(exception.Response.ReasonPhrase);
//            return exceptionContext.Request.CreateResponse(exception.Response.StatusCode, outcome);
            return new ObjectResult(outcome) {StatusCode = exception.Response.StatusCode.GetHashCode()};
        }

        private IActionResult InternalCreateResponse(Exception exception)
        {
            if (exception == null)
                return new BadRequestResult();

            OperationOutcome outcome = new OperationOutcome().AddAllInnerErrors(exception);
            //            return exceptionContext.Request.CreateResponse(HttpStatusCode.InternalServerError, outcome);
            return new ObjectResult(outcome) {StatusCode = HttpStatusCode.InternalServerError.GetHashCode()};
//            return new BadRequestObjectResult(outcome);
        }
    }
}