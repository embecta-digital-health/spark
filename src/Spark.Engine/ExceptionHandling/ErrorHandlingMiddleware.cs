using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mindscape.Raygun4Net;

namespace Spark.Engine.ExceptionHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly IExceptionResponseMessageFactory _exceptionResponseMessageFactory;
        private readonly RequestDelegate next;
        private readonly IRaygunAspNetCoreClientProvider _clientProvider;
        private readonly IOptions<RaygunSettings> _settings;

        public ErrorHandlingMiddleware(RequestDelegate next, IExceptionResponseMessageFactory exceptionResponseMessageFactory, IRaygunAspNetCoreClientProvider clientProvider, IOptions<RaygunSettings> raygunSettings)
        {
            this.next = next;
            _exceptionResponseMessageFactory = exceptionResponseMessageFactory;
            _clientProvider = clientProvider;
            _settings = raygunSettings;
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            IActionResult actionResult = _exceptionResponseMessageFactory.GetResponseOutcome(exception);
            var actionContext = new ActionContext(context, null, null);
            
            RaygunClient raygunClient = _clientProvider.GetClient(_settings.Value, context);
            raygunClient.SendInBackground(exception);

            return actionResult.ExecuteResultAsync(actionContext);

            //            string result = JsonConvert.SerializeObject(new { error = exception.Message });
            //            context.Response.ContentType = "application/json";
            //            context.Response.StatusCode = (int)code;
            //            return context.Response.WriteAsync(result);
        }

        public async Task Invoke(HttpContext context, ILogger<ErrorHandlingMiddleware> logger)
        {
            Debug.WriteLine("---- Start exception catcher ----");
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError("From " + GetType().Name + ": " + ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
    }
}