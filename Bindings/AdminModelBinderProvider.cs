using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;

public class AdminModelBinderProvider : IModelBinderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public AdminModelBinderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(User))
        {
            return new BinderTypeModelBinder(typeof(AdminModelBinder));
        }

        return null;
    }
}
