using System;
using System.Web.Http;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Spark.Engine.ExceptionHandling
{
    public interface IExceptionResponseMessageFactory
    {
        IActionResult GetResponseOutcome(Exception exception);
    }
}