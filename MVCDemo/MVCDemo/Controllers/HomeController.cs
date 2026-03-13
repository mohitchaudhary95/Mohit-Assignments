using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCDemo.Models;

namespace MVCDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string sampledemo1()
        {
            return "Mohit";
        }

        public string sampledemo2(int age, string name)
        {
            return "The Name is " + name + " and having age " + age;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult sampledemo3()
        {
            int age = 99;
            string name = "Mohit Chaudhary";
            ViewBag.Name = name;
            ViewBag.Age = age;
            ViewData["Message"] = " Welcome to asp.net core learning ";
            ViewData["Year"] = DateTime.Now.Year;

            return View();
        }

        Employee obj = new Employee()
        {
            empid = 101,
            empname = "Mohit",
            salary=1000000
        };
        List<Employee> emplist = new List<Employee>()
        {
            new Employee{empid=101,empname="Travel", salary=100, imageurl="/images/IMG_3361.JPG",deptid=30},
            new Employee{empid=102,empname="Mohit", salary=100000, imageurl="/images/IMG_3687.jpg",deptid=20}
        };

        public IActionResult Details(int id)
        {
            var employee = emplist.FirstOrDefault(e => e.empid == id);
            if (employee==null)
            {
                return NotFound();
            }
            return View(employee);
        }
        public IActionResult Searchemp(int id)
        {
            Employee emp = (from e1 in emplist where e1.empid==id select e1).FirstOrDefault();
           
            return View(emp);
        }
        public IActionResult collectionofobjectpassing()
        {

            return View(emplist);
        }
        public IActionResult singleobjectpassing()
        {

            return View(obj);
        }

        List<DepartMent> deptlist = new List<DepartMent>()
     {
         new DepartMent{deptid=10,deptname="Sales"},
         new DepartMent{deptid=20,deptname="HR"},
         new DepartMent{deptid=30,deptname="Software"}
     };

        public IActionResult mixedobjectpassing(int id)
        {
            var query1 = deptlist.ToList();
            Employee emp=emplist.Where(x=>x.empid==id).FirstOrDefault();
            var query2 = emp;
            EmployeeDeptViewModel obj = new EmployeeDeptViewModel()
            {
                deptlist=query1,
                emp=query2,
                date=DateTime.Now
            };
            return View(obj);        
        }

        public IActionResult collectionofdepts()
        {
            return View(deptlist);
        }
       public IActionResult EmpsInDept(int id)
        {
            var employees = emplist.Where(e => e.deptid == id).ToList();
            return View(employees);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
