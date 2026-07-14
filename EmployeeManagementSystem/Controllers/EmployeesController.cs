using EmployeeManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeDbContext _context;

        public EmployeesController(EmployeeDbContext context)
        {
            _context = context;
        }

        private IActionResult CheckLogin()
        {
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return null;
        }
        private bool HasRole(params string[] roles)
        {
            var role = HttpContext.Session.GetString("Role");
            return role != null && roles.Contains(role);
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString, string sortOrder, int? page)
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SalarySort"] = sortOrder == "salary" ? "salary_desc" : "salary";

            var employees = from e in _context.Employees
                            select e;

            // Search
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e =>
                    e.Name.Contains(searchString) ||
                    e.Email.Contains(searchString) ||
                    e.Department.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(e => e.Name);
                    break;

                case "salary":
                    employees = employees.OrderBy(e => e.Salary);
                    break;

                case "salary_desc":
                    employees = employees.OrderByDescending(e => e.Salary);
                    break;

                default:
                    employees = employees.OrderBy(e => e.Name);
                    break;
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        // GET: Employees/Create
        public IActionResult Create()
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            if (!HasRole("Admin", "HR"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            ViewBag.Departments = new List<string>
    {
        "Developer",
        "Tester",
        "HR",
        "Manager",
        "Support"
    };

            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Department,Salary,JoiningDate")] Employee employee)
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            if (!HasRole("Admin", "HR"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new List<string>
    {
        "Developer",
        "Tester",
        "HR",
        "Manager",
        "Support"
    };

            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            if (!HasRole("Admin", "HR"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Departments = new List<string>
    {
        "Developer",
        "Tester",
        "HR",
        "Manager",
        "Support"
    };

            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Department,Salary,JoiningDate")] Employee employee)
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            if (!HasRole("Admin", "HR"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = new List<string>
{
    "Developer",
    "Tester",
    "HR",
    "Manager",
    "Support"
};
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            if (!HasRole("Admin"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = CheckLogin();
            if (result != null)
                return result;

            if (!HasRole("Admin"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
