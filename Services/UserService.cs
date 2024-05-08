using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevJobsBackend.Contracts.Services;
using DevJobsBackend.Entities;

namespace DevJobsBackend.Services
{
    public class UserService : IUserService
    {
        public Task<List<User>> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}