using System.Collections.Generic;
using TanvirArjel.EFCore.GenericRepository;

namespace AspNetCore3._1.Data.Models
{
    public class Department : BaseEntity<int>
    {
        public string Name { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
