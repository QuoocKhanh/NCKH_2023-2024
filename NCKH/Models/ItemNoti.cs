using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Noti")]
    public class ItemNoti
    {
        [Key]
        public int Noti_ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public string PostLink { get; set; }
        public int Type { get; set; }
    }
}
