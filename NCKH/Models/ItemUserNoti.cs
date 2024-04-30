using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("UserNoti")]
    public class ItemUserNoti
    {
        [Key]
        public int UserNoti_ID { get; set; }
        public int Noti_ID { get; set; }
        public int User_ID { get; set; }
        public int IsRead { get; set; }

    }
}
