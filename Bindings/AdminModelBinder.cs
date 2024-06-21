using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;
using DevJobsBackend.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

public class AdminModelBinder : IModelBinder
{
    private readonly IAuthService _authService;

    public AdminModelBinder(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var user = await _authService.GetUserByAccessToken(token);

            if (user != null && user.TypeUser == UserType.ADMIN)
            {
                bindingContext.Result = ModelBindingResult.Success(user);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
        catch
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
