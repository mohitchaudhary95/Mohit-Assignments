using Microsoft.AspNetCore.Mvc;
using Routing.Models;

namespace Routing.Controllers
{
    public class StudentController : Controller
    {
        List<Student> students = new List<Student>()
        {
            new Student{Id=1,Name="John",Class="10th" },
        new Student{Id=2,Name="Jane",Class="9th"},
        new Student{Id=3,Name="Doe",Class="8th"},
new  Student{Id= 4, Name= "Smith", Class= "7th"}
            };
        public IActionResult GetAllStudent()
        {
            return View(students);
        }
        public IActionResult GetStudent(int id)
        {
            var student=students.FirstOrDefault(s=>s.Id==id);
            return View(student);
        }
        public IActionResult fewcolumns()
        {
            var fewcolumns = students.Select(s => new { s.Name, s.Class }).ToList();
            return View(fewcolumns);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
