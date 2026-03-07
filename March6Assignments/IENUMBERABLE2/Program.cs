namespace IENUMBERABLE2
{
    internal class Program
    {
        class Employee
        {
            public string Name { set; get; }
            public int Salary { set; get; }
            public int DepartmentId { set; get; }

            public Employee(string name1, int salary1, int deptid)
            {
                Name = name1;
                Salary = salary1;
                DepartmentId = deptid;
            }
        }

        public class EmployeeCollection
        {
            private readonly List<Employee> _employees;

            public EmployeeCollection()
            {
                _employees = new List<Employee>
            {

           new Employee("Alice", 50000, 1),
            new Employee("Bob", 80000, 1),
            new Employee("Charlie", 60000, 2),
            new Employee("Diana", 90000, 2)

            };
            }
        }
        static void Main(string[] args)
        {
            var employees = new EmployeeCollection();
            Console.WriteLine("\nDisplaying all employees");
            foreach (Employee e in employees)
            {
                Console.WriteLine($"{e.Name}: ${e.Salary}, Dept {e.DepartmentId}");
            }

            int[] numbers = { 1, 2, 3 };
            foreach (int n in numbers)
            {
                Console.WriteLine(n);
            }// Works!

            List<string> names = new List<string> { "Alice", "Bob" };
            foreach (string name in names)
            {
                Console.WriteLine(name);
            }// Works!

            Dictionary<int, string> dict = new Dictionary<int, string> { { 1, "One" } };
            foreach (var kvp in dict)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}")
                    ;  // Works!
            }

            string text = "Hello";
            foreach (char c in text)
            {
                Console.WriteLine(c);
            }// Works! (chars)

            Console.ReadLine();
        }
    }
}
