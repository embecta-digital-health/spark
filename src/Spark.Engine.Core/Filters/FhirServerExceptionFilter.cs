/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */


using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Spark.Engine.ExceptionHandling;
using ExceptionFilterAttribute = Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute;

namespace Spark.Filters
{
    public class FhirServerExceptionFilter : ExceptionFilterAttribute
    {
        private static IExceptionResponseMessageFactory _exceptionResponseMessageFactory;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ILogger<FhirServerExceptionFilter> _logger;

        public FhirServerExceptionFilter(ILogger<FhirServerExceptionFilter> logger)//, IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider)
        {
            _logger = logger;
            _exceptionResponseMessageFactory = new ExceptionResponseMessageFactory();
//            _hostingEnvironment = hostingEnvironment;
//            _modelMetadataProvider = modelMetadataProvider;
        }

        /// <summary>
        /// https://blog.kloud.com.au/2016/03/23/aspnet-core-tips-and-tricks-global-exception-handling/
        /// </summary>
        /// <param name="exceptionContext"></param>
        public override void OnException(ExceptionContext exceptionContext)
        {

            _logger.LogError("From " + this.GetType().Name + ": " + exceptionContext.Exception.Message);
            IActionResult actionResult = _exceptionResponseMessageFactory.GetResponseOutcome(exceptionContext.Exception);
            exceptionContext.Result = (actionResult);

            
            //            if (!_hostingEnvironment.IsDevelopment())
//            {
//                 do nothing
//                return;
//            }
//            var result2 = new ViewResult { ViewName = "CustomError" };
//            result2.ViewData = new ViewDataDictionary(_modelMetadataProvider, exceptionContext.ModelState);
//            result2.ViewData.Add("Exception", exceptionContext.Exception);
//             TODO: Pass additional detailed data via ViewData
//            exceptionContext.Result = result2;
        }
    }
}