namespace MVCDemo.Models
{
    public class Employee
    {
        public int empid { set; get; }
        public string? empname { set; get; }
        public int salary { set; get; }
        public string? imageurl { set; get; }

        public int deptid { set; get; }
        public DepartMent? deptname { set; get; }
    }
}
