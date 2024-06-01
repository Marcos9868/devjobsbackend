using DevJobsBackend.Contracts.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

public class UserModelBinder : IModelBinder
{
    private readonly IAuthService _authService;
    private readonly ILogger<UserModelBinder> _logger;

    public UserModelBinder(IAuthService authService, ILogger<UserModelBinder> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

        _logger.LogInformation("Authorization Header: {AuthorizationHeader}", authorizationHeader);

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            _logger.LogWarning("Authorization header is null or empty.");
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        if (!authorizationHeader.StartsWith("Bearer "))
        {
            _logger.LogWarning("Authorization header does not start with 'Bearer '.");
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        _logger.LogInformation("Token: {Token}", token);

        try
        {
            var user = await _authService.GetUserByAccessToken(token);

            if (user != null)
            {
                bindingContext.Result = ModelBindingResult.Success(user);
            }
            else
            {
                _logger.LogWarning("User not found for token: {Token}", token);
                bindingContext.Result = ModelBindingResult.Failed();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while binding the user model.");
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
