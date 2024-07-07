namespace DevJobsBackend.Contracts.Factories;

public interface ITokenFactory
    {
        string CreateAccessToken(string email);
        string CreateRefreshToken(string email);
        string CreateForgotPasswordToken(string email);
        string CreateDeleteAccountToken(string email);
    }
