#region

using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mindscape.Raygun4Net;

#endregion

namespace Spark.Engine.ExceptionHandling
{
    public class FhirGlobalExceptionFilter : ExceptionFilterAttribute
    {
        private static IExceptionResponseMessageFactory _exceptionResponseMessageFactory;
        private readonly IRaygunAspNetCoreClientProvider _clientProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<FhirGlobalExceptionFilter> _logger;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IOptions<RaygunSettings> _settings;

        public FhirGlobalExceptionFilter(ILogger<FhirGlobalExceptionFilter> logger, IRaygunAspNetCoreClientProvider clientProvider, IOptions<RaygunSettings> raygunSettings) //, IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider)
        {
            _logger = logger;
            _exceptionResponseMessageFactory = new ExceptionResponseMessageFactory();
            _clientProvider = clientProvider;
            _settings = raygunSettings;
//            _hostingEnvironment = hostingEnvironment;
//            _modelMetadataProvider = modelMetadataProvider;
        }

        /// <inheritdoc />
        /// <summary>
        ///     https://blog.kloud.com.au/2016/03/23/aspnet-core-tips-and-tricks-global-exception-handling/
        /// </summary>
        /// <param name="exceptionContext"></param>
        public override void OnException(ExceptionContext exceptionContext)
        {
            IActionResult actionResult = _exceptionResponseMessageFactory.GetResponseOutcome(exceptionContext.Exception);
            //exceptionContext.HttpContext.Request.EnableRewind();
            RaygunClient raygunClient = _clientProvider.GetClient(_settings.Value, exceptionContext.HttpContext);
            int? statusCode = StatusCode(actionResult);
            string user = exceptionContext.HttpContext.User.Identity.Name;
            raygunClient.SendInBackground(exceptionContext.Exception);
            _logger.LogError("{{ 'who': '{who}', 'when': '{when}', 'what': '{what}', 'detail': ' statusCode {statusCode}: class: {class}  message: {message} ---- stacktrace: {stacktrace} '}}",
                user, DateTime.Now, "exception", statusCode, GetType().Name, exceptionContext.Exception.Message, exceptionContext.Exception.Demystify());
            exceptionContext.Result = actionResult;
        }

        private static int? StatusCode(IActionResult actionResult)
        {
            int? statusCode = null;
            try
            {
                statusCode = ((ObjectResult) actionResult).StatusCode;
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine(e);
            }

            return statusCode;
        }
    }
}