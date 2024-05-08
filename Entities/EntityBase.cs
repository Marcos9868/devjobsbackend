using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevJobsBackend.Entities
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public int TypeUser { get; set; }
    }
}