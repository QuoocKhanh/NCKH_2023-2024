using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("Post")]
    public class ItemPost
    {
        [Key]
        public int Post_ID { get; set; }
        public int Class_ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }


    }
}
