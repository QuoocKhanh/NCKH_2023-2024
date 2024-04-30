using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("StudentStatus")]
    public class ItemStudentStatus
    {
        [Key]
        public int ID { get; set; }
        public int LanThu { get; set; }
        public int Student_ID { get; set; }
        public int Bonus_ID { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public string Note { get; set; }

    }
}
