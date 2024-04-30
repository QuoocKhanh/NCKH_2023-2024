using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NCKH.Models
{
    [Table("GroupChat")]
    public class ItemGroupChat
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
