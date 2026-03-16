using StudentCourses.Models;
namespace StudentCourses.Data
{
    public class DataStore
    {
        public static List<Student> Students { get; } = new();
        public static List<Course> Courses { get; } = new();
        public static List<Enrollment> Enrollments { get; } = new();

        static DataStore()
        {
            // Seed Courses
            Courses.AddRange(new[]
            {
                new Course { CourseId = 1, Title = "Data Structures",  Credits = 4, Department = "CSE" },
                new Course { CourseId = 2, Title = "Algorithms",       Credits = 4, Department = "CSE" },
                new Course { CourseId = 3, Title = "Databases",        Credits = 3, Department = "CSE" },
                new Course { CourseId = 4, Title = "Web Development",  Credits = 3, Department = "CSE" },
                new Course { CourseId = 5, Title = "Operating Systems",Credits = 4, Department = "CSE" },
            });

            // Seed Students
            Students.AddRange(new[]
            {
                new Student { StudentId = 1, Name = "Alice",   Branch = "CSE" },
                new Student { StudentId = 2, Name = "Bob",     Branch = "CSE" },
                new Student { StudentId = 3, Name = "Charlie", Branch = "ECE" },
                new Student { StudentId = 4, Name = "Diana",   Branch = "ME"  },
                new Student { StudentId = 5, Name = "Eve",     Branch = "CSE" },
                new Student { StudentId = 6, Name = "Frank",   Branch = "ECE" },
            });

            // Seed Enrollments (10+ entries)
            var rawEnrollments = new[]
            {
                (StudentId: 1, CourseId: 1, Grade: "A",  Attempt: 1),
                (StudentId: 1, CourseId: 2, Grade: "A-", Attempt: 1),
                (StudentId: 1, CourseId: 3, Grade: "B+", Attempt: 1),
                (StudentId: 2, CourseId: 1, Grade: "B",  Attempt: 1),
                (StudentId: 2, CourseId: 4, Grade: "A",  Attempt: 1),
                (StudentId: 2, CourseId: 5, Grade: "B-", Attempt: 1),
                (StudentId: 3, CourseId: 2, Grade: "C+", Attempt: 1),
                (StudentId: 3, CourseId: 2, Grade: "B",  Attempt: 2), // retake
                (StudentId: 3, CourseId: 3, Grade: "A-", Attempt: 1),
                (StudentId: 4, CourseId: 4, Grade: "B+", Attempt: 1),
                (StudentId: 4, CourseId: 5, Grade: "A",  Attempt: 1),
                (StudentId: 5, CourseId: 1, Grade: "A+", Attempt: 1),
                (StudentId: 5, CourseId: 2, Grade: "A",  Attempt: 1),
                (StudentId: 6, CourseId: 3, Grade: "B",  Attempt: 1),
                (StudentId: 6, CourseId: 4, Grade: "A-", Attempt: 1),
            };

            foreach (var e in rawEnrollments)
            {
                var student = Students.First(s => s.StudentId == e.StudentId);
                var course = Courses.First(c => c.CourseId == e.CourseId);

                var enrollment = new Enrollment
                {
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    Grade = e.Grade,
                    AttemptNumber = e.Attempt,
                    Student = student,
                    Course = course
                };

                Enrollments.Add(enrollment);
                student.Enrollments.Add(enrollment);
                course.Enrollments.Add(enrollment);
            }
        }
    }
}
