using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Class")]
    public class ItemClass
    {
        [Key]
        public int Class_ID { get; set; }
        public string ClassName { get; set; } 
        public int NumberStudent { get; set; }
        public int Advisor_ID { get; set; }
        public int Program_ID { get; set; }

    }
}
