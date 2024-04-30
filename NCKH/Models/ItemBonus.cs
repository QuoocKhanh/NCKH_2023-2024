using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Bonus")]
    public class ItemBonus
    {
        [Key]
        public int Bonus_ID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
    }
}
