using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Department { get; set; }

        [Range(1, 9999999)]
        public decimal Salary { get; set; }

        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; }
    }
}