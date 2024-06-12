using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Enums;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AdminAttribute : Attribute, IAsyncActionFilter
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

            if (user != null && user.TypeUser == UserType.ADMIN)
            {
                await next();
            }
            else
            {
                SetForbiddenResult(context, "Access denied. You do not have the required permissions.");
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

    private void SetForbiddenResult(ActionExecutingContext context, string message)
    {
        var response = new ResponseBase<object>
        {
            Data = null,
            Message = message,
            Status = false
        };
        context.Result = new JsonResult(response) { StatusCode = 403 }; // Forbidden
    }
}
