using AutoMapper;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.IoC.ProfileMapping;
using DevJobsBackend.Services;

namespace DevJobsBackend.IoC.Services
{
    public static class ServicesMapping
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            // AutoMapper
            var mapperConfig = new MapperConfiguration(mc => 
            {
                mc.AddProfile(new MappingProfile());
            });
            services.AddSingleton(mapperConfig.CreateMapper());
        }
    }
}