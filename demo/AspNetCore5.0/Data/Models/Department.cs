using System.Collections.Generic;
using TanvirArjel.EFCore.GenericRepository;

namespace AspNetCore5._0.Data.Models
{
    public class Department :BaseEntity<int>
    {
        public string Name { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
