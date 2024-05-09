using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevJobsBackend.Entities;

namespace DevJobsBackend.Contracts.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
    }
}