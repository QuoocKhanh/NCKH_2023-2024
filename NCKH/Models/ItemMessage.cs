using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Message")]
    public class ItemMessage
    {
        [Key]
        public int ID { get; set; }
        public string Content { get; set; }
        public int? ID_UserChat { get; set; }
        public int ID_GroupChat { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
    }
}
