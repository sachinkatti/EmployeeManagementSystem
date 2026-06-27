using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmployeeDbContext _context;

        public HomeController(EmployeeDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalEmployees = _context.Employees.Count();

            ViewBag.TotalDepartments = _context.Employees
                                              .Select(e => e.Department)
                                              .Distinct()
                                              .Count();

            ViewBag.HighestSalary = _context.Employees.Any()
                ? _context.Employees.Max(e => e.Salary)
                : 0;

            return View();
        }
    }
}