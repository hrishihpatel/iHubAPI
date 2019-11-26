using iHubAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iHubAPI.Filters
{
    public class StandardException : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            //Store the information somewhere.
            var details = new List<string>()
            {
                //$"TraceID: {context.Result.}"
                $"StackTrace: {context?.Exception?.StackTrace }",
                $"Message: {context?.Exception?.Message }",
                $"InnerException: {context?.Exception?.InnerException?.ToString() }"
            };

            var status = new Status
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                //This is a place holder, i would not include the exception details in the response.
                StatusDetails = details,
                StatusMessage = "Exception"
            };

            // Log the issue somewhere... :)

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new JsonResult(status);
            
            base.OnException(context);
        }
    }
}
