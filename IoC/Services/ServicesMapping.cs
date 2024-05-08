using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Services;

namespace DevJobsBackend.IoC.Services
{
    public static class ServicesMapping
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
        }
    }
}