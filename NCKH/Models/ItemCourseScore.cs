using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("CourseScore")]
    public class ItemCourseScore
    {
        [Key]
        public int ID { get; set; }
        public int LanHocThu { get; set; }
        public int Student_ID { get; set; }
        public int Course_ID { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public double Score { get; set; }
        public string TextScore { get; set; }


    }
}
