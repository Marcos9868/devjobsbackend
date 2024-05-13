

namespace DevJobsBackend.Services{

using BCrypt.Net;
    using DevJobsBackend.Entities;
    using DevJobsBackend.Data;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    public AuthService(DataContext context)
    {
        _context = context;
    }

    public Task<string> generateHashPassword(string password)
    {
        string hashedPassword = BCrypt.HashPassword(password);

        System.Diagnostics.Debug.WriteLine(hashedPassword);
        return Task.FromResult(hashedPassword);
    }
}}
