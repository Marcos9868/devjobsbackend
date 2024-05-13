using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevJobsBackend.Entities
{
    public class Common : User
    {
        public int JobsCreated { get; set; } = 0;
        public int JobsEnded { get; set; } = 0;
    }
}