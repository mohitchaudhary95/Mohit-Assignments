using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmpController : Controller
    {
        List<Employee> emplist = new List<Employee>()
              {

                  new Employee{EmployeeID=101,EmpName="ravi",salary=23000},

                  new Employee{EmployeeID=102,EmpName="sita",salary=43000},

                  new Employee{EmployeeID=103,EmpName="mahesh",salary=53000},

                  new Employee{EmployeeID=104,EmpName="radhika",salary=22000},

              };
        public IActionResult Index()
        {
            return View(emplist);
        }
    }
}
