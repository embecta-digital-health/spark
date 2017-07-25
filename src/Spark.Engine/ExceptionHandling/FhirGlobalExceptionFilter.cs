/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */


using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mindscape.Raygun4Net;
using ExceptionFilterAttribute = Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute;

namespace Spark.Engine.ExceptionHandling
{
    public class FhirGlobalExceptionFilter : ExceptionFilterAttribute
    {
        private static IExceptionResponseMessageFactory _exceptionResponseMessageFactory;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly ILogger<FhirGlobalExceptionFilter> _logger;
        private readonly IRaygunAspNetCoreClientProvider _clientProvider;
        private readonly IOptions<RaygunSettings> _settings;

        public FhirGlobalExceptionFilter(ILogger<FhirGlobalExceptionFilter> logger, IRaygunAspNetCoreClientProvider clientProvider, IOptions<RaygunSettings> raygunSettings)//, IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider)
        {
            _logger = logger;
            _exceptionResponseMessageFactory = new ExceptionResponseMessageFactory();
            _clientProvider = clientProvider;
            _settings = raygunSettings;
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
            RaygunClient raygunClient = _clientProvider.GetClient(_settings.Value, exceptionContext.HttpContext);
            raygunClient.SendInBackground(exceptionContext.Exception);
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