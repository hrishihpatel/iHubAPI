using iHubAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Collections.Generic;

namespace iHubAPI.Filters
{
    public class StandardException : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            string tracerID = context.HttpContext.TraceIdentifier;
            //Store the information somewhere.
            var details = new List<string>
            {
                $"TraceID: {tracerID}",
                $"StackTrace: {context?.Exception?.StackTrace }",
                $"Message: {context?.Exception?.Message }",
                $"InnerException: {context?.Exception?.InnerException?.ToString() }"
            };

            Log.Error(details.ToString());
            var status = new Status
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                StatusDetails = new List<string> { $"TraceIdentifier: {tracerID}", "An Unexpected error has occured, please reffer to above traceID." },
                StatusMessage = "Exception"
            };

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new JsonResult(status);
            
            base.OnException(context);
        }
    }
}
