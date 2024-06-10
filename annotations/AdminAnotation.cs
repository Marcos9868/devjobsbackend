using DevJobsBackend.Entities;
using DevJobsBackend.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

public class AdminAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Obtém o CurrentUser do contexto
        if (context.HttpContext.Items.TryGetValue("CurrentUser", out var currentUserObj) && currentUserObj is User currentUser)
        {
            if ( currentUser.TypeUser == UserType.ADMIN)
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
        else
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
