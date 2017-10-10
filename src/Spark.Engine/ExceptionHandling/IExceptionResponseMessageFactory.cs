using System;
using Microsoft.AspNetCore.Mvc;

namespace Spark.Engine.ExceptionHandling
{
    public interface IExceptionResponseMessageFactory
    {
        IActionResult GetResponseOutcome(Exception exception);
    }
}