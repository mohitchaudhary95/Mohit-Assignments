namespace MVCDemo.Models
{
    public class DepartMent
    {
        public int deptid { get; set; }
        public string? deptname { set; get; }

        public List<Employee> emplist { set; get; }=new List<Employee>();
    }
}
