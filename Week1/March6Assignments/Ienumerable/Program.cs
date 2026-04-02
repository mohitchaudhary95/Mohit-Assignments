namespace Ienumerable
{
    class Employee
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime HireDate { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Employee> emp = new List<Employee>() {
                new Employee{ Name = "John", Id = 1, HireDate = new DateTime(2022, 5, 1) },
                new Employee{ Name = "Adam", Id = 2, HireDate = new DateTime(2020, 10, 3) },
                new Employee{ Name = "Dex", Id = 3, HireDate = new DateTime(2021, 12, 11) }
            };

            IEnumerable<Employee> emp2 = from e in emp where e.HireDate.Year > 2020 select e;

            foreach (Employee e in emp2)
            {
                Console.WriteLine($"{ e.Name}");
            }


            Console.ReadLine();
        }
    }
}
