using System.ComponentModel.DataAnnotations;
namespace MVCDemo.Models
{
    public class Dog
    {
        [Required(ErrorMessage="ID is requireed")]
        public int id { get; set; }

        [Required(ErrorMessage = "Name is requireed"),MaxLength(222)]
        public string? name { get; set; }

        [Required(ErrorMessage = "Name is requireed"), Range(0,20,ErrorMessage ="Age should be between o and 20 only")]
        public int age { get; set; }
    }
}
