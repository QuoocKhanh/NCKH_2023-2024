using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("ProgramCourse")]
    public class ItemProgramCourse
    {
        [Key]
        public int ID { get; set; }
        public int Course_ID { get; set; }
        public int Program_ID { get; set; }
        public string Semester { get; set; }

    }
}
