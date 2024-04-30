using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Student")]
    public class ItemStudent
    {
        [Key]
        public int Student_ID { get; set; }
        public string StudentName { get; set; }
        public string Address { get; set; }
        public string StudentCode { get; set; }
        public string? Note { get; set; }
        public int User_ID { get; set; }
        public int Class_ID { get; set; }   
        public string? Role { get; set;}


    }
}
