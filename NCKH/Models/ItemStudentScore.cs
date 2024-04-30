using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("StudentScore")]
    public class ItemStudentScore
    {
        [Key]
        public int ID { get; set; }
        public int Student_ID { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public double Score { get; set; }
        public double Score4 { get; set; }
    }
}
