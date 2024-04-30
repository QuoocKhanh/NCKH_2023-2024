using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("TrainingProgram")]
    public class ItemTrainingProgram
    {
        [Key]
        public int Program_ID { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramName { get; set; }
        public string MajorName { get; set; }
        public int TotalCredits { get; set; }

    }
}
