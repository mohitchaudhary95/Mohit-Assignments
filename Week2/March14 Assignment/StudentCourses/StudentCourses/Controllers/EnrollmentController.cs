using Microsoft.AspNetCore.Mvc;
using StudentCourses.Models;
using StudentCourses.Data;

namespace StudentCourses.Controllers
{
    public class EnrollmentController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = DataStore.Students
                .Select(s => new StudentCoursesVM
                {
                    StudentId = s.StudentId,
                    Name = s.Name,
                    Branch = s.Branch,
                    CourseTitles = s.Enrollments
                                    .Select(e => e.Course!.Title)
                                    .Distinct()
                                    .ToList()
                })
                .ToList();

            return View(viewModel);
        }

        public IActionResult Details(int studentId)
        {
            var student = DataStore.Students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null) return NotFound();

            var courses = student.Enrollments
                .GroupBy(e => e.CourseId)
                .OrderByDescending(g => g.Max(e => e.AttemptNumber))
                .Select(g =>
                {
                    var latest = g.OrderByDescending(e => e.AttemptNumber).First();
                    return new CourseDetailVM
                    {
                        Title = latest.Course!.Title,
                        Credits = latest.Course.Credits,
                        Department = latest.Course.Department,
                        LatestGrade = latest.Grade
                    };
                })
                .ToList();

            var viewModel = new StudentDetailVM
            {
                Name = student.Name,
                Branch = student.Branch,
                Courses = courses
            };

            return View(viewModel);
        }
    }
}
