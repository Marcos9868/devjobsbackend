using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class CurrentUserAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var authService = context.HttpContext.RequestServices.GetService<IAuthService>();
        var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            SetUnauthorizedResult(context, "Authorization header is missing or invalid.");
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var user = await authService.GetUserByAccessToken(token);

            if (user != null)
            {
                context.HttpContext.Items["CurrentUser"] = user;
                await next();
            }
            else
            {
                SetUnauthorizedResult(context, "Invalid token or user not authenticated.");
            }
        }
        catch
        {
            SetUnauthorizedResult(context, "Invalid token or user not authenticated.");
        }
    }

    private void SetUnauthorizedResult(ActionExecutingContext context, string message)
    {
        var response = new ResponseBase<object>
        {
            Data = null,
            Message = message,
            Status = false
        };
        context.Result = new JsonResult(response) { StatusCode = 401 }; // Unauthorized
    }
}
