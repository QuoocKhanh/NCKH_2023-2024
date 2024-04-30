using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Comment")]
    public class ItemComment
    {
        [Key]
        public int Comment_ID { get; set; }
        public int Post_ID { get; set; }
        public int User_ID { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }


    }
}
