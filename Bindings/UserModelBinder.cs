using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Threading.Tasks;

public class UserModelBinder : IModelBinder
{
    private readonly IAuthService _authService;

    public UserModelBinder(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            bindingContext.ModelState.AddModelError("Authorization", "Authorization header is missing or invalid");
            bindingContext.Result = ModelBindingResult.Failed();
            SetResponse(bindingContext, null, "Authorization header is missing or invalid", false);
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var user = await _authService.GetUserByAccessToken(token);

            if (user != null)
            {
                bindingContext.Result = ModelBindingResult.Success(user);
                SetResponse(bindingContext, user, "User authenticated successfully", true);
            }
            else
            {
                bindingContext.ModelState.AddModelError("Authorization", "Invalid token");
                bindingContext.Result = ModelBindingResult.Failed();
                SetResponse(bindingContext, null, "Invalid token", false);
            }
        }
        catch
        {
            bindingContext.ModelState.AddModelError("Authorization", "An error occurred while processing the token");
            bindingContext.Result = ModelBindingResult.Failed();
            SetResponse(bindingContext, null, "An error occurred while processing the token", false);
        }
    }

    private void SetResponse(ModelBindingContext bindingContext, object? data, string message, bool status)
    {
        var response = new ResponseBase<object>
        {
            Data = data,
            Message = message,
            Status = status
        };

        bindingContext.HttpContext.Items["Response"] = response;
    }
}