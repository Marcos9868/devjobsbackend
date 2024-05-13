using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevJobsBackend.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevJobsBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {}

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}