using System.ComponentModel.DataAnnotations;
using TanvirArjel.EFCore.GenericRepository;

namespace AspNetCore5._0.Data.Models
{
    public class Employee : BaseEntity<long>
    {
        public int DepartmentId { get; set; }

        [Required]
        public string EmployeeName { get; set; }

        [Required]
        public string DepartmentName { get; set; }

        public Department Department { get; set; }
    }
}
