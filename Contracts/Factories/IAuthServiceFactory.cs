using DevJobsBackend.Contracts.Services;

namespace DevJobsBackend.Contracts.Factories
{
    public interface IAuthServiceFactory
    {
        IAuthService Create();
    }
}
