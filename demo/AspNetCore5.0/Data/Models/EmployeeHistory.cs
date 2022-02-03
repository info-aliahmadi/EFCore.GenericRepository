using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TanvirArjel.EFCore.GenericRepository;

namespace AspNetCore5._0.Data.Models
{
    public class EmployeeHistory : BaseEntity<long>
    {

        public long EmployeeId { get; set; }

        public int DepartmentId { get; set; }

        [Required]
        public string EmployeeName { get; set; }
    }
}
