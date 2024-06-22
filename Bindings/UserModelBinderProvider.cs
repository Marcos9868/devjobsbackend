using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

public class UserModelBinderProvider : IModelBinderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public UserModelBinderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(User))
        {
            return new BinderTypeModelBinder(typeof(UserModelBinder));
        }

        return null;
    }
}
