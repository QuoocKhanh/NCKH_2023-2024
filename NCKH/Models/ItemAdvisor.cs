using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Advisor")]
    public class ItemAdvisor
    {
        [Key]
        public int Advisor_ID { get; set; }
        public string AdvisorName { get; set; }
        public string AR { get; set; }
        public string Degree { get; set; }
        public int User_ID { get; set; }
    }
}
