using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;
using DevJobsBackend.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        var user = httpContext.Items["CurrentUser"] as User;

        if (user == null)
        {
            bindingContext.ModelState.AddModelError("Authorization", "Invalid token or user not authenticated");
            bindingContext.Result = ModelBindingResult.Failed();
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Success(user);
        }
    }
}
