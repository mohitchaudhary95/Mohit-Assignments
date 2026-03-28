using EmployeeApp.Data;
using EmployeeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;

namespace EmployeeApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        // ─── 1. GET ALL with Search, Filter, Pagination ───────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? search = null,
            string? department = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 5)
        {
            var query = _context.Employees.AsQueryable();

            // Search by name or email
            if (!string.IsNullOrEmpty(search))
                query = query.Where(e =>
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search));

            // Filter by department
            if (!string.IsNullOrEmpty(department))
                query = query.Where(e => e.Department == department);

            // Filter by active status
            if (isActive.HasValue)
                query = query.Where(e => e.IsActive == isActive.Value);

            // Total count before pagination (for frontend to know total pages)
            var totalCount = await query.CountAsync();

            // Pagination
            var employees = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = employees
            });
        }

        // ─── 2. GET BY ID ──────────────────────────────────────────────────
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound(new { Message = $"Employee with ID {id} not found." });

            return Ok(employee);
        }

        // ─── 3. GET STATS (Dashboard) ──────────────────────────────────────
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var total = await _context.Employees.CountAsync();
            var active = await _context.Employees.CountAsync(e => e.IsActive);
            var inactive = await _context.Employees.CountAsync(e => !e.IsActive);

            var byDepartment = await _context.Employees
                .GroupBy(e => e.Department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                Total = total,
                Active = active,
                Inactive = inactive,
                ByDepartment = byDepartment
            });
        }

        // ─── 4. CREATE ─────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check duplicate email
            bool emailExists = await _context.Employees
                .AnyAsync(e => e.Email == employee.Email);

            if (emailExists)
                return BadRequest(new { Message = "An employee with this email already exists." });

            employee.CreatedAt = DateTime.Now;
            employee.IsActive = true;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = employee.EmployeeId }, employee);
        }

        // ─── 5. UPDATE ─────────────────────────────────────────────────────
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee updated)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound(new { Message = $"Employee with ID {id} not found." });

            // Check duplicate email (exclude current employee)
            bool emailExists = await _context.Employees
                .AnyAsync(e => e.Email == updated.Email && e.EmployeeId != id);

            if (emailExists)
                return BadRequest(new { Message = "Another employee with this email already exists." });

            // Update fields
            employee.FirstName = updated.FirstName;
            employee.LastName = updated.LastName;
            employee.Email = updated.Email;
            employee.Phone = updated.Phone;
            employee.Department = updated.Department;
            employee.Designation = updated.Designation;
            employee.Salary = updated.Salary;
            employee.DateOfJoining = updated.DateOfJoining;

            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        // ─── 6. TOGGLE ACTIVE / INACTIVE ──────────────────────────────────
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound(new { Message = $"Employee with ID {id} not found." });

            employee.IsActive = !employee.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Employee is now {(employee.IsActive ? "Active" : "Inactive")}.",
                IsActive = employee.IsActive
            });
        }

        // ─── 7. DELETE ─────────────────────────────────────────────────────
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound(new { Message = $"Employee with ID {id} not found." });

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Employee '{employee.FirstName} {employee.LastName}' deleted successfully." });
        }
    }
}

//---

//## What Each Endpoint Does

//| # | Method | Route | Purpose |
//| ---| ---| ---| ---|
//| 1 | GET | `/ api / employees` | Get all + search + filter + pagination |
//| 2 | GET | `/ api / employees /{ id}` | Get single employee |
//| 3 | GET | `/api/employees/stats` | Dashboard stats |
//| 4 | POST | `/api/employees` | Create new employee |
//| 5 | PUT | `/ api / employees /{ id}` | Update employee |
//| 6 | PATCH | `/ api / employees /{ id}/ toggle - status` | Toggle Active / Inactive |
//| 7 | DELETE | `/ api / employees /{ id}` | Delete employee |

//---

//## Test It in Swagger

//Run the project and go to:
//```
//https://localhost:{PORT}/swagger